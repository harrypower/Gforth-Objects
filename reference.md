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
      - nstring-source$ is split each time nstring-split$ is found and placed in this strings object
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
