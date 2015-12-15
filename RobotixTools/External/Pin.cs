using System;
using WiringPiSharp;
using static WiringPiSharp.WiringPi;

namespace Robotix.External
{
    /// <summary>
    /// Containing a pinstate, when passed to a class attached to IPhysicalCommandCommunication
    /// will apply to a physical GPIO pin.
    /// </summary>
    public class DigitalPin : GpioItem, IDisposable
    {
        /// <summary>
        /// The initial value for the pin
        /// </summary>
        protected readonly bool InitialValue;

        /// <summary>
        /// The direction of the pin
        /// </summary>
        public readonly PinMode Direction;

        /// <summary>
        /// The pin communicating with the memory
        /// Change to enum later
        /// </summary>
        public readonly WPiPinout PhysicalPin;

        /// <summary>
        /// The current state of the item
        /// </summary>
        public bool CurrentState { get; protected set; }

        /// <summary>
        /// True if the runtime should poll the pin for changes. Pin have to be added to the polling engine.
        /// If turned off you will not get a indication if the pin "just changed" to a state
        /// </summary>
        public bool PollingAvalible { get; set; } = false;

        /// <summary>
        /// Containing a keystate
        /// </summary>
        public DigitalPin(WPiPinout pin, PinMode direction, bool initialValue) : this(pin, direction, initialValue, null, PullMode.Off) { }
        /// <summary>
        /// Containing a keystate
        /// </summary>
        public DigitalPin(WPiPinout pin, PinMode direction, bool initialValue, string friendlyName, PullMode resistor)
        {
            PhysicalPin = pin;
            FriendlyName = friendlyName;
            InitialValue = initialValue;
            Direction = direction;
            GPIO.PinMode(pin, direction);

            if (direction == PinMode.Input)
            {
                GPIO.PullUpDnControl(pin, resistor);
            }
        }

        /// <summary>
        /// Update the current value for the pin, used by the polling functions. Returns true if value changed from last scan
        /// </summary>
        public bool PollingUpdate()
        {
            bool temp = CurrentState;
            CurrentState = GPIO.DigitalReading(PhysicalPin);
            if (CurrentState != temp)
            {
                JustChanged = true;
            }
            else
            {
                JustChanged = false;
            }
            return JustChanged;
        }

        /// <summary>
        /// Returns the value of the pin. True for high. If the pin are polled, the value will be the cached state
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (!PollingAvalible)
            {
                if (Direction == PinMode.Input)
                {
                    return GPIO.DigitalReading(PhysicalPin);
                }
                else
                {
                    return CurrentState;
                }
            }
            else
            {
                return CurrentState;
            }
        }

        /// <summary>
        /// Toggles the value for the pin
        /// </summary>
        public void Write()
        {
            Write(!CurrentState);
        }
        /// <summary>
        /// Writes the value high or low to the pin. True for high
        /// </summary>
        /// <param name="value">Value to write</param>
        public void Write(bool value)
        {
            if (Direction == PinMode.Output)
            {
                GPIO.DigitalWrite(PhysicalPin, value);
                CurrentState = value;
                JustChanged = true;
            }
        }


        /// <summary>
        /// Checking if the pin has the specified value and just changed to that value. Returns <code>true</code> if a match is found. Will use cached data.
        /// </summary>
        /// <param name="value">The value of the pin. True for high</param>
        /// <returns></returns>
        public bool JustChangedTo(bool value)
        {
            if (value == CurrentState && JustChanged == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (Direction == PinMode.Output)
                {
                    Write(false);
                }
            } catch { }
        }
    }
}