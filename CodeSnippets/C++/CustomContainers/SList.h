/// <summary>
/// This is the header file of a templated-singly linked list. It initializes as an empty list meaning it contains no nodes.
/// Then methods can be used to either manipulate the list or infer information about the list. Each node can contain data 
/// and a reference to the next node if such exist. The templated version of this means that any primatives or custom objects
/// can be the declaration of SList. For example, SList of object.
/// </summary>

#pragma once
#include "pch.h"

namespace FieaGameEngine
{
	/// <summary>
	/// The templated-singly linked list class.
	/// </summary>
	template<typename T>
	class SList
	{
	private:
		struct Node
		{
			T Data;
			Node* Next;
			Node(T data, Node* next) :
				Data(data), Next(next)
			{
			}
		};

		Node* mFront = nullptr;
		Node* mBack = nullptr;
		size_t mSize = 0;

	public:
		class Iterator
		{
			friend SList;

		public:
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
			/// Initializes the Iterator class with an empty Iterator. The current node is the first position of the SList Owner. If the list
			/// is empty then the current position is set to a null position.
			/// </summary>
			Iterator(const SList *owner, Node* node = nullptr);

			const SList* mOwner = nullptr;
			Node* mNode = nullptr;
		};

		/// <summary>
		/// Initializes the SList class with an empty singly-linked list. The front and back nodes of the list are 
		/// initialized as null pointers.
		/// </summary>
		SList();
		/// <summary>
		/// The destructor that is called when the lifetime of the SList class object ends. It frees up the memory that the linked
		/// list was using at the time this object ends.
		/// </summary>
		~SList();
		/// <summary>
		/// This is the copy constructor. This initializes the SList object with values from another SList object that was already 
		/// initialized. This must take in a SList argument where the values come from in order to do the deep copy into a new
		/// initialized copied version of the list.
		/// </summary>
		/// <param name="list">The list to be deep copied into a new initialized list.</param>
		SList(const SList &list);
		/// <summary>
		/// An overloaded operator method that creates a deep-copy of the SList that is being passed into this method. The left hand
		/// side and right hand side of the operation will be already initialized linked-lists.
		/// </summary>
		/// <param name="rhs">The list to be deep copied into an already initialized list.</param>
		/// <returns>Returns a deepy-copy of a SList that is passed in as an argument.</returns>
		SList<T> &operator=(const SList<T> &rhs);
		/// <summary>
		/// This method returns the front item of the linked list. If the list is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the first item in the linked list</returns>
		T& Front();
		/// <summary>
		/// <summary>
		/// This method returns the front item of the linked list as constant. If the list is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the first item in the linked list</returns>
		const T& Front() const;
		/// <summary>
		/// This method returns the back item of the linked list. If the list is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the back item in the linked list</returns>
		T& Back();
		/// <summary>
		/// This method returns the back item of the linked list as constant. If the list is empty, it will return an object of the function's 
		/// return type with an empty initializer.
		/// </summary>
		/// <returns>Returns the back item in the linked list</returns>
		const T& Back() const;
		/// <summary>
		/// This method returns the number of nodes in the list. If the list is empty, it will return a size of zero.
		/// </summary>
		/// <returns>Returns the number of nodes in the list</returns>
		size_t Size() const;
		/// <summary>
		/// This method inserts an object of the function's type before the front of the list. This item becomes appointed as the front
		/// of the list.
		/// </summary>
		/// <param name="value">The node of function's type inserted at the front of the list</param>
		void PushFront(const T& value);
		/// <summary>
		/// This method removes an object of the function's type from the front of the list. This item no longer is appointed as the front of
		/// the list and is deleted. The next node becomes the appointed front of the list, and if none such exist then the front
		/// of the list is empty.
		/// </summary>
		void PopFront();
		/// <summary>
		/// This method removes an object of the function's type from the back of the list. This item no longer is appointed as the back of
		/// the list and is deleted. The previous node becomes the appointed back of the list, and if none such exist then the back
		/// of the list is empty.
		/// </summary>
		void PopBack();
		/// <summary>
		/// This method inserts an object of the function's type after the back of the list. This item becomes the back pointer of
		/// the list.
		/// </summary>
		/// <param name="value">The node of function's type inserted at the back of the list</param>
		void PushBack(const T& value);
		/// <summary>
		/// This method checks to see if the number of nodes in the list is zero.
		/// </summary>
		/// <returns>Returns true if there are no nodes in the list, and false if the number of nodes is greater than zero.</returns>
		bool IsEmpty() const;
		/// <summary>
		/// This method frees the memory of the linked list by deleting every node in the list and setting the size of the list to zero.
		/// </summary>
		void Clear();
		/// <summary>
		/// This method appends the given item after the item the given iterator points to.
		/// </summary>
		/// <param name="T&">The item that will append after the position the iterator points to.</param>
		void InsertAfter(const Iterator& it, const T& value);
		/// <summary>
		/// This method finds an iterator pointing to a given item.
		/// </summary>
		/// <param name="T&">The item the method will search for in the SList.</param>
		/// <returns>Returns an iterator pointing to the given item.</returns>
		Iterator Find(const T& value) const;
		/// <summary>
		/// This method removes the item associated with the given data and maintain list integrity.
		/// </summary>
		/// <param name="T&">The item the method will search for to remove.</param>
		void Remove(const T& value);
		/// <summary>
		/// This method returns an iterator pointer to the head of the list.
		/// </summary>
		/// <returns>Returns an iterator pointing to the head of the list</returns>
		Iterator begin();
		/// <summary>
		/// This method returns an iterator pointer to the tail of the list.
		/// </summary>
		/// <returns>Returns an iterator pointing to the tail of the list</returns>
		Iterator end();
		/// <summary>
		/// This method returns an iterator pointer to the head of the list as constant.
		/// </summary>
		/// <returns>Returns an iterator pointing to the head of the list</returns>
		Iterator begin() const;
		/// <summary>
		/// This method returns an iterator pointer to the tail of the list as constant.
		/// </summary>
		/// <returns>Returns an iterator pointing to the tail of the list</returns>
		Iterator end() const;
	};
}

#include "SList.inl"