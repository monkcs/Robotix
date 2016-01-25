using System;
using WiringPiSharp;

namespace Robotix.External
{
	/// <summary>
	/// Containing a pinstate for a pwm pin
	/// </summary>
	public class PwmPin : DigitalPin, IDisposable
	{
		public int PwmValue { get; protected set; } = 0;
		public int PwmRange { get; protected set; } = 0;

		/// <summary>
		/// Containing a pinstate for PWM pin
		/// </summary>
		/// <param name="pin">The pin communicating with the memory</param>
		/// <param name="value"></param>
		/// <param name="range"></param>
		public PwmPin (WiringPi.WPiPinout pin, int value, int range) : this(pin,value,range,null) {}
		/// <summary>
		/// Containing a pinstate for PWM pin
		/// </summary>
		/// <param name="pin">The pin communicating with the memory</param>
		/// <param name="value"></param>
		/// <param name="range"></param>
		/// <param name="friendlyName">A friendly name for the pin</param> 
		public PwmPin (WiringPi.WPiPinout pin, int value, int range, string friendlyName) : base(pin,WiringPi.PinMode.PwmOutput,false,friendlyName,WiringPi.PullMode.Off)
		{
			PwmValue = value;
			PwmRange = range;
			WiringPiSharp.SoftPwm.SoftPwmCreate (pin, value, range);

		}

		/// <summary>
		/// Update the current value for the pin, used by the polling functions. Returns true if value changed from last scan
		/// </summary>
		public override bool PollingUpdate()
		{
			return JustChanged;
		}

		/// <summary>
		/// Returns the value of the pin. True for high. If the pin are polled, the value will be the cached state
		/// </summary>
		/// <returns></returns>
		public override bool Read()
		{
			return CurrentState;
		}
		/// <summary>
		/// Returns the pwm value of the pin
		/// </summary>
		/// <returns></returns>
		public virtual int ReadPwm()
		{
			return PwmValue;
		}

		/// <summary>
		/// Toggles the value for the pin between specified pwm value and off
		/// </summary>
		public override void Write()
		{
			Write(!CurrentState);
		}
		/// <summary>
		/// Writes the value high or low to the pin. True for specified pwm value, false for off
		/// </summary>
		/// <param name="value">Value to write</param>
		public override void Write(bool value)
		{
			if (value == true)
			{
				SoftPwm.SoftPwmWrite (PhysicalPin, PwmValue);
				CurrentState = value;
				JustChanged = true;
			}
			else
			{
				SoftPwm.SoftPwmWrite (PhysicalPin, 0);
				CurrentState = value;
				JustChanged = true;
			}
		}
		/// <summary>
		/// Writes the specified pwm value to the pin
		/// </summary>
		/// <param name="pwmValue">Value to write</param>
		public virtual void Write(int pwmValue)
		{
			SoftPwm.SoftPwmWrite (PhysicalPin, pwmValue);
			CurrentState = true;
			JustChanged = true;
		}
	}
}