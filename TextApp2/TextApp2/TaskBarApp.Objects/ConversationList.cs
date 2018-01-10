using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class ConversationList
	{
		public int total
		{
			get;
			set;
		}

		public List<Conversation> response
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

		public long size
		{
			get;
			set;
		}
	}
}
