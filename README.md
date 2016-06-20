Robotix
=======

A .NET library to make easy use of Raspberry Pi GPIO pins, by manage all parts necessary to get input and output from the GPIO.
It encapsulates the pins into easy-to-use objects, polling the input pins, and also support "key objects" based of  `System.Console.ConsoleKey` for control by a user.

The Robotix library uses the C# wrapper [WiringPiSharp](https://github.com/monkcs/WiringPiSharp) to access and call Gordon's [WiringPi](http://wiringpi.com "WiringPi homepage") library written in C.

Functionally
------------
The pin classes gives you abillity to without low-level coding use and access following listed devices and protocol
#####Currently Robotix contains classes for following pintype:

- [x] Digital pin (simply a switch true/false, or input true/false)
- [x] PWM pin (output pulse-width modulation signals. [Wiki](https://en.wikipedia.org/wiki/Pulse-width_modulation))
  - [x] (In work) Servo control
- [x] SPI pins (Serial Peripheral Interface Bus. [Wiki](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus))
  - [ ] (In work) 7-segment displays ([MAX7219, MAX7221](https://www.maximintegrated.com/en/products/power/display-power-control/MAX7221.html))
- [ ] I²C pins (Inter-Integrated Circuit. [Wiki](https://en.wikipedia.org/wiki/I%C2%B2C))

Example code
------------

This example will setup a pin for input named *button* and a output pin named *led*

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
