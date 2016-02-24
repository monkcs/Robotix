Robotix
=======

A .NET library to make easy use of Raspberry Pi GPIO pins, by manage all parts necessary to get input and output from the GPIO.
It encapsulates the pins into easy-to-use objects, polling the input pins, and also support "key objects" based of  `System.Console.ConsoleKey` for control by a user.

Functionally
------------

The Robotix library uses the C# wrapper [WiringPiSharp](https://github.com/monkcs/WiringPiSharp) to access and call Gordon's [WiringPi](http://wiringpi.com "WiringPi homepage") library. So everything *WiringPi* can do, Robotix can call in a more "manage" way. 

The pin classes gives you abillity to:


#####Currently Robotix contains classes for following pintype:

- [x] Digital pin (simply a switch true/false, or input true/false)
- [x] PWM pin (output pulse-width modulation signals. [Wiki](https://en.wikipedia.org/wiki/Pulse-width_modulation))
- [ ] SPI pins (Serial Peripheral Interface Bus. [Wiki](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus))
- [ ] I²C pins (Inter-Integrated Circuit. [Wiki](https://en.wikipedia.org/wiki/I%C2%B2C))
