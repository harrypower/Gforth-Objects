# Gforth-Objects
Gforth tools written with objects.fs dependancy.  Can be used where Gforth is used!

stringobj.fs  This is a string and a strings object.  String object will create
and manipulate a string.  Strings object will create and manipulate an array of string.
Strings object uses string object internally.  All memory is managed
and the heap is used.  Some method names are similar to the Gforth's internal string
words but are not the same as them so should not give compiler errors nor does this
object use the internal string words!
