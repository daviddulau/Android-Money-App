using Android.Widget;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ManyManager
{
	public static class Data
	{
		#region Colors

		public static readonly SKColor TextColor = SKColors.Gray;

		public static readonly SKColor[] Colors =
		{
			SKColor.Parse("#266489"),
			SKColor.Parse("#68B9C0"),
			SKColor.Parse("#90D585"),
			SKColor.Parse("#F3C151"),
			SKColor.Parse("#F37F64"),
			SKColor.Parse("#424856"),
			SKColor.Parse("#8F97A4"),
			SKColor.Parse("#DAC096"),
			SKColor.Parse("#76846E"),
			SKColor.Parse("#DABFAF"),
			SKColor.Parse("#A65B69"),
			SKColor.Parse("#97A69D"),
			SKColor.Parse("#0101DF"),
			SKColor.Parse("#FF00FF"),
			SKColor.Parse("#FFFF00"),
			SKColor.Parse("#FF4000"),
		};

		private static int ColorIndex = 0;

		public static SKColor NextColor()
		{
			var result = Colors[ColorIndex];
			ColorIndex = (ColorIndex + 1) % Colors.Length;
			return result;
		}

		#endregion

		public static Dictionary<string, Chart> CreateCharts(List<Transaction> transactions, string displayCategory, string displayChart)
		{
			Dictionary<string, float> categoriesDistionary = new Dictionary<string, float>();
			List<string> categoriesList = new List<string>();
			if (displayCategory == "Categories")
			{
				if (Filtering.ShowAllCategoriesOrTags)
				{
					categoriesList.AddRange(CategoriesGlobals.CategoriesNameList);
					foreach (string item in categoriesList)
					{
						categoriesDistionary.Add(item, 0);
					}
				}

				foreach (Transaction transaction in transactions)
				{
					if (transaction.Category != null)
					{
						if (!Filtering.ShowAllCategoriesOrTags && !categoriesList.Contains(transaction.Category.Name))
						{
							categoriesList.Add(transaction.Category.Name);
							categoriesDistionary.Add(transaction.Category.Name, 0);
						}
						if (categoriesDistionary.ContainsKey(transaction.Category.Name))
						{
							categoriesDistionary[transaction.Category.Name] += Convert.ToSingle(transaction.Sum);
						}
					}
				}
			}
			else // Tags
			{
				if (Filtering.ShowAllCategoriesOrTags)
				{
					foreach (Tag tag in TagsGlobals.TagsList)
					{
						categoriesList.Add(tag.Name);
					}
					foreach (string item in categoriesList)
					{
						categoriesDistionary.Add(item, 0);
					}
				}

				foreach (Transaction transaction in transactions)
				{
					if (transaction.Tag != null)
					{
						if (!categoriesList.Contains(transaction.Tag.Name))
						{
							categoriesList.Add(transaction.Tag.Name);
							categoriesDistionary.Add(transaction.Tag.Name, 0);
						}
						if (categoriesDistionary.ContainsKey(transaction.Tag.Name))
						{
							categoriesDistionary[transaction.Tag.Name] += Convert.ToSingle(transaction.Sum);
						}
					}
				}
			}

			//filter by sum, categories and date was handled outside
			foreach (string category in categoriesList.Reverse<string>())
			{
				double min = 0, max = 0;
				if (Filtering.MinSum != "" && Filtering.MaxSum != "")
				{
					min = Utils.ConvertStringToDouble(Filtering.MinSum);
					max = Utils.ConvertStringToDouble(Filtering.MaxSum);
					if (min > Utils.ConvertStringToDouble(categoriesDistionary[category].ToString()) || max < Utils.ConvertStringToDouble(categoriesDistionary[category].ToString()))
					{
						categoriesList.Remove(category);
					}
				}
				else if (Filtering.MaxSum != "")
				{
					max = Utils.ConvertStringToDouble(Filtering.MaxSum);
					if (max < Utils.ConvertStringToDouble(categoriesDistionary[category].ToString()))
					{
						categoriesList.Remove(category);
					}
				}
				else if (Filtering.MinSum != "")
				{
					min = Utils.ConvertStringToDouble(Filtering.MinSum);
					if (min > Utils.ConvertStringToDouble(categoriesDistionary[category].ToString()))
					{
						categoriesList.Remove(category);
					}
				}
			}

			//add categories or Tags in entries for chart
			List<Entry> entries = new List<Entry>();
			foreach (string category in categoriesList)
			{
				Entry newEntry = new Entry(categoriesDistionary[category])
				{
					Label = category,
					ValueLabel = categoriesDistionary[category].ToString(),
					Color = NextColor()
				};
				entries.Add(newEntry);
			}

			//Generate each type of chart
			return new Dictionary<string, Chart>
			{
				{ "Bar", new BarChart() { Entries = entries } },
				{ "Point", new PointChart() { Entries = entries } },
				{ "Line", new LineChart() { Entries = entries } },
				{ "Donut", new DonutChart() { Entries = entries } },
				{ "Radial", new RadialGaugeChart() { Entries = entries } },
				{ "Radar", new RadarChart() { Entries = entries } }
			};
		}
	}
}
