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
using SkiaSharp.Views.Android;
using static Android.Widget.ExpandableListView;
using static ManyManager.Resource;

namespace ManyManager
{
	public class ExpandableDataAdapter : BaseExpandableListAdapter
	{
		readonly Activity Context;
		//List<Transaction> listTransactions;
		protected List<Month> MonthList { get; set; }
		protected List<MonthlyReport> MonthlyReports = new List<MonthlyReport>();
		public ExpandableDataAdapter(Activity newContext, List<Month> newList) : base()
		{
			Context = newContext;
			MonthList = newList;
			for (int i = 0; i < MonthList.Count; i++)
			{
				foreach (Transaction transaction in MonthList[i].Transactions)
				{
					if (transaction.HasCategory)
					{
						bool found = false;
						int j = 0;
						for (j = 0; j < MonthlyReports.Count; j++)
						{
							if (MonthlyReports[j].CategoryOrTag == transaction.Category.Name 
								&& MonthlyReports[j].Month == MonthList[i].month 
								&& MonthlyReports[j].Year == MonthList[i].year)
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							MonthlyReports.Add(new MonthlyReport(MonthList[i].month, MonthList[i].year, transaction.Category.Name, transaction.Sum, transaction));
						}
						else
						{
							MonthlyReports[j].Transactions.Add(transaction);
							MonthlyReports[j].Sum += transaction.Sum;
						}
					}
					if (transaction.Tag != null)
					{
						bool found = false;
						int j = 0;
						for (j = 0; j < MonthlyReports.Count; j++)
						{
							if (MonthlyReports[j].CategoryOrTag == transaction.Tag.Name
								&& MonthlyReports[j].Month == MonthList[i].month
								&& MonthlyReports[j].Year == MonthList[i].year)
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							MonthlyReports.Add(new MonthlyReport(MonthList[i].month, MonthList[i].year, transaction.Tag.Name, transaction.Sum, transaction));
						}
						else
						{
							MonthlyReports[j].Transactions.Add(transaction);
							MonthlyReports[j].Sum += transaction.Sum;
						}
					}
				}
			}
		}

		#region Events

		public void imgRemoveTransaction_Click(object sender, EventArgs e)
		{
			if (sender != null)
			{
				Globals.Activity.btnRemoveTransaction_Click(sender);
			}
		}

		public void imgModifyTransaction_Click(object sender, EventArgs e)
		{
			if (sender != null)
			{
				Globals.Activity.btnModifyTransaction_Load(sender);
			}
		}

		#endregion

		public ExpandableListView getExpandableListView()
		{
			ExpandableListView mExpandableListView = new ExpandableListView(Context);
			AbsListView.LayoutParams lp = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			mExpandableListView.LayoutParameters = lp;
			mExpandableListView.DividerHeight = 0;
			mExpandableListView.SetChildDivider(null);
			mExpandableListView.SetGroupIndicator(null);
			return mExpandableListView;
		}


		public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			View header = convertView;
			if (header == null)
			{
				header = Context.LayoutInflater.Inflate(Layout.ListGroupMonths, null);
			}

			header.FindViewById<TextView>(Id.DataHeader).Text = new DateTime(MonthList[groupPosition].year, MonthList[groupPosition].month, 1).ToString("MMMM yyyy");

			return header;
		}

		public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			Month month = MonthList[groupPosition];
			MonthlyReport group = MonthlyReports.FindAll(x => x.Month == month.month && x.Year == month.year)[childPosition];

			ExpandableListView childListView = getExpandableListView();
			MonthlyTransactionsReportAdapter adapter = new MonthlyTransactionsReportAdapter(Context, group.Transactions, group.CategoryOrTag + " : " + group.Sum.ToString());
			childListView.SetAdapter(adapter);
			childListView.SetOnGroupExpandListener(new MyOnGroupExpandListener(Context, group.Transactions.Count, childListView));
			childListView.SetOnGroupCollapseListener(new MyOnGroupCollapseListener(Context, group.Transactions.Count, childListView));

			return childListView;
		}

		public override int GetChildrenCount(int groupPosition)
		{
			return MonthlyReports.FindAll(x => x.Month == MonthList[groupPosition].month && x.Year == MonthList[groupPosition].year).Count;
		}

		public override int GroupCount
		{
			get
			{
				return MonthList.Count;
			}
		}

		#region implemented abstract members of BaseExpandableListAdapter

		public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
		{
			throw new NotImplementedException();
		}

		public override long GetChildId(int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override Java.Lang.Object GetGroup(int groupPosition)
		{
			throw new NotImplementedException();
		}

		public override long GetGroupId(int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable(int groupPosition, int childPosition)
		{
			return false;
		}

		public override bool HasStableIds
		{
			get
			{
				return true;
			}
		}

		#endregion

		public class MyOnGroupExpandListener : Java.Lang.Object, IOnGroupExpandListener
		{
			Context context;
			int count;
			ExpandableListView expandableListView;
			public MyOnGroupExpandListener(Context context, int count, ExpandableListView expandableListView)
			{
				this.context = context;
				this.count = count;
				this.expandableListView = expandableListView;
			}
			public void OnGroupExpand(int groupPosition)
			{
				AbsListView.LayoutParams lp = new AbsListView.LayoutParams(
				ViewGroup.LayoutParams.MatchParent,
				count * 88 + 95);
				expandableListView.LayoutParameters = lp;
			}
		}


		public class MyOnGroupCollapseListener : Java.Lang.Object, IOnGroupCollapseListener
		{
			Context context;
			int count;
			ExpandableListView expandableListView;
			public MyOnGroupCollapseListener(Context context, int count, ExpandableListView expandableListView)
			{
				this.context = context;
				this.count = count;
				this.expandableListView = expandableListView;
			}

			public void OnGroupCollapse(int groupPosition)
			{
				AbsListView.LayoutParams lp = new AbsListView.LayoutParams(
						ViewGroup.LayoutParams.MatchParent, 88);
				expandableListView.LayoutParameters = lp;
			}
		}
	}
}