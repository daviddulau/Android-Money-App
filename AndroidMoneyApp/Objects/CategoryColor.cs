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
	public class CTColor
	{
		public SKColor Color;
		public string CategoryOrTagName;
		public bool HasCategory;

		public CTColor(SKColor Color, string CategoryOrTagName, bool HasCategory)
		{
			this.Color = Color;
			this.CategoryOrTagName = CategoryOrTagName;
			this.HasCategory = HasCategory;
		}

		public static bool ContainsKey(string CategoryOrTagName)
		{
			foreach (CTColor item in Globals.SKCategoriesOrTagColor)
			{
				if (item.CategoryOrTagName == CategoryOrTagName)
				{
					return true;
				}
			}
			return false;
		}

		public static void Add(string CategoryOrTagName, SKColor Color, bool HasCategory = true)
		{
			//if (Color == SKColor.Empty)
			//{
			//	Remove(CategoryOrTagName);
			//	return;
			//}
			foreach (CTColor item in Globals.SKCategoriesOrTagColor)
			{
				if (item.CategoryOrTagName == CategoryOrTagName)
				{
					//already inserted
					return;
				}
			}
			Globals.SKCategoriesOrTagColor.Add(new CTColor(Color, CategoryOrTagName, HasCategory));

			//populate with color in list to display
			foreach (Transaction item in MoneyGlobals.TransactionsList)
			{
				if (item.Category.Name == CategoryOrTagName || (item.Tag != null && item.Tag.Name == CategoryOrTagName))
				{
					item.HasCategory = HasCategory;
					item.Color = Color;
					break;
				}
			}

			bool modified = false;
			//populate with color in monthslist transactions from xml
			foreach (Month month in Globals.MonthsList)
			{
				modified = false;
				foreach (Transaction item in month.Transactions)
				{
					if (item.Category.Name == CategoryOrTagName || (item.Tag != null && item.Tag.Name == CategoryOrTagName))
					{
						item.HasCategory = HasCategory;
						item.Color = Color;
						modified = true;
						break;
					}
				}
				if (modified)
				{
					XML.SaveMonth(month);
				}
			}
		}

		public static void RemoveTags()
		{
			for (int i = Globals.SKCategoriesOrTagColor.Count - 1; i >= 0; i--)
			{
				CTColor element = Globals.SKCategoriesOrTagColor.ElementAt(i);
				if (!element.HasCategory)
				{
					Globals.SKCategoriesOrTagColor.RemoveAt(i);

					Remove(element);
				}
			}

		}

		public static SKColor GetColor(string CategoryOrTagName)
		{
			for (int i = Globals.SKCategoriesOrTagColor.Count - 1; i >= 0; i--)
			{
				var item = Globals.SKCategoriesOrTagColor.ElementAt(i);
				if (item.CategoryOrTagName == CategoryOrTagName)
				{
					return item.Color;
				}
			}
			return SKColor.Empty;
		}

		public static CTColor GetElement(string CategoryOrTagName)
		{
			for (int i = Globals.SKCategoriesOrTagColor.Count - 1; i >= 0; i--)
			{
				var item = Globals.SKCategoriesOrTagColor.ElementAt(i);
				if (item.CategoryOrTagName == CategoryOrTagName)
				{
					return item;
				}
			}
			return null;
		}

		public static void Remove(string CategoryOrTagName)
		{
			foreach (CTColor element in Globals.SKCategoriesOrTagColor)
			{
				if (element.CategoryOrTagName == CategoryOrTagName)
				{
					Globals.SKCategoriesOrTagColor.Remove(element);

					Remove(element);
					return;
				}
			}
		}

		static void Remove(CTColor element)
		{
			//remove in display list
			foreach (Transaction item in MoneyGlobals.TransactionsList)
			{
				if (item.Category != null && item.HasCategory)
				{
					if (item.Category.Name == element.CategoryOrTagName)
					{
						item.Color = element.Color;
						break;
					}
				}
				else if (item.Tag != null)
				{
					if (item.Tag.Name == element.CategoryOrTagName)
					{
						item.Color = element.Color;
						break;
					}
				}
			}

			bool modified = false;
			//remove in monthslist from xml
			foreach (Month month in Globals.MonthsList)
			{
				modified = false;
				foreach (Transaction item in month.Transactions)
				{
					if (item.Category != null && item.HasCategory)
					{
						if (item.Category.Name == element.CategoryOrTagName)
						{
							item.Color = element.Color;
							modified = true;
							break;
						}
					}
					else if (item.Tag != null)
					{
						if (item.Tag.Name == element.CategoryOrTagName)
						{
							item.Color = element.Color;
							modified = true;
							break;
						}
					}
				}
				if (modified)
				{
					XML.SaveMonth(month);
				}
			}
		}
	}
}