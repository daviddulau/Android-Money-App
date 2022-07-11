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
	public class Checker
	{
		public string Name;
		public bool State;

		public Checker(string Name, bool State)
		{
			this.Name = Name;
			this.State = State;
		}
	}

	public class Category
	{
		/// <summary>
		/// Unique
		/// </summary>
		public string Name;
		/// <summary>
		/// If Include or Exclude Charity this one could be set
		/// </summary>
		public double CharityPercentage;
		public bool Checked;
		public SKColor Color;
		/// <summary>
		/// IncIncome, IncExpense, IncGift, IncCharity, ExcIncome, ExcExpense, ExcGift, ExcCharity
		/// </summary>
		public List<Checker> Checkers;

		public void LoadCheckers()
		{
			Checkers = new List<Checker>();
			Checkers.Add(new Checker("IncIncome", false));
			Checkers.Add(new Checker("IncExpense", false));
			Checkers.Add(new Checker("IncCharity", false));
			Checkers.Add(new Checker("ExcIncome", false));
			Checkers.Add(new Checker("ExcExpense", false));
			Checkers.Add(new Checker("ExcCharity", false));
		}

		public Category()
		{
			LoadCheckers();
		}

		public Category(string Name, double CharityPercentage, SKColor Color)
		{
			LoadCheckers();
			this.Name = Name;
			this.CharityPercentage = CharityPercentage;
			this.Color = Color;
		}

		public Category(string Name, double CharityPercentage, SKColor Color, List<Checker> Checkers)
		{
			this.Name = Name;
			this.CharityPercentage = CharityPercentage;
			this.Color = Color;
			this.Checkers = Checkers;
		}
	}
}