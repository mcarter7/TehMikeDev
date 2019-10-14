/// <summary>
/// This is the header file of a templated hashmap. The purpose of this class is to
/// take a value and pass it through a hash function to store into an array, which then
/// is paired with a key. These keys are used to be able to relocated the correct corresponding
/// values.
/// </summary>

#pragma once
#include "SList.h"
#include "Vector.h"
#include "DefaultHash.h"

namespace FieaGameEngine
{
	template <typename TKey, typename TData, typename FUNC = DefaultHash<TKey>>
	class HashMap final
	{
	public:
		using PairType = std::pair<const TKey, TData>;
		using ChainType = SList<PairType>;
		using BucketType = Vector<ChainType>;

		class Iterator final
		{
			friend HashMap;
			friend class ConstIterator;

		public:
			/// <summary>
			/// Initializes the Iterator class with an empty Iterator.
			/// </summary>
			Iterator() = default;
			/// <summary>
			/// The destructor that is called when the lifetime of the Iterator class object ends. It frees up the memory that the
			/// Iterator was using.
			/// </summary>
			~Iterator() = default;
			/// <summary>
			/// This is the copy constructor. This initializes the Iterator object with another Iterator object that was already 
			/// initialized. This must take in a Vector argument in order to do the deep copy into a new initialized copied version.
			/// </summary>
			/// <param name="iterator">The iterator that will be deep copied into another initialized Iterator object</param>
			Iterator(const Iterator &iterator) = default;
			/// <summary>
			/// An overloaded assignment operator method that creates a deep-copy of the Iterator that is being passed into this method.
			/// The left hand side and right hand side of the operation will be already initialized Iterators.
			/// </summary>
			/// <param name="rhs">The Iterator to be deep copied into an already initialized Iterator.</param>
			/// <returns>Returns a deepy-copy of a Iterator that is passed in as an argument.</returns>
			Iterator &operator=(const Iterator &rhs) = default;
			/// <summary>
			/// An overloaded operator method that checks the equality of two iterators.
			/// </summary>
			/// <param name="iterator">The right hand side of the equality check.</param>
			/// <returns>Returns true if the two iterators are equal, and false if they are not equal.</returns>
			bool operator==(const Iterator& rhs) const;
			/// <summary>
			/// An overloaded operator method that checks if two iterators are not equal
			/// </summary>
			/// <param name="iterator">The right hand side of the not equal check.</param>
			/// <returns>Returns true if the two iterators are not equal, and false if they are equal.</returns>
			bool operator!=(const Iterator& rhs) const;
			/// <summary>
			/// An overloaded operator method that pre-increments an iterator to the next available position.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the pre-increment of the interator</returns>
			Iterator& operator++();
			/// <summary>
			/// An overloaded operator method that post-increments an iterator to the next available position.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the post-increment of the interator</returns>
			Iterator operator++(int);
			/// <summary>
			/// An overloaded operator method that dereferences the item contained by the node the iterator points to.
			/// </summary>
			/// <param name="T&">The right hand side of the dereference.</param>
			/// <returns>Returns the dereferenced value</returns>
			PairType& operator*() const;
			/// <summary>
			/// An overloaded operator method that dereferences the item contained by the node the iterator points to.
			/// </summary>
			/// <param name="T&">The right hand side of the dereference.</param>
			/// <returns>Returns the dereferenced value</returns>
			PairType* operator->() const;

		private:
			/// <summary>
			/// Initializes the Iterator class with an owner and key.
			/// </summary>
			Iterator(HashMap *owner, size_t index, typename const SList<PairType>::Iterator &sListIt);

			HashMap* mOwner = nullptr;
			size_t mIndex;
			typename SList<PairType>::Iterator mSListIt;

		};

		class ConstIterator final
		{

