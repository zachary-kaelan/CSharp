using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class AccountItem
	{
		public string session
		{
			get;
			set;
		}

		public string password
		{
			get;
			set;
		}

		public string countryCode
		{
			get;
			set;
		}

		public string number
		{
			get;
			set;
		}

		public string title
		{
			get;
			set;
		}

		public List<TextMessage> unReadMessageList
		{
			get;
			set;
		}

		public DateTime? lastSyncDate
		{
			get;
			set;
		}

		public string connectionStatus
		{
			get;
			set;
		}

		public string messagePeek
		{
			get;
			set;
		}
	}
}
