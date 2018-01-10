using System;

namespace TaskBarApp.Objects
{
	public class TextMessageSendResponse
	{
		public TextMessageSendResponseDetails response
		{
			get;
			set;
		}

		public object sessions
		{
			get;
			set;
		}

		public bool success
		{
			get;
			set;
		}
	}
}
