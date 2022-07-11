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
	public class Filter
	{
		//todo delete
		public int ID;
		/// <summary>
		/// Unique
		/// </summary>
		public string Name;
		public DateTime BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public DateTime EndDate = DateTime.Now;

		public TransactionDisplay TransactionDisplay = TransactionDisplay.List;

		public bool ShowAllCategoriesOrTags = false;
		public int SpinnerToSet = -1;

		public string MinSum = "";
		public string MaxSum = "";

		//data to display
		public string CategoryOrTagFilter = "No Category;None";
		public string SumFilter = "";
		public string DateFilter = "This Month";
		public string AllFilters = "";

		public Filter(string Name, int ID = -1, DateTime? beginDate = null, DateTime? endDate = null, TransactionDisplay transactionDisplay = TransactionDisplay.List, bool showAllCategoriesOrTags = false, string MinSum = "", string MaxSum = "", string CategoryOrTagFilter = "")
		{
			this.Name = Name;
			if (ID < 0)
			{
				ID = 1;
				foreach (Filter filter in Filtering.FilterList)
				{
					if (filter.ID == ID)
					{
						ID++;
					}
				}
			}
			this.ID = ID;

			if (beginDate == null)
			{
				BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			}
			else
			{
				BeginDate = beginDate.Value;
			}

			if (endDate == null)
			{
				EndDate = DateTime.Now;
			}
			else
			{
				EndDate = endDate.Value;
			}

			this.TransactionDisplay = transactionDisplay;
			this.ShowAllCategoriesOrTags = showAllCategoriesOrTags;
			this.MinSum = MinSum;
			this.MaxSum = MaxSum;
			this.CategoryOrTagFilter = CategoryOrTagFilter;
		}
	}
}