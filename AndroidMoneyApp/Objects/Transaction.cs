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
	public class Transaction
	{
		/// <summary>
		/// Unique
		/// </summary>
		public int ID;
		public double Sum;
		public DateTime Date;
		public string Name;
		public Tag Tag;
		public Category Category;
		public SKColor Color;
		public bool Checked;
		/// <summary>
		/// Has Category(true) Or Tag(false)
		/// </summary>
		public bool HasCategory;
		public bool HasCarConsumption;
		public double Kilometers;
		public Unit Unit;
		public double Liters;
		public double Price;
		public string RepetitiveInterval;
		public List<Car> Cars = new List<Car>();
		public List<MiniNote> MiniNotes = new List<MiniNote>();

		public Transaction(int ID, double Sum, DateTime Date, string Name, Tag Tag, Category Category, SKColor Color, bool HasCategory, bool HasCarConsumption, double Kilometers = 0, double Liters = 0, double Price = 0, Unit Unit = Unit.NotSet, string RepetitiveInterval = "")
		{
			this.Sum = Sum;
			this.Date = Date;
			this.Name = Name;
			this.Tag = Tag;
			this.Category = Category;
			this.Color = Color;
			this.HasCategory = HasCategory;
			this.Checked = false;
			this.HasCarConsumption = HasCarConsumption;
			this.Kilometers = Kilometers;
			this.Liters = Liters;
			this.Price = Price;
			if (Unit == Unit.NotSet)
			{
				Unit = UnitType.Unit;
			}
			this.Unit = Unit;
			this.RepetitiveInterval = RepetitiveInterval;
			//todo test ID's
			foreach (Month month in Globals.MonthsList)
			{
				foreach (Transaction transaction in month.Transactions)
				{
					if (transaction.ID == ID)
					{
						ID++;
					}
				}
			}
			this.ID = ID;
		}

		/// <summary>
		/// clones an existing transaction
		/// </summary>
		/// <param name="trans"></param>
		public Transaction(Transaction trans)
		{
			this.Sum = trans.Sum;
			this.Date = trans.Date;
			this.Name = trans.Name;
			this.Tag = trans.Tag;
			this.Category = trans.Category;
			this.Color = trans.Color;
			this.HasCategory = trans.HasCategory;
			this.Checked = false;
			this.HasCarConsumption = trans.HasCarConsumption;
			this.Kilometers = trans.Kilometers;
			this.Liters = trans.Liters;
			this.Price = trans.Price;
			if (trans.Unit == Unit.NotSet)
			{
				trans.Unit = UnitType.Unit;
			}
			this.Unit = trans.Unit;
			this.RepetitiveInterval = trans.RepetitiveInterval;
			//todo test ID's
			foreach (Month month in Globals.MonthsList)
			{
				foreach (Transaction transaction in month.Transactions)
				{
					if (transaction.ID == trans.ID)
					{
						trans.ID++;
					}
				}
			}
			this.ID = trans.ID;
		}

		public Transaction(double Sum, DateTime Date, string Name, Tag Tag, Category Category, SKColor Color, bool HasCategory, bool HasCarConsumption, double Kilometers = 0, double Liters = 0, double Price = 0, Unit Unit = Unit.NotSet, string RepetitiveInterval = "")
		{
			this.Sum = Sum;
			this.Date = Date;
			this.Name = Name;
			this.Tag = Tag;
			this.Category = Category;
			this.Color = Color;
			this.HasCategory = HasCategory;
			this.Checked = false;
			this.HasCarConsumption = HasCarConsumption;
			this.Kilometers = Kilometers;
			this.Liters = Liters;
			this.Price = Price;
			if (Unit == Unit.NotSet)
			{
				Unit = UnitType.Unit;
			}
			this.Unit = Unit;
			this.RepetitiveInterval = RepetitiveInterval;
			this.ID = 1;
			foreach (Month month in Globals.MonthsList)
			{
				foreach (Transaction transaction in month.Transactions)
				{
					if (transaction.ID == ID)
					{
						ID++;
					}
				}
			}
		}
	}
}