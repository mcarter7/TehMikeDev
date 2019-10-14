#pragma 
#include "Vector.h"
#include "Hashmap.h"
#include <algorithm>
#include <initializer_list>

namespace FieaGameEngine 
{
	template <typename T>
	std::uint32_t Vector<T>::operator()(std::uint32_t size, std::uint32_t capacity) const
	{
		if (capacity < size)
			capacity = size;

		capacity += (capacity + size);
		return capacity;
	}

#pragma region Vector

	inline std::size_t operator "" _z(unsigned long long int x)
	{
		return static_cast<size_t>(x);
	}

	template<typename T>
	inline Vector<T>::Vector()
	{
		Reserve(mDefaultCapacity);
	}

	template <typename T>
	inline Vector<T>::~Vector()
	{
		Clear();
		free(mData);
	}


	template <typename T>     
	inline Vector<T>::Vector(const Vector& rhs) : 
		mCapacity(rhs.mCapacity), mSize(rhs.mSize) 
	{
		mData = reinterpret_cast<T*>(malloc(rhs.mCapacity * sizeof(T)));
		for (size_t i = 0; i < rhs.mSize; i++) 
		{ 
			new (mData + i)T(rhs.mData[i]); 
		}
	}

	template<typename T>
	inline bool Vector<T>::operator==(const Vector& rhs) const
	{
		if (Size() != rhs.Size())
		{
			return false;
		}

		for (size_t i = 0; i < Size(); i++)
		{
			/*if (mData[i] != rhs.mData[i])
			{
				return false;
			}*/
		}
		return true;
	}

	template<typename T>
	inline bool Vector<T>::operator==(const Vector & rhs)
	{
		if (Size() != rhs.Size())
		{
			return false;
		}

		for (size_t i = 0; i < Size(); i++)
		{
			if (mData[i] != rhs.mData[i])
			{
				return false;
			}
		}
		return true;
	}

	template<typename T>
	inline bool Vector<T>::operator!=(const Vector & rhs) const
	{
		return !operator==(rhs);
	}

	template<typename T>
	inline bool Vector<T>::operator!=(const Vector<T> & rhs)
	{
		return !operator==(rhs);
	}
	
	template <typename T>
	inline Vector<T>::Vector(const std::initializer_list<T>& rhs)
	{
		Reserve(rhs.size());
		for (const auto& value : rhs)
		{
			PushBack(value);
		}
	}

	template<typename T>
	inline Vector<T>::Vector(Vector && rhs) :
		mSize(rhs.mSize), mCapacity(rhs.mCapacity), mData(rhs.mData)
	{
		rhs.mSize = 0;
		rhs.mCapacity = 0;
		rhs.mData = nullptr;
	}

	template<typename T>
	inline T & Vector<T>::operator[](size_t i)
	{
		if(i > mSize)
			throw std::exception("Invalid index. Out of range!");

		return mData[i];
	}

	template<typename T>
	inline const T & Vector<T>::operator[](size_t i) const
	{
		if (i > mSize)
			throw std::exception("Invalid index. Out of range!");

		return mData[i];
	}

	template<typename T>
	inline Vector<T>& Vector<T>::operator=(const Vector<T>& rhs)
	{
		if (this != &rhs)
		{
			T* tmp = (T*)malloc(rhs.mCapacity * sizeof(T));

			for (size_t i = 0; i < rhs.mSize; i++)
			{
				new (tmp + i)T(rhs.mData[i]);
			}

			Clear();
			free(mData);
			mData = tmp;
			mCapacity = rhs.mCapacity;
			mSize = rhs.mSize;
		}

		return *this;
	}

	template<typename T>
	inline Vector<T>& Vector<T>::operator=(Vector<T>&& rhs)
	{
		if (this != &rhs)
		{
			Clear();
			mData = rhs.mData;
			mDefaultCapacity = rhs.mDefaultCapacity;
			mCapacity = rhs.mCapacity;
			mSize = rhs.mSize;

			rhs.mSize = 0;
			rhs.mCapacity = 0;
			rhs.mData = nullptr;
		}
		return *this;
	}

	template<typename T>
	inline T & Vector<T>::At(size_t i)
	{
		if (i > mSize)
			throw std::exception("Invalid index. Out of range!");

		return mData[i];
	}

