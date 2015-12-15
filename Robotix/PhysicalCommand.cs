using System;
using System.Collections.Generic;
using System.Threading;
using Robotix.External;
using WiringPiSharp;
using static WiringPiSharp.WiringPi;

namespace Robotix
{
    /// <summary>
    /// Class control for the GPIO pin 
    /// </summary>
    public class PhysicalCommand : IDisposable
    {
        /// <summary>
        /// State changed in communication with physical equipment
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Cache holding just arrived data
        /// </summary>
        Cache<Key> IncomingKeyCache = new Cache<Key>();
        /// <summary>
        /// Cache holding just arrived data
        /// </summary>
        Cache<object> IncomingObjectCache = new Cache<object>();

        /// <summary>
        /// Data that are stored untill a new state arrive
        /// </summary>
        List<Key> CurrentValueKey = new List<Key>();
        /// <summary>
        /// Data that are stored untill a new state arrive
        /// </summary>
        List<object> CurrentValueObject = new List<object>();

        /// <summary>
        /// List of Pin's that are initiated
        /// </summary>
        List<DigitalPin> AvaliblePin = new List<DigitalPin>();

        /// <summary>
        /// Temp data holder for incoming data
        /// </summary>
        volatile List<Key> IncomingKey;

        /// <summary>
        /// The runner thread
        /// </summary>
        Thread Runner;

        /// <summary>
        /// Class control for the GPIO pin 
        /// </summary>
        public PhysicalCommand()
        {

        }

        /// <summary>
        /// Will initiate all pins. Place all your pin configuration here.
        /// </summary>
        protected virtual void Initiate()
        {
            Setup.WiringPiPiSetup();
            AvaliblePin.Add(new DigitalPin(WPiPinout.P0, PinMode.Output, true, "led1", PullMode.Off));
        }

        /// <summary>
        /// The main method for the robots logic
        /// Will run several times per second. Don't block this thread
        /// </summary>
        protected virtual void Update()
        {
            /*
            if (GetKeyJustChanged(ConsoleKey.A, true))
            {
                //  GetPin<SoftwarePWM>("led2").Start();
            }
            if (GetKeyJustChanged(ConsoleKey.S, true))
            {
                GetPin<DigitalPin>("led1").Write(true);
                

            }
            //*/
            GetPin<DigitalPin>("test").Read

        }

        #region Background Thread
        /// <summary>
        /// Start the polling and <code>UpdateRunner()</code>. Use <code>Stop()</code> to terminate the background thread
        /// </summary>
        public void Start()
        {
            IncomingKey = new List<Key>();
            Initiate();

            Runner = new Thread(() =>
                {
                    while (true)
                    {
                        UpdateRunner();
                    }
                });
            Runner.IsBackground = true;
            Runner.Start();
        }
        /// <summary>
        /// Stopping the background thread
        /// </summary>
        public void Stop()
        {
            Runner.Abort();
        }

        /// <summary>
        /// Collecting data from the cache, running the user logic, and polling GPIO pin values
        /// </summary>
        protected void UpdateRunner()
        {
            // Swap cache
            IncomingKey.Clear();
            IncomingKey.AddRange(IncomingKeyCache.GetCachedData());

            // Sets old key to not changed right now
            for (int counter = 0; counter < CurrentValueKey.Count; counter++)
            {
                CurrentValueKey[counter].JustChanged = false;
            }

            // Move new keys from cache
            foreach (Key item in IncomingKey)
            {
                Key temp = item;

                int index = CurrentValueKey.FindIndex(item2 => item2.CurrentKey == temp.CurrentKey);
                if (index != -1)
                {
                    if (CurrentValueKey[index].CurrentState != temp.CurrentState)
                    {
                        temp.JustChanged = true;
                    }
                    else
                    {
                        temp.JustChanged = false;
                    }
                    temp.FriendlyName = CurrentValueKey[index].FriendlyName;
                    CurrentValueKey.RemoveAt(index);
                }
                CurrentValueKey.Add(temp);
            }

            // Collect values from pins
            foreach (DigitalPin item in AvaliblePin)
            {
                if (item.PollingAvalible)
                {
                    item.PollingUpdate();
                }
            }

            // Run the update code
            Update();
        }
        #endregion

