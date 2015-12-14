using RaspberryPiDotNet;
using System;

namespace Robotix.External
{
    /// <summary>
    /// Containing a pinstate, when passed to a class attached to IPhysicalCommandCommunication
    /// will apply to a physical GPIO pin.
    /// </summary>
    public class Pin : GpioItem, IDisposable
    {
        /// <summary>
        /// The initial value for the pin
        /// </summary>
        protected bool InitialValue;

        /// <summary>
        /// The pin communicating with the memory
        /// </summary>
        public GPIOMem PhysicalPin { get; protected set; }
        /// <summary>
        /// The current state of the item
        /// </summary>
        public virtual PinState CurrentState { get; protected set; }
        /// <summary>
        /// True if the runtime should poll the pin for changes.
        /// Turn it off to prevent lag on timecritic applications. 
        /// If turned off you will not get a value from the <code>GetValue()</code> methods, you have to use the native library read method
        /// </summary>
        public bool AvalibleForPolling { get; set; } = true;

        /// <summary>
        /// Containing a keystate
        /// </summary>
        public Pin(GPIOPins pin, GPIODirection direction, bool initialValue) : this(pin, direction, initialValue, null) { }
        /// <summary>
        /// Containing a keystate
        /// </summary>
        public Pin(GPIOPins pin, GPIODirection direction, bool initialValue, string friendlyName)
        {
            PhysicalPin = new GPIOMem(pin, direction, initialValue);
            FriendlyName = friendlyName;
            InitialValue = initialValue;
        }

        /// <summary>
        /// Update the current value for the pin. Returns true if value changed from last scan
        /// </summary>
        public bool ReadValue()
        {
            if (PhysicalPin != null)
            {
                PinState temp = CurrentState;
                CurrentState = PhysicalPin.Read();
                if (CurrentState != temp)
                {
                    JustChanged = true;
                }
                else
                {
                    JustChanged = false;
                }
            }
            return JustChanged;
        }

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (PhysicalPin.PinDirection == GPIODirection.Out)
                {
                    PhysicalPin.Write(false);
                }
                if (!PhysicalPin.IsDisposed)
                {
                    PhysicalPin.Dispose();
                }
            } catch { }
        }
    }
}