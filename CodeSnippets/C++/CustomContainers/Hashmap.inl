/// <summary>
/// The implementations of inline functions from the HashMap header. This component is made
/// to use the hashmap structer templated class.
/// </summary>

#pragma 
#include "Hashmap.h"

#pragma region HashMap

template<typename TKey, typename TData, typename FUNC>
inline FieaGameEngine::HashMap<TKey, TData, FUNC>::HashMap(size_t bucketSize)
{
	if (bucketSize == 0)
	{
		throw std::exception("Bucket size must be greater than zero.");
	}

	mBuckets.Reserve(bucketSize);

	for (size_t i = 0; i < bucketSize; ++i)
	{
		mBuckets.PushBack(ChainType());
	}
}

template<typename TKey, typename TData, typename FUNC>
inline FieaGameEngine::HashMap<TKey, TData, FUNC>::HashMap(std::initializer_list<PairType> list) :
	HashMap(list.size())
{
	for (const auto& item : list)
	{
		Insert(item);
	}
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator FieaGameEngine::HashMap<TKey, TData, FUNC>::Find(const TKey & key)
{
	DefaultHash<TKey> functor;
	size_t index = functor(key) % this->mBuckets.Size();
	ChainType& list = mBuckets[index];
	SList<PairType>::Iterator it = list.begin();

	for (; it != list.end(); ++it)
	{
		PairType pair = *it;
		if (key == pair.first)
		{
			break;
		}
	}

	if (it == list.end())
	{
		SList<PairType>::Iterator empty;
		return Iterator(this, (this->mBuckets.Size()), empty);
	}
	else
		return Iterator(this, index, it);
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator FieaGameEngine::HashMap<TKey, TData, FUNC>::Insert(const PairType & entry)
{
	auto it = Find(entry.first);

	if (it == end())
	{
		DefaultHash<TKey> functor;
		size_t index = functor(entry.first) % this->mBuckets.Size();
		ChainType& list = mBuckets[index];

		++mPopulation;
		list.PushBack(entry);
		it = Find(entry.first);
	}

	return it;
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator FieaGameEngine::HashMap<TKey, TData, FUNC>::Insert(const PairType & entry, bool& entryCreated)
{
	auto it = Find(entry.first);

	if (it == end())
	{
		DefaultHash<TKey> functor;
		size_t index = functor(entry.first) % this->mBuckets.Size();
		ChainType& list = mBuckets[index];

		++mPopulation;
		list.PushBack(entry);
		it = Find(entry.first);
		entryCreated = true;
	}
	else 
	{
		entryCreated = false;
	}

	return it;
}

template<typename TKey, typename TData, typename FUNC>
inline typename TData & FieaGameEngine::HashMap<TKey, TData, FUNC>::operator[](const TKey & key)
{
	return (*Insert(PairType(key, TData()))).second;
}

template<typename TKey, typename TData, typename FUNC>
inline void FieaGameEngine::HashMap<TKey, TData, FUNC>::Remove(const TKey & key)
{
	bool containsKey = ContainsKey(key);

	if (containsKey)
	{
		DefaultHash<TKey> functor;
		Iterator remove = Find(key);
		size_t index = functor(key) % mBuckets.Size();
		ChainType& list = mBuckets[index];

		--mPopulation;
		list.Remove(*remove);
	}
}

template<typename TKey, typename TData, typename FUNC>
inline void FieaGameEngine::HashMap<TKey, TData, FUNC>::Clear()
{
	for (size_t i = 0; i < this->mBuckets.Size(); i++)
	{
		mBuckets[i].Clear();
	}

	mPopulation = 0;
}

template<typename TKey, typename TData, typename FUNC>
inline size_t FieaGameEngine::HashMap<TKey, TData, FUNC>::Size() const
{
	return mPopulation;
}

template<typename TKey, typename TData, typename FUNC>
inline bool FieaGameEngine::HashMap<TKey, TData, FUNC>::ContainsKey(const TKey & key)
{
	return Find(key) != end();
}

template<typename TKey, typename TData, typename FUNC>
inline bool FieaGameEngine::HashMap<TKey, TData, FUNC>::ContainsKey(const TKey & key, Iterator & it)
{
	it = Find(key);

	if (it == end())
	{
		return false;
	}
	else
	{
		PairType pair = *it;
		return (pair.first == key) ? true : false;
	}
}

template<typename TKey, typename TData, typename FUNC>
inline bool FieaGameEngine::HashMap<TKey, TData, FUNC>::ContainsKey(const TKey & key, ConstIterator & it) const
{
	it = Find(key);

	if (it == end())
	{
		return false;
	}
	else
	{
		PairType pair = *it;
		return (pair.first == key) ? true : false;
	}
}

template<typename TKey, typename TData, typename FUNC>
inline TData & FieaGameEngine::HashMap<TKey, TData, FUNC>::At(const TKey & key)
{
	bool containsKey = ContainsKey(key);

	if (!containsKey)
	{
		throw std::exception("The key does not exist!");
	}

	return (*Find(key)).second;
}

template<typename TKey, typename TData, typename FUNC>
inline const TData & FieaGameEngine::HashMap<TKey, TData, FUNC>::At(const TKey & key) const
{
	bool containsKey = ContainsKey(key);

	if (!containsKey)
	{
		std::pair<TKey, TData> newEntry(key, TData());
		Insert(newEntry);
	}

	return (*Find(key)).second;
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator FieaGameEngine::HashMap<TKey, TData, FUNC>::begin()
{
	for (std::size_t i = 0; i < mBuckets.Size(); ++i)
	{
		ChainType& chain = mBuckets[i];
		if (chain.IsEmpty() == false)
		{
			return Iterator(this, i, chain.begin());
		}
	}

	return end();
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator FieaGameEngine::HashMap<TKey, TData, FUNC>::begin() const
{
	ChainType& list = this->mOwner->mBuckets[0];
	ChainType& list = mBuckets[0];
	ChainType::Iterator it = list.begin();
	size_t index = 0;

	if (list.IsEmpty())
	{
		for (size_t i = 1; i < this->mBuckets.Size(); ++i)
		{
			list = mBuckets[i];
			index = i;

			if (!list.IsEmpty())
			{
				it = list.begin();
				break;
			}
		}
	}

	return ConstIterator(this, index, it);
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator FieaGameEngine::HashMap<TKey, TData, FUNC>::end()
{
	SList<PairType>::Iterator it;
	return Iterator(this, mBuckets.Size(), it);
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator FieaGameEngine::HashMap<TKey, TData, FUNC>::end() const
{
	SList<PairType>::Iterator it;
	return ConstIterator(this, mBuckets.Size(), it);
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator FieaGameEngine::HashMap<TKey, TData, FUNC>::cbegin() const
{
	ChainType& list = this->mOwner->mBuckets[0];
	ChainType& list = mBuckets[0];
	ChainType::Iterator it = list.begin();
	size_t index = 0;

	if (list.IsEmpty())
	{
		for (size_t i = 1; i < this->mBuckets.Size(); ++i)
		{
			list = mBuckets[i];
			index = i;

			if (!list.IsEmpty())
			{
				it = list.begin();
				break;
			}
		}
	}

	return ConstIterator(this, index, it);
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator FieaGameEngine::HashMap<TKey, TData, FUNC>::cend() const
{
	SList<PairType>::Iterator it;
	return ConstIterator(this, this->mBuckets.Size(), it);
}

#pragma endregion

#pragma region Iterator

template<typename TKey, typename TData, typename FUNC>
inline FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator::Iterator(HashMap *owner, size_t index, typename const SList<PairType>::Iterator& sListIt) :
	mOwner(owner), mIndex(index), mSListIt(sListIt)
{
}

template<typename TKey, typename TData, typename FUNC>
inline typename bool FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator::operator==(const Iterator& rhs) const
{
	return (!(operator!=(rhs)));
}

template<typename TKey, typename TData, typename FUNC>
inline typename bool FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator::operator!=(const Iterator& rhs) const
{
	return ((mOwner != rhs.mOwner) || (mIndex != rhs.mIndex) || (mSListIt != rhs.mSListIt));
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator & FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator::operator++()
{
	if (mOwner == nullptr)
		throw std::runtime_error("HashMap is empty!");

	ChainType& list = this->mOwner->mBuckets[mIndex];

	++mSListIt;

	if (mSListIt == list.end())
	{
		for (size_t i = (mIndex + 1); i < this->mOwner->mBuckets.Size(); ++i)
		{
			list = this->mOwner->mBuckets[i];
			mSListIt = list.begin();
			mIndex = i;

			if (!list.IsEmpty())
			{
				break;
			}
		}
	}

	return *this;
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator::operator++(int)
{
	if (mOwner == nullptr)
		throw std::runtime_error("HashMap is empty!");

	Iterator previous = *this;
	++(*this);
	return previous;
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::PairType & FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator::operator*() const
{
	if (this->mOwner == nullptr)
		throw std::runtime_error("There is no owner for this iterator!");

	return *mSListIt;
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::PairType* FieaGameEngine::HashMap<TKey, TData, FUNC>::Iterator::operator->() const
{
	if (this->mOwner == nullptr)
		throw std::runtime_error("There is no owner for this iterator!");

	return &(operator*());
}

#pragma endregion

#pragma region ConstIterator

template<typename TKey, typename TData, typename FUNC>
inline FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::ConstIterator(const HashMap *owner, size_t index, typename const SList<PairType>::Iterator sListIt) :
	mOwner(owner), mIndex(index), mSListIt(sListIt)
{
}

template<typename TKey, typename TData, typename FUNC>
inline FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::ConstIterator(const Iterator & iterator)
{
	mOwner = iterator.mOwner;
	mIndex = iterator.mIndex;
	mSListIt = iterator.mSListIt;
}

template<typename TKey, typename TData, typename FUNC>
inline typename bool FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::operator==(const ConstIterator& rhs) const
{
	return (!(operator!=(rhs)));
}

template<typename TKey, typename TData, typename FUNC>
inline typename bool FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::operator!=(const ConstIterator& rhs) const
{
	return ((mOwner != rhs.mOwner) || (mIndex != rhs.mIndex) || (mSListIt != rhs.mSListIt));
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator & FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::operator++()
{
	if (mOwner == nullptr)
		throw std::runtime_error("HashMap is empty!");

	ChainType& list = this->mOwner->mBuckets[mIndex];

	++mSListIt;

	if (mSListIt == list.end())
	{
		for (size_t i = (mIndex + 1); i < this->mOwner->Size(); ++i)
		{
			list = this->mOwner->mBucket[i];
			mSListIt = list.begin();
			mIndex = i;

			if (!list.IsEmpty())
			{
				break;
			}
		}
	}

	return *this;
}

template<typename TKey, typename TData, typename FUNC>
inline typename FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::operator++(int)
{
	if (mOwner == nullptr)
		throw std::runtime_error("HashMap is empty!");

	ChainType& list = this->mOwner->mBuckets[mIndex];
	ChainType::ConstIterator tmp = mSListIt;
	size_t tmpIndex = mIndex;

	++mSListIt;

	if (mSListIt == list.end())
	{
		for (size_t i = (mIndex + 1); i < this->mOwner->Size(); ++i)
		{
			list = this->mOwner->mBucket[i];
			mSListIt = list.begin();
			mIndex = i;

			if (!list.IsEmpty())
			{
				break;
			}
		}
	}

	return ConstIterator(this, tmpIndex, tmp);
}

template<typename TKey, typename TData, typename FUNC>
inline typename const FieaGameEngine::HashMap<TKey, TData, FUNC>::PairType & FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::operator*() const
{
	if (this->mOwner == nullptr)
		throw std::runtime_error("There is no owner for this iterator!");

	return *(this->mSListIt);
}

template<typename TKey, typename TData, typename FUNC>
inline typename const FieaGameEngine::HashMap<TKey, TData, FUNC>::PairType & FieaGameEngine::HashMap<TKey, TData, FUNC>::ConstIterator::operator->() const
{
	if (this->mOwner == nullptr)
		throw std::runtime_error("There is no owner for this iterator!");

	return *(this->mSListIt);
}


#pragma endregion