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

	public enum SortBy
	{
		/// <summary>
		/// Default
		/// </summary>
		DateLatest = 0,
		DateOldest = 1,
		AmountSmallest = 2,
		AmountBiggest = 3,
		IDAscend = 4,
		IDDescend = 5,
		NameAscend = 6,
		NameDescend = 7,
		CategoryAscend = 8,
		CategoryDescend = 9,
		TagAscend = 10,
		TagDescend = 11,
	}

	/// <summary>
	/// Class used to store actual data from sorts
	/// </summary>
	public static class Sorting
	{

		internal static SortBy currentSort = SortBy.DateLatest;
		public static SortBy CurrentSort
		{
			get
			{
				return currentSort;
			}
			set
			{
				if (currentSort != value)
				{
					currentSort = value;
					switch (value)
					{
						case SortBy.DateLatest:
							DisplayCurrentSort = "Newest Date";
							break;
						case SortBy.DateOldest:
							DisplayCurrentSort = "Oldest Date";
							break;
						case SortBy.AmountSmallest:
							DisplayCurrentSort = "Smallest Amount";
							break;
						case SortBy.AmountBiggest:
							DisplayCurrentSort = "Biggest Amount";
							break;
						case SortBy.IDAscend:
							DisplayCurrentSort = "ID Ascend";
							break;
						case SortBy.IDDescend:
							DisplayCurrentSort = "ID Descend";
							break;
						case SortBy.NameAscend:
							DisplayCurrentSort = "Ascend Name";
							break;
						case SortBy.NameDescend:
							DisplayCurrentSort = "Descend Name";
							break;
						case SortBy.CategoryAscend:
							DisplayCurrentSort = "Ascend Category";
							break;
						case SortBy.CategoryDescend:
							DisplayCurrentSort = "Descend Category";
							break;
						case SortBy.TagAscend:
							DisplayCurrentSort = "Ascend Tag";
							break;
						case SortBy.TagDescend:
							DisplayCurrentSort = "Descend Tag";
							break;
					}
				}
			}
		}

		//data to display
		public static string DisplayCurrentSort = "Newest Date";

		public static List<Transaction> SortTransactions(List<Transaction> list)
		{
			switch (CurrentSort)
			{
				case SortBy.DateLatest:
					return list.OrderByDescending(o => o.Date.ToOADate()).ToList();
				case SortBy.DateOldest:
					return list.OrderBy(o => o.Date.ToOADate()).ToList();
				case SortBy.AmountSmallest:
					return list.OrderBy(o => o.Sum).ToList();
				case SortBy.AmountBiggest:
					return list.OrderByDescending(o => o.Sum).ToList();
				case SortBy.IDAscend:
					return list.OrderBy(o => o.ID).ToList();
				case SortBy.IDDescend:
					return list.OrderByDescending(o => o.ID).ToList();
				case SortBy.NameAscend:
					return list.OrderBy(o => o.Name).ToList();
				case SortBy.NameDescend:
					return list.OrderByDescending(o => o.Name).ToList();
				case SortBy.CategoryAscend:
					return list.OrderBy(o => o.Category?.Name).ToList();
				case SortBy.CategoryDescend:
					return list.OrderByDescending(o => o.Category?.Name).ToList();
				case SortBy.TagAscend:
					return list.OrderBy(o => o.Tag?.Name).ToList();
				case SortBy.TagDescend:
					return list.OrderByDescending(o => o.Tag?.Name).ToList();
			}
			return list;
		}
	}
}