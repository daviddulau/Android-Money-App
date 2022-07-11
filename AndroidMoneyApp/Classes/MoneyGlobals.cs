using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;
using static ManyManager.Resource;

namespace ManyManager
{
	public static class MoneyGlobals
	{
		#region Public Properties

		public static bool SetTimeInterval = false;
		public static string RepetitiveInterval;
		public static bool TransactionsEditState = false;
		public static string DisplayChart = "Bar";
		public static string DisplayCategoryOrTag = "Categories";
		public static bool onTransactionMenuForm = false;
		public static Transaction GlobalTransaction = null;
		public static bool IsCopiedTransaction = false;
		public static Transaction PendingTransaction = null;

		/// <summary>
		/// Used To Store Transactions Name
		/// </summary>
		public static List<string> TransactionsNameList = new List<string>();

		internal static List<Transaction> transactionsList = null;
		/// <summary>
		/// Used to store all transactions
		/// </summary>
		public static List<Transaction> TransactionsList
		{
			get
			{
				return transactionsList;
			}
			set
			{
				transactionsList = value;
				TransactionsNameList = new List<string>();
				foreach (Transaction transaction in transactionsList)
				{
					TransactionsNameList.Add(transaction.Name);
				}
			}
		}

		/// <summary>
		/// Used to store transactions sorted and filtered and ready to be displayed in transactionsmenu
		/// </summary>
		public static List<Transaction> DisplayTransactions = new List<Transaction>();

		//filters shown
		public static bool ThisMonth = true;
		public static bool LastMonth = false;
		public static bool ThisYear = false;
		public static bool LastYear = false;
		public static bool CustomDates = false;
		public static bool DatesFromFilter = false;
		public static bool Last3Months = false;
		public static bool AllTime = false;

		public static bool ClearFilters;

		#endregion

		#region Public Members

		#endregion

		#region Public Methods

