using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp.Views.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ManyManager.Resource;

namespace ManyManager
{
    public class MonthlyTransactionsReportAdapter : BaseExpandableListAdapter
    {
        Activity CurrentAdapterContext;
        List<Transaction> listTransactions;
        private object key;


        public MonthlyTransactionsReportAdapter(Activity CurrentAdapterContext, List<Transaction> listTransactions, string key)
        {
            this.listTransactions = listTransactions;
            this.CurrentAdapterContext = CurrentAdapterContext;
            this.key = key;

        }
        public override int GroupCount
        {
            get
            {
                if (listTransactions.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return listTransactions.Count;
                }
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)CurrentAdapterContext.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Globals.GridViewTransactionItem, null);
            }
            Transaction item = listTransactions[childPosition];

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

            var chkRemoveTransaction = convertView.FindViewById<CheckBox>(Id.chkRemoveTransaction);
            var imgCopyTransaction = convertView.FindViewById<ImageView>(Id.imgCopyTransaction);
            var imgModifyTransaction = convertView.FindViewById<ImageView>(Id.imgModifyTransaction);
            //if (MoneyGlobals.TransactionsEditState)
            //{
            //    chkRemoveTransaction.Visibility = ViewStates.Visible;
            //    imgRemoveTransaction.Visibility = ViewStates.Invisible;
            //}
            //else if (!MoneyGlobals.TransactionsEditState)
            //{
                //chkRemoveTransaction.Visibility = ViewStates.Invisible;
                //imgRemoveTransaction.Visibility = ViewStates.Visible;
            //}
            chkRemoveTransaction.Visibility = ViewStates.Invisible;
            imgCopyTransaction.Visibility = ViewStates.Invisible;
            //todo attach transaction modify and delete events
            //if (!chkRemoveTransaction.HasOnClickListeners)
            //{
            //    chkRemoveTransaction.CheckedChange += TransactionsAdapter.chkRemoveTransaction_CheckChanged;
            //}

            //if (!imgModifyTransaction.HasOnClickListeners)
            //{
            //    imgModifyTransaction.Click += imgModifyTransaction_Click;
            //}

            //if (!imgRemoveTransaction.HasOnClickListeners)
            //{
            //    imgRemoveTransaction.Click += imgRemoveTransaction_Click;
            //}

            return convertView;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)CurrentAdapterContext.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Layout.ListChildMonth, null);
            }

            string textGroup = (string)GetGroup(groupPosition);
            TextView textViewGroup = convertView.FindViewById<TextView>(Id.DataHeader);
            textViewGroup.Text = textGroup;
            return convertView;
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return key.ToString();
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return listTransactions.Count;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}