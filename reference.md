## Reference for objects using objects.fs

* ### multi-cell-array
  mdcs-obj.fs
  * `construct`                 _( umaxdim0 ... umaxdimx udimension-quantity multi-cell-array -- )_
      - udimension-quantity refers to umaxdim values proceeding call for fixed heap allocation table creation
  * `destruct`                  _( multi-cell-array -- )_
  * `cell-array!`               _( nvalue udim0 ... udimx multi-cell-array -- )_
      - store nvalue at cell address udim0 ... udimx
  * `cell-array@`               _( udim0 ... udimx multi-cell-array -- nvalue )_
      - retrieve nvalue at cell address udim0 ... udimx
  * `cell-array-dimensions@`    _( multi-cell-array -- udim0 ... udimx udimension-quantity )_
      - return the dimensions where udimension-quantity refers to udim values on stack that constitute the dimension  
  * `print`                     _( multi-cell-array -- )_
      - print some information of internal values for this multi cell array object

* ### string
  stringobj.fs
  * `construct`           _( string -- )_
  * `destruct`            _(string -- )_
  * `!$`                  _( caddr u string -- )_
      - store string caddr u
  * `@$`                  _( string -- caddr u )_
      - retrieve string
  * `!+$`                 _( caddr u string -- )_
      - add string caddr u to this string at end of!
  * `!<+$`                _( caddr u string -- )_
      - add string caddr u to begining of this string
  * `split$`              _( caddr u string -- caddr1 u1 caddr2 u2 nflag )_
      - split this string object stored string by caddr u if caddr u is found in stored string
      - caddr1 u1 will be the split string before caddr u if caddr u is found in stored string
      - caddr2 u2 will be the split string after caddr u if caddr us is found in stored string
      - if no match found caddr1 u1 returns an empty string and caddr2 u2 contains this objects string
      - nflag is true if string is split and false if this objects string is returned without being split
      - Note the returned strings are valid until a new string is placed in this string object
      - This string object does not get changed in any way because of this operation!
  * `len$`                _( string -- u )_
      - report string size
  * `print`               _( string -- )_
      - retrieve string object info

* ### strings
  stringobj.fs
  * `construct`           _( strings -- )_
  * `destruct`            _( strings -- )_
  * `!$x`                 _( caddr u strings -- )_
      - store a string caddr u in handler
  * `@$x`                 _( strings -- caddr u )_
      - retrieve string from array at next index
  * `[]@$`                _( nindex strings -- caddr u nflag )_
      - retrieve nindex string from strings array
      - caddr u contain the string if nflag is false
      - if nflag is true caddr u do not contain nindex string
  * `split$s`             _( caddr u strings -- caddr1 u1 caddr2 u2 )_
      - retrieve string from this strings object array at next index then split that next string at caddr u if possible and removing caddr u in the process
      - caddr1 u1 is the string found before caddr u and could be a zero size string
      - caddr1 u1 will be a zero size string if caddr u is not found
      - caddr2 u2 contains the left over string if caddr u string is found and removed or it will simply be the full string from this string object array
  * `split$to$s`          _( nstring-split$ nstring-source$ strings -- )_
      - split up nstring-source$ with nstring-split$
      - nstring-source$ is spl__it each time nstring-split$ is found and placed in this strings object
      - nstring-split$ is removed each time it is found and when no more are found process is done
      - Note nstring-source$ string will contain the last split string but that string it is also placed in this strings object
      - Note null strings or strings of zero size could be found and placed in this strings object during spliting process
  * `split$>$s`           _( ncaddrfd u ncaddrsrc u1 strings -- )_
      - split up ncaddrsrc u1 string with ncaddrfd u string same as split$to$s method but temporary strings are passed to this code
      - ncaddrfd u is the string to find
      - ncaddrsrc u1 is the string to find ncaddrfd u string in
      - Note null strings or strings of zero size could be found and placed in this strings object during spliting process
  * `$qty`                _( strings -- u )_
      - report quantity of strings array
  * `reset`               _( strings -- )_
      - reset index to start of strings list for output purposes
  * `copy$s`              _( nstrings strings -- )_
      - copy strings object to this strings object
  * `print`               _( strings -- )_
      - print debug info on this strings object

