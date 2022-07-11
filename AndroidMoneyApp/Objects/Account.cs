using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ManyManager
{
	public class Account
	{
		public int ID;
		public string Name;
		public string FilePath;

		public Account()
		{

		}

		public Account(int ID, string Name, string FilePath)
		{
			this.ID = 1;
			foreach (Account item in AccountsGlobals.AccountsList)
			{
				if (item.ID == this.ID)
				{
					this.ID++;
				}
			}
			if (ID != this.ID)
			{

			}
			this.Name = Name;
			this.FilePath = FilePath;
		}

		public Account(string Name, string FilePath)
		{
			ID = 1;
			foreach (Account item in AccountsGlobals.AccountsList)
			{
				if (item.ID == ID)
				{
					ID++;
				}
			}
			this.Name = Name;
			this.FilePath = FilePath;
		}
	}
}