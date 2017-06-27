\    Copyright (C) 2016 2017  Philip K. Smith
\    This program is free software: you can redistribute it and/or modify
\    it under the terms of the GNU General Public License as published by
\    the Free Software Foundation, either version 3 of the License, or
\    (at your option) any later version.

\    This program is distributed in the hope that it will be useful,
\    but WITHOUT ANY WARRANTY; without even the implied warranty of
\    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\    GNU General Public License for more details.

\    You should have received a copy of the GNU General Public License
\    along with this program.  If not, see <http://www.gnu.org/licenses/>.
\
\ This code is for string handling.   There are two objects.
\ Object string is for single string and all memory is managed by object.
\ Object strings uses object string to handle as many stings of any size you want!
\ You can think of string as a container for a string and strings is a container for
\ any collection of strings you want.

require ./objects.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
  cell% inst-var string-addr
  cell% inst-var string-size
  m: ( string -- ) \ initalize the string but this will leak memory if string does exist.  Use destruct method to deallocate memory first.
    0 string-addr !
    0 string-size ! ;m overrides construct
  m: ( string -- ) \ free allocated memory
    string-addr @ 0> if string-addr @ free throw then
    this [current] construct ;m overrides destruct
  m: ( caddr u string -- ) \ store string
    dup 0>
    if
      dup allocate throw { u caddr1 }
      caddr1 u move
      string-addr @ 0>
      if
        string-addr @ free throw
        0 string-addr !
      then
      caddr1 string-addr !
      u string-size !
    else 2drop this [current] destruct then ;m method !$
  m: ( string -- caddr u ) \ retrieve string
    string-addr @ 0>
    if
       string-addr @ string-size @
    else 0 0
    then ;m method @$
  m: ( caddr u string -- ) \ add a string to this string at end of!
    string-addr @ 0>
    if \ resize
       dup 0>
       if
          dup string-size @ + string-addr @ swap resize throw
          dup string-addr ! string-size @ + swap dup string-size @ + string-size ! move
       else 2drop
       then
    else \ allows a new string to be created if no string currently present
       this [current] !$
    then ;m method !+$
  m: ( caddr u string -- ) \ add string to begining of this string
    string-addr @ 0>
    if \ resize
       dup 0>
       if
          dup string-size @ + string-addr @ swap resize throw
          string-addr !
          dup string-addr @ + string-addr @ swap string-size @ move
          dup string-size @ + string-size !
          string-addr @ swap move
       else 2drop
       then
    else \ make new string
       this [current] !$
    then ;m method !<+$
  m: ( caddr u string -- caddr1 u1 caddr2 u2 nflag ) \ split this string object stored string by caddr u if caddr u is found in stored string
  \ caddr1 u1 will be the split string before caddr u if caddr u is found in stored string
  \ caddr2 u2 will be the split string after caddr u if caddr us is found in stored string
  \ if no match found caddr1 u1 returns an empty string and caddr2 u2 contains this objects string
  \ nflag is true if string is split and false if this objects string is returned without being split
  \ Note the returned strings are valid until a new string is placed in this string object
  \ This string object does not get changed in any way because of this operation!
  	{ caddr u }
  	string-addr @ string-size @ caddr u search true =
  	if
       dup string-size @ swap - string-addr @ swap 2swap u - swap u + swap true
  	else
       2drop 0 0 string-addr @ string-size @ false
  	then ;m method split$
  m: ( string -- u ) \ report string size
    string-size @ ;m method len$
  m: ( string -- ) \ retrieve string object info
    this [parent] print
    s"  addr:" type string-addr @ .
    s"  size:" type string-size @ .
    s"  string:" type string-addr @ string-size @ type ;m overrides print
end-class string

