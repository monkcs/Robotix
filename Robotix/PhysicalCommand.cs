using System;
using System.Collections.Generic;
using System.Threading;
using RaspberryPiDotNet;
using Robotix.External;

namespace Robotix
{
    /// <summary>
    /// Class control for the GPIO pin
    /// </summary>
    public class PhysicalCommand : IPhysicalCommandCommunication, IDisposable
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
        /// List of Pin's and that are initiated
        /// </summary>
        List<Pin> AvaliblePin = new List<Pin>();

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
            AvaliblePin.Add(new Pin(GPIOPins.V2_GPIO_17, GPIODirection.Out, true, "led1"));
            AvaliblePin.Add(new SoftwarePWM(GPIOPins.V2_GPIO_18, 100f, 0.2f, false, "led2"));
        }

        /// <summary>
        /// The main method for the robots logic
        /// Will run several times per second. Don't block this thread
        /// </summary>
        protected virtual void Update()
        {
            if (GetValueIfJustChanged(ConsoleKey.A, State.Active))
            {
                GetPin<SoftwarePWM>("led2").Start();
                /*
                new Thread(() =>
                {
                    while (true)
                    {

                        for (float i = 0f; i <= 0.9f; i = i + 0.01f)
                        {
                            Thread.Sleep(5);
                            duty = i;
                            wait1 = (int)((1000 / frequency) * duty);
                            //Console.WriteLine(duty);
                            //Console.ReadKey(true);
                        }
                        for (float i = 0.9f; i >= 0f; i = i - 0.01f)
                        {
                            Thread.Sleep(5);
                            duty = i;
                            wait1 = (int)((1000 / frequency) * (1 - duty));
                        }
                    }
                }).Start();
                Console.WriteLine("Nu kör vi!");
                //*/
            }
            if (GetValueIfJustChanged(ConsoleKey.S, State.Active))
            {
                SetValue("led", PinState.Low);
            }
        }

        #region Background Thread
        /// <summary>
        /// Start the polling and <code>UpdateRunner()</code>. Use <code>Stop()</code> to terminate the background thread
        /// </summary>
        public void Start()
        {
            Runner = new Thread(() =>
                {
                    IncomingKey = new List<Key>();
                    Initiate();

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

            for (int counter = 0; counter < CurrentValueKey.Count; counter++)
            {
                CurrentValueKey[counter].JustChanged = false;
            }

            // Move new pins and keys from cache
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

            // Collect values from Pins
            foreach (Pin item in AvaliblePin)
            {
                if (item.AvalibleForPolling)
                {
                    item.ReadValue();
                }
            }

            // Run the update code
            Update();
        }
        #endregion

        #region Invoke methods
        /// <summary>
        /// Will submit the object to the qualified handler. May not work properly. Do not use if possible
        /// </summary>
        /// <param name="rawData">The object to submit</param>
        public void Invoke(object rawData)
        {
            throw new NotImplementedException();
        }

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

        #region Add/Remove pins
        /// <summary>
        /// Add a pin to the avalible list. Pin can't be controlled if not added to this list. Will dispose and replace existing pin.
        /// </summary>
        /// <param name="pinToAdd"></param>
        protected void Add(Pin pinToAdd)
        {
            int index = AvaliblePin.FindIndex(item2 => item2.PhysicalPin.Pin == pinToAdd.PhysicalPin.Pin);
            if (index != -1)
            {
                try
                {
                    if (AvaliblePin[index].PhysicalPin.PinDirection == GPIODirection.Out)
                    {
                        AvaliblePin[index].PhysicalPin.Write(false);
                    }
                    AvaliblePin[index].PhysicalPin.Dispose();
                } catch { }

                AvaliblePin.RemoveAt(index);
            }
            AvaliblePin.Add(pinToAdd);
        }
        /// <summary>
        /// Will remove a pin from the avalible list and dispose it. Will return true if pin exist at list. Use instead <code>Add(Pin)</code> if replacing pin
        /// </summary>
        /// <param name="pinToRemove"></param>
        protected bool Remove(Pin pinToRemove)
        {
            int index = AvaliblePin.FindIndex(item2 => item2.PhysicalPin.Pin == pinToRemove.PhysicalPin.Pin);
            if (index != -1)
            {
                AvaliblePin[index].Dispose();
                AvaliblePin.RemoveAt(index);
                return true;
            }
            return false;
        }
        #endregion

        #region Get/Set value from keys/pins

        /// <summary>
        /// Checking if the following key has the specified value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <param name="currentState">State of the key to check</param>
        /// <returns></returns>
        protected bool GetValue(ConsoleKey key, State currentState)
        {
            return CurrentValueKey.Exists(element => element.CurrentKey == key && element.CurrentState == currentState);
        }
        /// <summary>
        /// Checking if the following pin has the specified value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="pin">Pin to check</param>
        /// <param name="currentState">State of the pin to check</param>
        /// <returns></returns>
        protected bool GetValue(GPIOPins pin, PinState currentState)
        {
            return AvaliblePin.Exists(element => element.PhysicalPin.Pin == pin && element.CurrentState == currentState);
        }
        /// <summary>
        /// Checking if the following item has the specified value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="friendlyName">Item to check</param>
        /// <param name="currentState">State of the item to check</param>
        /// <returns></returns>
        protected bool GetValue<T>(string friendlyName, State currentState) where T : Key
        {
            return CurrentValueKey.Exists(element => element.FriendlyName == friendlyName && element.CurrentState == currentState);
        }
        /// <summary>
        /// Checking if the following item has the specified value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="friendlyName">Item to check</param>
        /// <param name="currentState">State of the item to check</param>
        /// <returns></returns>
        protected bool GetValue<T>(string friendlyName, PinState currentState) where T : Pin
        {
            return AvaliblePin.Exists(element => element.FriendlyName == friendlyName && element.CurrentState == currentState);
        }


        /// <summary>
        /// Checking if the following key has the specified value and just changed to that value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <param name="currentState">State of the key to check</param>
        /// <returns></returns>
        protected bool GetValueIfJustChanged(ConsoleKey key, State currentState)
        {
            return IncomingKey.Exists(element => element.CurrentKey == key && element.CurrentState == currentState && element.JustChanged == true);
        }
        /// <summary>
        /// Checking if the following pin has the specified value and just changed to that value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <param name="pin">Pin to check</param>
        /// <param name="currentState">State of the pin to check</param>
        /// <returns></returns>
        protected bool GetValueIfJustChanged(GPIOPins pin, PinState currentState)
        {
            return AvaliblePin.Exists(element => element.PhysicalPin.Pin == pin && element.CurrentState == currentState && element.JustChanged == true);
        }
        /// <summary>
        /// Checking if the following item has the specified value and just changed to that value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <typeparam name="T">Object derived from a Pin</typeparam>
        /// <param name="friendlyName">Item to check</param>
        /// <param name="currentState">State of the item to check</param>
        /// <returns></returns>
        protected bool GetValueIfJustChanged<T>(string friendlyName, PinState currentState) where T : Pin
        {
            return AvaliblePin.Exists(element => element.FriendlyName == friendlyName && element.CurrentState == currentState && element.JustChanged == true);
        }
        /// <summary>
        /// Checking if the following item has the specified value and just changed to that value. Returns <code>true</code> if a match is found
        /// </summary>
        /// <typeparam name="T">Object derived from a Key</typeparam>
        /// <param name="friendlyName">Item to check</param>
        /// <param name="currentState">State of the item to check</param>
        /// <returns></returns>
        protected bool GetValueIfJustChanged<T>(string friendlyName, State currentState) where T : Key
        {
            return IncomingKey.Exists(element => element.FriendlyName == friendlyName && element.CurrentState == currentState && element.JustChanged == true);
        }

        /// <summary>
        /// Returns the refrence to an object in AvaliblePin list
        /// </summary>
        /// <typeparam name="T">Pin or derived type of pin</typeparam>
        /// <param name="pin">Pin of the object</param>
        /// <returns></returns>
        protected T GetPin<T>(GPIOPins pin) where T :Pin
        {
            try
            {
                return (T)(object)AvaliblePin.Find(element => element.PhysicalPin.Pin == pin);
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
        protected T GetPin<T>(string friendlyName) where T : Pin
        {
            try
            {
                return (T)(object)AvaliblePin.Find(element => element.FriendlyName == friendlyName);
            }
            catch
            {
                throw new InvalidCastException("Not possible to find object with specified friendly name");
            }
        }


        /// <summary>
        /// Will change the specified pin if it is initiated and Direction is out
        /// </summary>
        /// <param name="pin">Pin to change state of</param>
        /// <param name="currentState">The state to change to</param>
        protected void SetValue(GPIOPins pin, PinState currentState)
        {
            try
            {
                AvaliblePin.Find(element => element.PhysicalPin.Pin == pin && element.PhysicalPin.PinDirection == GPIODirection.Out).PhysicalPin.Write(currentState);
            }
            catch
            {
                throw new NullReferenceException("Not possible to find pin in cache. Pin must be initiated in order to be stored in the cache");
            }
        }
        /// <summary>
        /// Will change the specified pin based from a friendly name
        /// </summary>
        /// <param name="friendlyName">Pin to change state of</param>
        /// <param name="currentState">The state to change to</param>
        protected void SetValue(string friendlyName, PinState currentState)
        {
            try
            {
                AvaliblePin.Find(element => element.FriendlyName == friendlyName && element.PhysicalPin.PinDirection == GPIODirection.Out).PhysicalPin.Write(currentState);
            }
            catch
            {
                throw new NullReferenceException("Not possible to find pin in cache. Pin must be initiated in order to be stored in the cache");
            }
        }

        /// <summary>
        /// Will change the specified pin if it is initiated
        /// </summary>
        /// <param name="pin">Pin to change state of</param>
        protected void SetValueSwap(GPIOPins pin)
        {
            try
            {
                Pin temp = AvaliblePin.Find(element => element.PhysicalPin.Pin == pin && element.PhysicalPin.PinDirection == GPIODirection.Out);
                if(temp.CurrentState == PinState.High)
                {
                    temp.PhysicalPin.Write(PinState.Low);
                }
                else
                {
                    temp.PhysicalPin.Write(PinState.High);
                }
            }
            catch
            {
                throw new NullReferenceException("Not possible to find pin in cache. Pin must be initiated in order to be stored in the cache");
            }
        }
        /// <summary>
        /// Will change the specified pin if it is initiated
        /// </summary>
        /// <param name="pinToChange">Pin to change state of. Will need to have a refrence to the physical pin</param>
        protected void SetValueSwap(Pin pinToChange)
        {
            SetValueSwap(pinToChange.PhysicalPin.Pin);
        }
        /// <summary>
        /// Will change the specified pin based from a friendly name
        /// </summary>
        /// <param name="friendlyName">Pin to change state of</param>
        protected void SetValueSwap(string friendlyName)
        {
            try
            {
                Pin temp = AvaliblePin.Find(element => element.FriendlyName == friendlyName && element.PhysicalPin.PinDirection == GPIODirection.Out);
                if (temp.CurrentState == PinState.High)
                {
                    temp.PhysicalPin.Write(PinState.Low);
                }
                else
                {
                    temp.PhysicalPin.Write(PinState.High);
                }
            }
            catch
            {
                throw new NullReferenceException("Not possible to find pin in cache. Pin must be initiated in order to be stored in the cache");
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
            foreach (Pin item in AvaliblePin)
            {
                item.Dispose();
            }
        }
    }
}
