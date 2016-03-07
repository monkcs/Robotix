//﻿using FastSerializer;
using System;
using System.Runtime.Serialization;

namespace Robotix.External
{
    /// <summary>
    /// Baseclass for I/O objects like keys and pins
    /// </summary>
    [Serializable]
    public abstract class GpioItem
	{
        [NonSerialized]
        private string _friendlyName = "";
        /// <summary>
        /// A friendly name for easy remembering of item
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if (_friendlyName != "")
                {
                    return _friendlyName;
                }
                else
                {
                    // Set a random displayname for the item
                    _friendlyName = GetType().ToString() + new Random().Next(500).ToString();
                    return _friendlyName;
                }
            }
            set
            {
                if (value != null)
                {
                    _friendlyName = value;
                }
            }
        }
        /// <summary>
        /// Holding value if the state of the item just changed
        /// </summary>
        public bool JustChanged { get; set; }

        /// <summary>
        /// Baseclass for I/O objects like keys and pins
        /// </summary>
        public GpioItem()
        {

        }
    }
}
