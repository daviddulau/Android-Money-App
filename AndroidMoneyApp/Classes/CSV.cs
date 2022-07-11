using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;
using Encoding = System.Text.Encoding;

namespace ManyManager
{
    public static class CSV
	{

		#region Properties

		public static string CSVContent;
		public static List<Month> months = null;

		#endregion

		public static void ImportData()
		{
			months = null;
			CSVContent = string.Empty;
			//todo was depreciated
			//GetFile();
		}

        private static void LoadCSVFileData()
        {
			//each transactions is
			//year;month;ID;sum;date;name;category;paytype;;
			try
			{
				Utils.ShowToast("Please wait...Importing");
				Month month = null;
				string[] transactions = CSVContent.Split("~~~");
				foreach (string transaction in transactions)
				{
					string[] items = transaction.Split(";");
                    if (items.Length <=1)
                    {
						break;
                    }
                    if (months.Count > 0)
                    {
						month = months.Find(x => x.year == Convert.ToInt32(items[0]));
                    }
					if (month == null)
					{
						month = new Month();
						month.year = Convert.ToInt32(items[0]);
						month.month = Convert.ToInt32(items[1]);
					}
					if (items[8] == "") //categorycolor
					{
						//todo test export and import of CSV's
						////year and month
						//CSVContent += month.year0.ToString() + ";" + month.month1.ToString() + ";";
						////money part
						//CSVContent += transaction.ID2.ToString() + ";" + transaction.Sum3.ToString() + ";" + Globals.SetXMLDate(transaction.Date4) + ";" + transaction.Name5 + ";"
						//	+ transaction.Tag6 == null ? "" : transaction.Tag.Name + ";" + transaction.Category7 == null ? "" : transaction.Category.Name + ";" + categoryColor8 + ";"
						//	+ transaction.HasCategory9.ToString() + ";" + transaction.RepetitiveInterval10 + ";";
						////car part
						//CSVContent += transaction.HasCarConsumption11.ToString() + ";" + transaction.Kilometers12 + ";" + transaction.Liters13 + ";" + transaction.Price14 + ";" + transaction.Unit15;
						//CSVContent += "~~~" + System.Environment.NewLine;
						month.Transactions.Add(new Transaction(Convert.ToInt32(items[2]), Convert.ToDouble(items[3]), Convert.ToDateTime(items[4]), items[5],
							TagsGlobals.SearchOrAddNewTag(items[6], false),	CategoriesGlobals.SearchOrAddNewCategory(items[7], false), SKColor.Empty, Convert.ToBoolean(items[9]),
							Convert.ToBoolean(items[11]), Convert.ToDouble(items[12]), Convert.ToDouble(items[13]), Convert.ToDouble(items[14]),  (Unit)Convert.ToInt32(items[15]), items[10]));
					}
					else
					{
						month.Transactions.Add(new Transaction(Convert.ToInt32(items[2]), Convert.ToDouble(items[3]), Convert.ToDateTime(items[4]), items[5],
							TagsGlobals.SearchOrAddNewTag(items[6], false), CategoriesGlobals.SearchOrAddNewCategory(items[7], false), items[8] != "" ? SKColor.Parse(items[8]) : SKColor.Empty, Convert.ToBoolean(items[9]),
							Convert.ToBoolean(items[11]), Convert.ToDouble(items[12]), Convert.ToDouble(items[13]), Convert.ToDouble(items[14]), (Unit)Convert.ToInt32(items[15]), items[10]));
					}
                    if (months.Find(x=>x.year == month.year && x.month == month.month) == null)
                    {
						months.Add(month);
                    }
				}
				Utils.ShowToast("Data Import");
			}
            catch
            {
				Utils.ShowAlert("Warning", "Data Import Failed!");
				months = null;
            }
        }

