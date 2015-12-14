using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspberryPiDotNet;
using System.Threading;

namespace Robotix.External
{
    /// <summary>
    /// Class handling software based pulse width modulation (PWM)
    /// </summary>
    public class SoftwarePWM : Pin, IDisposable
    {
        protected Thread Pwm;

        /// <summary>
        /// The waiting time after setting pin to "on"
        /// </summary>
        protected int WaitTime1 = 0;
        /// <summary>
        /// The waiting time after setting pin to "off"
        /// </summary>
        protected int WaitTime2 = 0;

        protected float _frequency = 0f;
        /// <summary>
        /// The frequency of the pulse in milliseconds
        /// </summary>
        public float Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                WaitTime1 = (int)((1000 / Frequency) * value);
                WaitTime2 = (int)((1000 / Frequency) * (1 - value));
            }
        }

        protected float _width = 0f;
        /// <summary>
        /// The width of the "on" part of each pulse in procent of each pulse
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                WaitTime1 = (int)((1000 / Frequency) * value);
                WaitTime2 = (int)((1000 / Frequency) * (1 - value));
            }
        }

        /// <summary>
        /// Class handling software based pulse width modulation (PWM)
        /// Use Start() to start the PWM
        /// </summary>
        /// <param name="pin">The pin to use PWM on</param>
        public SoftwarePWM(GPIOPins pin) : this(pin, false) { }
        /// <summary>
        /// Class handling software based pulse width modulation (PWM)
        /// Use Start() to start the PWM
        /// </summary>
        /// <param name="pin">The pin to use PWM on</param>
        /// <param name="initialValue">The initial value for the pin</param>
        public SoftwarePWM(GPIOPins pin, bool initialValue) : this(pin, 0f, 0f, initialValue, null) { }
        /// <summary>
        /// Class handling software based pulse width modulation (PWM)
        /// Use Start() to start the PWM
        /// </summary>
        /// <param name="pin">The pin to use PWM on</param>
        /// <param name="frequency">The frequency of the pulse in milliseconds</param>
        /// <param name="width">The width of the "on" part of each pulse</param>
        /// <param name="friendlyName">A friendly name for the pin</param>
        /// <param name="initialValue">The initial value for the pin</param>
        public SoftwarePWM(GPIOPins pin, float frequency, float width, bool initialValue, string friendlyName) : base(pin, GPIODirection.Out, initialValue, friendlyName)
        {
            Frequency = frequency;
            Width = width;
            AvalibleForPolling = false;

            Pwm = new Thread(() =>
            {
                while (true)
                {
                    PhysicalPin.Write(true);
                    Thread.Sleep(WaitTime1);
                    PhysicalPin.Write(false);
                    Thread.Sleep(WaitTime2);
                }
            });
        }

        /// <summary>
        /// Start the PWM
        /// </summary>
        public void Start()
        {
            Pwm.Start();
        }
        /// <summary>
        /// Stop the PWM
        /// </summary>
        public void Stop()
        {
            Pwm.Abort();
            PhysicalPin.Write(InitialValue);
        }
    }
}
