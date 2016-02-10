#!/bin/bash
cp ../WiringPiSharp/WiringPiSharp.dll WiringPiSharp.dll
mcs /target:library /out:RobotixTools.dll -r:WiringPiSharp.dll RobotixTools/*.cs RobotixTools/External/*.cs
mcs /target:library /out:Robotix.dll -r:WiringPiSharp.dll Robotix/*.cs
