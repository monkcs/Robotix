using System;

namespace Robotix.External
{
    /// <summary>
    /// Baseclass for I/O objects like keys and pins
    /// </summary>
    public abstract class GpioItem
	{
		/// <summary>
		/// The hardware lockout. If a gpioitem is passed out of the PhysicalCommand class, the lock should be set to <code>true</code>, to prevent null refrence call by outside code.
		/// </summary>
		protected bool _hardwareLockout = false;

		/// <summary>
		/// The hardware lockout. If a gpioitem is passed out of the PhysicalCommand class, the lock should be set to <code>true</code>, to prevent null refrence call by outside code.
		/// It is not possible to revers a call to set HardwareLockout back to false
		/// </summary>
		/// <value><c>true</c> if hardware lockout; otherwise, <c>false</c>.</value>
		public bool HardwareLockout {
			get {
				return _hardwareLockout;
			}
			set {
				if (_hardwareLockout != true) {
					_hardwareLockout = value;
				} 
			}
		}

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
