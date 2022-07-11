using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ManyManager
{
	public class Repair
	{
		//todo delete
		public int ID;
		/// <summary>
		/// Unique
		/// </summary>
		public string Name;
		public string Description;
		public DateTime Date;
		public double Kilometers;
		public Unit Unit;
		public double Sum;
		public Car Car;

		public Repair(Car Car)
		{
			ID = 1;
			foreach (Repair repair in Car.Repairs)
			{
				if (repair.ID == ID)
				{
					ID++;
				}
			}
			this.Car = Car;
		}

		public Repair(int ID, string Name, string Description, DateTime Date, double Kilometers, double Sum, Car Car)
		{
			this.ID = ID;
			this.Name = Name;
			this.Description = Description;
			this.Date = Date;
			this.Kilometers = Kilometers;
			this.Unit = UnitType.Unit;
			this.Sum = Sum;
			this.Car = Car;
		}

		public Repair(string Name, string Description, DateTime Date, double Kilometers, double Sum, Car Car)
		{
			ID = 1;
			foreach (Repair repair in Car.Repairs)
			{
				if (repair.ID == ID)
				{
					ID++;
				}
			}
			this.Name = Name;
			this.Description = Description;
			this.Date = Date;
			this.Kilometers = Kilometers;
			this.Unit = UnitType.Unit;
			this.Sum = Sum;
			this.Car = Car;
		}

		public Repair(int ID, string Name, string Description, DateTime Date, double Kilometers, Unit Unit, double Sum, Car Car)
		{
			this.ID = ID;
			this.Name = Name;
			this.Description = Description;
			this.Date = Date;
			this.Kilometers = Kilometers;
			this.Unit = Unit;
			this.Sum = Sum;
			this.Car = Car;
		}

		public Repair(string Name, string Description, DateTime Date, double Kilometers, Unit Unit, double Sum, Car Car)
		{
			ID = 1;
			foreach (Repair repair in Car.Repairs)
			{
				if (repair.ID == ID)
				{
					ID++;
				}
			}
			this.Name = Name;
			this.Description = Description;
			this.Date = Date;
			this.Kilometers = Kilometers;
			this.Unit = Unit;
			this.Sum = Sum;
			this.Car = Car;
		}
	}
}