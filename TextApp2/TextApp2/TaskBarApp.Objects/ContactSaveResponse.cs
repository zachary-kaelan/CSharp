using System;

namespace TaskBarApp.Objects
{
	public class ContactSaveResponse
	{
		public Contact response
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
	}
}
