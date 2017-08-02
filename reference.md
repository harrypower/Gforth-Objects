## Reference for objects using objects.fs

* ### multi-cell-array
  mdcs-obj.fs
  * construct                 _( umaxdim0 ... umaxdimx udimension-quantity multi-cell-array -- )_
    udimension-quantity refers to umaxdim values proceeding call for fixed heap allocation table creation
  * destruct                  _( multi-cell-array -- )_
  * cell-array!               _( nvalue udim0 ... udimx multi-cell-array -- )_
    store nvalue at cell address udim0 ... udimx
  * cell-array@               _( udim0 ... udimx multi-cell-array -- nvalue )_
    retrieve nvalue at cell address udim0 ... udimx
  * cell-array-dimensions@    _( multi-cell-array -- udim0 ... udimx udimension-quantity )_
    return the dimensions where udimension-quantity refers to udim values on stack that constitute the dimension  
  * print                     _( multi-cell-array -- )_
    print some information of internal values for this multi cell array object

* ### string
  stringobj.fs
  * construct           _( string -- )_
