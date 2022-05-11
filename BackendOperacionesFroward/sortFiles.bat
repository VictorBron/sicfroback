set arg1=%1
shift
D:
cd %arg1%
DEL /F/Q/S *. * > NUL
mkdir lib
move ./*.dll ./lib