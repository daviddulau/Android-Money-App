using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static ManyManager.Resource;

namespace ManyManager
{
	public class CarObjectsAdapter : BaseAdapter<CarObject>
	{
		Activity CurrentAdapterActivity;
		List<CarObject> listCarObjects;

		public CarObjectsAdapter(Activity currentContext, List<CarObject> lstcarobjectInfo)
		{
			CurrentAdapterActivity = currentContext;
			listCarObjects = lstcarobjectInfo;
		}


		#region Events

		private void imgModifyCarObject_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridCarObjectID);
				if (txtID.Text != "ID")
				{
					for (int i = CarsGlobals.SelectedCar.CarObjects.Count - 1; i >= 0; i--)
					{
						if (CarsGlobals.SelectedCar.CarObjects[i].ID == Convert.ToInt32(txtID.Text))
						{
							CarsGlobals.SelectedCarObject = CarsGlobals.SelectedCar.CarObjects[i];
							CarsGlobals.AddNewCarObject = false;
							Globals.Activity.SetContentView(Globals.CarObject);
							break;
						}
					}
				}
			}
		}

		private void imgRemoveCarObject_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridCarObjectID);
				TextView txtName = lay.FindViewById<TextView>(Id.txtGridCarObjectName);
				if (txtID.Text != "ID")
				{
					for (int i = listCarObjects.Count - 1; i >= 0; i--)
					{
						if (listCarObjects[i].ID == Convert.ToInt32(txtID.Text))
						{
							Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
							alert.SetTitle("Remove This Belonging");
							alert.SetMessage("Are You Sure You Want To Remove " + txtName.Text + " ?");
							alert.SetPositiveButton("Yes", (senderAlert, args) =>
							{
								RemoveCarObject(listCarObjects[i]);
								UpdateAdapter(listCarObjects);
							});
							alert.SetNegativeButton("No", (senderAlert, args) =>
							{
							});
							Dialog dialog = alert.Create();
							dialog.Show();
							break;
						}
					}
				}
			}
		}

		#endregion

		#region Utils

		public void AddCarObject(string Name, DateTime beginDate, DateTime endDate)
		{
			CarsGlobals.SelectedCar.CarObjects.Add(new CarObject(Name, beginDate, endDate, CarsGlobals.SelectedCar));
			UpdateAdapter(CarsGlobals.SelectedCar.CarObjects);
			XML.RefreshCars();
		}

		public void AddCarObject(string Name)
		{
			CarsGlobals.SelectedCar.CarObjects.Add(new CarObject(Name, DateTime.Now, DateTime.Now.AddYears(1), CarsGlobals.SelectedCar));
			UpdateAdapter(CarsGlobals.SelectedCar.CarObjects);
			XML.RefreshCars();
		}

		public void RemoveCarObject(CarObject carObject)
		{
			//remove from global list
			CarsGlobals.SelectedCar.CarObjects.Remove(carObject);
			//find & remove from xml
			XML.RefreshCars();
			CarsGlobals.WhatCarStuffToDisplay();
		}

		public void UpdateAdapter(List<CarObject> carObjects)
		{
			//refresh
			this.listCarObjects = carObjects;
			CarsGlobals.SelectedCar.CarObjects = carObjects;
			NotifyDataSetChanged();
		}

		#endregion

		#region implemented abstract members of BaseAdapter

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			CarObject item = listCarObjects[position];

			if (convertView == null)
				convertView = CurrentAdapterActivity.LayoutInflater.Inflate(Globals.GridViewCarObjectItem, null);

			TextView txtItemName = convertView.FindViewById<TextView>(Id.txtGridCarObjectName);
			TextView textItemID = convertView.FindViewById<TextView>(Id.txtGridCarObjectID);
			TextView textItemExpiryDate = convertView.FindViewById<TextView>(Id.txtGridCarObjectExpiryDate);
			
			textItemID.Text = item.ID.ToString();
			txtItemName.Text = item.Name;

			double days = TimeSpan.FromDays(item.EndDate.ToOADate()).Days - TimeSpan.FromDays(DateTime.Now.ToOADate()).Days;
			int months = Convert.ToInt32(days / 30);
			string years = (days / 365).ToString("0.0");
			if (days < 0)
			{
				textItemExpiryDate.Text = "Expired";
			}
			else if (days == 0)
			{
				textItemExpiryDate.Text = "Expire Today";
			}
			else if (days == 1)
			{
				textItemExpiryDate.Text = "Expire Tomorrow";
			}
			else if (days < 30)
			{
				textItemExpiryDate.Text = "Expire in: " + days + " Days";
			}
			else if (days < 60)
			{
				textItemExpiryDate.Text = "1 Month";
				int remainingDays = Convert.ToInt32(days / 30);
				if (remainingDays != 0)
				{
					if (remainingDays == 1)
					{
						textItemExpiryDate.Text += " 1 Day";
					}
					else
					{
						textItemExpiryDate.Text += " " + remainingDays + " Days";
					}
				}
			}
			else if (days < 360)
			{
				textItemExpiryDate.Text = months + " Months";
				int remainingDays = Convert.ToInt32(days % 30);
				if (remainingDays != 0)
				{
					if (remainingDays == 1)
					{
						textItemExpiryDate.Text += " 1 Day";
					}
					else
					{
						textItemExpiryDate.Text += " " + remainingDays + " Days";
					}
				}
			}
			else if (days >= 360 && days < 365)
			{
				textItemExpiryDate.Text = "< 1 Year";
			}
			else if (days == 365)
			{
				textItemExpiryDate.Text = "1 Year";
			}
			else if (days > 730)
			{
				textItemExpiryDate.Text = years + " Years";
			}
			else if (days > 365)
			{
				textItemExpiryDate.Text = "> 1 Year";
			}

			if (days < 0)
			{
				textItemExpiryDate.SetTextColor(Android.Graphics.Color.Red);
			}
			else if (days <= 30)
			{
				textItemExpiryDate.SetTextColor(Android.Graphics.Color.Red);
			}
			else if (days <= 60)
			{
				textItemExpiryDate.SetTextColor(Android.Graphics.Color.Yellow);
			}
			else if (days >= 60)
			{
				textItemExpiryDate.SetTextColor(Android.Graphics.Color.Green);
			}

			//verify to have only one click event attached
			var imgModify = convertView.FindViewById<ImageView>(Id.imgModifyCarObject);
			if (!imgModify.HasOnClickListeners)
			{
				imgModify.Click += imgModifyCarObject_Click;
			}
			var imageRemove = convertView.FindViewById<ImageView>(Id.imgRemoveCarObject);
			if (!imageRemove.HasOnClickListeners)
			{
				imageRemove.Click += imgRemoveCarObject_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listCarObjects == null ? -1 : listCarObjects.Count;
			}
		}

		public override CarObject this[int position]
		{
			get
			{
				return listCarObjects == null ? null : listCarObjects[position];
			}
		}

		#endregion

	}
}