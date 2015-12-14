using FastSerializer;
using System;
using System.Runtime.Serialization;

namespace Robotix.External
{

    /// <summary>
    /// Containing a keystate, when passed to a class attached to IPhysicalCommandCommunication, it will act according to instructions for that key.
    /// </summary>
    [Serializable]
    public class Key : GpioItem, ISerializable
    {
        /// <summary>
        /// The current key
        /// </summary>
        public ConsoleKey CurrentKey { get; set; }
        /// <summary>
        /// The current state of the item
        /// </summary>
        public State CurrentState { get; set; }

        /// <summary>
        /// Containing a keystate, when passed to a class attached to IPhysicalCommandCommunication, it will act according to instructions for that key.
        /// </summary>
        public Key() { }
        public Key(SerializationInfo info, StreamingContext ctxt)
        {
            SerializationReader sr = SerializationReader.GetReader(info);
            FriendlyName = sr.ReadString();
            JustChanged = sr.ReadBoolean();

            CurrentKey = (ConsoleKey)sr.ReadInt32();
            CurrentState = (State)sr.ReadInt32();

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter sw = SerializationWriter.GetWriter();

            sw.Write(FriendlyName);
            sw.Write(JustChanged);

            sw.Write((int)CurrentKey);
            sw.Write((int)CurrentState);

            sw.AddToInfo(info);
        }
    }
}