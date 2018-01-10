namespace TaskBarApp.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    public class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Resources()
        {
        }

        public static Icon blank =>
            ((Icon) ResourceManager.GetObject("blank", resourceCulture));

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static CultureInfo Culture
        {
            get => 
                resourceCulture;
            set
            {
                resourceCulture = value;
            }
        }

        public static Bitmap Download =>
            ((Bitmap) ResourceManager.GetObject("Download", resourceCulture));

        public static Bitmap LoadFail =>
            ((Bitmap) ResourceManager.GetObject("LoadFail", resourceCulture));

        public static Bitmap locked =>
            ((Bitmap) ResourceManager.GetObject("locked", resourceCulture));

        public static Bitmap openlock =>
            ((Bitmap) ResourceManager.GetObject("openlock", resourceCulture));

        public static Bitmap Paperclip =>
            ((Bitmap) ResourceManager.GetObject("Paperclip", resourceCulture));

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new System.Resources.ResourceManager("TaskBarApp.Properties.Resources", typeof(Resources).Assembly);
                }
                return resourceMan;
            }
        }
    }
}

