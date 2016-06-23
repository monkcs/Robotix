@ECHO OFF
SETLOCAL
SET currentpath=%CD%
cd roslyn\Microsoft.Net.Compilers*
cd tools
SET path=%CD%
cd %currentpath%

cd WiringPiSharp
build.cmd
cd ..
copy WiringPiSharp\WiringPiSharp.dll WiringPiSharp.dll
copy WiringPiSharp\WiringPiSharp.xml WiringPiSharp.xml
csc /target:library /out:Robotix.dll /r:WiringPiSharp.dll Robotix\*.cs Robotix\External\*.cs /doc:Robotix.xml
