using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class ContactListResponse
	{
		public int total
		{
			get;
			set;
		}

		public List<Contact> response
		{
			get;
			set;
		}

		public object sessions
		{
			get;
			set;
		}

		public int page
		{
			get;
			set;
		}

		public int pages
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
