/// <summary>
/// This is the header file of a templated-singly Vector. It initializes as an empty list meaning it contains items.
/// Then methods can be used to either manipulate the list or infer information about the list. The templated version 
/// of this means that any primitives or custom objects can be the declaration of Vector.
/// </summary>

#pragma once
#include "pch.h"

namespace FieaGameEngine
{
	template <typename T>
	class Vector final
	{
	public:
		class Iterator final
		{
			friend Vector;
			friend class ConstIterator;

		public:
			using size_type = std::size_t;
			using value_type = T;
			using pointer = T * ;
			using reference = T & ;
			using difference_type = std::ptrdiff_t;
			using iterator_category = std::forward_iterator_tag;

			/// <summary>
			/// Initializes the Iterator class with an empty Iterator.
			/// </summary>
			Iterator() = default;
			/// <summary>
			/// The destructor that is called when the lifetime of the Iterator class object ends. It frees up the memory that the Iterator
			/// was using.
			/// </summary>
			~Iterator() = default;
			/// <summary>
			/// This is the copy constructor. This initializes the Iterator object with another Iterator object that was already 
			/// initialized. This must take in a Vector argument in order to do the deep copy into a new initialized copied version.
			/// </summary>
			/// <param name="iterator">The iterator that will be deep copied into another initialized Iterator object</param>
			Iterator(const Iterator &iterator) = default;
			/// <summary>
			/// An overloaded operator method that creates a deep-copy of the Iterator that is being passed into this method. The left hand
			/// side and right hand side of the operation will be already initialized Iterators.
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
			/// An overloaded operator method that pre-increments an interator to the next positon.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the pre-increment of the interator</returns>
			Iterator& operator++();
			/// <summary>
			/// An overloaded operator method that post-increments an interator to the next positon.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the post-increment of the interator</returns>
			Iterator operator++(int);
			/// <summary>
			/// An overloaded operator method that dereferences the item contained by the node the iterator points to.
			/// </summary>
			/// <param name="T&">The right hand side of the dereference.</param>
			/// <returns>Returns the dereferenced value</returns>
			T& operator*() const;

		private:
			/// <summary>
			/// Initializes the Iterator class with an owner and position.
			/// </summary>
			Iterator(Vector *owner, size_t index);

			Vector* mOwner = nullptr;
			size_t mIndex;

		};

		class ConstIterator final
		{
			friend Vector;

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
			/// Type cast contructor from an iterator
			/// </summary>
			/// <param name="iterator">Iterator to type cast</param>
			ConstIterator(const Iterator &iterator);
			/// <summary>
			/// This is the copy constructor. This initializes the Iterator object with another Iterator object that was already 
			/// initialized. This must take in a Vector argument in order to do the deep copy into a new initialized copied version.
			/// </summary>
			/// <param name="iterator">The iterator that will be deep copied into another initialized Iterator object</param>
			ConstIterator(const ConstIterator &iterator) = default;
			/// <summary>
			/// An overloaded operator method that creates a deep-copy of the Iterator that is being passed into this method. The left hand
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
			/// An overloaded operator method that pre-increments an interator to the next positon.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the pre-increment of the interator</returns>
			ConstIterator& operator++();
			/// <summary>
			/// An overloaded operator method that post-increments an interator to the next positon.
			/// </summary>
			/// <param name="iterator">The right hand side of the increment.</param>
			/// <returns>Returns the post-increment of the interator</returns>
			ConstIterator operator++(int);
			/// <summary>
			/// An overloaded operator method that dereferences the item contained by the node the iterator points to.
			/// </summary>
			/// <param name="T&">The right hand side of the dereference.</param>
			/// <returns>Returns the dereferenced value as a constant</returns>
			const T& operator*() const;

		private:
			/// <summary>
			/// Initializes the ConstIterator class with an owner, data and position.
			/// </summary>
			ConstIterator(const Vector *owner, size_t index);

			const Vector* mOwner = nullptr;
			size_t mIndex;
		};

		using value_type = T;
		using size_type = size_t;
		using difference_type = std::ptrdiff_t;
		using reference = T & ;
		using const_reference = const T&;
		using iterator = Iterator;
		using const_iterator = ConstIterator;

