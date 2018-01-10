using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class Session
	{
		public List<TextMessage> message
		{
			get;
			set;
		}

		public string session
		{
			get;
			set;
		}

		public List<Conversation> conversation
		{
			get;
			set;
		}

		public List<Contact> contact
		{
			get;
			set;
		}
	}
}
