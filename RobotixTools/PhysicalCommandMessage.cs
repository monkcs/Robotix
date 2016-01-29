using System;

namespace Robotix
{
	public class PhysicalCommandMessage
	{
		public MessageType Message {get;}
		public Action WriteMessage;
		public string Identifier { get;}
		public Type TypeOfObject;
		public object ValueObject;

		public PhysicalCommandMessage (MessageType message,Action writeMessage,string identifier, Type typeOfObject, Object valueObject)
		{
			Message = message;
			WriteMessage = writeMessage;
			Identifier = identifier;
			TypeOfObject = typeOfObject;
			ValueObject = valueObject;
		}
		enum MessageType
		{
			/// <summary>
			/// Information to show in the console
			/// </summary>
			Information,
			/// <summary>
			/// Value from robotix object
			/// </summary>
			RawData

		}
	}
}

