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
	public enum Unit
	{
		Metric = 0,
		USImperial = 1,
		UKImperial = 2,
		NotSet = 3
	}

	public static class UnitType
	{
		/// <summary>
		/// Current Measure Unit
		/// </summary>
		public static Unit Unit = Unit.Metric;
		public static double Kilometers = 1;
		public static double Liters = 1;
		public static List<UnitSystem> SystemUnits = new List<UnitSystem>();

		public static string GetUnitName(Unit UnitType = Unit.NotSet)
		{
			if (UnitType == Unit.NotSet)
			{
				UnitType = Unit;
			}
			switch (UnitType)
			{
				case Unit.Metric:
					return "Metric";
				case Unit.USImperial:
					return "US Imperial";
				case Unit.UKImperial:
					return "UK Imperial";
				case Unit.NotSet:
				default:
					return "Not Set";
			}
		}

		public static string GetUnitLengthType(Unit UnitType = Unit.NotSet)
		{
			if (UnitType == Unit.NotSet)
			{
				UnitType = Unit;
			}
			return UnitType == Unit.Metric ? Globals.Context.GetString(Resource.String.Kilometer) : Globals.Context.GetString(Resource.String.Mile);
		}

		public static string GetUnitVolumeType(Unit UnitType = Unit.NotSet)
		{
			if (UnitType == Unit.NotSet)
			{
				UnitType = Unit;
			}
			return UnitType == Unit.Metric ? Globals.Context.GetString(Resource.String.Liter) : UnitType == Unit.USImperial ? Globals.Context.GetString(Resource.String.USGallon) : Globals.Context.GetString(Resource.String.UKGallon);
		}

		/// <summary>
		/// Use to calculate differences between units
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public static void ConvertToUnit(Unit from, Unit to)
		{
			Kilometers = SystemUnits[(int)to].Kilometers / SystemUnits[(int)from].Kilometers;
			Liters = SystemUnits[(int)to].Liters / SystemUnits[(int)from].Liters;
		}

		/// <summary>
		/// Use to set the new unit for future transactions
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public static void ConvertToUnit(Unit to)
		{
			Unit = SystemUnits[(int)to].Unit;
			Kilometers = SystemUnits[(int)to].Kilometers;
			Liters = SystemUnits[(int)to].Liters;
		}

		/// <summary>
		/// Check and change, where needed, the unit type for transactions and cars
		/// </summary>
		public static void ChangeUnitType(Unit UnitType = Unit.NotSet)
		{
			if (UnitType == Unit.NotSet)
			{
				UnitType = Unit;
			}
			//for transactions
			foreach (Month month in Globals.MonthsList)
			{
				foreach (Transaction transaction in month.Transactions)
				{
					if (transaction.HasCarConsumption && transaction.Unit != UnitType)
					{
						if (transaction.Unit == Unit.NotSet)
						{
							transaction.Unit = UnitType;
						}
						else
						{
							ConvertToUnit(transaction.Unit, UnitType);
							transaction.Kilometers = transaction.Kilometers / Kilometers;
							transaction.Liters = transaction.Liters / Liters;
						}
					}
				}
			}

			//for repair
			foreach (Car car in CarsGlobals.CarsList)
			{
				foreach (Repair repair in car.Repairs)
				{
					if (repair.Unit != UnitType)
					{
						if (repair.Unit == Unit.NotSet)
						{
							repair.Unit = UnitType;
						}
						else
						{
							ConvertToUnit(repair.Unit, UnitType);
							repair.Kilometers = repair.Kilometers / Kilometers;
						}
					}
				}
			}
		}
	}

	public class UnitSystem
	{
		public Unit Unit;
		public double Kilometers;
		public double Liters;

		public UnitSystem (Unit Unit, double Kilometers, double Liters)
		{
			this.Unit = Unit;
			this.Kilometers = Kilometers;
			this.Liters = Liters;
		}
	}
}