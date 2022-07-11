using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;

namespace ManyManager
{
	public class SKColorImage
	{
		public int ID;
		public string DisplayName;
		public SKColor Color;

		public SKColorImage(int ID, string DisplayName, SKColor Color)
		{
			this.ID = ID;
			this.DisplayName = DisplayName;
			this.Color = Color;
		}
	}
}