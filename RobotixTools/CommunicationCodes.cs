using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotix
{
    /// <summary>
    /// Will be used to match derived GpioItem for serialization, and for control codes for the server connection
    /// </summary>
    public enum CommunicationCodes
    {
        // CC - Control codes no. 0 - 59 //

        /// <summary>
        /// Message that all is good
        /// </summary>
        CCAllGood = 0,
        /// <summary>
        /// Message that a fatal error has occurred
        /// </summary>
        CCFatalError = 1,
        /// <summary>
        /// That the data containing a RobotixMessage
        /// </summary>
        CCMessageFromRobotix = 2,

        /// <summary>
        /// Message that a fatal error occurred in Robotix, and that the runtime will restart robotix in 60 seconds, if not the user return 'false'
        /// </summary>
        CCRequestingRebootRobotix = 3,
        /// <summary>
        /// Message that a fatal error occurred in Robotix, and that the runtime will restart itselves in 60 seconds, if not the user return 'false'
        /// </summary>
        CCRequestingRebootProgram = 4,
        /// <summary>
        /// Message that a fatal error occurred in Robotix, and that the runtime will restart the machine in 60 seconds, if not the user return 'false'
        /// </summary>
        CCRequestingRebootMachine = 5,

        /// <summary>
        /// Message that the runtime immediately should stop network communication and start automatic control program
        /// </summary>
        CCStopNetworkCommunication = 6,

        // GI - Match derived GpioItem //

        GIGpioItem = 60,
        GIKey = 61,
        GIPin = 62,
    }
}
