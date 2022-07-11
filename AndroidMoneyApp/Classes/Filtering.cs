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

namespace ManyManager
{
	/// <summary>
	/// Class used to store local and actual data from filters
	/// </summary>
	public static class Filtering
	{

		#region Public Members

		public static List<Filter> FilterList = new List<Filter>();

		public static DateTime BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public static DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23, 59, 59);

		public static TransactionDisplay TransactionDisplay = TransactionDisplay.List;

		public static bool ShowAllCategoriesOrTags = false;
		public static int SpinnerToSet = -1;

		public static string MinSum = "";
		public static string MaxSum = "";

		//data to display
		public static string CategoryOrTagFilter = "No Category;None";
		public static string SumFilter = "";
		public static string DateFilter = "This Month";
		public static string AllFilters = "";

		#endregion

		public static void SetFilterSpinners(Filter filter = null)
		{
			if (filter != null)
			{
				string name = filter.CategoryOrTagFilter.Split(";")[1];
				if (CategoriesGlobals.CategoriesNameList.Contains(name))
				{
					Globals.spinnerCategoryAndTagFilterChooser.SetSelection(1);
					filter.SpinnerToSet = CategoriesGlobals.CategoriesNameList.IndexOf(name);
					SpinnerToSet = filter.SpinnerToSet;
				}
				else if (TagsGlobals.TagsNameList.Contains(name))
				{
					Globals.spinnerCategoryAndTagFilterChooser.SetSelection(2);
					filter.SpinnerToSet = TagsGlobals.TagsNameList.IndexOf(name);
					SpinnerToSet = filter.SpinnerToSet;
				}
				else
				{
					Utils.ShowAlert("Tag Warning!", "Tag not Handled!");
				}
			}
			else
			{
				string name = CategoryOrTagFilter.Split(";")[1];
				if (CategoriesGlobals.CategoriesNameList.Contains(name))
				{
					Globals.spinnerCategoryAndTagFilterChooser.SetSelection(1);
					SpinnerToSet = CategoriesGlobals.CategoriesNameList.IndexOf(name);
				}
				else if (TagsGlobals.TagsNameList.Contains(name))
				{
					Globals.spinnerCategoryAndTagFilterChooser.SetSelection(2);
					SpinnerToSet = TagsGlobals.TagsNameList.IndexOf(name);
				}
				else
				{
					Utils.ShowAlert("Category Warning!", "Category not Handled!");
				}
			}
		}

		public static List<Transaction> FilterTransactions(List<Transaction> list = null, DateTime? beginDate = null, DateTime? endDate = null)
		{
			list = GetTransactionsByDate(beginDate, endDate, list);
			list = GetTransactionsByCategoriesAndTags(list);
			list = GetTransactionsBySum(list);
			return list;
		}

		public static List<Transaction> GetTransactionsBySum(List<Transaction> list)
		{
			double min = 0, max = 0;
			if (MinSum != "" && MaxSum != "")
			{
				min = Utils.ConvertStringToDouble(MinSum);
				max = Utils.ConvertStringToDouble(MaxSum);
				if (min > 0 && max > 0)
				{
					foreach (Transaction item in list.Reverse<Transaction>())
					{
						if (item.Sum < min || item.Sum > max)
						{
							list.Remove(item);
						}
					}
				}
			}
			else if (MaxSum != "")
			{
				max = Utils.ConvertStringToDouble(MaxSum);
				if (max > 0)
				{
					foreach (Transaction item in list.Reverse<Transaction>())
					{
						if (item.Sum > max)
						{
							list.Remove(item);
						}
					}
				}
			}
			else if (MinSum != "")
			{
				min = Utils.ConvertStringToDouble(MinSum);
				if (min > 0)
				{
					foreach (Transaction item in list.Reverse<Transaction>())
					{
						if (item.Sum < min)
						{
							list.Remove(item);
						}
					}
				}
			}
			return list;
		}

		public static List<Transaction> GetTransactionsByDate(DateTime? startDate = null, DateTime? endDate = null, List<Transaction> list = null)
		{
			//check dates. When all null, display curent month
			if (startDate == null)
			{
				startDate = Utils.ConvertStringToDate("01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString());
			}
			if (endDate == null)
			{
				endDate = DateTime.Now;
			}
			//get months
			List<Transaction> listItem = new List<Transaction>();
			if (list == null)
			{
				for (int i = startDate.Value.Year; i <= endDate.Value.Year; i++)
				{
					int endMonth = i == endDate.Value.Year ? endDate.Value.Month : 12;
					for (int j = i == startDate.Value.Year ? startDate.Value.Month : 1; j <= endMonth; j++)
					{
						//populate
						Month month = Globals.GetMonthFromDate(new DateTime(i, j, 1));
						if (month != null)
						{
							foreach (Transaction transaction in month.Transactions)
							{
								if (transaction.Date.ToOADate() >= startDate.Value.ToOADate() &&
									transaction.Date.ToOADate() <= endDate.Value.ToOADate())
								{
									listItem.Add(transaction);
								}
							}
						}
					}
				}
			}
			else
			{
				foreach (Transaction transaction in list)
				{
					if (transaction.Date.ToOADate() >= startDate.Value.ToOADate() &&
						transaction.Date.ToOADate() <= endDate.Value.ToOADate())
					{
						listItem.Add(transaction);
					}
				}
			}

			return listItem;
		}

		public static List<Transaction> GetTransactionsByCategoriesAndTags(List<Transaction> list = null)
		{
			string selectedItem = CategoryOrTagFilter;

			if (list == null)
			{
				return GetTransactionsByDate();
			}
			else if (selectedItem != "No Category;None")
			{
				List<Transaction> results = new List<Transaction>();
				selectedItem = selectedItem.Split(";")[1];
				//Categories
				bool found = false;
				foreach (string item in CategoriesGlobals.CategoriesNameList)
				{
					if (item == selectedItem)
					{
						foreach (Transaction transaction in list)
						{
							if (transaction.Category != null && transaction.Category.Name.Contains(selectedItem))
							{
								results.Add(transaction);
							}
						}
						found = true;
						break;
					}
				}
				if (!found)
				{
					//Tags
					foreach (Transaction transaction in list)
					{
						if (transaction.Tag != null && transaction.Tag.Name == selectedItem)
						{
							results.Add(transaction);
						}
					}
				}
				return results;
			}
			return list;
		}
	}
}