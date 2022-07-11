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
	public static class CarsGlobals
	{

		#region Enums

		public enum CarElement
		{
			Object = 0,
			Repair = 1,
			Transaction = 2,
			Consumption = 3,
			Tag = 4,
			Properties = 5
		}


		#endregion

		#region Public Properties

		/// <summary>
		/// Store all Car & theirs CarObjects
		/// </summary>
		public static List<Car> CarsList = new List<Car>();

		/// <summary>
		/// Car to work with it's CarObjects
		/// </summary>
		public static Car SelectedCar;

		public static CarObject SelectedCarObject;
		public static Repair SelectedCarRepair;

		public static bool CarsEditState = false;
		public static bool AddNewCar = false;
		public static bool NewCarWasAdded = false;

		public static bool AddNewCarObject = false;
		public static bool NewCarObjectWasAdded = false;

		public static CarElement PopulatedAdapter = CarElement.Object;

		public static bool AddNewCarRepair = false;

		#endregion

		#region Public Methods

		public static void WhatCarStuffToDisplay()
		{
			bool show = PopulatedAdapter == CarElement.Object || PopulatedAdapter == CarElement.Repair;
			Globals.layoutCarStuffs.Visibility = show ? ViewStates.Invisible : ViewStates.Visible;
			Globals.layoutCarCarObjectsRepairs.Visibility = show ? ViewStates.Visible : ViewStates.Invisible;
			
			switch (PopulatedAdapter)
			{
				case CarElement.Object:
					Globals.txtNoCarObjectsOrRepairs.Text = SelectedCar.CarObjects.Count == 0 ? "No Car Belongings To Display" : "";
					Globals.txtNoCarObjectsOrRepairs.BringToFront();
					Globals.txtNoCarObjectsOrRepairs.Visibility = SelectedCar.CarObjects.Count == 0 ? ViewStates.Visible : ViewStates.Invisible;
					break;
				case CarElement.Repair:
					Globals.txtNoCarObjectsOrRepairs.Text = SelectedCar.Repairs.Count == 0 ? "No Car Repair History To Display" : "";
					Globals.txtNoCarObjectsOrRepairs.BringToFront();
					Globals.txtNoCarObjectsOrRepairs.Visibility = SelectedCar.Repairs.Count == 0 ? ViewStates.Visible : ViewStates.Invisible;
					break;
				case CarElement.Transaction:
					Globals.txtNoCarStuffs.Text = MoneyGlobals.DisplayTransactions.Count == 0 ? "No Car Transactions To Display" : "";
					Globals.txtNoCarStuffs.BringToFront();
					Globals.txtNoCarStuffs.Visibility = MoneyGlobals.DisplayTransactions.Count == 0 ? ViewStates.Visible : ViewStates.Invisible;
					break;
				case CarElement.Tag:
					Globals.txtNoCarStuffs.Text = TagsGlobals.DisplayTagsList.Count == 0 ? "No Car Tags To Display" : "";
					Globals.txtNoCarStuffs.BringToFront();
					Globals.txtNoCarStuffs.Visibility = TagsGlobals.DisplayTagsList.Count == 0 ? ViewStates.Visible : ViewStates.Invisible;
					break;
				case CarElement.Consumption:
					Globals.tvwTransactionsNoData.Visibility = MoneyGlobals.DisplayTransactions.Count == 0 ? ViewStates.Visible : ViewStates.Invisible;
					Globals.grdCarConsumption.Visibility = MoneyGlobals.DisplayTransactions.Count == 0 ? ViewStates.Invisible : ViewStates.Visible;
					break;
				case CarElement.Properties:
					break;
			}
		}

		public static void WhatCarToDisplay()
		{
			Globals.txtNoCars.Visibility = CarsList.Count > 0 ? ViewStates.Invisible : ViewStates.Visible;
			Globals.grdCars.Visibility = CarsList.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
		}

		public static void RemoveDuplicated(List<Car> list)
		{
			foreach (Car car in list.Reverse<Car>())
			{
				int count = CarCountInList(list, car);
				if (count > 1)
				{
					list.Remove(car);
				}
			}
		}

		public static List<Car> MoveElements(List<Car> oldList, List<Car> newList)
		{
			foreach (Car element in oldList)
			{
				newList.Add(element);
			}
			return newList;
		}

		/// <summary>
		/// If list has the car will be added from that list. If doesn't exist will be searched in Global List
		/// </summary>
		/// <param name="list"></param>
		/// <param name="car"></param>
		public static void AddCarToList(List<Car> list, Car car)
		{
			int i = CarIndexInList(list, car);
			if (i != -1)
			{
				list.Remove(car);
				list.Add(car);
			}
			else
			{
				i = CarIndexInList(CarsList, car);
				list.Add(CarsList[i]);
			}
		}

		public static void AssignTransactionToCar(Transaction transaction, Transaction oldTransaction = null)
		{
			if (oldTransaction == null)
			{
				oldTransaction = transaction;
			}
			if (Globals.SelectedCarsForTransaction.Count > 0)
			{
				foreach (Car selectedCar in Globals.SelectedCarsForTransaction.Reverse<Car>())
				{
					if (Globals.SelectedCarsForTransaction.Count > 0)
					{
						foreach (Car item in CarsList)
						{
							if (item.ID == selectedCar.ID && item.Plate == selectedCar.Plate)
							{
								//remove from list and then process it
								Globals.SelectedCarsForTransaction.Remove(selectedCar);

								RemoveDuplicated(transaction.Cars);

								if (selectedCar.Checked)
								{
									if (CarInList(oldTransaction.Cars, selectedCar))
									{
										int i = CarIndexInList(oldTransaction.Cars, selectedCar);
										if (i >= 0)
										{
											oldTransaction.Cars.RemoveAt(i);
										}
									}
									if (!CarInList(transaction.Cars, selectedCar))
									{
										transaction.Cars.Add(selectedCar);
									}

									if (MoneyGlobals.TransactionInList(selectedCar.Transactions, oldTransaction))
									{
										int i = MoneyGlobals.TransactionIndexInList(selectedCar.Transactions, oldTransaction);
										if (i >= 0)
										{
											selectedCar.Transactions.RemoveAt(i);
										}
									}
									if (!MoneyGlobals.TransactionInList(selectedCar.Transactions, transaction))
									{
										selectedCar.Transactions.Add(transaction);
									}
								}
								else if (!selectedCar.Checked)
								{
									if (CarInList(transaction.Cars, selectedCar))
									{
										transaction.Cars.Remove(selectedCar);
									}
									if (MoneyGlobals.TransactionInList(selectedCar.Transactions, transaction))
									{
										selectedCar.Transactions.Remove(transaction);
									}
								}
								break;
							}
						}
					}
				}
			}
			else
			{
				//if no modification and Globals.SelectedCarsForTransaction.Count is 0 check if there's already attached and don't remove by default
				foreach (Car car in CarsList)
				{
					if (!car.Checked)
					{
						if (car.Transactions.Contains(transaction))
						{
							car.Transactions.Remove(transaction);
						}
						if (transaction.Cars.Contains(car))
						{
							transaction.Cars.Remove(car);
						}
					}
				}
			}
			XML.RefreshCars();
			Globals.SelectedCarsForTransaction.Clear();
			Globals.CarToAddInTransaction = false;
			MoneyGlobals.PendingTransaction = null;
		}

		public static int CarCountInList(List<Car> list, Car obj)
		{
			int j = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (CarMatch(list[i], obj))
				{
					j++;
				}
			}
			return j;
		}

		public static int CarIndexInList(List<Car> list, Car obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CarMatch(list[i], obj))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool CarInList(List<Car> list, Car obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CarMatch(list[i], obj))
				{
					return true;
				}
			}
			return false;
		}

		public static bool CarMatch(Car c1, Car c2)
		{
			if (c1.Brand != c2.Brand)
			{
				return false;
			}
			else if (c1.Model != c2.Model)
			{
				return false;
			}
			else if (c1.Plate != c2.Plate)
			{
				return false;
			}
			else if (c1.Year != c2.Year)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static int CarObjectIndexInList(List<CarObject> list, CarObject obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CarObjectMatch(list[i], obj))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool CarObjectInList(List<CarObject> list, CarObject obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CarObjectMatch(list[i], obj))
				{
					return true;
				}
			}
			return false;
		}

		public static bool CarObjectMatch(CarObject c1, CarObject c2)
		{
			if (c1.Name != c2.Name)
			{
				return false;
			}
			else if (c1.BeginDate != c2.BeginDate)
			{
				return false;
			}
			else if (c1.EndDate != c2.EndDate)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static int CarIndexInList(List<Repair> list, Repair repair)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CarRepairMatch(list[i], repair))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool CarRepairInList(List<Repair> list, Repair repair)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (CarRepairMatch(list[i], repair))
				{
					return true;
				}
			}
			return false;
		}

		public static bool CarRepairMatch(Repair r1, Repair r2)
		{
			if (r1.Name != r2.Name)
			{
				return false;
			}
			else if (r1.Description != r2.Description)
			{
				return false;
			}
			else if (r1.Date != r2.Date)
			{
				return false;
			}
			else if (r1.Kilometers != r2.Kilometers)
			{
				return false;
			}
			else if (r1.Sum != r2.Sum)
			{
				return false;
			}
			else if (r1.Unit != r2.Unit)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static void VisibleAll(bool visible = true, List<Car> list = null)
		{
			if (list == null)
			{
				list = CarsList;
			}
			foreach (Car car in list)
			{
				car.Visible = visible;
			}
		}

		public static void CheckAll(bool check = false, List <Car> list = null)
		{
			if (list == null)
			{
				list = CarsList;
			}
			foreach (Car car in list)
			{
				car.Checked = check;
			}
		}

		#endregion
	}
}