		/// <summary>
		/// Initializes the Vector class with an empty vector. The capacity and size are defaulted to zero.
		/// </summary>
		Vector();
		/// <summary>
		/// The destructor that is called when the lifetime of the Vector class object ends. It frees up the memory that the Vector
		/// was using at the time this object ends.
		/// </summary>
		~Vector();
		/// <summary>
		/// This is the copy constructor. This initializes the Vector object with values from another SList object that was already 
		/// initialized. This must take in a Vector argument where the values come from in order to do the deep copy into a new
		/// initialized copied version of the list.
		/// </summary>
		/// <param name="list">The list to be deep copied into a new initialized list.</param>
		Vector(const Vector &rhs);
		/// <summary>
		/// Compares two vectors together, returns true if they are equal and false if they are not equal.
		/// </summary>
		/// <param name="rhs">Vector for comparison</param>
		/// <returns>True if they are equal, and false if they are not equal</returns>
		bool operator==(const Vector& rhs) const;
		/// <summary>
		/// Compares two vectors together, returns true if they are equal and false if they are not equal.
		/// </summary>
		/// <param name="rhs">Vector for comparison</param>
		/// <returns>True if they are equal, and false if they are not equal</returns>
		bool operator==(const Vector& rhs);
		/// <summary>
		/// Compares two vectors together, returns true if they are equal and false if they are not equal.
		/// </summary>
		/// <param name="rhs">Vector for comparison</param>
		/// <returns>True if they are equal, and false if they are not equal</returns>
		bool operator!=(const Vector& rhs) const;
		/// <summary>
		/// Compares two vectors together, returns true if they are equal and false if they are not equal.
		/// </summary>
		/// <param name="rhs">Vector for comparison</param>
		/// <returns>True if they are equal, and false if they are not equal</returns>
		bool operator!=(const Vector& rhs);

