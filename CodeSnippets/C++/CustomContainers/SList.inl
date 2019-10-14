/// <summary>
/// The implementations of inline functions from the SList header. This component is made
/// so that the linked list can be reusable by custom types.
/// </summary>

#pragma 
#include "SList.h"

namespace FieaGameEngine
{

#pragma region SList

	template <typename T>
	inline SList<T>::SList() :
		mFront(nullptr), mBack(nullptr)
	{
	}

	template<typename T>
	inline SList<T>::~SList()
	{
		Clear();
	}

	template <typename T>
	inline SList<T>::SList(const SList<T> &list) :
		mFront(nullptr), mBack(nullptr)
	{
		for (const auto& value : list)
		{
			PushBack(value);
		}
	}

	template<typename T>
	inline T& SList<T>::Front()
	{
		if (mFront == nullptr)
			throw std::exception("The list is empty!");

		return mFront->Data;
	}

	template<typename T>
	inline const T& SList<T>::Front() const
	{
		return const_cast<SList*>(this)->Front();
	}

	template<typename T>
	inline T& SList<T>::Back()
	{
		if (mBack == nullptr)
			throw std::exception("The list is empty!");

		return mBack->Data;
	}

	template<typename T>
	inline const T& SList<T>::Back() const
	{
		return const_cast<SList*>(this)->Back();
	}


	template<typename T>
	inline size_t SList<T>::Size() const
	{
		return mSize;
	}

	template<typename T>
	inline void SList<T>::PushFront(const T& value)
	{
		mFront = new Node(value, mFront);

		if (mBack == nullptr)
			mBack = mFront;

		++mSize;
	}

	template<typename T>
	inline void SList<T>::PopFront()
	{
		if (mSize == 0)
			throw std::exception("The list is empty!");

		Node *newNode = mFront;

		mFront = mFront->Next;
		delete newNode;

		mSize -= 1;

		if (mSize == 0)
		{
			mFront = mBack = nullptr;
		}
	}

	template<typename T>
	inline void SList<T>::PopBack()
	{
		if (mSize == 0)
			throw std::exception("The list is empty!");

		Node *current = mFront;
		Node *previous = mFront;

		while (current->Next != nullptr)
		{
			previous = current;
			current = current->Next;
		}

		mBack = previous;
		previous->Next = nullptr;
		delete current;
		current = nullptr;


		mSize -= 1;

		if (mSize == 0)
		{
			mBack = mFront = nullptr;
		}
	}

	template<typename T>
	inline void SList<T>::PushBack(const T& value)
	{
		Node *newNode = new Node(value, nullptr);

		if (mSize == 0)
		{
			mFront = newNode;
		}
		else
		{
			mBack->Next = newNode;
		}

		mBack = newNode;

		++mSize;
	}

	template<typename T>
	inline SList<T> &FieaGameEngine::SList<T>::operator=(const SList<T> &rhs)
	{
		if (this != &rhs)
		{
			Clear();

			for (const auto& value : rhs)
			{
				PushBack(value);
			}
		}

		return *this;
	}

	template<typename T>
	inline bool SList<T>::IsEmpty() const
	{
		if (mSize == 0)
			return true;
		else
			return false;
	}

	template<typename T>
	inline void SList<T>::Clear()
	{
		if (mSize == 0)
			return;

		Node *current = mFront;
		Node *previous = mFront;

		while (current != nullptr)
		{
			previous = current;
			current = current->Next;
			delete previous;
		}

		mSize = 0;
		mFront = nullptr;
		mBack = nullptr;
	}

	template<typename T>
	inline typename void SList<T>::InsertAfter(const Iterator& it, const T& value)
	{
		if (it.mOwner != this)
			throw std::exception("Invalid iterator. Owner by different container.");

		if ((it == end()) || (mBack == it.mNode))
		{
			PushBack(value);
		}
		else
		{
			Node* node = new Node(value, it.mNode->Next);
			it.mNode->Next = node;
			++mSize;
		}

	}

	template<typename T>
	inline typename SList<T>::Iterator SList<T>::Find(const T& value) const
	{
		Iterator it = begin();

		for (; it != end(); ++it)
		{
			if (value == *it)
				break;
		}

		return it;
	}

	template<typename T>
	inline void SList<T>::Remove(const T & value)
	{
		Iterator it = Find(value);

		if (it.mOwner != this)
			throw std::exception("Invalid iterator. Owned by different container.");

		if (it != end())
		{
			if (it.mNode == mBack)
			{
				PopBack();
			}
			else
			{
				Node *next = it.mNode->Next;
				it.mNode->Data.~T();
				new (&it.mNode->Data)T(std::move(next->Data));
				it.mNode->Next = next->Next;
				delete next;

				if (it.mNode->Next == nullptr)
				{
					mBack = it.mNode;
				}

				--mSize;
			}
		}
	}

#pragma endregion

#pragma region Iterator

	template<typename T>
	inline SList<T>::Iterator::Iterator(const SList *owner, Node* node) :
		mOwner(owner), mNode(node)
	{
	}

	template<typename T>
	inline bool SList<T>::Iterator::operator==(const Iterator & rhs) const
	{
		return (!(operator!=(rhs)));
	}

	template<typename T>
	inline bool SList<T>::Iterator::operator!=(const Iterator & rhs) const
	{
		return ((mOwner != rhs.mOwner) || (mNode != rhs.mNode));
	}

	template<typename T>
	inline typename SList<T>::Iterator & SList<T>::Iterator::operator++()
	{
		if (mOwner == nullptr)
			throw std::runtime_error("List is empty!");

		if (mNode != nullptr)
			mNode = mNode->Next;

		return *this;
	}

	template<typename T>
	inline typename SList<T>::Iterator SList<T>::Iterator::operator++(int)
	{
		Iterator previousPtr = *this;
		++(*this);
		return previousPtr;
	}

	template<typename T>
	inline T & SList<T>::Iterator::operator*() const
	{
		if (mOwner == nullptr)
			throw std::runtime_error("List is empty!");

		if (mNode == nullptr)
			throw std::runtime_error("Iterator is empty!");

		return mNode->Data;
	}

	template<typename T>
	inline typename SList<T>::Iterator SList<T>::begin()
	{
		return Iterator(this, mFront);
	}
	template<typename T>
	inline typename SList<T>::Iterator SList<T>::end()
	{
		return Iterator(this, nullptr);
	}

	template<typename T>
	inline typename SList<T>::Iterator SList<T>::begin() const
	{
		return Iterator(this, mFront);
	}
	template<typename T>
	inline typename SList<T>::Iterator SList<T>::end() const
	{
		return Iterator(this, nullptr);
	}

#pragma endregion

}