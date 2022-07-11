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
	public class Month
	{
		public List<Transaction> Transactions = new List<Transaction>();
		public double Profit;
		public double Income;
		public double Expense;
		public double Charity;
		public double CharityPercentage;
		public double Ratio;
		public int month;
		public int year;

		public void RemoveTransaction(Transaction transaction)
		{
			foreach (Transaction item in Transactions)
			{
				if (item == transaction)
				{
					Transactions.Remove(item);
					return;
				}
			}
		}
	}
}