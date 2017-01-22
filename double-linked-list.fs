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
  cell% inst-var size-link
  cell% inst-var first-link
  cell% inst-var last-link
  cell% inst-var current-link
  cell% inst-var dll-test?
  protected
  struct
    cell% field next-forward-link
    cell% field next-back-link
  end-struct link-links
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
      0 size-link @ = if 0 dll-test? ! exitm then  \ nothing to deallocate
    then
  ;m overrides destruct
  m: ( -- ) \ print info
  ;m overrides print
end-class double-linked-list
