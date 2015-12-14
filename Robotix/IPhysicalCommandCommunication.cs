using System;
using Robotix.External;

namespace Robotix
{
    /// <summary>
    /// Handler for communication between the remote control software and GPIO logic
    /// </summary>
    public interface IPhysicalCommandCommunication
    {
        /// <summary>
        /// State changed in communication with physical equipment
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Will execute the keystate according to instructions for that key
        /// </summary>
        /// <param name="keyToInvoke">The key to submit</param>
        void Invoke(Key keyToInvoke);

        /// <summary>
        /// Will submit the object to the qualified handler. May not work properly. Do not use if possible
        /// </summary>
        /// <param name="rawData">The object to submit</param>
        void Invoke(object rawData);

        /// <summary>
        /// Start the polling and <code>UpdateRunner()</code>
        /// </summary>
        void Start();

        /// <summary>
        /// Stopping the background thread
        /// </summary>
        void Stop();

        /// <summary>
        /// Dispose all resources
        /// </summary>
        void Dispose();
    }
}
