#!/bin/bash
mcs /target:library /out:RobotixTools.dll -r:/../WiringPiSharp/WiringPiSharp.dll RobotixTools/*.cs RobotixTools/External/*.cs
mcs /target:library /out:Robotix.dll -r:/../WiringPiSharp/WiringPiSharp.dll Robotix/*.cs
