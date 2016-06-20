Robotix
=======

A .NET library to make easy use of Raspberry Pi GPIO pins, by manage all parts necessary to get input and output from the GPIO.
It encapsulates the pins into easy-to-use objects, polling the input pins, and also support "key objects" based of  `System.Console.ConsoleKey` for control by a user.

The Robotix library uses the C# wrapper [WiringPiSharp](https://github.com/monkcs/WiringPiSharp) to access and call Gordon's [WiringPi](http://wiringpi.com "WiringPi homepage") library written in C.

Functionally
------------
The pin classes gives you abillity to without low-level coding use and access following listed devices and protocol. If you want to implement a specific device please fill out a issue or make a pull request.
#####Currently Robotix contains classes for following pintype:

- [x] Digital pin (simply a switch true/false, or input true/false)
- [x] PWM pin (output pulse-width modulation signals. [Wiki](https://en.wikipedia.org/wiki/Pulse-width_modulation))
  - [x] Servo control
  - [x] Motor control
- [x] SPI pins (Serial Peripheral Interface Bus. [Wiki](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus))
  - [ ] 7-segment displays ([MAX7219, MAX7221](https://www.maximintegrated.com/en/products/power/display-power-control/MAX7221.html))
- [ ] I²C pins (Inter-Integrated Circuit. [Wiki](https://en.wikipedia.org/wiki/I%C2%B2C))

Compilation and Installation
----------------------------

The Robotix library depends on the _WiringPiSharp_ and the _WiringPi_ library. Robotix and WiringPiSharp can both be compiled to .dll -files on the development machine together with your code. WiringPi on the other hand must be compiled on the Raspberry Pi itself.

We provide easy to use cmd and bash install scripts over at the installation [wiki page](https://github.com/monkcs/Robotix/wiki/Installation)

Example code
------------

This example will setup a pin for input named __*button*__ and a output pin named __*led*__. When the button pin returns `true` (the button is pressed) the __*led*__ pin will output current (and light up the connected LED).

```C#
using System;
using Robotix;
using Robotix.External;
using WiringPiSharp;
using static WiringPiSharp.WiringPi;

namespace Robot
{
	class Program
	{
		public static void Main() 
		{
			RobotControl myRobot = new RobotControl ();
			myRobot.Start ();

			Console.WriteLine ("Press any key to stop robot");
			Console.ReadKey (true);

			myRobot.Stop ();
			myRobot.Dispose ();
		}
	}



	class RobotControl : PhysicalCommand
	{
		public RobotControl () : base() { }
		
     /* Will initiate all pins. Place all your pin configuration here. */
		protected override void Initiate ()
		{
			base.Initiate ();

			Add<DigitalPin> (new DigitalPin (
				WPiPinout.P2,   // Pin number
				PinMode.Input,  // Input or output
				false,          // Initial output value
				"button",       // A friendly name for the pin
				PullMode.Down   // Internal resistor pull mode for input pins
			));
			Add<DigitalPin> (new DigitalPin (WPiPinout.P27, PinMode.Output, false, "led", PullMode.Off));
		}
		
     /* The main method for the robots logic, runs many times per second */
		protected override void Update ()
		{
			if (GetPin<DigitalPin> ("button").JustChangedTo (true))
			{
				GetPin<DigitalPin> ("led").Write (true);
			} 
			else
			{
				GetPin<DigitalPin> ("led").Write (false);
			}
		}
	}
}
```
