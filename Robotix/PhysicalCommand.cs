using System;
using System.Collections.Generic;
using System.Threading;
using Robotix.External;
using WiringPiSharp;

namespace Robotix
{
    /// <summary>
    /// Class control for the GPIO pin 
    /// </summary>
    public class PhysicalCommand : IDisposable
    {
		/// <summary>
		/// The delegate for event when an state change in the physival command
		/// </summary>
		public delegate void RawDataEventHandler(object sender, GpioItem e);

        /// <summary>
        /// State changed in communication with physical equipment
        /// </summary>
		public event RawDataEventHandler GpioStates;
		/// <summary>
		/// Use this method to invoke GpioStates event from derived class
		/// </summary>
		protected void OnGpioStatesChanged(GpioItem eventArgs)
		{
			RawDataEventHandler handler = GpioStates;
			if (handler != null) {
				handler (this, eventArgs);
			}
		}

		/// <summary>
		/// Exceptions from Robotix
		/// </summary>
		public event UnhandledExceptionEventHandler Exceptions;
		/// <summary>
		/// Use this method to invoke Exceptions event from derived class
		/// </summary>
		protected void OnExceptionsRaised(UnhandledExceptionEventArgs eventArgs)
		{
			UnhandledExceptionEventHandler handler = Exceptions;
			if (handler != null) {
				handler (this, eventArgs);
			}
		}

		/// <summary>
		/// Gets a Boolean value indicating whether<see cref="Robotix.PhysicalCommand"/>working thread is running.
		/// </summary>
		/// <value><c>true</c> if thread running; otherwise, <c>false</c>.</value>
		public bool ThreadRunning
		{
			get {
				if (Runner != null) {
					return Runner.IsAlive;
				} else {
					return false;
				}
			}
		}

        /// <summary>
        /// Cache holding just arrived data
        /// </summary>
        Cache<Key> IncomingKeyCache = new Cache<Key>();

        /// <summary>
        /// Data that are stored untill a new state arrive
        /// </summary>
        List<Key> CurrentValueKey = new List<Key>();

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
            try
            {
                Setup.WiringPiPiSetup();
            }
            catch (Exception e)
            {
				object temp = Exceptions;
				if (temp != null)
				{
					Exceptions.Invoke (this, new UnhandledExceptionEventArgs (e, false));
				}
            }
        }


        /// <summary>
        /// The main method for the robots logic
        /// Will run several times per second. Don't block this thread
        /// </summary>
        protected virtual void Update()
        {
            #region Example code

            #endregion
        }

        #region Background Thread
        /// <summary>
        /// Start the polling and <code>UpdateRunner()</code>. Use <code>Stop()</code> to terminate the background thread
        /// </summary>
        public void Start()
        {
            IncomingKey = new List<Key>();
			try
			{
				Initiate();
			}
			catch (Exception e)
			{
				object temp = Exceptions;
				if (temp != null)
					Exceptions.Invoke (this, new UnhandledExceptionEventArgs (e, true));
			}

            Runner = new Thread(() =>
                {
					try
					{
                    	while (true)
                    	{
                        	UpdateRunner();
						}
					}
					catch (Exception e)
					{
						object temp = Exceptions;
						if (temp != null)
						{
							Exceptions.Invoke (this, new UnhandledExceptionEventArgs (e, true));
						}
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
			try 
			{
            	Runner.Abort();
			} 
			catch { }
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
        /// Add a pin to the avalible list. Pin can't be polled if not added to this list. Will dispose and replace existing pin. Will set PollingAvalible to true
        /// </summary>
        /// <param name="pinToAdd">Pin to add</param>
        protected void Add<T>(T pinToAdd) where T : DigitalPin
        {
            Add<T>(pinToAdd, true);
        }
        /// <summary>
        /// Add a pin to the avalible list. Pin can't be polled if not added to this list. Will dispose and replace existing pin.
        /// </summary>
        /// <param name="pinToAdd">Pin to add</param>
        /// <param name="pollingAvalible">True if the pin should be avalible for polling</param>
        protected void Add<T>(T pinToAdd, bool pollingAvalible) where T : DigitalPin
        {
            pinToAdd.PollingAvalible = pollingAvalible;
            int index = AvaliblePin.FindIndex(item2 => item2.PhysicalPin == pinToAdd.PhysicalPin);
            if (index != -1)
            {
                try
                {
                    if (AvaliblePin[index].Direction == WiringPi.PinMode.Output)
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
        protected T GetPin<T>(WiringPi.WPiPinout pin) where T : DigitalPin
		{
			T temp = (T)AvaliblePin.Find (element => element.PhysicalPin == pin);
			if (temp != null) {
				return temp;
			} else {

				object tempException = Exceptions;
				if (tempException != null) {
					Exceptions.Invoke (this,
						new UnhandledExceptionEventArgs (
							new Exception ("Not possible to find pin \"" + pin.ToString () + "\""), false));
				}
				return null;
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
				T temp = (T)AvaliblePin.Find(element => element.FriendlyName == friendlyName);
				if (temp != null)
				{
					return temp;
				}
				else 
				{
					throw new Exception();
				}
			}
			catch
			{
				object temp = Exceptions;
				if (temp != null)
					Exceptions.Invoke (this,
						new UnhandledExceptionEventArgs (
							new Exception ("Not possible to find pin \"" + friendlyName + "\""), false));
				return null;
			}
        }
        #endregion

        /// <summary>
        /// Dispose all resources
        /// </summary>
        public void Dispose()
		{
			Stop ();

			try
			{
				// Disposes all pins
				foreach (DigitalPin item in AvaliblePin) {
					if (item != null) {
						item.Dispose ();
					}
				}
			}
			catch {
			}
		}
    }
}
