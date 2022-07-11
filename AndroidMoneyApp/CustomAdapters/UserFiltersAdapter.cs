using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;
using SkiaSharp.Views.Android;
using static ManyManager.Resource;

namespace ManyManager
{
	public class UserFiltersAdapter : BaseAdapter<Filter>
	{
		Activity CurrentAdapterActivity;
		List<Filter> listFilter;
		int SelectedPosition = -1;

		public UserFiltersAdapter(Activity currentContext, List<Filter> lstfilterInfo)
		{
			CurrentAdapterActivity = currentContext;
			listFilter = lstfilterInfo;
		}


		#region Events

		private void imgRemoveFilter_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridFilterItemID);
				if (txtID.Text != "ID")
				{
					for (int i = listFilter.Count - 1; i >= 0; i--)
					{
						if (listFilter[i].ID == Convert.ToInt32(txtID.Text))
						{
							Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
							alert.SetTitle("Remove Filter");
							alert.SetMessage("Are You Sure You Want To Remove This Filter ?");
							alert.SetPositiveButton("Yes", (senderAlert, args) =>
							{
								RemoveItem(listFilter[i]);
								UpdateAdapter(listFilter);
							});
							alert.SetNegativeButton("No", (senderAlert, args) =>
							{
							});
							Dialog dialog = alert.Create();
							dialog.Show();
							break;
						}
					}
				}
			}
		}

		private void txtItem_Click(object sender, EventArgs e)
		{
			TextView t = sender as TextView;
			foreach (Filter item in Filtering.FilterList)
			{
				if (item.Name == t.Text)
				{
					//set selected filtering
					Filtering.BeginDate = item.BeginDate;
					Filtering.EndDate = item.EndDate;
					MoneyGlobals.DatesFromFilter = true;
					MoneyGlobals.SetSelectedDatesFilter(false, CustomDates:true);
					MoneyGlobals.DatesFromFilter = false;
					Filtering.MaxSum = item.MaxSum;
					Filtering.MinSum = item.MinSum;
					Filtering.ShowAllCategoriesOrTags = item.ShowAllCategoriesOrTags;
					Filtering.TransactionDisplay = item.TransactionDisplay;
					Filtering.SpinnerToSet = item.SpinnerToSet;
					Filtering.CategoryOrTagFilter = item.CategoryOrTagFilter;
					//call filter again
					Globals.Activity.Filters_Events();
					Filtering.SetFilterSpinners(item);
					SetSelection(Filtering.FilterList.IndexOf(item));
					break;
				}
			}
		}

		#endregion

		#region Utils

		public void AddFilter(string Name)
		{
			Filtering.FilterList.Add(new Filter(Name, -1, Filtering.BeginDate, Filtering.EndDate, Filtering.TransactionDisplay, Filtering.ShowAllCategoriesOrTags, Filtering.MinSum, Filtering.MaxSum, Filtering.CategoryOrTagFilter));
			UpdateAdapter(Filtering.FilterList);
			XML.SaveFilters();
		}

		public void RemoveItem(Filter filter)
		{
			//remove from global list
			Filtering.FilterList.Remove(filter);
			//find & remove from xml
			XML.SaveFilters();
		}

		public void UpdateAdapter(List<Filter> filterList)
		{
			//refresh
			this.listFilter = filterList;
			Filtering.FilterList = filterList;
			NotifyDataSetChanged();
		}

		public void SetSelection(int selection)
		{
			SelectedPosition = selection;
			UpdateAdapter(Filtering.FilterList);
		}

		#endregion

		#region implemented abstract members of BaseAdapter

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			Filter item = listFilter[position];

			if (convertView == null)
				convertView = CurrentAdapterActivity.LayoutInflater.Inflate(Globals.GridViewFiltersItem, null);

			TextView txtItemName = convertView.FindViewById<TextView>(Resource.Id.txtGridFilterItemName);
			TextView textItemID = convertView.FindViewById<TextView>(Resource.Id.txtGridFilterItemID);
			if (position == SelectedPosition)
			{
				(txtItemName.Parent as LinearLayout).SetBackgroundColor(Android.Graphics.Color.DarkKhaki);
			}
			else
			{
				(txtItemName.Parent as LinearLayout).SetBackgroundColor(Android.Graphics.Color.Transparent);
			}

			textItemID.Text = Convert.ToString(position + 1);
			txtItemName.Text = item.Name;

			//verify to have only one click event attachedg
			if (!txtItemName.HasOnClickListeners)
			{
				txtItemName.Click += txtItem_Click;
			}
			if (!textItemID.HasOnClickListeners)
			{
				textItemID.Click += txtItem_Click;
			}

			var imageRemove = convertView.FindViewById<ImageView>(Id.imgRemoveFilter);
			if (!imageRemove.HasOnClickListeners)
			{
				imageRemove.Click += imgRemoveFilter_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listFilter == null ? -1 : listFilter.Count;
			}
		}

		public override Filter this[int position]
		{
			get
			{
				return listFilter == null ? null : listFilter[position];
			}
		}

		#endregion

	}
}