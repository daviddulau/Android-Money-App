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
using static ManyManager.Resource;

namespace ManyManager
{
	public static class StoreControlsData
	{

		private static ArrayAdapter<string> temp_SpinnerRepeatFor;
		public static ArrayAdapter<string> SpinnerRepeatFor
		{
			get
			{
				if (temp_SpinnerRepeatFor == null)
				{
					string[] adapter = new string[] { "Once", "2 Times", "3 Times", "4 Times", "Until I Stop It" };
					temp_SpinnerRepeatFor = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerRepeatFor.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerRepeatFor;
			}
		}

		private static ArrayAdapter<string> temp_SpinnerSetIntervalTimeAdapter;
		public static ArrayAdapter<string> SpinnerSetIntervalTimeAdapter
		{
			get
			{
				if (temp_SpinnerSetIntervalTimeAdapter == null)
				{
					string[] adapter = new string[] { "Interval Of", "On Each Day Of Week", "On Each Week Of Month", "On Each Day Of Month" };
					temp_SpinnerSetIntervalTimeAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerSetIntervalTimeAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerSetIntervalTimeAdapter;
			}
		}

		private static ArrayAdapter<string> temp_SpinnerTransactionSortByAdapter;
		public static ArrayAdapter<string> SpinnerTransactionsSortByAdapter
		{
			get
			{
				if (temp_SpinnerTransactionSortByAdapter == null)
				{
					string[] adapter = new string[] { "Latest Date", "Oldest Date", "Smallest Amount", "Biggest Amount", "ID Ascend", "ID Descend", "Ascend Name", "Descend Name", "Ascend Category", "Descend Category", "Ascend Tag", "Descend Tag" };
					temp_SpinnerTransactionSortByAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerTransactionSortByAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerTransactionSortByAdapter;
			}
		}

		private static ArrayAdapter<string> temp_UnitsTypeChangeAdapter;
		public static ArrayAdapter<string> UnitsTypeChangeAdapter
		{
			get
			{
				if (temp_UnitsTypeChangeAdapter == null)
				{
					string[] adapter = new string[] { UnitType.GetUnitName(Unit.Metric), UnitType.GetUnitName(Unit.USImperial), UnitType.GetUnitName(Unit.UKImperial) };
					temp_UnitsTypeChangeAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_UnitsTypeChangeAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_UnitsTypeChangeAdapter;
			}
		}

		private static ArrayAdapter<string> temp_SpinnerIntervalIntervalOfAdapter;
		public static ArrayAdapter<string> SpinnerIntervalIntervalOfAdapter
		{
			get
			{
				if (temp_SpinnerIntervalIntervalOfAdapter == null)
				{
					string[] adapter = new string[] { "Days", "Weeks", "Months" };
					temp_SpinnerIntervalIntervalOfAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerIntervalIntervalOfAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerIntervalIntervalOfAdapter;
			}
		}

		private static ArrayAdapter<string> temp_SpinnerIntervalOnEachDayOfWeekAdapter;
		public static ArrayAdapter<string> SpinnerIntervalOnEachDayOfWeekAdapter
		{
			get
			{
				if (temp_SpinnerIntervalOnEachDayOfWeekAdapter == null)
				{
					string[] adapter = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
					temp_SpinnerIntervalOnEachDayOfWeekAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerIntervalOnEachDayOfWeekAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerIntervalOnEachDayOfWeekAdapter;
			}
		}

		private static ArrayAdapter<string> temp_SpinnerIntervalOnEachWeekOfMonthAdapter;
		public static ArrayAdapter<string> SpinnerIntervalOnEachWeekOfMonthAdapter
		{
			get
			{
				if (temp_SpinnerIntervalOnEachWeekOfMonthAdapter == null)
				{
					string[] adapter = new string[] { "First", "Second", "Third", "Last" };
					temp_SpinnerIntervalOnEachWeekOfMonthAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerIntervalOnEachWeekOfMonthAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerIntervalOnEachWeekOfMonthAdapter;
			}
		}

		private static ArrayAdapter<string> temp_CharityPeriodChangeChooserAdapter;
		public static ArrayAdapter<string> CharityPeriodChangeChooserAdapter
		{
			get
			{
				if (temp_CharityPeriodChangeChooserAdapter == null)
				{
					string[] adapter = new string[] { "Current Month", "Last Month", "Next Month", "Entire Year", "Until End Of The Year", "From Begin Of Year Until Now" };
					temp_CharityPeriodChangeChooserAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_CharityPeriodChangeChooserAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_CharityPeriodChangeChooserAdapter;
			}
		}

		private static ArrayAdapter<string> temp_ChooseDisplayChartAdapter;
		public static ArrayAdapter<string> ChooseDisplayChartAdapter
		{
			get
			{
				if (temp_ChooseDisplayChartAdapter == null)
				{
					string[] adapter = new string[] { "Bar", "Point", "Line", "Donut", "Radial", "Radar" };
					temp_ChooseDisplayChartAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_ChooseDisplayChartAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_ChooseDisplayChartAdapter;
			}
		}

		private static ArrayAdapter<string> temp_CategoriesOrTagsAdapter;
		public static ArrayAdapter<string> CategoriesOrTagsAdapter
		{
			get
			{
				if (temp_CategoriesOrTagsAdapter == null)
				{
					string[] adapter = new string[] { "Categories", "Tags" };
					temp_CategoriesOrTagsAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_CategoriesOrTagsAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_CategoriesOrTagsAdapter;
			}
		}

		private static ArrayAdapter<string> temp_SpinnerCategoryOrTagFilterChooserAdapter;
		public static ArrayAdapter<string> SpinnerCategoryOrTagFilterChooserAdapter
		{
			get
			{
				if (temp_SpinnerCategoryOrTagFilterChooserAdapter == null)
				{
					string[] adapter = new string[] { "No Category", "Categories", "Tags" };
					temp_SpinnerCategoryOrTagFilterChooserAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerCategoryOrTagFilterChooserAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerCategoryOrTagFilterChooserAdapter;
			}
		}

		private static ArrayAdapter<string> temp_SpinnerCategoryOrTagFilterAdapter;
		public static ArrayAdapter<string> SpinnerCategoryOrTagFilterAdapter
		{
			get
			{
				if (temp_SpinnerCategoryOrTagFilterAdapter == null)
				{
					string[] adapter = new string[] { "None" };
					temp_SpinnerCategoryOrTagFilterAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
					temp_SpinnerCategoryOrTagFilterAdapter.SetDropDownViewResource(Drawable.spinner_item);
				}
				return temp_SpinnerCategoryOrTagFilterAdapter;
			}
		}
	}
}