	template<typename T>
	inline const T & Vector<T>::At(size_t i) const
	{
		if (i > mSize)
			throw std::exception("Invalid index. Out of range!");

		return mData[i];
	}

	template<typename T>
	inline void Vector<T>::PopBack()
	{
		if (!IsEmpty()) 
		{
			mData[mSize - 1].~T();
			--mSize;
		}
	}

	template<typename T>
	inline void Vector<T>::PushBack(const T & value)
	{
		if (mSize == mCapacity)
		{
			size_t capacity = std::max(mCapacity + 1, mCapacity * 2);
			Reserve(capacity);
		}

		new (mData + mSize)T(value);
		++mSize;
	}

	template<typename T>
	inline void Vector<T>::PushBack(const T * value)
	{
		if (mSize == mCapacity)
		{
			size_t capacity = std::max(mCapacity + 1, mCapacity * 2);
			Reserve(capacity);
		}

		new (mData + mSize)T(value);
		++mSize;
	}

	template<typename T>
	inline void Vector<T>::PushBack(T && value)
	{
		if ((mSize + 1) > mCapacity)
		{
			size_t capacity = std::max(mCapacity + 1, mCapacity * 2);
			Reserve(capacity);
		}

		new (mData + mSize)T(std::move(value));
		++mSize;
	}

	template<typename T>
	inline bool Vector<T>::IsEmpty() const
	{
		return (mSize == 0) ? true : false;
	}

	template<typename T>
	inline T & Vector<T>::Front()
	{
		if (mSize == 0)
			throw std::exception("The vector is empty!");

		return mData[0];
	}

	template<typename T>
	inline const T & Vector<T>::Front() const
	{
		return const_cast<Vector*>(this)->Front();
	}

	template<typename T>
	inline T & Vector<T>::Back()
	{
		if (mSize == 0)
			throw std::exception("The vector is empty!");

		return mData[mSize - 1];
	}

	template<typename T>
	inline const T & Vector<T>::Back() const
	{
		return const_cast<Vector*>(this)->Back();
	}

	template<typename T>
	inline size_t Vector<T>::Size() const
	{
		return mSize;
	}

	template<typename T>
	inline size_t Vector<T>::Capacity() const
	{
		return mCapacity;
	}

	template<typename T>
	inline typename Vector<T>::Iterator Vector<T>::begin()
	{
		return Iterator(this, 0); 
	}

	template<typename T>
	inline typename Vector<T>::ConstIterator Vector<T>::begin() const
	{
		return ConstIterator(this, 0);
	}

	template<typename T>
	inline typename Vector<T>::Iterator Vector<T>::end()
	{
		return Iterator(this, mSize);
	}

	template<typename T>
	inline typename Vector<T>::ConstIterator Vector<T>::end() const
	{
		return ConstIterator(this, mSize);
	}

	template<typename T>
	inline typename Vector<T>::ConstIterator Vector<T>::cbegin() const
	{
		return ConstIterator(this, 0);
	}

	template<typename T>
	inline typename Vector<T>::ConstIterator Vector<T>::cend() const
	{
		return ConstIterator(this, mSize);
	}

	template<typename T>
	inline void Vector<T>::Reserve(size_t capacity)
	{
		if (capacity > mCapacity)
		{
			mData = reinterpret_cast<T*>(realloc(mData, sizeof(T) * capacity));
			mCapacity = capacity;
		}
	}

	template<typename T>
	inline typename Vector<T>::Iterator Vector<T>::Find(T& value)
	{
		Iterator it = begin();

		for (; it != end(); ++it)
		{
			if (value == *it)
				break;
		}

		return it;
	}

	template<typename T> typename
	inline Vector<T>::Iterator Vector<T>::Find(const T & value)
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
	inline typename const Vector<T>::Iterator Vector<T>::Find(T& value) const
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
	inline void Vector<T>::Clear()
	{
		for (size_t i = 0; i < mSize; ++i)
		{
			mData[i].~T();
		}
		mSize = 0;
	}

	template<typename T>
	inline void Vector<T>::Remove(T & value)
	{
		Iterator it = Find(value);

		if (it.mOwner != this)
			throw std::exception("Invalid iterator. Owned by different container.");


		if (it != end())
		{
			if (*it == mData[mSize - 1])
			{
				PopBack();
			}
			else
			{
				size_t currentIndex = it.mIndex;
				(*it).~T();
				memmove(mData + currentIndex, mData + (currentIndex + 1), sizeof(T) * (mSize - currentIndex - 1));
				--mSize;
			}
		}
	}

