# Gforth-Objects
Gforth tools written with objects.fs dependency.  Can be used where Gforth is used!

## stringobj.fs
This is a string and a strings object.  String object will create
and manipulate a string.  Strings object will create and manipulate an array of string.
Strings object uses string object internally.  All memory is managed
and the heap is used.  Some method names are similar to the Gforth's internal string
words but are not the same as them so should not give compiler errors nor does this
object use the internal string words!

## objects.fs
This is the dependency for the code in this repo.  It is a copy of the file included with
Gforth.  I include it here so non Gforth systems can use these objects.  Objects.fs uses
standard forth words so should work on other systems.  I have not modified this file in any way.
The file is from the current version of Gforth 0.7.9_20160923

## double-linked-list.fs
This is a basic double linked list object that can take a payload of memory.  The link list along with the memory payload
is allocated on the heap.  Inherit this object to add more methods or just use it to store some dynamic sized memory items!
Stringobj.fs does not use this at this moment but i may convert strings to use this link list in the future!

## object-struct-helper.fs
This is an object to make working with structures easier!  
The object is called struct-base and all it does is allocate memory at time of creation with construct member
The idea is to inherit from this class to make a specific structure with the retrieve and store words for the specific structure.
