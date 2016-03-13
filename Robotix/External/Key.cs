//﻿using FastSerializer;
using System;
using System.Runtime.Serialization;

namespace Robotix.External
{

    /// <summary>
    /// Containing a keystate, when passed to a class attached to IPhysicalCommandCommunication, it will act according to instructions for that key.
    /// </summary>
    [Serializable]
    public class Key : GpioItem
    {
        /// <summary>
        /// The current key
        /// </summary>
        public ConsoleKey CurrentKey { get; set; }
        /// <summary>
        /// The current state of the item. Ture for Active value
        /// </summary>
        public bool CurrentState { get; set; }

        /// <summary>
        /// Containing a keystate, when passed to a class attached to IPhysicalCommandCommunication, it will act according to instructions for that key.
        /// </summary>
        public Key() { }
    }
}