		/// <summary>
		/// This is the copy constructor. This initializes the Vector object with values from another SList object that was already 
		/// initialized. This must take in a Vector argument where the values come from in order to do the deep copy into a new
		/// initialized copied version of the list.
		/// </summary>
		/// <param name="list">The list to be deep copied into a new initialized list.</param>
		Vector(const std::initializer_list<T>& rhs);
		/// <summary>
		/// This is the copy constructor with copy semantics. This initializes the Vector object with values from another SList object 
		/// that was already  initialized. This must take in a Vector argument where the values come from in order to do the deep copy 
		/// into a new initialized copied version of the list.
		/// </summary>
		/// <param name="list">The list to be deep copied into a new initialized list.</param>
		Vector(Vector &&rhs);
		/// <summary>
		/// This overload method takes in a given index and returns the item by reference. If the index is invalid, this will throw an 
		/// exception.
		/// </summary>
		/// <param name="i">The given index that will align with an item location</param>
		/// <returns>The item by reference</returns>
		T& operator[](size_t i);
		/// <summary>
		/// This overload method takes in a given index and returns the constant item by reference. If the index is invalid, this will throw an 
		/// exception.
		/// </summary>
		/// <param name="i">The given index that will align with an item location</param>
		/// <returns>The item by reference as a constant</returns>
		const T& operator[](size_t i) const;
		/// <summary>
		/// An overloaded operator method that creates a deep-copy of the Vector that is being passed into this method. The left hand
		/// side and right hand side of the operation will be already initialized Vectors.
		/// </summary>
		/// <param name="rhs">The list to be deep copied into an already initialized list.</param>
		/// <returns>Returns a deepy-copy of a Vector that is passed in as an argument.</returns>
		Vector<T> &operator=(const Vector<T> &rhs);
		/// <summary>
		/// An overloaded operator method that creates a deep-copy of the Vector that is being passed into this method. The left hand
		/// side and right hand side of the operation will be already initialized Vectors.
		/// </summary>
		/// <param name="rhs">The list to be deep copied into an already initialized list.</param>
		/// <returns>Returns a deepy-copy of a Vector that is passed in as an argument.</returns>
		Vector<T> &operator=(Vector<T> &&rhs);
		/// <summary>
		/// This method takes in a given index and returns the item by reference. If the index is invalid, this will throw an 
		/// exception.
		/// </summary>
		/// <param name="i">The given index that will align with an item location</param>
		/// <returns>The item by reference</returns>
		T& At(size_t i);
		/// <summary>
		/// This method takes in a given index and returns the constant item by reference. If the index is invalid, this will throw an 
		/// exception.
		/// </summary>
		/// <param name="i">The given index that will align with an item location</param>
		/// <returns>The item by reference as a constant</returns>
		const T& At(size_t i) const;
		/// <summary>
		/// This method removes an object of the function's type from the back of the list. This item no longer is appointed as the back of
		/// the list and is deleted. This does not reduce the capacity of the container.
		/// </summary>
		void PopBack();
		/// <summary>
		/// This method inserts an object of the function's type after the back of the list. This will increase array capacity if necessary.
		/// </summary>
		/// <param name="value">The value of function's type inserted at the back of the list</param>
		void PushBack(const T& value);
		void PushBack(const T * value);
		/// <summary>
		/// This method inserts an object of the function's type after the back of the list. This will increase array capacity if necessary.
		/// </summary>
		/// <param name="value">The value of function's type inserted at the back of the list</param>
		void PushBack(T && value);
		/// <summary>
		/// This method checks to see if the number of items in the list is zero.
		/// </summary>
		/// <returns>Returns true if there are no items in the list, and false if the number of items is greater than zero.</returns>
		bool IsEmpty() const;
		/// <summary>
		/// This method returns the front item of the vector. If the vector is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the first item in the vector</returns>
		T& Front();
		/// <summary>
		/// This method returns the front item of the vector as a constant. If the vector is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the first item in the vector as a constant</returns>
		const T& Front() const;
		/// <summary>
		/// This method returns the back item of the vector. If the vector is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the back item in the vector</returns>
		T& Back();
		/// <summary>
		/// This method returns the back item of the vector as a constant. If the vector is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the back item in the vector as a constant</returns>
		const T& Back() const;
		/// <summary>
		/// Returns an unsigned integer indicating the number of items in the container
		/// </summary>
		size_t Size() const;
		/// <summary>
		/// Returns an unsigned integer indicating the number of items allocated within the container
		/// </summary>
		size_t Capacity() const; 
		/// <summary>
		/// This method returns an iterator pointer to the head of the list.
		/// </summary>
		/// <returns>Returns an iterator pointing to the head of the list</returns>
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
		/// <summary>
		/// Takes an unsigned integer indicating the capacity to reserve for the array. This will not shrink
		/// the array if there is already capacity allocated, but the array can expand.
		/// </summary>
		/// <param name="capacity">Unsigned integer indicating the capacity to reserve for the array</param>
		void Reserve(size_t capacity);
		/// <summary>
		/// This method finds an iterator pointing to a given item.
		/// </summary>
		/// <param name="T&">The item the method will search for in the Vector.</param>
		/// <returns>Returns an iterator pointing to the given item.</returns>
		Iterator Find(T& value);
		/// <summary>
		/// This method finds an iterator pointing to a given item.
		/// </summary>
		/// <param name="T&">The item the method will search for in the Vector.</param>
		/// <returns>Returns an iterator pointing to the given item.</returns>
		const Iterator Find(T& value) const;
		/// <summary>
		/// This method finds an iterator pointing to a given item.
		/// </summary>
		/// <param name="T&">The item the method will search for in the Vector.</param>
		/// <returns>Returns an iterator pointing to the given item.</returns>
		Iterator Find(const T& value);
		/// <summary>
		/// This method empties the container
		/// </summary>
		void Clear();
		/// <summary>
		/// This method removes an element in the list.
		/// </summary>
		/// <param name="value">The given value to remove</param>
		void Remove(T& value);
		/// <summary>
		/// This method removes an element in the list.
		/// </summary>
		/// <param name="value">The given value to remove</param>
		void Remove(const T& value);
		/// <summary>
		/// This method removes an element in the list.
		/// </summary>
		/// <param name="value">The given value to remove</param>
		void Remove(const Vector::Iterator & it);
		/// <summary>
		/// This method removes a range of contiguous elements.
		/// </summary>
		/// <param name="first">An iterator for the starting point of the range.</param>
		/// <param name="last">An iterator for the ending point of the range.</param>
		void Remove(Iterator first, Iterator last); //const argus
		/// <summary>
		/// An increment functor that allows the end-user to supply their own reserve strategy.
		/// </summary>
		/// <param name="size">The size the user would like to specify for the vector.</param>
		/// <param name="capacity">The capacity the user would like to specify for the vector.</param>
		/// <returns>The function object for the specified reserve strategy.</returns>
		std::uint32_t operator()(std::uint32_t size, std::uint32_t capacity) const;

	private:

		size_t mCapacity = 0;
		size_t mSize = 0;
		size_t mDefaultCapacity = 1;
		T* mData = nullptr;
	};
}

#include "Vector.inl"