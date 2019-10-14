#include "pch.h"
#include "EventQueue.h"
#include <algorithm>

namespace FieaGameEngine
{
	void EventQueue::EnqueueEvent(std::shared_ptr<EventPublisher> event, const GameTime& gameTime, const DurationType& delay)
	{
		std::lock_guard<std::mutex> lock(mPendingQueueMutex);
		const auto& it = Find(mPendingQueue, event);

		if (it != mPendingQueue.end())
		{
			(*it).TimeEnqueued = gameTime.CurrentTime();
			(*it).Delay = delay;
		}
		else
		{
			mPendingQueue.EmplaceBack(std::move(event), gameTime.CurrentTime(), delay);
		}
	}

	void EventQueue::UpdateEvent(const std::shared_ptr<EventPublisher>& event, const DurationType& delay)
	{
		std::lock_guard<std::mutex> lock(mPendingQueueMutex);
		const auto& it = Find(mPendingQueue, event);

		if (it != mPendingQueue.end())
		{
			(*it).Delay = delay;
		}
	}

	bool EventQueue::RemoveEvent(const std::shared_ptr<EventPublisher>& event)
	{
		std::lock_guard<std::mutex> lock(mPendingQueueMutex);
		const auto& it = Find(mPendingQueue, event);

		if (it != mPendingQueue.end())
		{
			mPendingQueue.Remove(it);
			return true;
		}

		return false;
	}

	void EventQueue::Update(const GameTime& gameTime)
	{
		{
			// thread safe although currently the update calls are synchronous
			std::lock_guard<std::mutex> lock(mPendingQueueMutex);

			// move all expired events to the back of the list
			auto expiredBegin = std::partition(mPendingQueue.begin(), mPendingQueue.end(),
				[&gameTime](const QueueEntry& queued) { return !queued.IsExpired(gameTime.CurrentTime()); });

			// move all expired events from the pending queue to the expired queue
			std::move(expiredBegin, mPendingQueue.end(), std::back_inserter(mExpiredQueue));
			mPendingQueue.Remove(expiredBegin, mPendingQueue.end());
		}

		mJobs.Reserve(mExpiredQueue.Size());

		// deliver all expired events
		for (const auto& entry : mExpiredQueue)
		{
			mJobs.EmplaceBack(std::async(std::launch::async, &EventPublisher::Deliver, entry.Event));
		}

		for (auto& job : mJobs)
		{
			try
			{
				job.get();
			}
			catch (...)
			{
				mJobs.Clear();
				throw;
			}
		}

		// clear expired queue
		mExpiredQueue.Clear();
		mJobs.Clear();
	}

	void EventQueue::Clear()
	{
		std::lock_guard<std::mutex> lock(mPendingQueueMutex);
		mPendingQueue.Clear();
	}

	void EventQueue::ShrinkToFit()
	{
		std::lock_guard<std::mutex> lock(mPendingQueueMutex);
		mPendingQueue.ShrinkToFit();
		mExpiredQueue.ShrinkToFit();
	}

	bool EventQueue::IsEmpty() const
	{
		std::lock_guard<std::mutex> lock(mPendingQueueMutex);
		return mPendingQueue.IsEmpty();
	}

	size_t EventQueue::Size() const
	{
		std::lock_guard<std::mutex> lock(mPendingQueueMutex);
		return mPendingQueue.Size();
	}

	Vector<EventQueue::QueueEntry>::Iterator EventQueue::Find(Vector<QueueEntry>& list, const std::shared_ptr<EventPublisher>& event)
	{
		return std::find_if(list.begin(), list.end(),
			[&event](const QueueEntry& entry) { return entry.Event == event; });
	}

	EventQueue::QueueEntry::QueueEntry(std::shared_ptr<EventPublisher>&& event, const TimePointType& timeEnqueued, const DurationType& delay) :
		Event(std::move(event)), TimeEnqueued(timeEnqueued), Delay(delay)
	{
	}

	bool EventQueue::QueueEntry::IsExpired(const TimePointType& currentTime) const
	{
		return TimeEnqueued + Delay < currentTime;
	}
}
