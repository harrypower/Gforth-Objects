\ This Gforth code is a SVG chart maker
\    Copyright (C) 2015  Philip K. Smith

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

\ The code inherits svgmaker object and creates svgchartmaker object to work with SVG
\ output for using in a web server for example!
\ This svgchartmaker object can take large data sets for x and a y data set an make an svg chart
\ Also most attributes can be setup in this chart for how the data looks and how the text looks
\ and how the chart looks.
\ See example use at end of this document

require ./objects.fs
require ./svgmaker.fs
require ./stringobj.fs

string heap-new constant mytemppad$  \ global string for converting numbers to strings
: #to$ ( n -- c-addr u1 ) \ convert n to string
    s>d swap over dabs <<# #s rot sign #> #>> mytemppad$ !$ mytemppad$ @$ ;

create floatoutputbuffer 10 allot \ memory buffer for converting floating to string
: nd>fto$ ( f: r -- ) ( nd -- caddr u ) \ convert r from float stack to string with nd digits after deciaml
    floatoutputbuffer 10 rot 0 f>buf-rdp floatoutputbuffer 10 ;

svgmaker class
  destruction implementation
  cell% inst-var svgchartmaker-test \ used to see if construct is first being executed or not
  \ these variables and values are calcuated or used in the following code
  cell% inst-var mymin      \ will contain the chart data min absolute value
  cell% inst-var mymax      \ will contain the chart data max absolute value
  cell% inst-var myspread   \ will contain mymax - mymin
  cell% inst-var xstep      \ how many x px absolute values to skip between data point plots
  cell% inst-var yscale     \ this is the scaling factor of the y values to be placed on chart
  \ these values are inputs to the chart for changing its look or size
  \ set these values to adjust the look of the size and positions of the chart
  inst-value xmaxpoints    \ this will be the max allowed points to be placed on the chart
  inst-value xlablesize    \ the size taken up by the xlabel on the right side for chart
  inst-value xmaxchart     \ the max x absolute px of the chart .. change this value to make chart larger in x
  inst-value ymaxchart     \ the max y absolute px of the chart .. change this value to make chart larger in y
  inst-value ylablesize    \ the y label at bottom of chart size in absolute px
  inst-value ytoplablesize \ the y label at top of chart size in absolute px
  inst-value xlableoffset  \ the offset to place lable from xlabelsize edge
  inst-value ylableoffset  \ the offset to place lable from ( ymaxchart + ytoplablesize )
  inst-value ylabletextoff \ the offset of the text from ( ymaxchart + ytoplablesize + ylabeloffset )
  inst-value ylableqty     \ how many y lablelines and or text spots
  inst-value ymarksize     \ the size of the y lable marks
  inst-value ylabletxtpos  \ the offset of y lable text from svg window
  inst-value circleradius  \ the radius of the circle used on charts for lines
  inst-value xlablerot     \ the value for rotation orientation of xlable text
  inst-value ylablerot     \ the value for rotation orientation of ylable text
  inst-value ylable#dec    \ the numbers after deciaml point displayed in y lable calculated value numbers
  \ these will be string to hold string data
  inst-value working$       \ used to work on a string temporarily in chart code
  inst-value lableref$
  inst-value lablemark$
  inst-value ytransform$
  \ these will be strings to hold strings data
  inst-value ytempattr$s
  inst-value xtempattr$s
  inst-value working$s      \ this is used to work on strings temporarily in the chart code
  inst-value pathdata$      \ contains path strings for processing in makepath and makelable
  inst-value xlabdata$      \ x label data strings
  inst-value xlab-attr$     \ x label attribute strings
  inst-value ylab-attr$     \ y label attribute strings
  inst-value labline-attr$  \ label line attribute strings

  \ structure to hold the array of datas to be charted
  struct
   cell% field data$
   cell% field data-attr$
   cell% field circle-attr$
  end-struct data%
  inst-value index-data \ data index
  inst-value addr-data  \ addresss of data structure
  struct
  \ structure to hold the text string location and attributes
    cell% field text$
    cell% field text-x
    cell% field text-y
    cell% field text-attr$
  end-struct text%
  inst-value index-text \ text index
  inst-value addr-text  \ address of text structure

  protected
  m: ( -- ) \ this will free text and data strings and string that was dynamicaly created in object
    index-data 0 ?do
      addr-data data% %size i * + dup
      data$ @ dup [bind] strings destruct free throw dup
      data-attr$ @ dup [bind] strings destruct free throw
      circle-attr$ @ dup [bind] strings destruct free throw
    loop
    index-data 0 > if addr-data free throw then
    0 [to-inst] index-data
    0 [to-inst] addr-data
    index-text 0 ?do
      addr-text text% %size i * + dup
      text$ @ dup [bind] string destruct free throw
      text-attr$ @ dup [bind] strings destruct free throw
    loop
    index-text 0 > if addr-text free throw then
    0 [to-inst] index-text
    0 [to-inst] addr-text
  ;m method free-text-data

  public
  m: ( svgchart -- ) \ constructor to set some defaults
  	this [parent] construct
  	svgchartmaker-test svgchartmaker-test @ =
  	if
      working$    [bind] string construct
      lableref$   [bind] string construct
      lablemark$  [bind] string construct
      ytransform$ [bind] string construct

      working$s   [bind] strings construct
      pathdata$   [bind] strings construct
      xlabdata$   [bind] strings construct
      xlab-attr$  [bind] strings construct
      ylab-attr$  [bind] strings construct
      labline-attr$ [bind] strings construct
      ytempattr$s [bind] strings construct
      xtempattr$s [bind] strings construct
      this [current] free-text-data
  	else
	    \ dynamicaly created objects
	    string  heap-new [to-inst] working$
	    string  heap-new [to-inst] lableref$
	    string  heap-new [to-inst] lablemark$
	    string  heap-new [to-inst] ytransform$

	    strings heap-new [to-inst] working$s
	    strings heap-new [to-inst] pathdata$
	    strings heap-new [to-inst] xlabdata$
	    strings heap-new [to-inst] xlab-attr$
	    strings heap-new [to-inst] ylab-attr$
	    strings heap-new [to-inst] labline-attr$
	    strings heap-new [to-inst] ytempattr$s
	    strings heap-new [to-inst] xtempattr$s
	    0 [to-inst] index-data
	    0 [to-inst] addr-data
	    0 [to-inst] index-text
	    0 [to-inst] addr-text
	    svgchartmaker-test svgchartmaker-test ! \ set flag for first time svgmaker object constructed
  	then
  	0.0e mymin sf!
  	0.0e mymax sf!
  	0.0e myspread sf!
  	0.0e xstep sf!
  	0.0e yscale sf!
  	20   [to-inst] xmaxpoints
  	140  [to-inst] xlablesize
  	1000 [to-inst] xmaxchart
  	600  [to-inst] ymaxchart
  	140  [to-inst] ylablesize
  	70   [to-inst] ytoplablesize
  	10   [to-inst] xlableoffset
  	10   [to-inst] ylableoffset
  	30   [to-inst] ylabletextoff
  	10   [to-inst] ylableqty
  	20   [to-inst] ymarksize
  	0    [to-inst] ylabletxtpos
  	4    [to-inst] circleradius
  	90   [to-inst] xlablerot
  	0    [to-inst] ylablerot
  	0    [to-inst] ylable#dec
  ;m overrides construct

  m: ( svgchart -- ) \ destruct all allocated memory and free this object
  	svgchartmaker-test svgchartmaker-test @ =
  	if
	    working$      dup [bind] string destruct free throw
	    lableref$     dup [bind] string destruct free throw
	    lablemark$    dup [bind] string destruct free throw
	    ytransform$   dup [bind] string destruct free throw
	    working$s     dup [bind] strings destruct free throw
	    pathdata$     dup [bind] strings destruct free throw
	    xlabdata$     dup [bind] strings destruct free throw
	    xlab-attr$    dup [bind] strings destruct free throw
	    ylab-attr$    dup [bind] strings destruct free throw
	    labline-attr$ dup [bind] strings destruct free throw
	    ytempattr$s   dup [bind] strings destruct free throw
	    xtempattr$s   dup [bind] strings destruct free throw
	    this [current] free-text-data
	    this [parent] destruct
	    0 svgchartmaker-test !
    then ;m overrides destruct

  m: ( nxmaxpoints nxmaxchart nymaxchart -- ) \ values to change chart size and charting data use
  	\ nmaxpoints forces data points to be used up to this limit
  	\ nxmaxchart max x absolute px size of chart
  	\ nymaxchart max y absolute px size of chart
  	[to-inst] ymaxchart [to-inst] xmaxchart [to-inst] xmaxpoints ;m method setchart-prop

  m: ( ncircleradius -- ) \ the radius size of circles for each data point in data set ( in px ).
  	[to-inst] circleradius ;m method setdtpts-circle-prop

  m: ( nylablesize nytoplablesize nylableoffset nylableqty nymarksize  ) \ y lable position and quantity propertys
  	\ nylablesize y lable at bottom of chart size in absolute px
  	\ nytoplablesize y lable at top of chart size in absolute px
  	\ nylableoffset offset to place lable from ( ymaxchart + ytoplableoffset )
  	\ nylableqty how may y lable lines and or text spots
  	\ nymarksize size of the y lable marks
  	[to-inst] ymarksize [to-inst] ylableqty [to-inst] ylableoffset [to-inst] ytoplablesize [to-inst] ylablesize
  ;m method setylable-prop

  m: ( nylable#dec nylabletextoff nylabletxtpos nylablerot ) \ y lable text propertys
    \ nylable#dec this is the number of digits after the decimal
  	\ nylabletextoff offset of the text from ( ymaxchart + ytoplablesize + ylabeloffset )
  	\ nylabletxtpos offset of y lable text from svg window
  	\ nylablerot rotation orientation of ylable text
  	[to-inst] ylablerot [to-inst] ylabletxtpos [to-inst] ylabletextoff [to-inst] ylable#dec
  ;m method setylable-text-prop

  m: ( nxlablesize nxlableoffset nxlablerot -- ) \ x lable position and quantity propertys
  	\ nxlablesize   size taken up by the ylabel on the left side of chart
  	\ nxlableoffset offset to place lable from xlabelsize edge on left side of chart
  	\ nxlablerot    value of rotation orientation of xlable text
  	[to-inst] xlablerot [to-inst] xlableoffset [to-inst] xlablesize ;m method setxlable-prop

  \ fudge test words ... made private after object is done
  protected
  m: ( -- caddr u ) \ test word to show svg output
  	svg-output @ [bind] string @$ ;m method seeoutput
  m: ( --  caddr u )
  	working$ [bind] string @$ ;m method seeworking
  m: ( -- naddr )
  	working$s ;m method working$s@

  m: ( f: -- fmymin fmymax fmyspread fxstep fyscale )
  	mymin sf@
  	mymax sf@
  	myspread sf@
  	xstep sf@
  	yscale sf@ ;m method seecalculated
  m: ( -- naddr )
  	pathdata$ ;m method seepathdata$
  m: ( -- nxlabdata$ nxlab-attr$ nylab-attr$ nlabline-attr$ )
  	xlabdata$
  	xlab-attr$
  	ylab-attr$
  	labline-attr$ ;m method seelable

  \ some worker methods to do some specific jobs
  m: ( nindex-data svgchart -- nstrings-xdata nstrings-xdata-attr nstrings-xdata-circle-attr )
  	\ to retrieve the data and attributes for a given index value
  	data% %size * addr-data + dup
  	data$ @ swap dup
  	data-attr$ @ swap
  	circle-attr$ @ ;m method ndata@

  m: ( nindex-text svgchart -- nstring-text nx ny nstrings-attr )
  	\ to retrieve the text attributes for a given index value
  	text% %size * addr-text + dup
  	text$ @ swap dup
  	text-x @ swap dup
  	text-y @ swap
  	text-attr$ @ ;m method ntext@

  protected
  m: ( nxdata$ svgchart -- )  \ finds the min and max values of the localdata strings
  	\ note results stored in mymax and mymin float variables
  	{ xdata$ }
  	xdata$ [bind] strings $qty xmaxpoints min 0 ?do
  	    xdata$ [bind] strings @$x >float if fdup mymin sf@ fmin mymin sf! mymax sf@ fmax mymax sf! then
  	loop ;m method findminmaxdata

  m: ( svgchart -- ) \ will produce the svg header for this chart
  	working$s [bind] strings construct
  	s\" width=" working$ [bind] string !$ s\" \"" working$ [bind] string !+$
  	xmaxchart xlablesize + #to$ working$ [bind] string !+$ s\" \"" working$ [bind] string !+$
  	working$ [bind] string @$ working$s [bind] strings !$x

  	s\" height=" working$ [bind] string !$ s\" \"" working$ [bind] string !+$
  	ymaxchart ylablesize + ytoplablesize + #to$ working$ [bind] string !+$ s\" \"" working$ [bind] string !+$
  	working$ [bind] string @$ working$s [bind] strings !$x

  	s\" viewBox=" working$ [bind] string !$ s\" \"0 0 " working$ [bind] string !+$
  	xmaxchart xlablesize + #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	ymaxchart ylablesize + ytoplablesize + #to$ working$ [bind] string !+$ s\" \"" working$ [bind] string !+$
  	working$ [bind] string @$ working$s [bind] strings !$x

  	working$s this [parent] svgheader ;m method makeheader

  m: ( nattr$ svgchart -- ) \ will produce the cicle svg strings to be used in chart
  	\ *** use the pathdata$ to make the circle data into the svgoutput
  	string heap-new
  	{ attr$ atemp$ }
  	attr$ [bind] strings reset
  	pathdata$ [bind] strings reset
  	pathdata$ [bind] strings $qty 0 ?do
	    s"  " pathdata$ [bind] strings split$s 2swap 2drop atemp$ [bind] string !$ s"  "
	    atemp$ [bind] string split$ drop >float
	    if
    		>float if attr$ f>s f>s circleradius this [parent] svgcircle then
	    else
    		2drop
	    then
  	loop
  	atemp$ [bind] string destruct atemp$ free throw
  ;m method makecircles

  m: ( nxdata$ svgchart -- ) \ will produce the path strings to be used in chart
  	{ xdata$ }
  	xdata$ [bind] strings reset
  	pathdata$ [bind] strings construct
  	s" M " working$ [bind] string !$ xlablesize #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	xdata$ [bind] strings @$x >float
  	if
	    yscale sf@ f*
  	else \ if fist string is not a number just plot with mymin value
	    mymin sf@ yscale sf@ f*
  	then
  	mymax sf@ yscale sf@ f* fswap f- f>s ytoplablesize + #to$ working$ [bind] string !+$
  	working$ [bind] string @$ pathdata$ [bind] strings !$x
  	xdata$ $qty xmaxpoints min 1
  	?do
	    s" L " working$ [bind] string !$
	    i s>f xstep sf@ f* f>s xlablesize + #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
	    xdata$ [bind] strings @$x >float
	    if
    		yscale sf@ f*
	    else \ if string is not a number just plot with mymin value
    		mymin sf@ yscale sf@ f*
	    then
	    mymax sf@ yscale sf@ f* fswap f- f>s ytoplablesize + #to$ working$ [bind] string !+$
	    working$ [bind] string @$ pathdata$ [bind] strings !$x
  	loop ;m method makepath

  m: ( svgchart -- ) \ will make the chart lables both lines and text
  	pathdata$ [bind] strings construct
  	\ make the ylable line
  	s" M " working$ [bind] string !$ xlablesize xlableoffset - #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	ytoplablesize #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	working$ [bind] string @$ 2dup lableref$ [bind] string !$ pathdata$ [bind] strings !$x
  	s" L " working$ [bind] string !$ xlablesize xlableoffset - #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	ymaxchart ytoplablesize + ylableoffset + #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	working$ [bind] string @$ pathdata$ [bind] strings !$x
  	\ make the xlable line
  	s" L " working$ [bind] string !$ xmaxchart xlablesize + #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	ymaxchart ytoplablesize + ylableoffset + #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
  	working$ [bind] string @$ pathdata$ [bind] strings !$x
  	\ make ylable line marks
  	lableref$ [bind] string @$ pathdata$ [bind] strings !$x
  	s" l " working$ [bind] string !$ ymarksize -1 * #to$ working$ [bind] string !+$
  	s"  " working$ [bind] string !+$ 0 #to$ working$ [bind] string !+$
  	working$ [bind] string @$ 2dup lablemark$ [bind] string !$ pathdata$ [bind] strings !$x
  	ylableqty 1 + 1 ?do
	    lableref$ [bind] string @$ pathdata$ [bind] strings !$x
	    s" m " working$ [bind] string !$ 0 #to$ working$ [bind] string !+$ s"  " working$ [bind] string !+$
	    ymaxchart s>f ylableqty s>f f/ i s>f f* f>s #to$ working$ [bind] string !+$
	    working$ [bind] string @$ pathdata$ [bind] strings !$x
	    lablemark$ [bind] string @$ pathdata$ [bind] strings !$x
  	loop
  	labline-attr$ pathdata$ this [parent] svgpath
  	\ generate y lable text
  	ylableqty 1 + 0 ?do
	    ytempattr$s [bind] strings construct
	    ylab-attr$ ytempattr$s [bind] strings copy$s
	    ylabletxtpos ytoplablesize
	    yscale sf@ myspread sf@ ylableqty s>f f/ f* i s>f f* f>s + ( nx ny )
	    \ add transformation for ylable rotation
	    s\"  transform=\"rotate(" ytransform$ [bind] string !$ ylablerot #to$ ytransform$ [bind] string !+$ s" , " ytransform$ [bind] string !+$
	    swap dup #to$ ytransform$ [bind] string !+$ s" , " ytransform$ [bind] string !+$ swap dup #to$ ytransform$ [bind] string !+$
	    s"  " ytransform$ [bind] string !+$ s\" )\"" ytransform$ [bind] string !+$
	    ytransform$ [bind] string @$ ytempattr$s [bind] strings !$x ytempattr$s -rot
	    myspread sf@ ylableqty s>f f/ i s>f f* mymax sf@ fswap f- ylable#dec nd>fto$ ytransform$ [bind] string !$ ytransform$
	    this [parent] svgtext
  	loop
  	\ generate x lable text
  	xlabdata$ [bind] strings $qty 0 ?do
      xtempattr$s [bind] strings construct
      xlab-attr$ xtempattr$s [bind] strings copy$s
      xlablesize xmaxchart s>f xlabdata$ [bind] strings $qty s>f f/ i s>f f* f>s + ylableoffset ymaxchart + ytoplablesize + ylabletextoff +
      s\"  transform=\"rotate(" working$ [bind] string !$ xlablerot #to$ working$ [bind] string !+$ s" , " working$ [bind] string !+$
      swap dup #to$ working$ [bind] string !+$ s" , " working$ [bind] string !+$ swap dup #to$
      working$ [bind] string !+$ s"  " working$ [bind] string !+$
      s\" )\"" working$ [bind] string !+$ working$ [bind] string @$ xtempattr$s [bind] strings !$x xtempattr$s -rot xlabdata$
      this [parent] svgtext
  	loop ;m method makelables

  m: ( svgchart -- ) \ will put the text onto the chart
  	index-text 0 ?do
      addr-text i text% %size * + dup
      text-attr$ @ swap dup
      text-x @ swap dup
      text-y @ swap
      text$ @ this [parent] svgtext
  	loop ;m method maketexts

  public
  \ methods for giving data to svgchart and geting the svg from this object
  m: ( nstrings-xdata nstrings-xdata-attr nstrings-xdata-circle-attr svgchart -- )
  	\ to place xdata onto svg chart with xdata-attr and with circle-attr for each data point
  	\ note the xdata is a strings object that must have quantity must match xlabdata quantity
  	\ the data passed to this method is stored only once so last time it is called that data is used to make chart
  	index-data 0 >
  	if
	    addr-data data% %size index-data 1 + * resize throw [to-inst] addr-data index-data 1 + [to-inst] index-data
  	else
	    data% %alloc [to-inst] addr-data
	    1 [to-inst] index-data
  	then
  	addr-data index-data 1 - data% %size * + dup
  	-rot circle-attr$ strings heap-new dup rot ! [bind] strings copy$s dup
  	-rot data-attr$ strings heap-new dup rot ! [bind] strings copy$s
  	data$ strings heap-new dup rot ! [bind] strings copy$s ;m method setdata

  m: ( nstrings-xlabdata nstrings-xlab-attr nstrings-ylab-attr nstrings-labline-attr svgchart -- )
  	\ to place xlabel data onto svg chart with x,y text and line attributes
  	\ note xlabdata is a strings object containing all data to be placed on xlabel but quantity must match xdata quantity
  	\ the data passed to this method is stored only once so last time it is called that data is used to make chart
  	labline-attr$ [bind] strings copy$s
  	ylab-attr$ [bind] strings copy$s
  	xlab-attr$ [bind] strings copy$s
  	xlabdata$ [bind] strings copy$s ;m method setlabledataattr

  m: ( nstring-txt nx ny nstrings-attr svgchart -- ) \ to place txt on svg with x and y location and attributes
  	\ every time this is called before makechart method the string,x,y and attributes are stored to be placed into svgchart
  	index-text 0 >
  	if
	    addr-text text% %size index-text 1 + * resize throw [to-inst] addr-text index-text 1 + [to-inst] index-text
  	else
	    text% %alloc [to-inst] addr-text
	    1 [to-inst] index-text
  	then
  	addr-text index-text 1 - text% %size * + dup
  	-rot text-attr$ strings heap-new dup rot ! [bind] strings copy$s dup
  	-rot text-y ! dup
  	-rot text-x !
  	text$ string heap-new dup rot ! swap [bind] string @$ rot [bind] string !$ ;m method settext

  m: ( svgchart -- caddr u nflag )  \ top level word to make the svg chart
  	\ test for data available for chart making and valid
  	index-data 0 = if abort" No data for chart!" then
  	0 this [current] ndata@ 2drop [bind] strings $qty 0 = if abort" Data for chart is empty!" then
  	xlabdata$ [bind] strings $qty 0 = if abort" No lable data for chart!" then
  	0 this [current] ndata@ 2drop [bind] strings $qty index-data 1
  	?do dup i this [current] ndata@ 2drop [bind] strings $qty <> if abort" Data quantitys not the same!" then loop
  	xlabdata$ [bind] strings $qty <> if abort" Data quantitys not the same!" then
  	\ set mymin and mymax to start values
  	0 this [current] ndata@ 2drop dup [bind] strings reset [bind] strings @$x >float
  	if fdup mymax sf! mymin sf! else abort" Data not a number!" then
  	\ find all min and max values from all data sets
  	index-data 0 ?do
	    i this [current] ndata@ 2drop dup [bind] strings reset this [current] findminmaxdata
  	loop
  	\ calculate myspread
  	mymax sf@ mymin sf@ f- myspread sf!
  	\ calculate xstep
  	xmaxchart s>f 0 this [current] ndata@ 2drop [bind] strings $qty xmaxpoints min s>f f/ xstep sf!
  	\ calculate yscale
  	ymaxchart s>f myspread sf@ f/ yscale sf!
  	\ execute makeheader
  	this [current] makeheader
  	\ make path and cicle svg elements with there associated attributes
  	index-data 0 ?do
	    i this [current] ndata@ swap rot this [current] makepath pathdata$ this [parent] svgpath this [current] makecircles
  	loop
  	\ execute makelables
  	this [current] makelables
  	\ execute maketext
  	this [current] maketexts
  	\ finish svg with svgend to return the svg string
  	this [parent] svgend
  	\ return false if no other errors
  	false ;m method makechart

end-class svgchartmaker

\ \\\ This three slash word only works in gforth 0.7.9 and higher!  Simply will not interpret rest of file from here on!
\ uncomment the line above to generate the svg string this example creates

svgchartmaker heap-new value test

strings heap-new constant tdata
strings heap-new constant tda
strings heap-new constant tda2
strings heap-new constant tdca
strings heap-new constant tld
strings heap-new constant tlla
strings heap-new constant ta
string  heap-new constant tt
strings heap-new constant yla
strings heap-new constant xla

s\" fill=\"rgb(0,0,255)\""     ta bind strings !$x
s\" fill-opacity=\"1.0\""      ta bind strings !$x
s\" stroke=\"rgb(0,100,200)\"" ta bind strings !$x
s\" stroke-opacity=\"1.0\""    ta bind strings !$x
s\" stroke-width=\"2.0\""      ta bind strings !$x
s\" font-size=\"30px\""        ta bind strings !$x

s" 10"                         tdata bind strings !$x
s" 20"                         tdata bind strings !$x
s" 53.9"                       tdata bind strings !$x
s" 0.789"                      tdata bind strings !$x

 s\" fill-opacity=\"0.0\""      tda bind strings !$x
 s\" stroke=\"rgb(255,120,0)\"" tda bind strings !$x
 s\" stroke-opacity=\"1.0\""    tda bind strings !$x
 s\" stroke-width=\"2.0\""      tda bind strings !$x
\ s\" style=\"stroke: #ff0000; fill: #ffffff;\"" tda bind strings !$x

 s\" fill-opacity=\"0.0\""      tda2 bind strings !$x
 s\" stroke=\"rgb(0,0,255)\""   tda2 bind strings !$x
 s\" stroke-opacity=\"1.0\""    tda2 bind strings !$x
 s\" stroke-width=\"2.0\""      tda2 bind strings !$x
\ s\" style=\"stroke: #0000ff; fill: #000000;\"" tda2 bind strings !$x

s\" fill=\"rgb(0,255,0)\""     tdca bind strings !$x
s\" fill-opacity=\"0.7\""      tdca bind strings !$x
s\" stroke=\"rgb(255,0,0)\""   tdca bind strings !$x
s\" stroke-opacity=\"1.0\""    tdca bind strings !$x
s\" stroke-width=\"3.0\""      tdca bind strings !$x

s\" fill=\"rgb(0,0,255)\""     yla bind strings !$x
s\" fill-opacity=\"1.0\""      yla bind strings !$x
s\" stroke=\"rgb(0,120,255)\"" yla bind strings !$x
s\" stroke-opacity=\"1.0\""    yla bind strings !$x
s\" stroke-width=\"2.0\""      yla bind strings !$x
s\" font-size=\"20px\""        yla bind strings !$x

s\" fill=\"rgb(255,0,0)\""     xla bind strings !$x
s\" fill-opacity=\"1.0\""      xla bind strings !$x
s\" stroke=\"rgb(50,255,0)\""  xla bind strings !$x
s\" stroke-opacity=\"1.0\""    xla bind strings !$x
s\" stroke-width=\"2.0\""      xla bind strings !$x
s\" font-size=\"20px\""        xla bind strings !$x

s" first"                      tld bind strings !$x
s" second"                     tld bind strings !$x
s" third"                      tld bind strings !$x
s" fourth"                     tld bind strings !$x

s\" fill-opacity=\"0.0\""      tlla bind strings !$x
s\" stroke=\"rgb(0,0,255)\""   tlla bind strings !$x
s\" stroke-opacity=\"1.0\""    tlla bind strings !$x
s\" stroke-width=\"2.0\""      tlla bind strings !$x

s" here is first text"          tt bind string !$
tt 10 20  ta test bind svgchartmaker settext
s" second texts"                tt bind string !$
tt 200 300 ta test bind svgchartmaker settext

tld xla yla tlla test bind svgchartmaker setlabledataattr

tdata tda tdca test bind svgchartmaker setdata
tdata bind strings construct
s" 19" tdata bind strings !$x
s" 29" tdata bind strings !$x
s" 3.92" tdata bind strings !$x
s" 99.3" tdata bind strings !$x
tdata tda2 tdca test bind svgchartmaker setdata

5 1000 300 test bind svgchartmaker setchart-prop
10 test bind svgchartmaker setdtpts-circle-prop
200 40 10 10 10 test bind svgchartmaker setylable-prop
5 10 30 0 test bind svgchartmaker setylable-text-prop
140 10 70 test bind svgchartmaker setxlable-prop

test bind svgchartmaker makechart throw type
