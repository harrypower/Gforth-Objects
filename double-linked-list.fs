\    Copyright (C) 2017  Philip K. Smith
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
\ This is simply a double linked list to inherite from for this type of container.

require ./objects.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
  selector ll@
  cell% inst-var size-link
  cell% inst-var first-link
  cell% inst-var last-link
  cell% inst-var current-link
  cell% inst-var dll-test?
  protected
  struct
    cell% field next-forward-link
    cell% field next-back-link
    cell% field payload-size  \ note node payload data size limit to cell quantity
    cell% field node-payload
  end-struct link-links%
  public
  m: ( -- ) \ constructor
    dll-test? dll-test? @ <> if
      0 size-link !
      0 first-link !
      0 last-link !
      0 current-link !
      dll-test? dll-test? ! \ ensure future calls know constructor has allocated stuff
    else
      this destruct
      0 dll-test? ! \ dll memory is freed so reset the test!
    then
  ;m overrides construct
  m: ( -- ) \ destructor
    dll-test? dll-test? @ = if
      0 size-link @ = if exitm then  \ nothing to deallocate
      \ *** code to deallocate through the linked list nodes here ***
    then
  ;m overrides destruct
  m: ( -- ) \ print info
    cr size-link @ u. ." link list size" cr
    first-link @ u. ." start node's address" cr
    last-link @ u. ." the last node's address" cr
    current-link @ u. ." current node's address" cr
    dll-test? @ dll-test? = if ." construct done!" cr else ." construct not done!" cr then
    size-link @ 0 > dll-test? @ dll-test? = and true =
    if
      current-link @ next-back-link @ u. ." current node's back link address" cr
      current-link @ next-forward-link @ u. ." current node's forward link address" cr
      ." current node data dump:" cr
      this ll@ dump cr
    then
  ;m overrides print
  m: { caddr u -- } \ add to link list a node at the end and update all the link list node data
    \ caddr  is address of data to add to this node
    \ u is the quantity of bytes to add to this node
    size-link @ 0 = if
      link-links% %size u + allocate throw
      dup first-link !
      dup last-link !
      dup current-link !
      dup dup next-back-link !
      dup dup next-forward-link !
      dup caddr swap node-payload u move
      payload-size u swap !
      1 size-link !
    else
      link-links% %size u + allocate throw
      dup last-link @ next-forward-link !
      dup last-link @ swap next-back-link !
      dup last-link !
      dup caddr swap node-payload u move
      payload-size u swap !
      size-link @ 1+ size-link !
    then
  ;m method ll!
  m: ( -- caddr u ) \ get node data from current node
    size-link @ 0 <>
    if
      current-link @ dup node-payload swap payload-size @
    else
      0 0 \ return null data if there are no nodes in this linked list
    then
  ;m overrides ll@
end-class double-linked-list
