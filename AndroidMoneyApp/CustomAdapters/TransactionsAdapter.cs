using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SkiaSharp.Views.Android;
using static ManyManager.Resource;

namespace ManyManager
{
	public class TransactionsAdapter : BaseAdapter<Transaction>
	{
		Activity CurrentAdapterContext;
		List<Transaction> listTransactions;

		public TransactionsAdapter(Activity currentContext, List<Transaction> lstinsertInfo)
		{
			CurrentAdapterContext = currentContext;
			listTransactions = lstinsertInfo;
		}

		#region Events

		public void chkRemoveTransaction_CheckChanged(object sender, EventArgs e)
		{
			CheckBox t = sender as CheckBox;
			if (t.Parent != null && (t.Parent as View).FindViewById<ImageView>(Id.imgModifyTransaction) != null)
			{
				ImageView img = (t.Parent as View).FindViewById<ImageView>(Id.imgModifyTransaction);
				for (int i = listTransactions.Count - 1; i >= 0; i--)
				{
					Transaction transaction = MoneyGlobals.ViewToTransaction(img);
					if (transaction != null && MoneyGlobals.TransactionsMatch(listTransactions[i], transaction))
					{
						listTransactions[i].Checked = t.Checked;
						break;
					}
				}
				CheckCheckAllCheckBox();
			}
		}

		public void imgRemoveTransaction_Click(object sender, EventArgs e)
		{
			if (sender != null)
			{
				Globals.Activity.btnRemoveTransaction_Click(sender);
			}
		}

		public void imgCopyTransaction_Click(object sender, EventArgs e)
		{
			if (sender != null)
			{
				Globals.Activity.imgCopyTransaction_Click(sender);
			}
		}

		public void imgModifyTransaction_Click(object sender, EventArgs e)
		{
			if (sender != null)
			{
				Globals.Activity.btnModifyTransaction_Load(sender);
			}
		}

		public void LayoutClickableTransaction_LongClick(object sender, View.LongClickEventArgs e)
		{
			MoneyGlobals.TransactionsEditState = !MoneyGlobals.TransactionsEditState;
			EditState(MoneyGlobals.TransactionsEditState);
		}

		#endregion

		#region Utils

		public void CheckCheckAllCheckBox()
		{
			if (Globals.chkAllTransactions != null)
			{
				Globals.doNothing = true;
				int checkboxes = listTransactions.FindAll(x => x.Checked).Count;
				Globals.chkAllTransactions.Checked = listTransactions.Count == 0 ? false : listTransactions.Count == checkboxes;
				Globals.doNothing = false;
			}
		}

		public void RevertChanges()
		{
			for (int i = MoneyGlobals.DisplayTransactions.Count - 1; i >= 0; i--)
			{
				MoneyGlobals.DisplayTransactions[i].Checked = false;
			}
			for (int i = 0; i < Globals.grdTransactions.ChildCount; i++)
			{
				LinearLayout transactions = Globals.grdTransactions.GetChildAt(i).FindViewById<LinearLayout>(Resource.Id.LayoutRemoveTransaction);
				CheckBox chk = transactions.FindViewById<CheckBox>(Resource.Id.chkRemoveTransaction);
				chk.Checked = false;
			}
			(Globals.grdTransactions.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);
		}

		public void RemoveSelected()
		{
			for (int i = MoneyGlobals.DisplayTransactions.Count - 1; i >= 0; i--)
			{
				if (MoneyGlobals.DisplayTransactions[i].Checked)
				{
					RemoveItem(MoneyGlobals.DisplayTransactions[i]);
				}
			}
			MoneyGlobals.WhatTransactionToDisplay();
			EditState();
			(Globals.grdTransactions.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);
			MoneyGlobals.WhatTransactionToDisplay();
		}

		public void RemoveItem(Transaction transaction)
		{
			Month month = Globals.GetMonthFromTransaction(transaction);
			if (month != null)
			{
				month.RemoveTransaction(transaction);
				//remove from visual list
				MoneyGlobals.DisplayTransactions.Remove(transaction);
				//remove from global list
				MoneyGlobals.TransactionsList.Remove(transaction);
				//find & remove from xml too
				Globals.RefreshMonthWithNewTransaction(month);
				//refresh Tags & Colors
				MoneyGlobals.ReLoadTagAndCategoryColors();

				//refresh notes and cars
				Globals.ReLoadNotesAndCars();
			}
		}