		public:
			/// <summary>
			/// Initializes the Iterator class with an empty Iterator.
			/// </summary>
			ConstIterator() = default;
			/// <summary>
			/// The destructor that is called when the lifetime of the Iterator class object ends. It frees up the memory that the Iterator
			/// was using.
			/// </summary>
			~ConstIterator() = default;
			/// <summary>
			/// This is the copy constructor. This initializes the Iterator object with another Iterator object that was already 
			/// initialized. This must take in a Vector argument in order to do the deep copy into a new initialized copied version.
			/// </summary>
			/// <param name="iterator">The iterator that will be deep copied into another initialized Iterator object</param>
			ConstIterator(const ConstIterator &iterator) = default;
			/// <summary>
			/// This is the copy constructor. This initializes the Iterator object with another Iterator object that was already 
			/// initialized. This must take in a Vector argument in order to do the deep copy into a new initialized copied version.
			/// </summary>
			/// <param name="iterator">The iterator that will be deep copied into another initialized Iterator object</param>
			ConstIterator(const Iterator &iterator);
			/// <summary>
			/// An overloaded assignment operator method that creates a deep-copy of the Iterator that is being passed into this method. The left hand
			/// side and right hand side of the operation will be already initialized Iterators.
			/// </summary>
			/// <param name="rhs">The Iterator to be deep copied into an already initialized Iterator.</param>
			/// <returns>Returns a deepy-copy of a Iterator that is passed in as an argument.</returns>
			ConstIterator &operator=(const ConstIterator &rhs) = default;
			/// <summary>
			/// An overloaded operator method that checks the equality of two iterators.
			/// </summary>
			/// <param name="iterator">The right hand side of the equality check.</param>
			/// <returns>Returns true if the two iterators are equal, and false if they are not equal.</returns>
			bool operator==(const ConstIterator& rhs) const;
			/// <summary>
			/// An overloaded operator method that checks if two iterators are not equal
			/// </summary>
			/// <param name="iterator">The right hand side of the not equal check.</param>
			/// <returns>Returns true if the two iterators are not equal, and false if they are equal.</returns>
			bool operator!=(const ConstIterator& rhs) const;
			/// <summary>
			/// An overloaded operator method that pre-increments an iterator to the next available position.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the pre-increment of the interator</returns>
			ConstIterator& operator++();
			/// <summary>
			/// An overloaded operator method that post-increments an iterator to the next available position.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the post-increment of the interator</returns>
			ConstIterator operator++(int);
			/// <summary>
			/// An overloaded operator method that dereferences the item contained by the node the iterator points to.
			/// </summary>
			/// <param name="T&">The right hand side of the dereference.</param>
			/// <returns>Returns the dereferenced value</returns>
			const PairType& operator*() const;
			/// <summary>
			/// An overloaded operator method that dereferences the item contained by the node the iterator points to.
			/// </summary>
			/// <param name="T&">The right hand side of the dereference.</param>
			/// <returns>Returns the dereferenced value</returns>
			const PairType& operator->() const;

		private:
			/// <summary>
			/// Initializes the Iterator class with an owner and key.
			/// </summary>
			ConstIterator(const HashMap *owner, size_t index, typename const SList<PairType>::Iterator sListIt);

			const HashMap* mOwner = nullptr;
			size_t mIndex;
			typename SList<PairType>::Iterator mSListIt;

		};

