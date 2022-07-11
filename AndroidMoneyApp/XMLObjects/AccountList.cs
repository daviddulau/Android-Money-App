using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ManyManager
{
	[XmlRoot("Accounts")]
	public class AccountList
	{
		[XmlArray("AccountListing")]
		[XmlArrayItem("Account", typeof(Account))]
		public List<Account> Account = new List<Account>();
	}
}