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
	public class CarObject
	{
		//todo delete
		public int ID;
		/// <summary>
		/// Unique
		/// </summary>
		public string Name;
		public DateTime BeginDate;
		public DateTime EndDate;
		public Car Car;

		public CarObject(int ID, string name, DateTime beginDate, DateTime endDate, Car car)
		{
			this.ID = ID;
			this.Name = name;
			this.BeginDate = beginDate;
			this.EndDate = endDate;
			this.Car = car;
		}

		public CarObject(string name, DateTime beginDate, DateTime endDate, Car car)
		{
			ID = 1;
			foreach (CarObject carObject in car.CarObjects)
			{
				if (carObject.ID == ID)
				{
					ID++;
				}
			}
			this.Name = name;
			this.BeginDate = beginDate;
			this.EndDate = endDate;
			this.Car = car;
		}
	}
}