	template<typename T>
	inline void Vector<T>::Remove(const T & value)
	{
		Remove(Find(value));
	}

	template<typename T>
	inline void Vector<T>::Remove(const Vector::Iterator & it)
	{
		if (it != end())
		{
			if (*it == mData[mSize - 1])
			{
				PopBack();
			}
			else
			{
				size_t currentIndex = it.mIndex;
				(*it).~T();
				memmove(mData + currentIndex, mData + (currentIndex + 1), sizeof(T) * (mSize - currentIndex - 1));
				--mSize;
			}
		}
	}

	template<typename T>
	inline void Vector<T>::Remove(Iterator first, Iterator last)
	{
		if (first != end())
		{
			size_t numberOfItemsRemoved = 0;
			for (auto it = first; it != last; it++)
			{
				mData[it.mIndex].~T();
				numberOfItemsRemoved++;
			}

			std::memmove(mData + first.mIndex, mData + first.mIndex + numberOfItemsRemoved, (Size() - (first.mIndex + numberOfItemsRemoved)) * sizeof(T));
			mSize -= numberOfItemsRemoved;
		}
	}

#pragma endregion

#pragma region Iterator

	template<typename T>
	inline Vector<T>::Iterator::Iterator(Vector *owner, size_t index) :
		mOwner(owner), mIndex(index)
	{
	}

	template<typename T>
	inline bool Vector<T>::Iterator::operator==(const Iterator & rhs) const
	{
		return (!(operator!=(rhs)));
	}

	template<typename T>
	inline bool Vector<T>::Iterator::operator!=(const Iterator & rhs) const
	{
		return ((mOwner != rhs.mOwner) || (mIndex != rhs.mIndex));
	}

	template<typename T>
	inline typename Vector<T>::Iterator & Vector<T>::Iterator::operator++()
	{
		if (mOwner == nullptr)
			throw std::runtime_error("Vector is empty!");

		if (mIndex < mOwner->mSize)
		{
			++mIndex;
		}

		return *this;
	}

	template<typename T>
	inline typename Vector<T>::Iterator Vector<T>::Iterator::operator++(int)
	{
		Iterator tmp = *this;
		++(*this);
		return tmp;
	}

	template<typename T>
	inline T & Vector<T>::Iterator::operator*() const
	{
		if (mOwner == nullptr)
			throw std::runtime_error("Vector is empty!");

		return mOwner->operator[](mIndex);
	}

#pragma endregion

#pragma region ConstIterator

	template<typename T>
	inline Vector<T>::ConstIterator::ConstIterator(const Iterator & rhs) : 
		mOwner(rhs.mOwner), mIndex(rhs.mIndex)
	{
		
	}

	template<typename T>
	inline Vector<T>::ConstIterator::ConstIterator(const Vector *owner, size_t index) :
		mOwner(owner), mIndex(index)
	{
	}

	template<typename T>
	inline bool Vector<T>::ConstIterator::operator==(const ConstIterator & rhs) const
	{
		return (!(operator!=(rhs)));
	}

	template<typename T>
	inline bool Vector<T>::ConstIterator::operator!=(const ConstIterator & rhs) const
	{
		return ((mOwner != rhs.mOwner) || (mIndex != rhs.mIndex));
	}

	template<typename T>
	inline typename Vector<T>::ConstIterator & Vector<T>::ConstIterator::operator++()
	{
		if (mOwner == nullptr)
			throw std::runtime_error("Vector is empty!");

		if (mIndex < mOwner->mSize)
		{
			++mIndex;
		}

		return *this;
	}

	template<typename T>
	inline typename Vector<T>::ConstIterator Vector<T>::ConstIterator::operator++(int)
	{
		Iterator tmp = *this;
		++(*this);
		return tmp;
	}

	template<typename T>
	inline const T & Vector<T>::ConstIterator::operator*() const
	{
		if (mOwner == nullptr)
			throw std::runtime_error("Vector is empty!");

		return mOwner->operator[](mIndex);
	}

#pragma endregion

}