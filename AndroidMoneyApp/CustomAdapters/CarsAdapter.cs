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
	public class CarsAdapter : BaseAdapter<Car>
	{
		Activity CurrentCarsContext;
		List<Car> listCars;
		public int indexOnce = -1;

		public CarsAdapter(Activity currentContext, List<Car> lstcarsInfo)
		{
			CurrentCarsContext = currentContext;
			listCars = lstcarsInfo;
		}

		#region Events

		public void LayoutClickableCar_LongClick(object sender, EventArgs e)
		{
			//enter edit Cars state
			EditState(true);
			UpdateAdapter(CarsGlobals.CarsList);
		}

		public void LayoutClickableCar_Click(object sender, EventArgs e)
		{
			if (CarsGlobals.CarsEditState)
			{
				// don't enter in miniCar if edit state is on
				return;
			}
			//enter edit Cars state
			EditState();

			//Enter in Car
			LinearLayout layout = sender as LinearLayout;
			TextView txtID = layout.FindViewById<TextView>(Resource.Id.txtGridCarsID);
			if (txtID != null)
			{
				CarsGlobals.SelectedCar = CarsGlobals.CarsList.Find(x => x.ID == Convert.ToInt32(txtID.Text));
				if (CarsGlobals.SelectedCar != null)
				{
					CarsGlobals.NewCarWasAdded = true;
					Globals.Activity.SetContentView(Globals.CarMenu);
				}
			}
		}

		public void imgGridCarsRemove_Click(object sender, EventArgs e)
		{
			Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
			alert.SetTitle("Remove?");
			alert.SetMessage("Are you sure you want to remove the car" + (CarsGlobals.CarsList.FindAll(x => x.Checked == true).Count > 1 ? "s" : "") + " ?");
			alert.SetPositiveButton("Yes", (senderAlert, args) =>
			{
				ImageView t = sender as ImageView;
				TextView txtID = (t.Parent.Parent.Parent as RelativeLayout).FindViewById<TextView>(Resource.Id.txtGridCarsID);
				if (txtID.Text != "ID")
				{
					for (int i = listCars.Count - 1; i >= 0; i--)
					{
						if (listCars[i].ID == Convert.ToInt32(txtID.Text))
						{
							listCars.RemoveAt(i);
							break;
						}
					}
				}
				EditState();
				CheckAll();
				(Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter(listCars);
				XML.RefreshCars();
				CarsGlobals.WhatCarToDisplay();
			});
			alert.SetNeutralButton("No", (senderAlert, args) =>
			{
				EditState();
				CheckAll();
				(Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		private void chkGridCarsSelectionCheck_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			CheckBox t = sender as CheckBox;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Resource.Id.txtGridCarsID);
				if (txtID.Text != "ID")
				{
					for (int i = listCars.Count - 1; i >= 0; i--)
					{
						if (listCars[i].ID == Convert.ToInt32(txtID.Text))
						{
							listCars[i].Checked = t.Checked;
							break;
						}
					}
					CheckCheckAllCheckBox();
				}
			}
		}

		#endregion

		#region Utils

		public void CheckCheckAllCheckBox()
		{
			if (Globals.chkCarCheckAll != null)
			{
				Globals.doNothing = true;
				int checkboxes = listCars.FindAll(x => x.Checked).Count;
				Globals.chkCarCheckAll.Checked = listCars.Count == 0 ? false : listCars.Count == checkboxes;
				Globals.doNothing = false;
			}
		}

		public void BackToDesignCheck()
		{
			if (indexOnce < Globals.grdCars.ChildCount)
			{
				indexOnce = Globals.grdCars.ChildCount;
				for (int i = 0; i < Globals.grdCars.ChildCount; i++)
				{
					RelativeLayout Car = Globals.grdCars.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfCar);
					CheckBox chk = Car.FindViewById<CheckBox>(Resource.Id.chkGridCarsSelectionCheck);
					chk.Checked = listCars[i].Checked;
				}
				CarsGlobals.CarsList = listCars;
				(Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();
			}
		}

		public void DesignToBackCheck()
		{
			if (indexOnce < Globals.grdCars.ChildCount)
			{
				indexOnce = Globals.grdCars.ChildCount;
				for (int i = 0; i < Globals.grdCars.ChildCount; i++)
				{
					RelativeLayout Car = Globals.grdCars.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfCar);
					CheckBox chk = Car.FindViewById<CheckBox>(Resource.Id.chkGridCarsSelectionCheck);
					listCars[i].Checked = chk.Checked;
				}
				CarsGlobals.CarsList = listCars;
				(Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();
			}
		}

		public void CarToAddInTag()
		{
			Globals.SelectedCarsForTag.Clear();
			foreach (Car car in listCars)
			{
				if (car.Checked && car.Visible)
				{
					Globals.SelectedCarsForTag.Add(car);
				}
			}
			#region OldCode - Check to don't assign multiple tags to one car
				//string cars = string.Empty;
				//for (int i = listCars.Count - 1; i >= 0; i--)
				//{
				//	bool found = false;
				//	foreach (Tag tag in TagsGlobals.TagsList)
				//	{
				//		if (listCars[i].Checked && !TagsGlobals.TagMatch(TagsGlobals.SelectedTag, tag) && tag.Cars.Contains(listCars[i]))
				//		{
				//			cars += listCars[i].Plate + " ";
				//			found = true;
				//		}
				//	}
				//	if (!found)
				//	{
				//		Globals.SelectedCarsForTag.Add(listCars[i]);
				//	}
				//}
				//if (!string.IsNullOrEmpty(cars))
				//{
				//	Utils.ShowAlert("Can't Assign Car To Multiple Tags", "The Cars: " + cars + "Are Already Assigned To Different Tag!");
				//}
				#endregion
		}

		public void CarToAddInTransaction()
		{
			Globals.SelectedCarsForTransaction.Clear();
			for (int i = listCars.Count - 1; i >= 0; i--)
			{
				Globals.SelectedCarsForTransaction.Add(listCars[i]);
			}
		}

		public void RevertChanges()
		{
			CarsGlobals.CheckAll(false, listCars);

			for (int i = 0; i < Globals.grdCars.ChildCount; i++)
			{
				RelativeLayout Car = Globals.grdCars.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfCar);
				CheckBox chk = Car.FindViewById<CheckBox>(Resource.Id.chkGridCarsSelectionCheck);
				chk.Checked = false;
			}
			DesignToBackCheck();
			CarsGlobals.CarsList = listCars;
			(Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();
		}

		public void CheckAll(bool checkState = false)
		{
			CarsGlobals.CheckAll(checkState, listCars);

			for (int i = 0; i < Globals.grdCars.ChildCount; i++)
			{
				RelativeLayout Car = Globals.grdCars.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfCar);
				CheckBox chk = Car.FindViewById<CheckBox>(Resource.Id.chkGridCarsSelectionCheck);
				chk.Checked = checkState;
			}
			CarsGlobals.CarsList = listCars;
			(Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();
		}

		public void RemoveSelected()
		{
			for (int i = listCars.Count - 1; i >= 0; i--)
			{
				if (listCars[i].Checked)
				{
					listCars.RemoveAt(i);
				}
			}
			EditState();
			CheckAll();
			(Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter(listCars);
			XML.RefreshCars();
			CarsGlobals.WhatCarToDisplay();
		}

		public void UpdateAdapter(List<Car> listCars = null)
		{
			if (listCars == null)
			{
				listCars = CarsGlobals.CarsList;
			}
			//refresh
			this.listCars = listCars;
			CarsGlobals.CarsList = listCars;
			NotifyDataSetChanged();
		}

		public void EditState(bool variable = false)
		{
			if (Globals.CarToAddInTransaction || Globals.CarToAddInTag)
			{
				CarsGlobals.CarsEditState = true;
				Globals.layoutAddNewCar.Visibility = ViewStates.Invisible;
				Globals.imgDoneCars.Visibility = ViewStates.Visible;
				Globals.imgRemoveCar.Visibility = ViewStates.Invisible;
				Globals.chkCarCheckAll.Visibility = ViewStates.Visible;
			}
			else if (variable)
			{
				CarsGlobals.CarsEditState = true;
				Globals.layoutAddNewCar.Visibility = ViewStates.Visible;
				Globals.imgDoneCars.Visibility = ViewStates.Visible;
				Globals.imgRemoveCar.Visibility = ViewStates.Visible;
				Globals.chkCarCheckAll.Visibility = ViewStates.Visible;
			}
			else
			{
				CarsGlobals.CarsEditState = false;
				Globals.layoutAddNewCar.Visibility = ViewStates.Visible;
				Globals.imgDoneCars.Visibility = ViewStates.Invisible;
				Globals.imgRemoveCar.Visibility = ViewStates.Invisible;
				Globals.chkCarCheckAll.Visibility = ViewStates.Invisible;
				Globals.chkCarCheckAll.Checked = false;
			}
		}

		#endregion

		#region implemented abstract members of BaseAdapter

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			Car item = listCars[position];

			if (item == null)
			{
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).Dispose();
			}
			if (convertView == null)
				convertView = CurrentCarsContext.LayoutInflater.Inflate(Globals.GridViewCarItem, null);
			
			convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).Visibility = !item.Visible ? ViewStates.Gone : ViewStates.Visible;

			if (convertView.FindViewById<TextView>(Resource.Id.txtGridCarsID).Text == "ID")
			{
				convertView.FindViewById<TextView>(Resource.Id.txtGridCarsID).Text = item.ID.ToString();
			}
			(Globals.grdCars.Adapter as CarsAdapter).BackToDesignCheck();


			CheckBox checkbox = convertView.FindViewById<CheckBox>(Resource.Id.chkGridCarsSelectionCheck);

			checkbox.Visibility = CarsGlobals.CarsEditState ? ViewStates.Visible : ViewStates.Invisible;
			checkbox.Checked = item.Checked;
			convertView.FindViewById<TextView>(Resource.Id.txtGridCarsPlate).Text = item.Plate;
			convertView.FindViewById<TextView>(Resource.Id.txtGridCarsName).Text = item.Brand + " " + item.Model;
			convertView.FindViewById<ImageView>(Resource.Id.imgGridCarsRemove).Visibility = CarsGlobals.CarsEditState ? ViewStates.Visible : ViewStates.Invisible;

			Globals.layoutAddNewCar.Visibility = CarsGlobals.CarsEditState ? ViewStates.Invisible : ViewStates.Visible;
			Globals.imgDoneCars.Visibility = CarsGlobals.CarsEditState ? ViewStates.Visible : ViewStates.Invisible;

			//verify to have only one click event attached
			if (CarsGlobals.CarsEditState)
			{
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).LongClick -= LayoutClickableCar_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).LongClick += LayoutClickableCar_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).Click -= LayoutClickableCar_Click;
			}
			else if (!CarsGlobals.CarsEditState)
			{
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).LongClick -= LayoutClickableCar_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).LongClick += LayoutClickableCar_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).Click -= LayoutClickableCar_Click;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableCar).Click += LayoutClickableCar_Click;
			}
			if (!checkbox.HasOnClickListeners)
			{
				checkbox.CheckedChange += chkGridCarsSelectionCheck_CheckedChange;
			}
			if (!convertView.FindViewById<ImageView>(Resource.Id.imgGridCarsRemove).HasOnClickListeners)
			{
				convertView.FindViewById<ImageView>(Resource.Id.imgGridCarsRemove).Click += imgGridCarsRemove_Click;
			}
			return convertView;
		}

		public override int Count
		{
			get
			{
				return listCars == null ? -1 : listCars.Count;
			}
		}

		public override Car this[int position]
		{
			get
			{
				return listCars == null ? null : listCars[position];
			}
		}

		#endregion

	}
}