        public static async void ExportData(List<Month> monthsExport)
		{
			Utils.ShowToast("Export Started");
			//each transactions is
			//year;month;ID;sum;date;name;category;paytype;;
			CSVContent = string.Empty;
			string categoryColor = "";
            foreach (Month month in monthsExport)
            {
                foreach (Transaction transaction in month.Transactions)
                {
					categoryColor = transaction.Color.ToString() != SKColor.Empty.ToString() ? transaction.Color.ToString() : "";

					//year and month
					CSVContent += month.year.ToString() + ";" + month.month.ToString() + ";";
					//money part
					CSVContent += transaction.ID.ToString() + ";" + transaction.Sum.ToString() + ";" + Globals.SetXMLDate(transaction.Date) + ";" + transaction.Name + ";"
						+ transaction.Tag == null ? "" : transaction.Tag.Name + ";" + transaction.Category == null ? "" : transaction.Category.Name + ";" + categoryColor + ";"
						+ transaction.HasCategory.ToString() + ";" + transaction.RepetitiveInterval + ";";
					//car part
					CSVContent += transaction.HasCarConsumption.ToString() + ";" + transaction.Kilometers + ";" + transaction.Liters + ";" + transaction.Price + ";" + transaction.Unit;
					CSVContent += "~~~" + System.Environment.NewLine;
				}
            }
			string path = Globals.InternalPath + "/StoredData" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".csv";
			Java.IO.File csvFile = new Java.IO.File(path);
			if (!csvFile.Exists())
			{
				csvFile.CreateNewFile();
			}
            try
            {
				Utils.ShowToast("Please wait...Exporting");
				File.WriteAllText(path, CSVContent);
				Utils.ShowAlert("Information", "Data Export Succeed in " + path);
			}
			catch (Exception)
            {
				Utils.ShowAlert("Warning", "Can't Export Data");
            }
		}

		//TODO was depreciated find another plugin which selects files
		//public static async void GetFile()
		//{
		//	try
		//	{
		//		FileData fileData = await CrossFilePicker.Current.PickFile();
		//		if (fileData == null)
		//			return; // user canceled file picking

		//		if (fileData.FileName.EndsWith(".csv"))
  //              {
		//			CSVContent = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
		//			if (!string.IsNullOrWhiteSpace(CSVContent))
		//			{
		//				Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
		//				alert.SetTitle("Import");
		//				alert.SetMessage("Are You Sure You Want To Import Data From " + fileData.FileName + " ?");
		//				alert.SetPositiveButton("Yes", (senderAlert, args) =>
		//				{
		//					months = new List<Month>();
		//					LoadCSVFileData();
		//					if (months == null)
		//					{
		//						Utils.ShowAlert("Error", "Something Is Wrong With The CSV File!");
		//						return;
		//					}

		//					//populate globals
		//					//months
		//					Globals.MonthsList = months;
		//					//transactions for display
		//					MoneyGlobals.TransactionsList.Clear();
		//					foreach (Month item in Globals.MonthsList)
		//					{
		//						MoneyGlobals.TransactionsList.AddRange(item.Transactions);
		//					}

		//					List<DateTime> yearArray = new List<DateTime>();
		//					foreach (Month month in Globals.MonthsList)
		//					{
		//						DateTime dt = new DateTime(month.year, 12, 31);
		//						if (!yearArray.Contains(dt))
		//						{
		//							yearArray.Add(dt);
		//						}
		//					}
		//					foreach (DateTime item in yearArray)
		//					{
		//						Globals.RefreshYear(item);
		//					}

		//					Utils.ShowToast("Refreshed Data");
		//					//refresh current year
		//					Globals.RefreshYear();

		//					//repopulate all from object to xml objects
		//					foreach (Month month in Globals.MonthsList)
		//					{
		//						XML.SaveMonth(month);
		//					}

		//					//repopulate user data
		//					MoneyGlobals.ReLoadTagAndCategoryColors();

		//					Utils.ShowAlert("Information", "Data Import Succeed!");
		//				});
		//				alert.SetNegativeButton("No", (senderAlert, args) =>
		//				{
		//				});
		//				Dialog dialog = alert.Create();
		//				dialog.Show();
		//			}
		//		}
		//	}
		//	catch
		//	{
		//		Utils.ShowAlert("Warning", "Data Import Failed!");
		//	}
		//}
	}
}