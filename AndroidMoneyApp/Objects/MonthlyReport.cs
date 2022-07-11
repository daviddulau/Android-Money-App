using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManyManager
{
	public class MonthlyReport
	{
		public int Month;
		public int Year;
		public string CategoryOrTag;
		public List<Transaction> Transactions = new List<Transaction>();
		public double Sum;

		public MonthlyReport(int Month, int Year, string CategoryOrTag, double Sum, Transaction transaction)
		{
			this.Month = Month;
			this.Year = Year;
			this.CategoryOrTag = CategoryOrTag;
			this.Transactions.Add(transaction);
			this.Sum = Sum;
		}

		public MonthlyReport(int Month, int Year)
		{
			this.Month = Month;
			this.Year = Year;
		}
	}
}