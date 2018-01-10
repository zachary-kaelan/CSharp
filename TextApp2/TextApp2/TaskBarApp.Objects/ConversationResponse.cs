using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class ConversationResponse
	{
		public Conversation conversation
		{
			get;
			set;
		}

		public List<TextMessage> messages
		{
			get;
			set;
		}
	}
}
