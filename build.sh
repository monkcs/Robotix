#!/bin/bash
mkdir robotix-compiled
cd WiringPiSharp
./build.sh
cd ..
mv WiringPiSharp/WiringPiSharp.dll robotix-compiled/WiringPiSharp.dll
mv WiringPiSharp/WiringPiSharp.xml robotix-compiled/WiringPiSharp.xml
mcs -target:library -out:Robotix.dll -r:robotix-compiled/WiringPiSharp.dll *.cs External/*.cs -doc:Robotix.xml
mv Robotix.dll robotix-compiled/Robotix.dll
mv Robotix.xml robotix-compiled/Robotix.xml
