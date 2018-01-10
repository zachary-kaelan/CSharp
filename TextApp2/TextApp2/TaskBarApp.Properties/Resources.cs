using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TaskBarApp.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	public class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("TaskBarApp.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		public static Icon blank
		{
			get
			{
				return (Icon)Resources.ResourceManager.GetObject("blank", Resources.resourceCulture);
			}
		}

		public static Bitmap Download
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Download", Resources.resourceCulture);
			}
		}

		public static Bitmap LoadFail
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("LoadFail", Resources.resourceCulture);
			}
		}

		public static Bitmap locked
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("locked", Resources.resourceCulture);
			}
		}

		public static Bitmap openlock
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("openlock", Resources.resourceCulture);
			}
		}

		public static Bitmap Paperclip
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Paperclip", Resources.resourceCulture);
			}
		}

		internal Resources()
		{
		}
	}
}
