using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class TextMessageList
	{
		public int total
		{
			get;
			set;
		}

		public List<TextMessage> response
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
