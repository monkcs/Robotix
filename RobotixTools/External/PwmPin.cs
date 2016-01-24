using System;
using WiringPiSharp;

namespace Robotix.External
{
	/// <summary>
	/// Containing a pinstate for a pwm pin, when passed to a class attached to IPhysicalCommandCommunication
	/// will apply to a physical GPIO pin.
	/// </summary>
	public class PwmPin : DigitalPin, IDisposable
	{
		/// <summary>
		/// Containing a pinstate for PWM pin
		/// </summary>
		/// <param name="pin">The pin communicating with the memory</param>
		/// <param name="value"></param>
		/// <param name="range"></param>
		/// <param name="friendlyName">A friendly name for the pin</param> 
		public PwmPin (WiringPi.WPiPinout pin, int value, int range, string friendlyName)
		{
			base.SetupProperties (pin, WiringPi.PinMode.PwmOutput, false, friendlyName);
			WiringPiSharp.SoftPwm.SoftPwmCreate (pin, value, range);
		}
	}
}

