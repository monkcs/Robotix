#!/bin/bash
mkdir robotix-compiled
cd WiringPiSharp
./build.sh
cd ..
cp WiringPiSharp/WiringPiSharp.dll robotix-compiled/WiringPiSharp.dll
cp WiringPiSharp/WiringPiSharp.xml robotix-compiled/WiringPiSharp.xml
mcs -target:library -out:Robotix.dll -r:WiringPiSharp.dll Robotix/*.cs Robotix/External/*.cs -doc:Robotix.xml
cp Robotix.dll robotix-compiled/Robotix.dll
cp Robotix.xml robotix-compiled/Robotix.xml
