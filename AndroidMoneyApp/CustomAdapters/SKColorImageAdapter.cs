using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace ManyManager
{
	public class SKColorImageAdapter : BaseAdapter<SKColorImage>
	{
        List<SKColorImage> _SKColorSpinner = new List<SKColorImage>();
        List<SKColorImage> _SKColorSpinnerFiltered = new List<SKColorImage>();
        Activity _activity;

        public SKColorImageAdapter(Activity activity)
        {
            _activity = activity;
            FillSKColors();
        }

		#region Events

		public void SKColorName_Click(object sender, EventArgs e)
		{
            string text = "";
			if (sender is string str)
			{
                text = str;
            }
			else
			{
                text = (sender as TextView).Text;
            }
            KeyValuePair<string, SKColor> item = Globals.SKStringColors.FirstOrDefault(x => x.Key == text);
            SKColor = item.Value;
            SKColorString = item.Key;
            Globals.sKColorSelectedImage.SetBackgroundColor(SKColor.ToColor());
            Globals.sKColorSelectedName.Text = SKColorString;
            if (!string.IsNullOrWhiteSpace(SKColorString))
            {
                for (int i = 0; i < _SKColorSpinner.Count; i++)
                {
                    if (_SKColorSpinner[i].DisplayName == SKColorString)
                    {
                        SetSelection(i);
                        break;
                    }
                }
            }
            Update();
        }

        private void SKColorImage_Click(object sender, EventArgs e)
        {
            KeyValuePair<string, SKColor> item = Globals.SKStringColors.FirstOrDefault(x => x.Value == ((sender as ImageView).Background as ColorDrawable).Color.ToSKColor());
            SKColor = item.Value;
            SKColorString = item.Key;
            Globals.sKColorSelectedName.Text = SKColorString;
            Globals.sKColorSelectedImage.SetBackgroundColor(SKColor.ToColor());
        }

        #endregion

        #region Public Properties

        public SKColor SKColor;
        public string SKColorString;
        public int SelectedPosition = -1;
        public bool Filtered = false;

        #endregion

        #region Methods

        public void Update()
		{
            NotifyDataSetChanged();
        }

        public void Filtering(string text)
        {
            Filtered = text != "";
            if (text == "")
			{
                return;
			}
            _SKColorSpinnerFiltered.Clear();

            foreach (SKColorImage item in _SKColorSpinner)
			{
				if (item.DisplayName.ToUpper().Contains(text.ToUpper()))
				{
                    _SKColorSpinnerFiltered.Add(item);
				}
			}
             Update();
        }

        public void SetSelection(int selection)
		{
            SelectedPosition = selection;
            Update();
        }

        void FillSKColors()
        {
            int ID = 0;
			foreach (KeyValuePair<string, SKColor> item in Globals.SKStringColors)
			{
                _SKColorSpinner.Add(new SKColorImage(ID, item.Key, item.Value));
                ID++;
            }
        }

		#endregion

		#region BaseAdapter

		public override int Count
        {
            get 
            {
                if (Filtered)
                {
                    return _SKColorSpinnerFiltered.Count;
                }
                return _SKColorSpinner.Count; 
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (Filtered)
            {
                return _SKColorSpinnerFiltered[position].ID;
            }
            return _SKColorSpinner[position].ID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Globals.GridViewSKColorImageItem, parent, false);
			
            if (position < 0)
			{
                return view;
			}

            TextView SKColorName = view.FindViewById<TextView>(Resource.Id.SKColorName);
            if (position == SelectedPosition)
            {
                SKColorName.SetBackgroundColor(Color.DarkKhaki);
                Globals.sKColorSelectedName.Text = SKColorString;
                Globals.sKColorSelectedImage.SetBackgroundColor(SKColor.ToColor());
            }
            else
            {
                SKColorName.SetBackgroundColor(Color.Transparent);
            }
            SKColorName.Text = Filtered ? _SKColorSpinnerFiltered[position].DisplayName : _SKColorSpinner[position].DisplayName;
            SKColorName.Click -= SKColorName_Click;
            SKColorName.Click += SKColorName_Click;

            ImageView SKColorImage = view.FindViewById<ImageView>(Resource.Id.SKColorImage);
            SKColorImage.SetBackgroundColor(Filtered ? _SKColorSpinnerFiltered[position].Color.ToColor() : _SKColorSpinner[position].Color.ToColor());
            SKColorImage.Click -= SKColorImage_Click;
            SKColorImage.Click += SKColorImage_Click;
            return view;
        }

		public override SKColorImage this[int position]
        {
            get
            {
				if (Filtered)
				{
                    return _SKColorSpinnerFiltered == null ? null : _SKColorSpinnerFiltered[position];
                }
                return _SKColorSpinner == null ? null : _SKColorSpinner[position];
            }
        }

		#endregion

	}
}