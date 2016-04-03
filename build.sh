#!/bin/bash
cd WiringPiSharp
./build.sh
cd ..
cp WiringPiSharp/WiringPiSharp.dll WiringPiSharp.dll
cp WiringPiSharp/WiringPiSharp.xml WiringPiSharp.xml
mcs -target:library -out:Robotix.dll -r:WiringPiSharp.dll Robotix/*.cs Robotix/External/*.cs -doc:Robotix.xml
