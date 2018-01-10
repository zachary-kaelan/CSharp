using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class TextMessageSendResponseDetails
	{
		public string @class
		{
			get;
			set;
		}

		public string fingerprint
		{
			get;
			set;
		}

		public string root
		{
			get;
			set;
		}

		public List<TextMessageSendResponseToken> tokens
		{
			get;
			set;
		}
	}
}
