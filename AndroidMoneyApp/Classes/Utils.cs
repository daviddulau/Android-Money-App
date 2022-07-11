using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;

namespace ManyManager
{
	public static class Utils
	{

		public static void CheckTransactionCategoryFlag(Transaction transaction)
		{
			if (transaction.Tag != null)
			{
				transaction.HasCategory = false;
			}
			else if (transaction.Category != null)
			{
				transaction.HasCategory = true;
			}
			else
			{
				transaction.HasCategory = false;
			}
		}

		public static SKColor CategoryOrTagToSKColor(object Object = null)
		{
			if (Object != null && Globals.SKCategoriesOrTagColor.Count > 0)
			{
				if (Object is string s && CTColor.ContainsKey(s))
				{
					return CTColor.GetColor(s);
				}
			}
			return Globals.SKDefaultCategoriesColor;
		}

		public static void ShowToast(string text)
		{
			Toast t = Toast.MakeText(Globals.Context, text, ToastLength.Long);
			t.SetGravity(GravityFlags.Bottom, 25, 25);
			View view = t.View;
			view.SetBackgroundColor(Globals.CurrentTheme == Resource.Style.AppThemeDark ? Color.White : Color.Rgb(23,31,44));
			view.FindViewById<TextView>(Android.Resource.Id.Message).SetTextColor(Globals.CurrentTheme == Resource.Style.AppThemeDark ? Color.Rgb(23, 31, 44) : Color.White);
			t.Show();
		}

		public static void ShowAlert(string title, string content)
		{
			Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
			alert.SetTitle(title);
			alert.SetMessage(content);
			alert.SetPositiveButton("Ok", (senderAlert, args) =>
			{
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		public static EventHandler<DatePickerDialog.DateSetEventArgs> Dpd_DateSet;
		public static string DateSet;

		public static DatePickerDialog ShowDatePicker(DateTime? currentDate = null)
		{
			DateTime date = DateTime.Now;
			if (currentDate != null)
			{
				date = currentDate.Value;
			}
			DatePickerDialog dpd = new DatePickerDialog(Globals.Context);
			dpd.DateSet += Dpd_DateSet;
			dpd.UpdateDate(date);
			dpd.SetButton3("Today", Dpd_Click);
			dpd.Show();
			return dpd;
		}

		private static void Dpd_Click(object sender, DialogClickEventArgs e)
		{
			DateSet = DateTime.Now.ToShortDateString();
			DateTime date = DateTime.Now;
			if (DateTime.TryParse(DateSet, out date))
			{
			}
			ShowDatePicker(date);
		}

		public static DateTime ConvertStringToDate(string date, bool prompt = false, string promptDisplay = "")
		{
			try
			{
				return Convert.ToDateTime(date);
			}
			catch
			{
				if (prompt)
				{
					if (promptDisplay == string.Empty)
					{
						ShowAlert("", promptDisplay);
					}
					else
					{
						ShowAlert("Incorrect Date Format", "Use Format: dd/MM/yyyy");
					}
				}
				return DateTime.Now;
			}
		}

		public static double ConvertStringToDouble(string sum, bool prompt = false, string promptDisplay = "", double result = 0)
		{
			try
			{
				return Convert.ToDouble(sum);
			}
			catch
			{
				if (prompt)
				{
					if (promptDisplay == string.Empty)
					{
						ShowAlert("", promptDisplay);
					}
					else
					{
						ShowAlert("", "Incorrect Sum Inserted");
					}
					if (result != 0)
					{
						return result;
					}
				}
				return 0;
			}
		}

		public static string ConvertDoubleToString(double sum, bool prompt = false, string promptDisplay = "")
		{
			try
			{
				return sum.ToString("#,##0.00");
			}
			catch
			{
				if (prompt)
				{
					if (promptDisplay == string.Empty)
					{
						ShowAlert("", promptDisplay);
					}
					else
					{
						ShowAlert("", "Incorrect Text Sum");
					}
				}
				return "0.00";
			}
		}

		public static int ConvertSumStringToInt(string sum, bool prompt = false, string promptDisplay = "")
		{
			try
			{
				return Convert.ToInt32(sum);
			}
			catch
			{
				if (prompt)
				{
					if (promptDisplay == string.Empty)
					{
						ShowAlert("", promptDisplay);
					}
					else
					{
						ShowAlert("", "Incorrect Sum Inserted");
					}
				}
				return 0;
			}
		}

		public static int ConvertStringToInt(string value, bool prompt = false, string promptDisplay = "")
		{
			try
			{
				return Convert.ToInt32(value);
			}
			catch
			{
				if (prompt)
				{
					if (promptDisplay == string.Empty)
					{
						ShowAlert("", promptDisplay);
					}
					else
					{
						ShowAlert("", "Incorrect Value Inserted");
					}
				}
				return 0;
			}
		}
	}
}