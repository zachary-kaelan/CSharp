using System;

namespace TaskBarApp.Objects
{
	public class ConversationMetaData
	{
		public string fingerprint
		{
			get;
			set;
		}

		public bool lastMessageIsError
		{
			get;
			set;
		}

		public string lastMessageDirection
		{
			get;
			set;
		}

		public DateTime? lastMessageDate
		{
			get;
			set;
		}

		public long lastContactId
		{
			get;
			set;
		}
	}
}