		public void CheckAll(bool checkState = false)
		{
			for (int i = MoneyGlobals.DisplayTransactions.Count - 1; i >= 0; i--)
			{
				MoneyGlobals.DisplayTransactions[i].Checked = checkState;
			}
			for (int i = 0; i < Globals.grdTransactions.ChildCount; i++)
			{
				LinearLayout transactions = Globals.grdTransactions.GetChildAt(i).FindViewById<LinearLayout>(Resource.Id.LayoutRemoveTransaction);
				CheckBox chk = transactions.FindViewById<CheckBox>(Resource.Id.chkRemoveTransaction);
				chk.Checked = checkState;
			}
			(Globals.grdTransactions.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);
		}

		public void UpdateAdapter(List<Transaction> transactionList)
		{
			//refresh
			this.listTransactions = transactionList;
			MoneyGlobals.DisplayTransactions = transactionList;
			NotifyDataSetChanged();
		}

		public void EditState(bool variable = false)
		{
			(Globals.grdTransactions.Adapter as TransactionsAdapter).CheckAll();
			(Globals.grdTransactions.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);

			if (variable)
			{
				MoneyGlobals.TransactionsEditState = true;
				Globals.chkAllTransactions.Visibility = ViewStates.Visible;
				Globals.imgRemoveAllTransactions.Visibility = ViewStates.Visible;
			}
			else
			{
				MoneyGlobals.TransactionsEditState = false;
				Globals.chkAllTransactions.Visibility = ViewStates.Invisible;
				Globals.imgRemoveAllTransactions.Visibility = ViewStates.Invisible;
				Globals.chkAllTransactions.Checked = false;
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
			Transaction item = listTransactions[position];

			if (convertView == null)
				convertView = CurrentAdapterContext.LayoutInflater.Inflate(Globals.GridViewTransactionItem, null);

			Utils.CheckTransactionCategoryFlag(item);

			if (item.Category != null && item.HasCategory)
			{
				convertView.SetBackgroundColor(CTColor.GetColor(item.Category.Name).ToColor());
			}
			else if (item.Tag != null)
			{
				convertView.SetBackgroundColor(CTColor.GetColor(item.Tag.Name).ToColor());
			}

			convertView.FindViewById<TextView>(Id.txtGridTransactionItemID).Text = item.ID.ToString();
			convertView.FindViewById<TextView>(Id.txtGridTransactionItemSum).Text = item.Sum.ToString("#,##0.00");
			convertView.FindViewById<TextView>(Id.txtGridTransactionItemDate).Text = item.Date.ToShortDateString();
			convertView.FindViewById<TextView>(Id.txtGridTransactionItemNameOrTag).Text = item.Name;
			convertView.FindViewById<TextView>(Id.txtGridTransactionItemCategory).Text = item.Category == null ? "NO CATEGORY!" : item.Category.Name + (item.Tag != null ? " Tag:" + item.Tag.Name : "");

			//verify to have only one click event attached
			convertView.FindViewById<LinearLayout>(Id.LayoutClickableTransaction).LongClick -= LayoutClickableTransaction_LongClick;
			convertView.FindViewById<LinearLayout>(Id.LayoutClickableTransaction).LongClick += LayoutClickableTransaction_LongClick;

			var chkRemoveTransaction = convertView.FindViewById<CheckBox>(Id.chkRemoveTransaction);
			var imgCopyTransaction = convertView.FindViewById<ImageView>(Id.imgCopyTransaction);
			var imgModifyTransaction = convertView.FindViewById<ImageView>(Id.imgModifyTransaction);
			if (MoneyGlobals.TransactionsEditState)
			{
				chkRemoveTransaction.Visibility = ViewStates.Visible;
				imgCopyTransaction.Visibility = ViewStates.Invisible;
			}
			else if (!MoneyGlobals.TransactionsEditState)
			{
				chkRemoveTransaction.Visibility = ViewStates.Invisible;
				imgCopyTransaction.Visibility = ViewStates.Visible;
			}

			if (!chkRemoveTransaction.HasOnClickListeners)
			{
				chkRemoveTransaction.CheckedChange += chkRemoveTransaction_CheckChanged;
			}

			if (!imgModifyTransaction.HasOnClickListeners)
			{
				imgModifyTransaction.Click += imgModifyTransaction_Click;
			}

			if (!imgCopyTransaction.HasOnClickListeners)
			{
				imgCopyTransaction.Click += imgCopyTransaction_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listTransactions == null ? -1 : listTransactions.Count;
			}
		}

		public override Transaction this[int position]
		{
			get
			{
				return listTransactions == null ? null : listTransactions[position];
			}
		}

		#endregion

	}
}