object class
  destruction implementation
  cell% inst-var array \ contains first address of allocated string object
  cell% inst-var qty
  cell% inst-var index
  m: ( strings -- ) \ initalize strings object.  Note this will cause memory leaks strings are present when executed.
    \ use method destruct to deallocate memory!
       0 qty !
       0 index !
       0 array !
  ;m overrides construct
  m: ( strings -- ) \ free allocated memory
    array @ 0>
    if \ deallocate memory allocated for the array and free the string objects
       qty @ 0 ?do array @ i cell * + @ dup [bind] string destruct free throw loop
       array @ free throw
    then
    this [current] construct ;m overrides destruct
  m: ( caddr u strings -- ) \ store a string in handler
    array @ 0=
    if
       cell allocate throw array !
       1 qty !
       string heap-new dup array @ ! [bind] string !$
    else
       array @ cell qty @ 1 + * resize throw dup array !
       cell qty @ * + dup
       string heap-new swap ! @ [bind] string !$
       qty @ 1+ qty !
    then 0 index ! ;m method !$x
  m: ( strings -- caddr u ) \ retrieve string from array at next index
    qty @ 0>
    if
       array @ index @ cell * + @ [bind] string @$
       index @ 1+ index !
       index @ qty @ >=
       if 0 index ! then
    else 0 0 then ;m method @$x
  m: ( nindex strings -- caddr u nflag ) \ retrieve nindex string from strings array
  \ caddr u contain the string if nflag is false
  \ if nflag is true caddr u do not contain nindex string
    abs dup qty @ <
    qty @ 0> and
    if
       array @ swap cell * + @ [bind] string @$ false
    else drop 0 0 true
    then ;m method []@$
  m: ( caddr u strings -- caddr1 u1 caddr2 u2 ) \ retrieve string from this strings object array at next index then
  \ split that next string at caddr u if possible and removing caddr u in the process
  \ caddr1 u1 is the string found before caddr u and could be a zero size string
  \ caddr1 u1 will be a zero size string if caddr u is not found
  \ caddr2 u2 contains the left over string if caddr u string is found and removed or it will simply be the full string from this string object array
    dup 0> qty @ 0> and true =
    if
       array @ index @ cell * + @ [bind] string split$ drop
       index @ 1+ index !
       index @ qty @ >=
       if 0 index ! then
    else 2drop 0 0 0 0 then ;m method split$s
  m: ( nstring-split$ nstring-source$ strings -- ) \ split up nstring-source$ with nstring-split$
  \ nstring-source$ is split each time nstring-split$ is found and placed in this strings
  \ nstring-split$ is removed each time it is found and when no more are found process is done
  \ Note nstring-source$ string will contain the last split string but that string it is also placed in this strings object
  \ Note null strings or strings of zero size could be found and placed in this strings object during spliting process
    { sp$ src$ }
    sp$ [bind] string len$ 0> src$ [bind] string len$ 0> and true =
    if
       begin
          sp$ [bind] string @$ src$ [bind] string split$ true =
          if 2swap this [current] !$x src$ [bind] string !$ false else this [current] !$x 2drop true then
       until
    then ;m method split$to$s
  m: ( ncaddrfd u ncaddrsrc u1 strings -- ) \ split up ncaddrsrc u1 string with ncaddrfd u string
  \ same as split$to$s method but temporary strings are passed to this code
  \ ncaddrfd u is the string to find
  \ ncaddrsrc u1 is the string to find ncaddrfd u string in
  \ Note null strings or strings of zero size could be found and placed in this strings object during spliting process
  	string heap-new string heap-new { fd$ src$ }
  	src$ [bind] string !$ fd$ [bind] string !$
  	fd$ src$ this [current] split$to$s
  	fd$ [bind] string destruct src$ [bind] string destruct ;m method split$>$s
  m: ( strings -- u ) \ report size of strings array
    array @ 0>
    if qty @ else 0 then ;m method $qty
  m: ( strings -- ) \ reset index to start of strings list for output purposes
    0 index ! ;m method reset
  m: ( nstrings strings -- ) \ copy strings object to this strings object
    dup [current] reset
    dup [current] $qty 0 ?do dup [current] @$x this [current] !$x loop drop ;m method copy$s
  m: ( strings -- ) \ print object for debugging
    this [parent] print
    s" array:" type array @ .
    s" size:" type qty @ .
    s" iterate index:" type index @ . ;m overrides print
end-class strings
\ ***************************************************************************************************************************

\ \\\
string heap-new constant test$a
string heap-new constant test$b
strings heap-new constant test$s

s" x" test$a !$
s" x1x2x" test$b !$

test$a test$b test$s split$to$s
cr
test$s $qty . cr
test$s @$x .s dump
test$s @$x .s dump
test$s @$x .s dump
test$s @$x .s dump
test$s @$x .s dump
cr
test$b @$ .s  dump
cr
test$s bind strings destruct
s" x" s" xx1x2x" test$s split$>$s
test$s $qty . cr
test$s @$x .s dump
test$s @$x .s dump
test$s @$x .s dump
test$s @$x .s dump
test$s @$x .s dump

test$s bind strings destruct
cr
s" 1x2" test$s !$x
s" x" test$s split$s .s cr
dump
dump
