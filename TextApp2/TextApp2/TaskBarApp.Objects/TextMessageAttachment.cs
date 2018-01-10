using System;

namespace TaskBarApp.Objects
{
	public class TextMessageAttachment
	{
		public long fileSizeBytes
		{
			get;
			set;
		}

		public string mimeType
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
