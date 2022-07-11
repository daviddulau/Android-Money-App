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
using static ManyManager.Resource;

namespace ManyManager
{
	public class RepairsAdapter : BaseAdapter<Repair>
	{
		Activity CurrentAdapterActivity;
		List<Repair> listRepairs;

		public RepairsAdapter(Activity currentContext, List<Repair> lstRepairsInfo)
		{
			CurrentAdapterActivity = currentContext;
			listRepairs = lstRepairsInfo;
		}


		#region Events

		private void imgModifyRepair_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridRepairID);
				if (txtID.Text != "ID")
				{
					for (int i = CarsGlobals.SelectedCar.Repairs.Count - 1; i >= 0; i--)
					{
						if (CarsGlobals.SelectedCar.Repairs[i].ID == Convert.ToInt32(txtID.Text))
						{
							CarsGlobals.SelectedCarRepair = CarsGlobals.SelectedCar.Repairs[i];
							CarsGlobals.AddNewCarRepair = false;
							Globals.Activity.SetContentView(Globals.RepairMenu);
							break;
						}
					}
				}
			}
		}

		private void imgRemoveRepair_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridRepairID);
				TextView txtName = lay.FindViewById<TextView>(Id.txtGridRepairName);
				if (txtID.Text != "ID")
				{
					for (int i = listRepairs.Count - 1; i >= 0; i--)
					{
						if (listRepairs[i].ID == Convert.ToInt32(txtID.Text))
						{
							Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
							alert.SetTitle("Remove This Repair");
							alert.SetMessage("Are You Sure You Want To Remove " + txtName.Text + " ?");
							alert.SetPositiveButton("Yes", (senderAlert, args) =>
							{
								RemoveRepair(listRepairs[i]);
								UpdateAdapter(listRepairs);
								CarsGlobals.WhatCarStuffToDisplay();
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

		public void RemoveRepair(Repair repair)
		{
			//remove from global list
			CarsGlobals.SelectedCar.Repairs.Remove(repair);
			//find & remove from xml
			XML.RefreshCars();
			CarsGlobals.WhatCarStuffToDisplay();
		}

		public void UpdateAdapter(List<Repair> repairs)
		{
			//refresh
			this.listRepairs = repairs;
			CarsGlobals.SelectedCar.Repairs = repairs;
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
			Repair item = listRepairs[position];

			if (convertView == null)
				convertView = CurrentAdapterActivity.LayoutInflater.Inflate(Globals.GridViewRepairItem, null);

			TextView txtItemName = convertView.FindViewById<TextView>(Id.txtGridRepairName);
			TextView textItemID = convertView.FindViewById<TextView>(Id.txtGridRepairID);
			TextView textItemDate = convertView.FindViewById<TextView>(Id.txtGridRepairDate);
			TextView textItemKilometers = convertView.FindViewById<TextView>(Id.txtGridRepairKilometers);
			TextView textItemSum = convertView.FindViewById<TextView>(Id.txtGridRepairSum);

			textItemID.Text = item.ID.ToString();
			txtItemName.Text = item.Name;
			textItemDate.Text = item.Date.ToShortDateString();
			textItemKilometers.Text = item.Kilometers.ToString("#,##0.00");
			textItemSum.Text = item.Sum.ToString("#,##0.00");

			//verify to have only one click event attached
			var imgModify = convertView.FindViewById<ImageView>(Id.imgModifyCarRepair);
			if (!imgModify.HasOnClickListeners)
			{
				imgModify.Click += imgModifyRepair_Click;
			}
			var imageRemove = convertView.FindViewById<ImageView>(Id.imgRemoveCarRepair);
			if (!imageRemove.HasOnClickListeners)
			{
				imageRemove.Click += imgRemoveRepair_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listRepairs == null ? -1 : listRepairs.Count;
			}
		}

		public override Repair this[int position]
		{
			get
			{
				return listRepairs == null ? null : listRepairs[position];
			}
		}

		#endregion

	}
}