        #region Invoke methods
        /// <summary>
        /// Will execute the keystate according to instructions for that key
        /// </summary>
        /// <param name="keyToInvoke">The key to submit</param>
        public void Invoke(Key keyToInvoke)
        {
            keyToInvoke.JustChanged = true;
            IncomingKeyCache.Add(keyToInvoke);
        }
        #endregion

        #region Add / remove pins from AvalibleList
        /// <summary>
        /// Add a pin to the avalible list. Pin can't be polled if not added to this list. Will dispose and replace existing pin.
        /// </summary>
        /// <param name="pinToAdd">Pin to add</param>
        protected void Add<T>(T pinToAdd) where T : DigitalPin
        {
            int index = AvaliblePin.FindIndex(item2 => item2.PhysicalPin == pinToAdd.PhysicalPin);
            if (index != -1)
            {
                try
                {
                    if (AvaliblePin[index].Direction == PinMode.Output)
                    {
                        AvaliblePin[index].Write(false);
                    }
                    AvaliblePin[index].Dispose();
                }
                catch { }

                AvaliblePin.RemoveAt(index);
            }
            AvaliblePin.Add(pinToAdd);
        }
        /// <summary>
        /// Will remove a pin from the avalible list and dispose it. Will return true if pin exist at list. Use instead <code>Add(Pin)</code> if replacing pin
        /// </summary>
        /// <param name="pinToRemove"></param>
        protected bool Remove<T>(T pinToRemove) where T : DigitalPin
        {
            int index = AvaliblePin.FindIndex(item2 => item2.PhysicalPin == pinToRemove.PhysicalPin);
            if (index != -1)
            {
                AvaliblePin[index].Dispose();
                AvaliblePin.RemoveAt(index);
                return true;
            }
            return false;
        }
        #endregion

        #region Get value from keys

        /// <summary>
        /// Checking if the following key has the specified value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <param name="currentState">State of the key to check</param>
        /// <returns></returns>
        protected bool GetKey(ConsoleKey key, bool currentState)
        {
            return CurrentValueKey.Exists(element => element.CurrentKey == key && element.CurrentState == currentState);
        }

        /// <summary>
        /// Checking if the following key has the specified value and just changed to that value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <param name="currentState">State of the key to check</param>
        /// <returns></returns>
        protected bool GetKeyJustChanged(ConsoleKey key, bool currentState)
        {
            return IncomingKey.Exists(element => element.CurrentKey == key && element.CurrentState == currentState && element.JustChanged == true);
        }
        #endregion

        #region Get refrence to pin
        /// <summary>
        /// Returns the refrence to an object in AvaliblePin list
        /// </summary>
        /// <typeparam name="T">Pin or derived type of pin</typeparam>
        /// <param name="pin">Pin of the object</param>
        /// <returns></returns>
        protected T GetPin<T>(WPiPinout pin) where T : DigitalPin
        {
            try
            {
                return (T)AvaliblePin.Find(element => element.PhysicalPin == pin);
            }
            catch
            {
                throw new InvalidCastException("Not possible to find object with specified pin");
            }
        }
        /// <summary>
        /// Returns the refrence to an object in AvaliblePin list
        /// </summary>
        /// <typeparam name="T">Pin or derived type of pin</typeparam>
        /// <param name="friendlyName">Friendly name of the object</param>
        /// <returns></returns>
        protected T GetPin<T>(string friendlyName) where T : DigitalPin
        {
            try
            {
                return (T)AvaliblePin.Find(element => element.FriendlyName == friendlyName);
            }
            catch
            {
                throw new InvalidCastException("Not possible to find object with specified friendly name");
            }
        }
        #endregion

        /// <summary>
        /// Dispose all resources
        /// </summary>
        public void Dispose()
        {
            Stop();

            // Disposes all pins
            foreach (DigitalPin item in AvaliblePin)
            {
                item.Dispose();
            }
        }
    }
}