		/// <summary>
		/// Initializes the HasMap class with an unsigned integer which is the
		/// size of the hash table array.
		/// </summary>
		/// <param name="bucketSize">The size of the hash table array.</param>
		HashMap(size_t bucketSize = 11);
		/// <summary>
		/// The destructor that is called when the lifetime of the Vector class object ends. It frees up
		/// the memory that the hash table was using at the time this object ends.
		/// </summary>
		~HashMap() = default;
		/// <summary>
		/// This is the copy constructor. This initializes the Vector object with values from another SList object that was already 
		/// initialized. This must take in a Vector argument where the values come from in order to do the deep copy into a new
		/// initialized copied version of the list.
		/// </summary>
		/// <param name="list">The list to be deep copied into a new initialized list.</param>
		HashMap(std::initializer_list<PairType> list);
		/// <summary>
		/// Find method that takes a key value and finds the matching node. The node is returned as an
		/// iterator.
		/// </summary>
		/// <param name="TKey">The key value to match to the correct node</param>
		/// <returns>The matching node the key belongs to.</returns>
		Iterator Find(const TKey &key);
		/// <summary>
		/// Find method that takes a key value and finds the matching node. The node is returned as an
		/// iterator.
		/// </summary>
		/// <param name="TKey">The key value to match to the correct node</param>
		/// <returns>The matching node the key belongs to.</returns>
		//const Iterator Find(const TKey &key) const;
		/// <summary>
		/// Insert method that takes an entry pair to be hashed into the hash table array. This will return 
		/// an iterator. If such node already exist, then it will return the same node.
		/// </summary>
		/// <param name="entry">The pair of key and value to be inserted into the hash table.</param>
		/// <returns>The pair entry as an iterator.</returns>
		Iterator Insert(const PairType &entry);
		/// <summary>
		/// Insert method that takes an entry pair to be hashed into the hash table array. This will return 
		/// an iterator. If such node already exist, then it will return the same node.
		/// </summary>
		/// <param name="entry">The pair of key and value to be inserted into the hash table.</param>
		/// <returns>The pair entry as an iterator.</returns>
		Iterator Insert(const PairType &entry, bool& entryCreated);
		/// <summary>
		/// This overload method takes in a given index and returns the value by reference. If the index is invalid, then it will
		/// create a new entry pair in the hash table.
		/// </summary>
		/// <param name="key">The given index that will align with an item location</param>
		/// <returns>The value by reference</returns>
		TData& operator[](const TKey &key);
		/// <summary>
		/// Removed method takes a key argument and removes the matching entry to a pair. If the pair does not exist, this method
		/// will do nothing.
		/// </summary>
		/// <param name="key">The key that will be used to check for matching to the pair</param>
		void Remove(const TKey &key);
		/// <summary>
		/// Empty method empties the hash table and deletes any memory allocated
		/// </summary>
		void Clear();
		/// <summary>
		/// Size method reports the population of the table as an unsigned integer
		/// </summary>
		/// <returns>Number of nodes in the table</returns>
		size_t Size() const;
		/// <summary>
		/// This method returns true if the key exist in the table, and false if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool ContainsKey(const TKey &key);
		/// <summary>
		/// This method returns true if the key exist in the table, and false if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool ContainsKey(const TKey &key, Iterator& it);
		/// <summary>
		/// This method returns true if the key exist in the table, and false if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool ContainsKey(const TKey &key, ConstIterator& it) const;
		/// <summary>
		/// Takes a key argument of the appropriate type and which returns a reference to the TData part
		/// </summary>
		/// <param name="key">Key to be used to return data</param>
		/// <returns>Returns TData</returns>
		TData& At(const TKey &key);
		/// <summary>
		/// Takes a key argument of the appropriate type and which returns a reference to the TData part
		/// </summary>
		/// <param name="key">Key to be used to return data</param>
		/// <returns>Returns TData</returns>
		const TData& At(const TKey &key) const;
		/// <summary>
		/// This method returns an iterator pointer to the head of the list.
		/// </summary>
		/// <returns>Returns an iterator pointing to the head of the list.</returns>
		Iterator begin();
		/// <summary>
		/// This method returns an iterator pointer to the head of the list that is constant.
		/// </summary>
		/// <returns>Returns an iterator pointing to the head of the list that is constant</returns>
		ConstIterator begin() const;
		/// <summary>
		/// This method returns an iterator pointer to the end of the list thats out of range.
		/// </summary>
		/// <returns>Returns an iterator pointing to the end of the list</returns>
		Iterator end();
		/// <summary>
		/// This method returns an iterator pointer to the end of the list that is constant.
		/// </summary>
		/// <returns>Returns an iterator pointing to the end of the list that is constant and thats out of range</returns>
		ConstIterator end() const;
		/// <summary>
		/// This method returns an iterator pointer to the head of the list that is constant. This helps
		/// the user explicitly call this method on a non-const vector object.
		/// </summary>
		/// <returns>Returns an iterator pointing to the head of the list that is constant.</returns>
		ConstIterator cbegin() const;
		/// <summary>
		/// This method returns an iterator pointer to the end of the list that is constant. This helps
		/// the user explicitly call this method on a non-const vector object
		/// </summary>
		/// <returns>Returns an iterator pointing to the end of the list that is constant.</returns>
		ConstIterator cend() const;

	private:
		size_t mPopulation = 0;
		FUNC HashFunctor;
		BucketType mBuckets;
	};
}

#include "Hashmap.inl"