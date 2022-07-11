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
	public class Car
	{
		//todo delete
		public int ID;
		public bool Checked;
		/// <summary>
		/// Unique
		/// </summary>
		public string Plate;
		public string Brand;
		public string Model;
		public int Year;
		public Tag Tag;
		public List<Transaction> Transactions = new List<Transaction>();
		public List<CarObject> CarObjects = new List<CarObject>();
		public List<Repair> Repairs = new List<Repair>();
		public bool Visible;

		public Car()
		{
			CarObjects = new List<CarObject>();
		}

		public Car(int ID, string Plate, string Brand, string Model, int Year, Tag Tag, List<Transaction> Transactions, List<CarObject> CarObjects, List<Repair> Repairs)
		{
			this.ID = ID;
			this.Checked = false;
			this.Visible = true;
			this.Plate = Plate;
			this.Brand = Brand;
			this.Model = Model;
			this.Year = Year;
			this.Tag = Tag;
			this.Transactions = Transactions;
			this.CarObjects = CarObjects;
			this.Repairs = Repairs;
		}

		public Car(string Plate, string Brand, string Model, int Year, Tag Tag, List<Transaction> Transactions, List<CarObject> CarObjects, List<Repair> Repairs)
		{
			ID = 1;
			foreach (Car car in CarsGlobals.CarsList)
			{
				if (car.ID == ID)
				{
					ID++;
				}
			}
			this.Visible = true;
			this.Checked = false;
			this.Plate = Plate;
			this.Brand = Brand;
			this.Model = Model;
			this.Year = Year;
			this.Tag = Tag;
			this.Transactions = Transactions;
			this.CarObjects = CarObjects;
			this.Repairs = Repairs;
		}
	}
}