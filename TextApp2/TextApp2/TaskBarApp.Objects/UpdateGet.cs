using System;
using System.Collections.Generic;

namespace TaskBarApp.Objects
{
	public class UpdateGet
	{
		public object response
		{
			get;
			set;
		}

		public List<Session> sessions
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
