using System;

namespace TaskBarApp.Objects
{
	public class ConversationGet
	{
		public ConversationResponse response
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
