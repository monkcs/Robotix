#!/bin/bash
cd WiringPiSharp
./build.sh
cd ..
cp WiringPiSharp/WiringPiSharp.dll WiringPiSharp.dll
mcs -target:library -out:Robotix.dll -r:WiringPiSharp.dll Robotix/*.cs Robotix/External/*.cs
