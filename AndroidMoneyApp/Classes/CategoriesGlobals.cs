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
	public static class CategoriesGlobals
	{
		#region Internal Properties

		internal static List<Category> tempCategoriesList = new List<Category>();
		internal static List<Category> tempIncomeCategoriesList = new List<Category>();

		#endregion

		#region Public Properties

		/// <summary>
		/// Used To Store Categories Name
		/// </summary>
		public static List<string> CategoriesNameList = new List<string>();

		public static List<Category> DisplayCategoriesList = new List<Category>();

		/// <summary>
		/// Store all Categories
		/// </summary>
		public static List<Category> CategoriesList
		{
			get
			{
				return tempCategoriesList;
			}
			set
			{
				tempCategoriesList = value;

				CategoriesNameList = new List<string>();
				foreach (Category category in tempCategoriesList)
				{
					CategoriesNameList.Add(category.Name);
				}

				//modify in IncomeCategories too
				tempIncomeCategoriesList = tempCategoriesList.FindAll(x => x.Checkers.Find(y => y.Name == "IncIncome" && y.State == true)?.State == true);
			}
		}

		/// <summary>
		/// Categories to work with
		/// </summary>
		public static Category SelectedCategory;
		/// <summary>
		/// Include Income Categories
		/// </summary>
		public static List<Category> IncomeCategories
		{
			get
			{
				if (tempIncomeCategoriesList == null)
				{
					return tempIncomeCategoriesList = CategoriesList.FindAll(x => x.Checkers.Find(y => y.Name == "IncIncome" && y.State == true)?.State == true);
				}
				return tempIncomeCategoriesList;
			}
		}
		public static Category PendingCategory;

		public static bool CategoryEditState = false;
		public static bool AddNewCategory = false;

		#endregion

		#region Public Methods

		public static void RemoveItem(Category cat)
		{
			Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
			alert.SetTitle("Delete Category");
			alert.SetMessage("By Removing " + cat.Name + " It Will Be Removed From All Transactions Assigned. Proceed ?");
			alert.SetPositiveButton("Yes", (senderAlert, args) =>
			{
				//remove category from global list
				foreach (Category item in CategoriesList.Reverse<Category>())
				{
					if (CategoriesMatch(item, cat))
					{
						CategoriesList.Remove(item);
						break;
					}
				}
				//remove category from displaying
				foreach (Category item in DisplayCategoriesList.Reverse<Category>())
				{
					if (CategoriesMatch(item, cat))
					{
						DisplayCategoriesList.Remove(item);
						break;
					}
				}
				//find & remove from xml
				XML.RefreshCategories();

				//refresh all transactions from XML
				foreach (Month month in Globals.MonthsList)
				{
					foreach (Transaction transaction in month.Transactions)
					{
						if (transaction.Category != null)
						{
							transaction.Category = SearchOrAddNewCategory(transaction.Category.Name, false, false);
						}
					}
				}
				//save new transactions in XML
				XML.SaveYears();
				//and then reload transactions to be changed in global list of transactions
				XML.LoadYears(true);

				Globals.ReLoadNotesAndCars();

				(Globals.grdCategories.Adapter as CategoriesAdapter).EditState();
				(Globals.grdCategories.Adapter as CategoriesAdapter).CheckAll();
				(Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter(DisplayCategoriesList);
				WhatCategoryToDisplay();
			});
			alert.SetNegativeButton("No", (senderAlert, args) =>
			{
				(Globals.grdCategories.Adapter as CategoriesAdapter).EditState();
				(Globals.grdCategories.Adapter as CategoriesAdapter).CheckAll();
				(Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter(DisplayCategoriesList);
				WhatCategoryToDisplay();
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		/// <summary>
		/// Used to Add category to Global List and update CategoriesNameList
		/// </summary>
		/// <param name="category"></param>
		public static void CategoriesListAdd(Category category)
		{
			CategoriesList.Add(category);
			CategoriesList = CategoriesList;
		}

		/// <summary>
		/// Used to Add category to Global List and update CategoriesNameList
		/// </summary>
		/// <param name="category"></param>
		public static void CategoriesListRemove(Category category)
		{
			CategoriesList.Remove(category);
			CategoriesList = CategoriesList;
		}

		public static void WhatCategoryToDisplay()
		{
			if (Globals.CurrentForm == Globals.CategoriesMenu)
			{
				Globals.txtNoCategories.Visibility = DisplayCategoriesList.Count > 0 ? ViewStates.Invisible : ViewStates.Visible;
				Globals.grdCategories.Visibility = DisplayCategoriesList.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
			}
		}

		public static Category GetCategory(string Name)
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				return null;
			}
			if (CategoriesList != null && CategoriesList.Count > 0)
			{
				foreach (Category item in CategoriesList)
				{
					if (item.Name == Name)
					{
						return item;
					}
				}
			}
			return null;
		}

		public static Category SearchOrAddNewCategory(string Name, bool prompt = true, bool AddCategory = true)
		{
			if (Name == "")
			{
				return null;
			}
			Name = Name.ToUpper();
			if (CategoriesList != null)
			{
				foreach (Category item in CategoriesList)
				{
					if (item.Name == Name)
					{
						return item;
					}
				}
				//	//comes from XML and we need to create and to add it
				if (AddCategory)
				{
					Category newCategory = new Category(Name, 0, SKColor.Empty);
					CategoriesListAdd(newCategory);
					if (prompt)
					{
						Utils.ShowToast("New Category: " + Name + " Was Added");
					}
					return newCategory;
				}
			}
			return null;
		}

		public static int CategoryIndexInList(List<Category> list, Category obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CategoriesMatch(list[i], obj))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool CategoryInList(List<Category> list, Category obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CategoriesMatch(list[i], obj))
				{
					return true;
				}
			}
			return false;
		}

		public static bool CategoriesMatch(Category c1, Category c2)
		{
			if (c1 == null && c2 == null)
			{
				return true;
			}
			else if (c1 == null || c2 == null)
			{
				return false;
			}
			else if (c1.Name != c2.Name)
			{
				return false;
			}
			else if (c1.Color != c2.Color)
			{
				return false;
			}
			else
			{
				//check checkers to match
				for (int i = 0; i < c1.Checkers.Count; i++)
				{
					if (!CheckersMatch(c1.Checkers[i], c2.Checkers[i]))
					{
						return false;
					}
				}
				return true;
			}
		}

		public static bool CheckersMatch(Checker c1, Checker c2)
		{
			if (c1.Name != c2.Name)
			{
				return false;
			}
			else if (c1.State != c2.State)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static void CheckAll(bool check = false, List<Category> list = null)
		{
			if (list == null)
			{
				list = CategoriesList;
			}
			foreach (Category category in list)
			{
				category.Checked = check;
			}
			tempIncomeCategoriesList = CategoriesList.FindAll(x => x.Checkers.Find(y => y.Name == "IncIncome" && y.State == true)?.State == true);
		}

		public static void SaveCategoryCheckers(Category category)
		{
			category.Checkers[0].State = Globals.chkIncIncome.Checked;
			category.Checkers[1].State = Globals.chkIncExpense.Checked;
			category.Checkers[2].State = Globals.chkIncCharity.Checked;
			category.Checkers[3].State = Globals.chkExcIncome.Checked;
			category.Checkers[4].State = Globals.chkExcExpense.Checked;
			category.Checkers[5].State = Globals.chkExcCharity.Checked;
		}

		public static bool GetCategoryCheckersValue(Category category, int i)
		{
			return category.Checkers[i].State;
		}

		public static void SetCategoryCheckers(Category category, int i, bool value)
		{
			category.Checkers[i].State = value;
		}

		#endregion
	}
}