@ECHO OFF
SETLOCAL
SET currentpath=%CD%
cd roslyn\Microsoft.Net.Compilers*
cd tools
SET path=%CD%
cd %currentpath%

mkdir robotix-compiled
cd WiringPiSharp
call build.cmd %path%
cd ..
move WiringPiSharp\WiringPiSharp.dll robotix-compiled\WiringPiSharp.dll
move WiringPiSharp\WiringPiSharp.xml robotix-compiled\WiringPiSharp.xml
csc /target:library /out:Robotix.dll /r:robotix-compiled\WiringPiSharp.dll *.cs External\*.cs /doc:Robotix.xml

move Robotix.dll robotix-compiled\Robotix.dll
move Robotix.xml robotix-compiled\Robotix.xml
