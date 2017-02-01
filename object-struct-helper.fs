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
\  A base object to help with structures
\  This struct-base is to be used to make your specific structure object to your specific needs.
\  All this object does is allocate memory for structure and calculates address from index.
\  This means all the structure fetch and recall methods need to be created in another object that uses this base object.

require ./objects.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
	protected
	cell% inst-var size
	cell% inst-var field-size
	cell% inst-var place
	m: ( uindex struct-base -- uaddr )
		size @ * place @ +
	;m method ::
	public
	m: ( ustruct-size ustruct struct-base -- ) \ constructor Note structure memory allocated so call destruct before construct to free memory
		%size dup field-size ! * dup size ! allocate throw place !
		place @ size @ erase
	;m overrides construct
  m: ( struct-base -- ) \ destructor
    place @ 0 <> if place @ free throw then
  ;m overrides destruct
end-class struct-base

\\\ The following is an example of how to use this struct-base
\ comment the above line to see example
struct-base class  \ this would be the new structure to make and work with
	protected
	struct
		cell% field somea
		char% field someb
		cell% field somec
	end-struct somestruct%
	m: ( uindex mystruct -- uaddr )
		this [parent] ::
	;m overrides ::
	public
	m: ( ustruct-size mystruct -- )
		somestruct% this [parent] construct
	;m overrides construct
	m: ( n uindex mystruct -- ) this :: somea ! ;m method somea.!
	m: ( uindex mystruct -- n ) this :: somea @ ;m method somea.@
	m: ( c uindex mystruct -- ) this :: someb c! ;m method someb.c!
	m: ( uindex mystruct -- c ) this :: someb c@ ;m method someb.c@
	m: ( n uindex mystruct -- ) this :: somec ! ;m method somec.!
	m: ( uindex mystruct -- n ) this :: somec @ ;m method somec.@
end-class mystruct

20 mystruct heap-new value nextstruct \ this creates mystruct in memory to use as nextstruct
nextstruct destruct
25 nextstruct construct
cr
3234 0 nextstruct somea.!  \ examples of using the structure array just created
0 nextstruct somea.@ . cr
'a' 0 nextstruct someb.c!
0 nextstruct someb.c@ . cr
723 5 nextstruct somec.!
5 nextstruct somec.@ . cr
nextstruct destruct  \ free memory for structure used
20 nextstruct construct  \ allocate a new structure array of size 20
0 nextstruct somea.@ . cr  \ shows the data in structure starts all at 0
nextstruct destruct
