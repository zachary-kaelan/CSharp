using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class TextMessageAttachmentList
	{
		public int total
		{
			get;
			set;
		}

		public List<TextMessageAttachment> response
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

		public int size
		{
			get;
			set;
		}
	}
}
