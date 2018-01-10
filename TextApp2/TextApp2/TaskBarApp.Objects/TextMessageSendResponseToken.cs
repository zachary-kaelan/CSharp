using System;

namespace TaskBarApp.Objects
{
	public class TextMessageSendResponseToken
	{
		public string @class
		{
			get;
			set;
		}

		public long contact
		{
			get;
			set;
		}

		public long device
		{
			get;
			set;
		}

		public string fingerprint
		{
			get;
			set;
		}

		public string message
		{
			get;
			set;
		}
	}
}