* ### double-linked-list
  double-linked-list.fs
  * `construct`     _( double-linked-list -- )_
  * `destruct`      _( double-linked-list -- )_
  * `print`         _( double-linked-list -- )_
      - display information about objects contents
  * `ll!`           _( caddr u double-linked-list -- )_
      - add to link list a node at the end and update all the link list node data
      - caddr  is address of data to add to this node
      - u is the quantity of bytes to add to this node
      - note caddr u data is copied into a new heap location
  * `ll@`           _( double-linked-list -- caddr u )_
      - get node data from current node.
      - If there is no nodes in the linked list u will be 0 and caddr will be 0 indicating a null retrieval
  * `ll-size@`      _( double-linked-list -- u )_
      - get linked list node quantity
  * `ll<`           _( double-linked-list -- nflag )_
      - step one node back from current node.
      - nflag is true if step can not happend because at start node already or if there is no nodes in linked list to move to!
      - nflag is false if step did happen!
  * `ll>`           _( double-linked-list -- nflag )_
      - step one node forward from current node.
      - nflag is true if step can not happen because at last node already or if there is no nodes in linked list to move to!
      - nflag is false if step did happen!( double-linked-list -- )

  * `ll@>`          _( double-linked-list -- caddr u nflag )_
      - get node payload and step to next node
      - nflag is true if step can not happen because at last node already or if there is no nodes in linked list to step to!
      - nflag is false if step did happen
      - caddr and u will will be the node payload before the step if there are any linked list nodes
      - if there are no linked list nodes caddr and u will both be 0
  * `ll@<`         _( double-linked-list -- caddr u nflag )_
      - get node payload and step to next node
      - nflag is true if step can not happen because at first node already or if there is no nodes in linked list to step to!
      - nflag is false if step did happen
      - caddr and u will will be the node payload before the step if there are any linked list nodes
      - if there are no linked list nodes caddr and u will both be 0
  * `ll-set-start`  _( double-linked-list -- )_
      - set link list retrieve to the start of this linked list
  * `ll-set-end`    _( double-linked-list -- )_
      - set link list retrieve to the start of this linked list
  * `delete-last`   _( double-linked-list -- )_
      - delete the last node in this linked list
  * `delete-first`  _(double-linked-list -- )_
      - delete the fist node in this linked list
  * `delete-n-item` _( uindex double-linked-list -- )_
      - delete uindex item in list
  * `nll@`          _( uindex double-linked-list -- caddr u )_
      - retrieve uindex link list payload
  * `seedata`       _( uindex double-linked-list -- )_
      - testing ... see link data internal addresses for uindex node
  * `ll-cell!`      _( nnumber double-linked-list -- )_
      - store nnumber as next linked item
  * `ll-cell@`      _( double-linked-list -- nnumber )_
      - retrieve nnumber from linked list at next node
      - note if the linked list is empty nnumber will be 0
  * `nll-cell@`     _( uindex double-linked-list -- nnumber )_
      - retrieve nnumber from linked list at uindex location if it exists
      - note nnumber will be the last number in the linked list if uindex exceeds size of this linked list
      - note nnumber will be 0 if there are no nodes stored in link list

* ### svgmaker
  svgmaker.fs
  * `construct`         _( svgmaker -- )_
  * `destruct`          _( svgmaker -- )_
  * `svgheader`         _( nstrings-header svgmaker -- )_
      - start svg string and place nstrings contents as header to svg
  * `svgtext`           _( nstrings-attr nx ny nstring-text svgmaker -- )_
      - to make svg text
      - nstirngs-attr is strings for attribute of text
      - nx ny are text x and y of svg text tag
      - nstring-text is the string object address of the string
  * `svgpath`           _( nstrings-attr nstrings-pathdata svgmaker -- )_
      - make a svg path with nstring-attr and nstrings-pathdata
  * `svgcircle`         _( nstring-attr nx ny nr svgmaker -- )_
      - make a svg circle with nstring-attr at nx and ny with radius nr
  * `svgend`            _( svgmaker -- caddr u )_
      - finish forming the svg string and output it
  * `print`             _( svgmaker -- )_
      - print the svg string directly
