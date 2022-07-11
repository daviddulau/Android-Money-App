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
	public class AccountsAdapter : BaseAdapter<Account>
	{
		Activity CurrentAccountsActivity;
		List<Account> listAccounts;

		public AccountsAdapter(Activity currentContext, List<Account> lstAccountsInfo)
		{
			CurrentAccountsActivity = currentContext;
			listAccounts = lstAccountsInfo;
		}


		#region Events

		private void imgLoginAccount_Click(object sender, EventArgs e)
		{
			Button t = sender as Button;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridAccountID);
				if (txtID.Text != "ID")
				{
					for (int i = AccountsGlobals.AccountsList.Count - 1; i >= 0; i--)
					{
						if (AccountsGlobals.AccountsList[i].ID == Convert.ToInt32(txtID.Text))
						{
							AccountsGlobals.SelectedAccount = AccountsGlobals.AccountsList[i];
							Globals.UserName = AccountsGlobals.SelectedAccount.Name;
							Globals.Activity.SetContentView(Globals.MainMenu);
							break;
						}
					}
				}
			}
		}

		private void imgModifyAccount_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridAccountID);
				if (txtID.Text != "ID")
				{
					for (int i = AccountsGlobals.AccountsList.Count - 1; i >= 0; i--)
					{
						if (AccountsGlobals.AccountsList[i].ID == Convert.ToInt32(txtID.Text))
						{
							AccountsGlobals.SelectedAccount = AccountsGlobals.AccountsList[i];
							AccountsGlobals.NewAccountWasAdded = true;
							Globals.Activity.SetContentView(Globals.AccountMenu);
							Globals.PreviousForm = Globals.AccountsMenu;
							break;
						}
					}
				}
			}
		}

		private void imgRemoveAccount_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridAccountID);
				TextView txtName = lay.FindViewById<Button>(Id.btnGridLoginAccount);
				if (txtID.Text != "ID")
				{
					for (int i = listAccounts.Count - 1; i >= 0; i--)
					{
						if (listAccounts[i].ID == Convert.ToInt32(txtID.Text))
						{
							Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
							alert.SetTitle("Remove This Account");
							alert.SetMessage("All Data Will Be Lost. Are You Sure You Want To Remove " + txtName.Text.Replace(" Login", "") + " ?");
							alert.SetPositiveButton("Yes", (senderAlert, args) =>
							{
								RemoveAccount(listAccounts[i]);
								UpdateAdapter(listAccounts);
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

		public void RemoveAccount(Account account)
		{
			//remove from global list
			AccountsGlobals.AccountsList.Remove(account);
			//find & remove from xml
			XML.RefreshAccounts();
			XML.RemoveFolder(account);
		}

		public void UpdateAdapter(List<Account> accounts)
		{
			//refresh
			this.listAccounts = accounts;
			AccountsGlobals.AccountsList = accounts;
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
			Account item = listAccounts[position];

			if (convertView == null)
				convertView = CurrentAccountsActivity.LayoutInflater.Inflate(Globals.GridViewAccountItem, null);

			Button btnItemName = convertView.FindViewById<Button>(Id.btnGridLoginAccount);
			TextView textItemID = convertView.FindViewById<TextView>(Id.txtGridAccountID);

			textItemID.Text = item.ID.ToString();
			btnItemName.Text = item.Name + " Login";

			//verify to have only one click event attached
			if (!btnItemName.HasOnClickListeners)
			{
				btnItemName.Click += imgLoginAccount_Click;
			}

			var imgModify = convertView.FindViewById<ImageView>(Id.imgModifyAccount);
			if (!imgModify.HasOnClickListeners)
			{
				imgModify.Click += imgModifyAccount_Click;
			}

			var imageRemove = convertView.FindViewById<ImageView>(Id.imgRemoveAccount);
			if (!imageRemove.HasOnClickListeners)
			{
				imageRemove.Click += imgRemoveAccount_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listAccounts == null ? -1 : listAccounts.Count;
			}
		}

		public override Account this[int position]
		{
			get
			{
				return listAccounts == null ? null : listAccounts[position];
			}
		}

		#endregion
	}
}