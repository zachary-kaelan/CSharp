using System;
using System.Drawing;

namespace TaskBarApp.Objects
{
	public class MMSImage
	{
		public Image image
		{
			get;
			set;
		}

		public string mimeType
		{
			get;
			set;
		}

		public string ext
		{
			get;
			set;
		}

		public long messageID
		{
			get;
			set;
		}

		public string storageKey
		{
			get;
			set;
		}
	}
}