		public static Transaction ViewToTransaction(ImageView image)
		{
			if (image.Parent as View != null && image.Parent.Parent as View != null)
			{
				//get data from view
				View view = image.Parent.Parent as View;
				int id = Convert.ToInt32(view.FindViewById<TextView>(Id.txtGridTransactionItemID).Text);
				double sum = Utils.ConvertStringToDouble(view.FindViewById<TextView>(Id.txtGridTransactionItemSum).Text);
				DateTime date = Utils.ConvertStringToDate(view.FindViewById<TextView>(Id.txtGridTransactionItemDate).Text);
				string categoryOrTag = view.FindViewById<TextView>(Id.txtGridTransactionItemCategory).Text;
				string name = view.FindViewById<TextView>(Id.txtGridTransactionItemNameOrTag).Text;

				//get Transaction
				Month month = Globals.GetMonthFromDate(date);
				if (month != null && month.Transactions != null)
				{
					foreach (Transaction item in month.Transactions)
					{
						string category = (item.Tag != null && categoryOrTag.Contains(" Tag:") ? categoryOrTag.Substring(0, categoryOrTag.IndexOf(" Tag:")) : categoryOrTag);
						string tag = item.Tag != null && categoryOrTag.Contains(" Tag:") ? categoryOrTag.Substring(categoryOrTag.IndexOf(" Tag:") + " Tag:".Length, categoryOrTag.Length - categoryOrTag.IndexOf(" Tag:") - " Tag:".Length) : "";
						if (/*item.ID == id && */item.Sum == sum && item.Date.Date == date.Date)
						{
							if (item.Category != null && item.Tag != null && item.Category.Name == category && item.Tag.Name == tag)
							{
								return item;
							}
							else if (item.Category != null && item.Category.Name == category)
							{
								return item;
							}
							else if (item.Tag != null && item.Tag.Name == tag)
							{
								return item;
							}
							else if (name == item.Name)
							{
								return item;
							}
							else
							{
								continue;
							}
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Used for TransactionMenu to display correct text
		/// </summary>
		public static void DisplayUnitType()
		{
			Globals.textviewKilometersRan.Text = UnitType.GetUnitLengthType() + "s Ran: ";
			Globals.textviewLitersAdded.Text = UnitType.GetUnitVolumeType() + "s Added: ";
			Globals.textviewPriceAdded.Text = "Price per " + UnitType.GetUnitVolumeType() + ": ";
		}

		public static bool TransactionInList(List<Transaction> list, Transaction ins)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (TransactionsMatch(list[i], ins))
				{
					return true;
				}
			}
			return false;
		}

		public static int TransactionIndexInList(List<Transaction> list, Transaction obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (TransactionsMatch(list[i], obj))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool TransactionsMatch(Transaction t1, Transaction t2)
		{
			if (t1 == null || t2 == null)
			{
				return false;
			}
			else if (!TagsGlobals.TagMatch(t1.Tag, t2.Tag))
			{
				return false;
			}
			else if (t1.HasCategory != t2.HasCategory)
			{
				return false;
			}
			else if (t1.Date.ToShortDateString() != t2.Date.ToShortDateString())
			{
				return false;
			}
			//else if (i1.ID != i2.ID)
			//{
			//	return false;
			//}
			else if (t1.Name != t2.Name)
			{
				return false;
			}
			else if (t1.Category != null && t2.Category != null && !CategoriesGlobals.CategoriesMatch(t1.Category, t2.Category))
			{
				return false;
			}
			else if (t1.Sum != t2.Sum)
			{
				return false;
			}
			else if (t1.HasCarConsumption != t2.HasCarConsumption)
			{
				return false;
			}
			//else if (i1.Kilometers != i2.Kilometers)
			//{
			//	return false;
			//}
			//else if (i1.Liters != i2.Liters)
			//{
			//	return false;
			//}
			else
			{
				return true;
			}
		}

		public static void ClearUserFilterSelection()
		{
			(Globals.grdUserFilters?.Adapter as UserFiltersAdapter).SetSelection(-1);
		}

		/// <summary>
		/// Prevent naming Tag as Category - useless
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool AllowedTagName(string name)
		{
			foreach (string item in CategoriesGlobals.CategoriesNameList)
			{
				if (item.ToUpper() == name.ToUpper())
				{
					return false;
				}
			}
			return true;
		}

		public static void ReLoadTagAndCategoryColors()
		{
			if (Globals.MonthsList != null && Globals.MonthsList.Count > 0)
			{
				CTColor.RemoveTags();
				foreach (Month month in Globals.MonthsList)
				{
					if (month.Transactions != null && month.Transactions.Count > 0)
					{
						foreach (Transaction transaction in month.Transactions)
						{
							if (transaction.HasCategory && transaction.Category != null)
							{
								CTColor.Add(transaction.Category.Name, transaction.Color, transaction.HasCategory);
							}
							else if (transaction.Tag != null)
							{
								CTColor.Add(transaction.Tag.Name, transaction.Color, transaction.HasCategory);

							}
						}
					}
				}
			}
		}

		public static void GetSelectedCategoryOrTagFilter()
		{
			if (Globals.spinnerCategoryOrTagFilter.SelectedItemId == 0 && (string.IsNullOrWhiteSpace(Filtering.CategoryOrTagFilter) || Filtering.CategoryOrTagFilter == "No Category;None"))
			{
				return;
			}
			string name = Filtering.CategoryOrTagFilter.Split(";")[1];
			if (!string.IsNullOrWhiteSpace(name))
			{
				for (int i = 0; i < Globals.spinnerCategoryOrTagFilter.Count; i++)
				{
					if (Globals.spinnerCategoryOrTagFilter.GetItemAtPosition(i).ToString() == name)
					{
						Globals.spinnerCategoryOrTagFilter.SetSelection(i);
						break;
					}
				}
			}
			if (Globals.spinnerCategoryOrTagFilter.SelectedItemId > 0 && name != Globals.spinnerCategoryOrTagFilter.SelectedItem.ToString())
			{
				Filtering.CategoryOrTagFilter = Globals.spinnerCategoryAndTagFilterChooser.SelectedItem.ToString() + ";" + Globals.spinnerCategoryOrTagFilter.SelectedItem.ToString();
			}
		}

		public static void GetSelectedDatesFilter()
		{
			if (ThisMonth)
			{
				DateTime now = DateTime.Now;
				Filtering.BeginDate = new DateTime(now.Year, now.Month, 1);
				Filtering.EndDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
				Filtering.DateFilter = "This Month";
			}
			else if (LastMonth)
			{
				DateTime now = DateTime.Now.AddMonths(-1);
				Filtering.BeginDate = new DateTime(now.Year, now.Month, 1);
				Filtering.EndDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
				Filtering.DateFilter = "Last Month";
			}
			else if (AllTime)
			{
				//Filtering End & Begin Dates are already set;
				Filtering.DateFilter = "All Time";
			}
			else if (Last3Months)
			{
				DateTime now = DateTime.Now.AddMonths(-3);
				Filtering.BeginDate = new DateTime(now.Year, now.Month, 1);
				now = new DateTime(now.Year, now.Month, 1);
				now = now.AddDays(-1);
				Filtering.EndDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
				Filtering.DateFilter = "Last 3 Months";
			}
			else if (ThisYear)
			{
				DateTime now = DateTime.Now;
				Filtering.BeginDate = new DateTime(now.Year, 1, 1);
				Filtering.EndDate = new DateTime(now.Year, 12, 31);
				Filtering.DateFilter = "This Year";
			}
			else if (LastYear)
			{
				DateTime now = DateTime.Now.AddYears(-1);
				Filtering.BeginDate = new DateTime(now.Year, 1, 1);
				Filtering.EndDate = new DateTime(now.Year, 12, 31);
				Filtering.DateFilter = "Last Year";
			}
			else if (CustomDates)
			{
				if (DatesFromFilter)
				{
					Filtering.DateFilter = "From " + Filtering.BeginDate + " To " + Filtering.EndDate;
				}
				else
				{
					Filtering.BeginDate = DateTime.Parse(Globals.txtFilterBeginDate.Text);
					Filtering.EndDate = DateTime.Parse(Globals.txtFilterEndDate.Text);
					Filtering.DateFilter = "From " + Globals.txtFilterBeginDate.Text + " To " + Globals.txtFilterEndDate.Text;
				}
			}
		}

		public static void SetSelectedDatesFilter(bool ThisMonth = true, bool LastMonth = false, bool ThisYear = false, bool LastYear = false, bool CustomDates = false, bool Last3Months = false, bool AllTime = false)
		{
			MoneyGlobals.ThisMonth = ThisMonth;
			MoneyGlobals.LastMonth = LastMonth;
			MoneyGlobals.ThisYear = ThisYear;
			MoneyGlobals.LastYear = LastYear;
			MoneyGlobals.CustomDates = CustomDates;
			MoneyGlobals.Last3Months = Last3Months;
			MoneyGlobals.AllTime = AllTime;
			GetSelectedDatesFilter();
		}

		public static void SetSumFilter(bool ClearFilter = false)
		{
			if (ClearFilter)
			{
				Filtering.SumFilter = string.Empty;
				Filtering.MaxSum = string.Empty;
				Filtering.MinSum = string.Empty;
			}
			else
			{
				double min = 0, max = 0;
				if (Filtering.MinSum != string.Empty && Filtering.MaxSum != string.Empty)
				{ 
					min = Utils.ConvertStringToDouble(Filtering.MinSum);
					max = Utils.ConvertStringToDouble(Filtering.MaxSum);
					if (min > 0 && max > 0)
					{
						Filtering.SumFilter = "Between " + min + " and " + max;
					}
				}
				else if (Filtering.MaxSum != string.Empty)
				{
					max = Utils.ConvertStringToDouble(Filtering.MaxSum);
					if (max > 0)
					{
						Filtering.SumFilter = "Lower than " + max;
					}
				}
				else if (Filtering.MinSum != string.Empty)
				{
					min = Utils.ConvertStringToDouble(Filtering.MinSum);
					if (min > 0)
					{
						Filtering.SumFilter = "Greater than " + min;
					}
				}
				else
				{
					Filtering.SumFilter = string.Empty;
				}
			}
		}

		public static void WhatTransactionToDisplay(int values = -1, List<Transaction> list = null)
		{
			bool hide = false;

			//charts to display
			if (values != -1)
			{
				hide = values == 0;
			}
			//transactions to display
			else
			{
				if (list == null)
				{
					hide = DisplayTransactions == null || DisplayTransactions.Count == 0;
				}
				else
				{
					hide = list == null || list.Count == 0;
				}
			}
			if (hide)
			{
				if (Globals.CurrentForm == Globals.TransactionsMenu)
				{
					Globals.grdTransactions.Visibility = ViewStates.Invisible;
				}
			}
			if (Globals.CurrentForm == Globals.TransactionsMenu)
			{
				Globals.tvwTransactionsNoData.Visibility = !hide ? ViewStates.Invisible : ViewStates.Visible;
				Globals.tvwTransactionsNoData.BringToFront();
			}
			else if (Globals.CurrentForm == Globals.CarMenu)
			{
				CarsGlobals.WhatCarStuffToDisplay();
			}
		}

		public static void WhatTransactionTypeToDisplay(TransactionDisplay td)
		{
			Globals.grdTransactions.Visibility = ViewStates.Invisible;
			Globals.layoutCharts.Visibility = ViewStates.Invisible;
			Globals.elvGridTransactions.Visibility = ViewStates.Invisible;
			Globals.tvwTransactionsNoData.Visibility = ViewStates.Invisible;

			if (Filtering.TransactionDisplay == TransactionDisplay.Chart)
			{
				Globals.layoutCharts.Visibility = ViewStates.Visible;
			}
			else if (Filtering.TransactionDisplay == TransactionDisplay.List)
			{
				Globals.grdTransactions.Visibility = ViewStates.Visible;
			}
			else if (Filtering.TransactionDisplay == TransactionDisplay.Grid)
			{
				Globals.elvGridTransactions.Visibility = ViewStates.Visible;
			}
		}

		#endregion
	}
}