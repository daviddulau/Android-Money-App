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
	public static class AccountsGlobals
	{

		#region Members

		private static AccountList accountList = new AccountList();

		#endregion

		#region Public Properties

		public static List<Account> AccountsList
		{
			get
			{
				return accountList.Account;
			}
			set
			{
				accountList.Account = value;
			}
		}
		public static Account SelectedAccount;
		public static bool NewAccountWasAdded = false;

		#endregion

		#region Public Methods

		public static int AccountIndexInList(List<Account> list, Account obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (AccountMatch(list[i], obj))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool AccountMatch(Account a1, Account a2)
		{
			if (a1.Name != a2.Name)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

	}
}