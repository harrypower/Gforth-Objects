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
\ This is a multidimensional cell array object.

require ./objects.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class \ this is a multi dimension cell array
  destruction implementation
  protected
  cell% inst-var storage-location
  cell% inst-var dimensions
  cell% inst-var dimension-sizes
  cell% inst-var dimension-multiply
  m: ( udim0 ... udimx multi-cell-array -- )
    dimensions @ 0 ?do
      dimension-sizes @ i cells + !
    loop
  ;m method dimension-sizes!
  m: ( multi-cell-array -- udim0 ... udimx )
    dimensions @ 0 ?do
      dimension-sizes @ i cells + @
    loop
  ;m method dimension-sizes@

  m: ( multi-cell-array -- )
    cell dimension-multiply @ !
    dimensions @ 0 > if
    dimensions @ 1 ?do
      dimension-sizes @ i 1 - cells + @
      dimension-multiply @ i 1 - cells + @ *
      dimension-multiply @ i cells + !
    loop then
  ;m method dimension-multiply-make

  m: ( multi-cell-array -- umult0 ... umultx )
    dimensions @ 0 ?do
      i cells dimension-multiply @ + @
    loop
  ;m method dimension-multiply@

  m: ( udim0 ... udimx multi-cell-array -- uaddr )
    storage-location @ { uaddr }
    dimensions @ 0 ?do
      dimension-multiply @ i cells + @ * uaddr + to uaddr
    loop uaddr
  ;m method array-addr@
  public
  m: ( umaxdim0 ... umaxdimx udimension-quantity multi-cell-array -- )
  \ at construct time the dimensions of this object are fixed and heap memory allocated
  \ ex: 3 1 multi-cell-array heap-new constant single  -- this would make a 1 dimension array with 3 elements
  \ ex: 8 3 9 3 multi-cell-array heap-new constant 3d  -- this would make a 3 dimention array with 8 * 3 * 9 elements
  \ a cell is stored in each elememt !
  \ note no error checking is done or bound control is done at all in this object.
    dup dimensions !
    cells allocate throw dimension-sizes !
    this dimension-sizes!
    this dimension-sizes@ dimensions @ 1 ?do * loop cells allocate throw storage-location !
    dimensions @ cells allocate throw dimension-multiply !
    this dimension-multiply-make
  ;m overrides construct

  m: ( multi-cell-array -- )
  \ this will deallocate the memory and the dimensions for this object
    dimension-sizes @ free throw
    storage-location @ free throw
    dimension-multiply @ free throw
    0 dimensions !
    0 dimension-sizes !
    0 storage-location !
    0 dimension-multiply !
  ;m overrides destruct

  m: ( nvalue udim0 ... udimx multi-cell-array -- )
  \ store nvalue into the array at address udim0 ... udimx
  \ note the object knows the amount of stack items to remove so only the data and the address is needed on the stack
    this array-addr@ !
  ;m method cell-array!

  m: ( udim0 ... udimx multi-cell-array -- nvalue )
  \ retrieve the nvalue from address udim0 ... udimx of this object
    this array-addr@ @
  ;m method cell-array@

  m: ( multi-cell-array -- udim0 ... udimx udimension-quantity )
  \ return the array dimentions as defined at construct time.
    this dimension-sizes@
    dimensions @
  ;m method cell-array-dimensions@

  m: ( multi-cell-array -- )
    cr this [parent] print cr
    storage-location @ . ." storage-location @" cr
    dimensions @ . ." dimensions @ " cr
    this dimension-multiply@ dimensions @ 0 ?do . loop
     ." dimension-multiply@ in backward order!" cr
    this dimension-sizes@ dimensions @ 0 ?do . loop
    ." dimension-sizes@ in backward order!" cr
  ;m overrides print
end-class multi-cell-array

\ ***************************************************************************************************************************************
\ the following was used for testing and can be considered example of usage.
\\\
8 1 multi-cell-array heap-new constant testmulti
\ testmulti bind multi-cell-array cell-array-dimensions@ .s ." before single" cr

: singletest
  8 0 ?do i i testmulti [bind] multi-cell-array cell-array! loop
  8 0 ?do i dup . testmulti [bind] multi-cell-array cell-array@ . cr loop ;
singletest

\ testmulti bind multi-cell-array cell-array-dimensions@ .s ." after single " cr

testmulti bind multi-cell-array destruct
5 4 2 testmulti bind multi-cell-array construct
20 value stuff
: doubletest
  4 0 ?do
    5 0 ?do
      stuff dup 1 + to stuff i j testmulti [bind] multi-cell-array cell-array!
    loop
  loop
  4 0 ?do
    5 0 ?do
      i j testmulti [bind] multi-cell-array cell-array@
      . ." stored " i . ." i " j . ." j " cr
    loop
  loop
  ;

doubletest
testmulti bind multi-cell-array destruct
cr .s cr ." tripletest start " cr cr
5 5 5 3 testmulti bind multi-cell-array construct
testmulti bind multi-cell-array cell-array-dimensions@ .s ." before " cr

: tripletest
  5 0 ?do
    5 0 ?do
      5 0 ?do
        stuff dup 1 + to stuff i j k testmulti [bind] multi-cell-array cell-array!
      loop
    loop
  loop
  cr .s ." here" cr
  5 0 ?do
    5 0 ?do
      5 0 ?do
        i j k testmulti [bind] multi-cell-array cell-array@
        . ." stored " i . ." i " j . ." j " k . ." k " cr
      loop
    loop
  loop
  ;

testmulti bind multi-cell-array cell-array-dimensions@ .s ." final " cr
tripletest
testmulti bind multi-cell-array print
testmulti bind multi-cell-array destruct
