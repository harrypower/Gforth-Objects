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
  protected
  struct
    cell% field next-forward-link
    cell% field next-back-link
    cell% field payload-size  \ note node payload data size limit to cell quantity
    cell% field node-payload
  end-struct link-links%
  public
  m: ( double-linked-list -- ) \ constructor Note using this member will leak memory.. use destruct to deallocate memory
      0 size-link !
      0 first-link !
      0 last-link !
      0 current-link !
  ;m overrides construct
  m: ( double-linked-list -- ) \ destructor
    first-link @ 0 <> size-link @ 0 <> and if
      first-link @ current-link !
      size-link @ 0 ?do
        current-link @ next-forward-link @
        current-link @ free throw
        current-link !
      loop
      0 size-link !
      0 first-link !
      0 last-link !
      0 current-link !
    then ;m overrides destruct
  m: ( double-linked-list -- ) \ print info
    cr size-link @ u. ." link list size" cr
    first-link @ u. ." start node's address" cr
    last-link @ u. ." the last node's address" cr
    current-link @ u. ." current node's address" cr
    size-link @ 0 >
    if
      current-link @ next-back-link @ u. ." current node's back link address" cr
      current-link @ next-forward-link @ u. ." current node's forward link address" cr
      ." current node data dump:" cr
      this ll@ dump cr
    then ;m overrides print
  m: ( caddr u double-linked-list -- )
    { caddr u -- } \ add to link list a node at the end and update all the link list node data
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
  m: ( double-linked-list -- caddr u ) \ get node data from current node
    \ if there is no nodes in the linked list u will be 0 and caddr will be 0 indicating a null retrieval
    size-link @ 0 >
    if
      current-link @ dup node-payload swap payload-size @
    else
      0 0 \ return null data if there are no nodes in this linked list
    then ;m overrides ll@
  m: ( double-linked-list -- u ) \ get linked list node size
    size-link @ ;m method ll-size@
  m: ( double-linked-list -- nflag ) \ step one node back from current node.
    \ nflag is true if step can not happend because at start node already or if there is no nodes in linked list to move to!
    \ nflag is false if step did happen!
    size-link @ 0 <> current-link @ first-link @ <> and if
      current-link @ next-back-link @ current-link ! false
    else
      true
    then ;m method ll<
  m: ( double-linked-list -- nflag ) \ step one node forward from current node.
    \ nflag is true if step can not happen because at last node already or if there is no nodes in linked list to move to!
    \ nflag is false if step did happen!
    size-link @ 0 <> current-link @ last-link @ <> and if
      current-link @ next-forward-link @ current-link ! false
    else
      true
    then ;m method ll>
  m: ( double-linked-list -- caddr u nflag ) \ get node payload and step to next node
    \ nflag is true if step can not happen because at last node already or if there is no nodes in linked list to step to!
    \ nflag is false if step did happen
    \ caddr and u will will be the node payload before the step if there are any linked list nodes
    \ if there are no linked list nodes caddr and u will both be 0
    this ll@ this ll> ;m method ll@>
  m: ( double-linked-list -- caddr u nflag ) \ get node payload and step to next node
    \ nflag is true if step can not happen because at first node already or if there is no nodes in linked list to step to!
    \ nflag is false if step did happen
    \ caddr and u will will be the node payload before the step if there are any linked list nodes
    \ if there are no linked list nodes caddr and u will both be 0
    this ll@ this ll< ;m method ll@<
  m: ( double-linked-list -- ) \ set link list retrieve to the start of this linked list
    first-link @ current-link ! ;m method ll-set-start
  m: ( double-linked-list -- ) \ set link list retrieve to the end of this linked list
    last-link @ current-link ! ;m method ll-set-end
  m: ( double-linked-list -- ) \ delete last item in list
    size-link @ 0<> if
      size-link @ 1 = if
        this destruct
      else
        \ place here the delete last item code
      then
    then ;m method delete-last
  m: ( double-linked-list -- ) \ delete first item in list
    size-link @ 0<> if
      size-link @ 1 = if
        this destruct
      else
        \ place here the delete first item code
      then
    then ;m method delete-last
  m: ( unindex double-linked-list -- ) \ delete uindex item in list

  ;m method delete-n-item 
end-class double-linked-list

\\\ these three slashs cause the rest of the file to be not interpreted but only works in gforth version 0.7.9 and up
\ comment out the above line to run the following tests
double-linked-list heap-new value lltest
lltest destruct
lltest print
s" hello world" lltest ll!
s" next line" lltest ll!
s" line 3" lltest ll!
s" the last stuff in this list" lltest ll!
lltest print
lltest ll@> . cr type cr
lltest ll@> . cr type cr
lltest ll@> . cr type cr
lltest ll@> . cr type cr
lltest ll@< . cr type cr
lltest ll@  type cr
lltest ll-size@ . cr
lltest ll-set-end
lltest ll@ type cr
lltest ll-set-start
lltest ll@ type cr
lltest ll> throw lltest ll> throw
lltest ll@ type cr
lltest print
lltest destruct
lltest print
