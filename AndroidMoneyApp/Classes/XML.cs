using Java.IO;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ManyManager
{
    public static class XML
    {
        #region Public Properties

        public static XDocument XUserDocument;

        #endregion

        #region Accounts

        public static void LoadAccountsData()
		{
            Globals.CreateLoadDataFile("Accounts");

            LoadAccountsDataFromXML();

            DeserializeAccounts();
        }

        private static void LoadAccountsDataFromXML()
        {
            XUserDocument = new XDocument();
            if (!System.IO.File.Exists(Globals.AccountsDataFilePath))
            {
                FileStream stream = System.IO.File.Create(Globals.AccountsDataFilePath);
                stream.Close();

                XUserDocument = new XDocument(
                    new XElement("Accounts")
                );
                XUserDocument.Save(Globals.AccountsDataFilePath);
            }
            string data = System.IO.File.ReadAllText(Globals.AccountsDataFilePath);
            if (string.IsNullOrWhiteSpace(data))
            {
                XUserDocument = new XDocument(
                    new XElement("Accounts")
                );
                XUserDocument.Save(Globals.AccountsDataFilePath);
            }
            XUserDocument = XDocument.Load(Globals.AccountsDataFilePath);
            if (XUserDocument.Root == null)
            {
                XUserDocument = new XDocument(
                    new XElement("Accounts")
                );
            }
            XUserDocument.Save(Globals.AccountsDataFilePath);
        }

        #region Account Part

        public static void DeserializeAccounts()
        {
            //Read
            XmlSerializer ser = new XmlSerializer(typeof(AccountList));
            TextReader reader = new StreamReader(Globals.AccountsDataFilePath);
            object tempAccounts = ser.Deserialize(reader);
            reader.Close();
            //foreach (Account item in tempAccounts.Account)
            {
                //AccountsGlobals.AccountsList.Add(item);
            }
        }

        public static void SerializeAccounts()
        {
            //Write
            XmlSerializer ser = new XmlSerializer(typeof(AccountList));
            TextWriter reader = new StreamWriter(Globals.AccountsDataFilePath);
            ser.Serialize(reader, AccountsGlobals.AccountsList);
            reader.Close();
        }

        [Obsolete]
		public static void LoadAccounts()
		{
            XUserDocument = XDocument.Load(Globals.AccountsDataFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                AccountsGlobals.AccountsList = new List<Account>();
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        //one tag at a time
                        int ID = Convert.ToInt32(item.Element("ID").Value);
                        string name = item.Element("Name").Value;
                        string filePath = item.Element("FilePath").Value;
                        Account account = new Account(ID, name, filePath);

                        AccountsGlobals.AccountsList.Add(account);
                    }
                }
                XUserDocument.Save(Globals.AccountsDataFilePath);
            }
        }

        public static void RefreshAccounts()
		{
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                XElement Accounts = XUserDocument.Root;
                Accounts.RemoveAll();
                foreach (Account account in AccountsGlobals.AccountsList)
                {
                    Accounts.Add(new XElement("Account", new XElement("ID", account.ID), new XElement("Name", account.Name.ToUpper()), new XElement("FilePath", account.FilePath)));
                }
                XUserDocument.Save(Globals.AccountsDataFilePath);
            }
        }

		public static void RemoveFolder(Account account)
		{
            DirectoryInfo dir = Directory.GetParent(account.FilePath);
            Directory.Delete(dir.FullName, true);
		}

		public static void RefreshAccount(Account account, string NewName)
        {
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                //work on accounts file
                XElement Account = GetAccount(account);
                string newFilePath = string.Empty;
                XElement node = Account.Element("Name");

                if (node != null)
                {
					if (node.Value != NewName)
					{
                        //rename the user file
                        newFilePath = RenameUserFile(NewName, node.Value);
                    }
                    //rename in object
                    account.Name = NewName;
                    node.Value = NewName;
                }

                node = Account.Element("FilePath");
                if (node != null && !string.IsNullOrWhiteSpace(newFilePath))
				{
                    node.Value = newFilePath;
                    account.FilePath = newFilePath;
                }

                XUserDocument.Save(Globals.AccountsDataFilePath);
            }
        }

        private static XElement GetAccount(Account account)
        {
            XUserDocument = XDocument.Load(Globals.AccountsDataFilePath);
            if (XUserDocument != null && XUserDocument.Root != null && XUserDocument.Root.HasElements)
            {
                try
                {
                    foreach (XElement item in XUserDocument.Root.Elements())
                    {
                        if (item.HasElements)
                        {
                            if (item.Element("Name").Value == account.Name)
                            {
                                return item;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        public static string RenameUserFile(string newName, string oldName)
		{
            string folderOldPath = Path.Combine(Globals.InternalPath, oldName);
            string folderNewPath = Path.Combine(Globals.InternalPath, newName);
            if (!Directory.Exists(folderOldPath))
            {
                Directory.CreateDirectory(folderNewPath);
            }
			else if (Directory.Exists(folderOldPath))
			{
                Directory.Move(folderOldPath, folderNewPath);
			}

            string[] filesList = Directory.GetFiles(folderNewPath);
            foreach (string file in filesList)
            {
				if (file == Path.Combine(folderNewPath, oldName + ".xml"))
				{
                    string newPath = Path.Combine(folderNewPath, newName + ".xml");
                    System.IO.File.Move(file, newPath);
                    XDocument doc = XDocument.Load(newPath);
					if (doc != null && doc.Root != null && doc.Root.Element("Name") != null )
					{
                        doc.Root.Element("Name").Value = newName;
                    }
                    doc.Save(newPath);
                    return newPath;
                }
            }
            return "";
        }

        #endregion

        #endregion

        #region UserData

        public static void LoadDataFromXML(string path, string element)
        {
            XUserDocument = new XDocument();
            if (!System.IO.File.Exists(path))
            {
                FileStream stream = System.IO.File.Create(path);
                stream.Close();

                XUserDocument = new XDocument(
                    new XElement(element)
                );
                XUserDocument.Save(path);
            }
            string data = System.IO.File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(data))
            {
                XUserDocument = new XDocument(
                    new XElement(element)
                );
                XUserDocument.Save(path);
            }
            XUserDocument = XDocument.Load(path);
            if (XUserDocument.Root == null)
            {
                XUserDocument = new XDocument(
                    new XElement(element)
                );
            }
            XUserDocument.Save(path);
        }

        public static void LoadUserData()
        {
            //LoadCreateXMLs();
            //XUserDocument = XDocument.Load(Globals.UserDataFilePath);

            //Tags
            LoadTagsData();

            //Categories
            LoadCategoriesData();

            //Money
            LoadYears(true);
            //LoadMonth(DateTime.Now);
            MoneyGlobals.ReLoadTagAndCategoryColors();
            //Save User Settings - Internal or External Storage
            //SaveUserStoragePreference();

            Globals.ProcessRepetitives();

            //Filters
            LoadFiltersData();

            //Notes
            LoadNotesData();

            //Cars
            LoadCarsData();

            //Assign Car & Notes to Tags
            AssignTags();

            //UnitType
            LoadUnitType();
            UnitType.ChangeUnitType();
        }

		public static void SaveUserStoragePreference(bool locationChanged = false)
        {
            XDocument.Load(Globals.UserDataFilePath);
            XElement internalStorage = XUserDocument.Root.Element("InternalStorage");
            internalStorage.Value = Globals.internalStorage.ToString();
            XUserDocument.Save(Globals.UserDataFilePath);
            if (locationChanged)
            {
                ChangeUserStorageLocation();
            }
        }

		private static void ChangeUserStorageLocation()
        {
            string oldFilePath = Globals.UserDataFilePath;
            string data = System.IO.File.ReadAllText(oldFilePath);

            Java.IO.File[] sdCard = Globals.Activity.GetExternalFilesDirs(null);
            Java.IO.File dir;
            if (!Globals.internalStorage)
            {
                dir = new Java.IO.File(sdCard[0].AbsolutePath);
                string path = Path.Combine(sdCard[0].AbsolutePath, Globals.UserName + ".xml");
                Globals.UserDataFile = new Java.IO.File(path);
                Globals.UserDataFilePath = path;
                if (!Globals.UserDataFile.Exists())
                {
                    Globals.UserDataFile.CreateNewFile();
                }
                try
                {
                    System.IO.File.WriteAllText(Globals.UserDataFilePath, data);
                    System.IO.File.Delete(oldFilePath);
                }
                catch
                {
                    Utils.ShowAlert("Warning", "Location Change Error");
                    bool temp = Globals.internalStorage;
                    Globals.internalStorage = !temp;
                }
                LoadUserData();
            }
            else
            {
                dir = new Java.IO.File(sdCard[1].AbsolutePath);
                string path = Path.Combine(sdCard[1].AbsolutePath, Globals.UserName + ".xml");
                Globals.UserDataFile = new Java.IO.File(path);
                Globals.UserDataFilePath = path;
                if (!Globals.UserDataFile.Exists())
                {
                    Globals.UserDataFile.CreateNewFile();
                }
                try
                {
                    System.IO.File.WriteAllText(Globals.UserDataFilePath, data);
                    System.IO.File.Delete(oldFilePath);
                }
                catch (Exception ex)
                {
                    Utils.ShowAlert("Warning", "Location Change Error");
                    bool temp = Globals.internalStorage;
                    Globals.internalStorage = !temp;
                }
                LoadUserData();
            }
        }

		private static void LoadCreateXMLs()
		{
            XUserDocument = new XDocument();
            //if (!System.IO.File.Exists(Globals.UserDataFilePath))
            //{
            //    FileStream stream = System.IO.File.Create(Globals.UserDataFilePath);
            //    stream.Close();

            //    XUserDocument = new XDocument(
            //        new XElement("Account", new XElement("Name", Globals.UserName),
            //            new XElement("Years", new XElement("Year", new XElement("Number", DateTime.Now.Year), new XElement("Months"))),
            //            new XElement("InternalStorage", Globals.internalStorage),
            //            new XElement("Notes"), new XElement("Cars"), new XElement("Filters"), new XElement("Tags"), new XElement("Categories"), new XElement("UnitType")
            //        )
            //    );
            //    XUserDocument.Save(Globals.UserDataFilePath);
            //}
            //todo XML check schema
            //string data = System.IO.File.ReadAllText(Globals.UserDataFilePath);
            //if (string.IsNullOrWhiteSpace(data))
            //{
            //    XUserDocument = new XDocument(
            //        new XElement("Account", new XElement("Name", Globals.UserName),
            //            new XElement("Years", new XElement("Year", new XElement("Number", DateTime.Now.Year), new XElement("Months"))),
            //            new XElement("InternalStorage", Globals.internalStorage),
            //            new XElement("Notes"), new XElement("Cars"), new XElement("Filters"), new XElement("Tags"), new XElement("Categories"), new XElement("UnitType")
            //        )
            //    );
            //    XUserDocument.Save(Globals.UserDataFilePath);
            //}
            //load each file and check schema
            XUserDocument = XDocument.Load(Globals.YearFilePath);
            if (XUserDocument.Root == null)
			{
                XUserDocument = new XDocument(new XElement("Year", new XElement("Number", DateTime.Now.Year), new XElement("Months")));
                XUserDocument.Save(Globals.YearFilePath);
            }
            //if (XUserDocument.Root.Element("InternalStorage") == null)
            //{
            //	XUserDocument.Root.Add(new XElement("InternalStorage", Globals.internalStorage));
            //}
            XUserDocument = XDocument.Load(Globals.NotesFilePath);
            if (XUserDocument.Root == null)
			{
				XUserDocument = new XDocument(new XElement("Notes"));
                XUserDocument.Save(Globals.NotesFilePath);
            }
            XUserDocument = XDocument.Load(Globals.CarsFilePath);
            if (XUserDocument.Root == null)
			{
                XUserDocument = new XDocument(new XElement("Cars"));
                XUserDocument.Save(Globals.CarsFilePath);
            }
            XUserDocument = XDocument.Load(Globals.FiltersFilePath);
            if (XUserDocument.Root == null)
			{
                XUserDocument = new XDocument(new XElement("Filters"));
                XUserDocument.Save(Globals.FiltersFilePath);
            }
            XUserDocument = XDocument.Load(Globals.TagsFilePath);
            if (XUserDocument.Root == null)
			{
                XUserDocument = new XDocument(new XElement("Tags"));
                XUserDocument.Save(Globals.TagsFilePath);
            }
            XUserDocument = XDocument.Load(Globals.CategoriesFilePath);
            if (XUserDocument.Root == null)
            {
                XUserDocument = new XDocument(new XElement("Categories"));
                XUserDocument.Save(Globals.CategoriesFilePath);
            }
            if (XUserDocument.Root.Element("UnitType") == null)
			{
                //todo move UnitType element in Account file
                XUserDocument = new XDocument(new XElement("UnitType"));
                //XUserDocument.Save(Globals.CategoriesFilePath);
            }
			//XUserDocument.Save(Globals.UserDataFilePath);
		}

		#region Money Part

		private static XElement GetYear(DateTime Month)
        {
            XUserDocument = XDocument.Load(Globals.YearFilePath);
            if (XUserDocument.Root.HasElements)
            {
                try
                {
                    foreach (XElement item in XUserDocument.Root.Elements())
                    {
                        if (item.HasElements)
                        {
                            if (item.Element("Number").Value == Month.Year.ToString())
                            {
                                return item;
                            }
                        }
                    }
                    LoadYear(Month);
                    return GetYear(Month);
                }
                catch
                {
                }
            }
            else if (!XUserDocument.Root.HasElements)
            {
                XUserDocument.Root.Add(new XElement("Year", new XElement("Number", Month.Year), new XElement("Months")));
                XUserDocument.Save(Globals.YearFilePath);
                return GetYear(Month);
            }
            return null;
        }

        public static void LoadYears(bool loadInList = false)
        {
            //todo rewrite to read all files in a Years document where to store a full year data with months
            XUserDocument = XDocument.Load(Globals.YearFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
				if (loadInList)
				{
                    MoneyGlobals.TransactionsList = new List<Transaction>();
                }
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
						if (item.Element("Number") != null)
						{
							try
							{
                                int year = Convert.ToInt32(item.Element("Number").Value);
                                LoadYear(new DateTime(year, 1, 1), loadInList);
                            }
							catch
							{
							}
						}
                    }
                }
            }
        }

        public static void LoadYear(DateTime Year, bool loadInList = false)
        {
            XUserDocument = XDocument.Load(Globals.YearFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                //search if year exists else generate it
                bool exists = false;
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        if (item.Element("Number").Value == Year.Year.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                if (!exists)
                {
                    XUserDocument.Root.Add(new XElement("Year", new XElement("Number", Year.Year), new XElement("Months")));
                    XUserDocument.Save(Globals.YearFilePath);
                }
                //load entire year
                for (int i = 1; i < 13; i++)
                {
                    DateTime datetime = new DateTime(Year.Year, i, 1);
                    LoadMonth(datetime, loadInList);
                }
                XUserDocument.Save(Globals.YearFilePath);
            }
        }

        public static void SaveYears()
        {
            XUserDocument = XDocument.Load(Globals.YearFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        if (item.Element("Number") != null)
                        {
							try
							{
								int year = Convert.ToInt32(item.Element("Number").Value);
								SaveYear(new DateTime(year, 1, 1));
							}
                            catch { }
                        }
                    }
                }
                LoadYears(true);
            }
        }

        public static void SaveYear(DateTime Year)
        {
            XUserDocument = XDocument.Load(Globals.YearFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                //search if year exists else generate it
                bool exists = false;
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        if (item.Element("Number").Value == Year.Year.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                if (!exists)
                {
                    XUserDocument.Root.Add(new XElement("Year", new XElement("Number", Year.Year), new XElement("Months")));
                    XUserDocument.Save(Globals.YearFilePath);
                }
                //save entire year
                for (int i = 1; i < 13; i++)
                {
                    DateTime datetime = new DateTime(Year.Year, i, 1);
                    SaveMonthTransactions(datetime);
                }
                XUserDocument.Save(Globals.YearFilePath);
            }
        }

        public static void SaveMonthTransactions(DateTime date)
		{
            Month month = Globals.GetMonthFromDate(date);
            Globals.RefreshMonth(month);
            XElement monthElement = GetMonth(date);
			if (monthElement == null)
			{
                monthElement = new XElement("Month", new XElement("Transactions"), new XElement("Profit", 0),
                    new XElement("Income", 0), new XElement("Expense", 0),
                    new XElement("Charity", 0), new XElement("CharityPercentage", 10),
                    new XElement("Ratio", 1), new XElement("Month", date.Month));
            }
            XElement insert = null;
            string color = "";

            if (monthElement.Element("Transactions") is XElement node1)
            {
                node1.RemoveAll();

                List<Transaction> temp = MoneyGlobals.TransactionsList.FindAll(x=>x.Date.Month == date.Month);
                if (temp.Count > 0)
                {
                    foreach (Transaction item in temp)
                    {
						try
						{
                            color = item.Color.ToString() != SKColor.Empty.ToString() ? item.Color.ToString() : "";
                            insert = new XElement("Transaction", new XElement("ID", item.ID.ToString()), new XElement("Sum", item.Sum.ToString("#,##0.00"))
                                , new XElement("Date", Globals.SetXMLDate(item.Date)), new XElement("Name", item.Name)
                                , new XElement("TagName", item.Tag == null ? "" : item.Tag.Name), new XElement("CategoryName", item.Category == null ? "" : item.Category.Name), new XElement("Color", color)
                                , new XElement("HasCategory", item.HasCategory), new XElement("HasConsumption", item.HasCarConsumption), new XElement("Kilometers", item.Kilometers)
                                , new XElement("Liters", item.Liters), new XElement("Price", item.Price), new XElement("Unit", (int)UnitType.Unit), new XElement("Interval", item.RepetitiveInterval));
                            node1.Add(insert);
						}
						catch {	}
                    }
                    month.Transactions = temp;
                    Globals.RefreshMonth(month);
                }
            }
            if (monthElement.Element("Profit") is XElement node2)
            {
                node2.Value = month.Profit.ToString("#,##0.00");
            }
            if (monthElement.Element("Income") is XElement node3)
            {
                node3.Value = month.Income.ToString("#,##0.00");
            }
            if (monthElement.Element("Expense") is XElement node4)
            {
                node4.Value = month.Expense.ToString("#,##0.00");
            }
            if (monthElement.Element("Charity") is XElement node5)
            {
                node5.Value = month.Charity.ToString("#,##0.00");
            }
            if (monthElement.Element("CharityPercentage") is XElement node6)
            {
                node6.Value = month.CharityPercentage.ToString("0.00");
            }
            if (monthElement.Element("Ratio") is XElement node7)
            {
                node7.Value = month.Ratio.ToString("#,##0.0000");
            }
            if (monthElement.Element("Month") is XElement node8)
            {
                node8.Value = month.month.ToString();
            }
            XUserDocument.Save(Globals.YearFilePath);
        }

        public static void SaveMonth(Month month)
        {
            DateTime date = new DateTime(month.year, month.month, 1);
            XElement monthElement = GetMonth(date);
            XElement insert = null;
            string color = "";
            foreach (XElement node in monthElement.Elements())
            {
                if (node.Name.LocalName == "Transactions")
                {
                    node.RemoveAll();
                    if (month.Transactions.Count > 0)
                    {
                        foreach (Transaction item in month.Transactions)
                        {
                            color = item.Color.ToString() != SKColor.Empty.ToString() ? item.Color.ToString() : "";

                            insert = new XElement("Transaction", new XElement("ID", item.ID.ToString()), new XElement("Sum", item.Sum.ToString("#,##0.00"))
                                , new XElement("Date", Globals.SetXMLDate(item.Date)), new XElement("Name", item.Name)
                                , new XElement("TagName", item.Tag == null ? "" : item.Tag.Name), new XElement("CategoryName", item.Category == null ? "" : item.Category.Name), new XElement("Color", color)
                                , new XElement("HasCategory", item.HasCategory), new XElement("HasConsumption", item.HasCarConsumption), new XElement("Kilometers", item.Kilometers)
                                , new XElement("Liters", item.Liters), new XElement("Price", item.Price), new XElement("Unit", (int)UnitType.Unit), new XElement("Interval", item.RepetitiveInterval));
                            node.Add(insert);
                        }
                    }
                }
                else if (node.Name.LocalName == "Profit")
                {
                    node.Value = month.Profit.ToString("#,##0.00");
                }
                else if (node.Name.LocalName == "Income")
                {
                    node.Value = month.Income.ToString("#,##0.00");
                }
                else if (node.Name.LocalName == "Expense")
                {
                    node.Value = month.Expense.ToString("#,##0.00");
                }
                else if (node.Name.LocalName == "Charity")
                {
                    node.Value = month.Charity.ToString("#,##0.00");
                }
                else if (node.Name.LocalName == "CharityPercentage")
                {
                    node.Value = month.CharityPercentage.ToString("0.00");
                }
                else if (node.Name.LocalName == "Ratio")
                {
                    node.Value = month.Ratio.ToString("#,##0.0000");
                }
                else if (node.Name.LocalName == "Month")
                {
                    node.Value = month.month.ToString();
                }
            }
            XUserDocument.Save(Globals.YearFilePath);
        }

        private static void LoadMonth(DateTime MonthDate, bool LoadInList = false)
        {
            XElement element = GetMonth(MonthDate);
            if (element != null)
            {
                if (element.HasElements)
                {
                    Month month = new Month();
                    foreach (XElement node in element.Elements())
                    {
                        if (node.Name.LocalName == "Transactions" && node.HasElements)
                        {
                            month.Transactions = new List<Transaction>();
                            foreach (XElement transaction in node.Elements())
                            {
                                Transaction newTransaction = null;
                                if (transaction.Element("Color").Value != "")
								{
                                    newTransaction = new Transaction(Convert.ToInt32(transaction.Element("ID").Value), Utils.ConvertStringToDouble(transaction.Element("Sum").Value),
                                        Globals.GetXMLDate(transaction.Element("Date").Value), transaction.Element("Name").Value,
                                        TagsGlobals.SearchOrAddNewTag(transaction.Element("TagName").Value, false), CategoriesGlobals.SearchOrAddNewCategory(transaction.Element("CategoryName").Value, false), SKColor.Parse(transaction.Element("Color").Value),
                                        Convert.ToBoolean(transaction.Element("HasCategory").Value), Convert.ToBoolean(transaction.Element("HasConsumption").Value), RepetitiveInterval: transaction.Element("Interval").Value);
                                }
								else
								{
                                    newTransaction = new Transaction(Convert.ToInt32(transaction.Element("ID").Value), Utils.ConvertStringToDouble(transaction.Element("Sum").Value),
                                        Globals.GetXMLDate(transaction.Element("Date").Value), transaction.Element("Name").Value,
                                        TagsGlobals.SearchOrAddNewTag(transaction.Element("TagName").Value, false), CategoriesGlobals.SearchOrAddNewCategory(transaction.Element("CategoryName").Value, false),
                                        Utils.CategoryOrTagToSKColor(), Convert.ToBoolean(transaction.Element("HasCategory").Value), Convert.ToBoolean(transaction.Element("HasConsumption").Value), RepetitiveInterval: transaction.Element("Interval").Value);
                                }
								if (newTransaction.HasCarConsumption)
								{
                                    newTransaction.Kilometers = Utils.ConvertStringToDouble(transaction.Element("Kilometers").Value);
                                    newTransaction.Liters = Utils.ConvertStringToDouble(transaction.Element("Liters").Value);
                                    newTransaction.Price = Utils.ConvertStringToDouble(transaction.Element("Price").Value);
                                    newTransaction.Unit = (Unit)Utils.ConvertStringToInt(transaction.Element("Unit").Value);
                                }
                                month.Transactions.Add(newTransaction);
                            }
							if (LoadInList)
							{
                                MoneyGlobals.TransactionsList.AddRange(month.Transactions);
							}
                        }
                        else if (node.Name.LocalName == "Profit")
                        {
                            month.Profit = Utils.ConvertStringToDouble(node.Value);
                        }
                        else if (node.Name.LocalName == "Income")
                        {
                            month.Income = Utils.ConvertStringToDouble(node.Value);
                        }
                        else if (node.Name.LocalName == "Expense")
                        {
                            month.Expense = Utils.ConvertStringToDouble(node.Value);
                        }
                        else if (node.Name.LocalName == "Charity")
                        {
                            month.Charity = Utils.ConvertStringToDouble(node.Value);
                        }
                        else if (node.Name.LocalName == "CharityPercentage")
                        {
                            month.CharityPercentage = Utils.ConvertStringToDouble(node.Value);
                        }
                        else if (node.Name.LocalName == "Ratio")
                        {
                            month.Ratio = Utils.ConvertStringToDouble(node.Value);
                        }
                        else if (node.Name.LocalName == "Month")
                        {
                            month.month = Convert.ToInt32(node.Value);
                        }
                    }
                    month.year = Convert.ToInt32(element.Parent.Parent.Element("Number").Value);
                    Globals.RemoveMonth(MonthDate);
                    Globals.MonthsList.Add(month);
                }
                else if (!element.HasElements)
                {
                    element.Add(new XElement("Transactions"), new XElement("Profit", 0),
                    new XElement("Income", 0), new XElement("Expense", 0),
                    new XElement("Charity", 0), new XElement("CharityPercentage", 10),
                    new XElement("Ratio", 1), new XElement("Month", MonthDate.Month));
                }
            }
            else
            {
                XUserDocument = XDocument.Load(Globals.YearFilePath);
                //search year in years
                XElement xYear = GetYear(MonthDate);
                if (xYear != null)
                {
                    //check for months & month
                    if (xYear.HasElements && !xYear.Element("Months").HasElements)
                    {
                        if (XUserDocument.Root.Element("Years").Elements("Year").First(x => x.Element("Number").Value == xYear.Element("Number").Value) != null)
                        {
                            XUserDocument.Root.Element("Years").Elements("Year").First(x => x.Element("Number").Value == xYear.Element("Number").Value).Element("Months").Add(new XElement("Month",
                            new XElement("Transactions"), new XElement("Profit", 0),
                            new XElement("Income", 0), new XElement("Expense", 0),
                            new XElement("Charity", 0), new XElement("CharityPercentage", 10),
                            new XElement("Ratio", 1), new XElement("Month", MonthDate.Month)));
                        }
                    }
                    else if (xYear.Element("Months").HasElements)
                    {
                        XElement xMonth = GetMonth(MonthDate);
                        if (xMonth == null)
                        {
                            if (XUserDocument.Root.Element("Years").Elements("Year").First(x => x.Element("Number").Value == xYear.Element("Number").Value) != null)
                            {
                                XUserDocument.Root.Element("Years").Elements("Year").First(x => x.Element("Number").Value == xYear.Element("Number").Value).Element("Months").Add(new XElement("Month",
                                new XElement("Transactions"), new XElement("Profit", 0),
                                new XElement("Income", 0), new XElement("Expense", 0),
                                new XElement("Charity", 0), new XElement("CharityPercentage", 10),
                                new XElement("Ratio", 1), new XElement("Month", MonthDate.Month)));
                            }
                        }
                        else if (!xMonth.HasElements)
                        {
                            xMonth.Add(new XElement("Transactions"), new XElement("Profit", 0),
                            new XElement("Income", 0), new XElement("Expense", 0),
                            new XElement("Charity", 0), new XElement("CharityPercentage", 10),
                            new XElement("Ratio", 1), new XElement("Month", MonthDate.Month));
                        }
                    }
                }
                else
                {
                    //else create all
                    XUserDocument.Root.Add(new XElement("Year", new XElement("Number", MonthDate.Year), new XElement("Months", new XElement("Month",
                    new XElement("Transactions"), new XElement("Profit", 0),
                    new XElement("Income", 0), new XElement("Expense", 0),
                    new XElement("Charity", 0), new XElement("CharityPercentage", 10),
                    new XElement("Ratio", 1), new XElement("Month", MonthDate.Month)))));
                }
                XUserDocument.Save(Globals.YearFilePath);
            }
        }

        private static XElement GetMonth(DateTime Month)
        {
            //todo get correct year file from Month variable
            XUserDocument = XDocument.Load(Globals.YearFilePath);
            XElement year = GetYear(Month);
            if (year != null && year.Element("Months").HasElements)
            {
                try
                {
                    foreach (XElement item in year.Element("Months").Elements())
                    {
                        if (item.HasElements)
                        {
                            if (item.Element("Month").Name.LocalName == "Month" && item.Element("Month").Value == Month.Month.ToString())
                            {
                                return item;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        #endregion

        #region Filter Part

        public static void LoadFiltersData()
        {
            Globals.CreateLoadDataFile("Filters");

            LoadDataFromXML(Globals.FiltersFilePath, "Filters");
            XUserDocument = XDocument.Load(Globals.FiltersFilePath);

            DeserializeFilters();
        }

        public static void DeserializeFilters()
        {
            //Read
            XmlSerializer ser = new XmlSerializer(typeof(List<Filter>));
            TextReader reader = new StreamReader(Globals.FiltersFilePath);
            List<Filter> tempFilters = ser.Deserialize(reader) as List<Filter>;
            reader.Close();
            foreach (Filter item in tempFilters)
            {
                Filtering.FilterList.Add(item);
            }
        }

        public static void SerializeFilters()
        {
            //Write
            XmlSerializer ser = new XmlSerializer(typeof(List<Filter>));
            TextWriter reader = new StreamWriter(Globals.FiltersFilePath);
            ser.Serialize(reader, Filtering.FilterList);
            reader.Close();
        }

        public static void SaveFilters()
        {
            XUserDocument = XDocument.Load(Globals.FiltersFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                XElement Filters = XUserDocument.Root;
                Filters.RemoveAll();
                foreach (Filter filter in Filtering.FilterList)
                {
                    Filters.Add(new XElement("Filter", new XElement("ID", filter.ID), new XElement("Name", filter.Name), new XElement("BeginDate", Globals.SetXMLDate(filter.BeginDate)),
                        new XElement("EndDate", Globals.SetXMLDate(filter.EndDate)), new XElement("ShowTransactions", (int)filter.TransactionDisplay), new XElement("ShowAllCategories", filter.ShowAllCategoriesOrTags),
                        new XElement("MinSum", filter.MinSum), new XElement("MaxSum", filter.MaxSum), new XElement("CategoryOrTagFilter", filter.CategoryOrTagFilter)));
                }
                XUserDocument.Save(Globals.FiltersFilePath);
            }
        }

        [Obsolete]
        public static void LoadFilters()
        {
            XUserDocument = XDocument.Load(Globals.FiltersFilePath);
            if (XUserDocument != null && XUserDocument.Root != null && XUserDocument.Root.Element("Filters") != null)
            {
                Filtering.FilterList = new List<Filter>();
                //load them
                foreach (XElement item in XUserDocument.Root.Element("Filters").Elements())
                {
                    if (item.HasElements)
                    {
                        //one filter
                        int ID = Convert.ToInt32(item.Element("ID").Value);
                        string Name = item.Element("Name").Value;
                        DateTime BeginDate = Globals.GetXMLDate(item.Element("BeginDate").Value);
                        DateTime EndDate = Globals.GetXMLDate(item.Element("EndDate").Value);
                        TransactionDisplay TransactionDisplay = (TransactionDisplay)Convert.ToInt32(item.Element("ShowTransactions").Value);
                        bool ShowAllCategories = Convert.ToBoolean(item.Element("ShowAllCategories").Value);
                        string MinSum = item.Element("MinSum").Value;
                        string MaxSum = item.Element("MaxSum").Value;
                        string CategoryOrTagFilter = item.Element("CategoryOrTagFilter").Value;
                        Filtering.FilterList.Add(new Filter(Name, ID, BeginDate, EndDate, TransactionDisplay, ShowAllCategories, MinSum, MaxSum, CategoryOrTagFilter));
                    }
                }
                XUserDocument.Save(Globals.FiltersFilePath);
            }
        }

        #endregion

        #region Notes Part

        public static void LoadNotesData()
        {
            Globals.CreateLoadDataFile("Notes");

            LoadDataFromXML(Globals.NotesFilePath, "Notes");
            XUserDocument = XDocument.Load(Globals.NotesFilePath);

            DeserializeNotes();
        }

        public static void DeserializeNotes(bool AddTag = true, bool AddCategory = true)
        {
            //Read
            XmlSerializer ser = new XmlSerializer(typeof(List<Note>));
            TextReader reader = new StreamReader(Globals.NotesFilePath);
            List<Note> tempNotes = ser.Deserialize(reader) as List<Note>;
            reader.Close();
            foreach (Note item in tempNotes)
            {
                NotesGlobals.NotesList.Add(item);
                LoadMiniNote(item, AddTag, AddCategory);
            }
        }

        public static void SerializeNotes()
        {
            //Write
            XmlSerializer ser = new XmlSerializer(typeof(List<Note>));
            TextWriter reader = new StreamWriter(Globals.NotesFilePath);
            ser.Serialize(reader, NotesGlobals.NotesList);
            reader.Close();
        }

        /// <summary>
        /// Used To Reload cars after some tag was deleted to don't add that tag again from XML
        /// </summary>
        /// <param name="AddTag"></param>
        public static void ReLoadNotes(bool AddTag = false, bool AddCategory = false)
        {
            DeserializeNotes(AddTag, AddCategory);
            RefreshNotes();
        }

        [Obsolete]
        public static void LoadNotes(bool AddTag = true, bool AddCategory = true)
        {
            XUserDocument = XDocument.Load(Globals.NotesFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                NotesGlobals.NotesList = new List<Note>();
                //load them
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        //one note
                        int ID = Convert.ToInt32(item.Element("ID").Value);
                        DateTime createdDate = Globals.GetXMLDate(item.Element("CreatedDate").Value);
                        DateTime modifiedDate = Globals.GetXMLDate(item.Element("LastModifiedDate").Value);
                        string title = item.Element("Title").Value;
                        string shortDescription = item.Element("ShortDescription").Value;
                        NotesGlobals.NoteType NoteType = (NotesGlobals.NoteType)Convert.ToInt32(item.Element("NoteType").Value);
                        double totalGoal = Utils.ConvertStringToDouble(item.Element("TotalGoal").Value);
                        bool HasTotal = Convert.ToBoolean(item.Element("HasTotal").Value);
                        bool HasMiniNotes = Convert.ToBoolean(item.Element("HasMiniNotes").Value);
                        int TagID = Convert.ToInt32(item.Element("TagID").Value);
                        string TagName = item.Element("TagName").Value;
                        Tag Tag = TagsGlobals.SearchTag(TagID, TagName);
                        Note note = new Note(ID, createdDate, modifiedDate, title, shortDescription, Tag, HasTotal, NoteType, totalGoal, HasMiniNotes, new List<MiniNote>());
                        //LoadMiniNote(note, item, AddTag, AddCategory);
                        NotesGlobals.NotesList.Add(note);
                    }
                }
                XUserDocument.Save(Globals.NotesFilePath);
            }
        }

        private static void LoadMiniNote(Note note, bool AddTag = true, bool AddCategory = true)
        {
            //load it
            foreach (MiniNote item in note.MiniNotesList)
            {
                //one mininote
                int ID = item.ID;
                double sum = item.Sum;
                if (sum != 0)
                {
                    note.HasTotal = true;
                }
                string description = item.Description;
                bool hasTransaction = item.HasTransaction;
                bool check = false;
                if (note.NoteType == NotesGlobals.NoteType.ShopList)
				{
                    check = item.Checked;
				}
                MiniNote mininote = null;

                if (hasTransaction)
				{
                    Transaction element = item.Transaction;
                    Transaction transaction;
					if (element.Color != SKColor.Empty)
					{
                        transaction = new Transaction(/*Convert.ToInt32(element.Element("ID").Value), */sum,
							element.Date, element.Name,
                            element.Tag == null ? null : TagsGlobals.SearchOrAddNewTag(element.Tag.Name, false, AddTag),
                            element.Category == null ? null : CategoriesGlobals.SearchOrAddNewCategory(element.Category.Name, false, AddCategory),
                            element.Color, element.HasCategory, element.HasCarConsumption, RepetitiveInterval: element.RepetitiveInterval);
					}
					else
					{
                        transaction = new Transaction(/*Convert.ToInt32(element.Element("ID").Value), */sum,
							element.Date, element.Name,
                            element.Tag == null ? null : TagsGlobals.SearchOrAddNewTag(element.Tag.Name, false, AddTag),
                            element.Category == null ? null : CategoriesGlobals.SearchOrAddNewCategory(element.Category.Name, false, AddCategory),
                            Utils.CategoryOrTagToSKColor(), element.HasCategory, element.HasCarConsumption, RepetitiveInterval: element.RepetitiveInterval);
					}

                    if (transaction.HasCarConsumption)
                    {
                        transaction.Kilometers = element.Kilometers;
                        transaction.Liters = element.Liters;
                        transaction.Price = element.Price;
                        transaction.Unit = element.Unit;
                    }

                    if (MoneyGlobals.TransactionsList != null && MoneyGlobals.TransactionsList.Count > 0)
					{
                        bool found = false;
						foreach (Month month in Globals.MonthsList)
						{
							foreach (Transaction tran in month.Transactions)
							{
								if (MoneyGlobals.TransactionsMatch(transaction, tran))
								{
                                    found = true;
                                    mininote = new MiniNote(note, ID, tran);
									if (!NotesGlobals.MiniNoteInList(tran.MiniNotes, mininote))
									{
                                        tran.MiniNotes.Add(mininote);
									}
                                    break;
                                }
							}
							if (found)
							{
                                break;
							}
						}
                    }
                }
				else
				{
                    mininote = new MiniNote(note, ID, description, sum, check);
				}
                //todo check for obsolescence
                if (!NotesGlobals.MiniNoteInList(note.MiniNotesList, mininote))
                {
                    note.MiniNotesList.Add(mininote);
                }
            }
        }

        public static void RefreshNotes(/*bool NoSet = false*/)
        {
            XUserDocument = XDocument.Load(Globals.NotesFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                XElement Notes = XUserDocument.Root;
                Notes.RemoveAll();
                foreach (Note note in NotesGlobals.NotesList)
                {
                    //check for nulls and set flag
					foreach (MiniNote mininote in note.MiniNotesList.Reverse<MiniNote>())
					{
						if (mininote == null)
						{
                            note.MiniNotesList.Remove(mininote);
						}
					}

                    //write the rest
                    XElement mininotes = new XElement("MiniNotes");
                    if (note.HasMiniNotes && note.MiniNotesList != null && note.MiniNotesList.Count > 0)
                    {
                        foreach (MiniNote miniNote in note.MiniNotesList)
                        { 
							if (miniNote.HasTransaction)
							{
                                string color  = miniNote.Transaction.Color.ToString() != SKColor.Empty.ToString() ? miniNote.Transaction.Color.ToString() : "";
                                mininotes.Add(new XElement("MiniNote", new XElement("ID", miniNote.ID), new XElement("Description", miniNote.Description), new XElement("Sum", miniNote.Sum),
                                new XElement("HasTransaction", miniNote.HasTransaction), new XElement("Transaction", new XElement("ID", miniNote.Transaction.ID.ToString()), new XElement("Sum", miniNote.Transaction.Sum.ToString("#,##0.00"))
                                , new XElement("Date", Globals.SetXMLDate(miniNote.Transaction.Date)), new XElement("Name", miniNote.Transaction.Name)
                                , new XElement("TagName", miniNote.Transaction.Tag == null ? "" : miniNote.Transaction.Tag.Name), new XElement("CategoryName", miniNote.Transaction.Category == null ? "" : miniNote.Transaction.Category.Name), new XElement("Color", color)
                                , new XElement("HasCategory", miniNote.Transaction.HasCategory), new XElement("HasConsumption", miniNote.Transaction.HasCarConsumption)
                                , new XElement("Kilometers", miniNote.Transaction.Kilometers), new XElement("Liters", miniNote.Transaction.Liters), new XElement("Price", miniNote.Transaction.Price), new XElement("Unit", (int)UnitType.Unit), new XElement("Interval", miniNote.Transaction.RepetitiveInterval))));
                            }
							else
							{
                                XElement check = new XElement("Checked", false);
								if (note.NoteType == NotesGlobals.NoteType.ShopList)
								{
                                    check = new XElement("Checked", miniNote.Checked);
                                }
                                mininotes.Add(new XElement("MiniNote", new XElement("ID", miniNote.ID), new XElement("Description", miniNote.Description), new XElement("Sum", miniNote.Sum), new XElement("HasTransaction", miniNote.HasTransaction), check));
                            }
                        }
                    }
                    Notes.Add(new XElement("Note", new XElement("ID", note.ID), new XElement("Title", note.Title), new XElement("ShortDescription", note.ShortDescription),
                        new XElement("NoteType", (int)note.NoteType), new XElement("TotalGoal", note.TotalGoal), new XElement("TagID", note.Tag == null ? -1 : note.Tag.ID), new XElement("TagName", note.Tag == null ? "" : note.Tag.Name), new XElement("HasTotal", note.HasTotal), new XElement("HasMiniNotes", note.HasMiniNotes),
                        new XElement("CreatedDate", Globals.SetXMLDate(note.CreatedDate)), new XElement("LastModifiedDate", Globals.SetXMLDate(note.LastModifiedDate)), mininotes));
                }
                XUserDocument.Save(Globals.NotesFilePath);
            }
        }

        #endregion

        #region Cars Part

        public static void LoadCarsData()
        {
            Globals.CreateLoadDataFile("Cars");

            LoadDataFromXML(Globals.CarsFilePath, "Cars");
            XUserDocument = XDocument.Load(Globals.CarsFilePath);

            DeserializeCars();
        }

        public static void DeserializeCars(bool AddTag = true, bool AddCategory = true)
        {
            //Read
            XmlSerializer ser = new XmlSerializer(typeof(List<Car>));
            TextReader reader = new StreamReader(Globals.CarsFilePath);
            List<Car> tempCars = ser.Deserialize(reader) as List<Car>;
            reader.Close();
            foreach (Car item in tempCars)
            {
                if (MoneyGlobals.TransactionsList != null && MoneyGlobals.TransactionsList.Count > 0)
                {
                    List<Transaction> unverifiedTransactions = new List<Transaction>();
                    foreach (Transaction transactionElement in item.Transactions)
                    {
                        Transaction newTransaction = null;
                        if (transactionElement.Color != SKColor.Empty)
                        {
                            newTransaction = new Transaction(/*Convert.ToInt32(transactionElement.Element("ID").Value), */transactionElement.Sum,
                                transactionElement.Date, transactionElement.Name,
                                transactionElement.Tag == null ? null : TagsGlobals.SearchOrAddNewTag(transactionElement.Tag.Name, false, AddTag),
                                transactionElement.Category == null ? null : CategoriesGlobals.SearchOrAddNewCategory(transactionElement.Category.Name, false, AddCategory)
                                ,transactionElement.Color, transactionElement.HasCategory,
                                transactionElement.HasCarConsumption, RepetitiveInterval: transactionElement.RepetitiveInterval);
                        }
                        else
                        {
                            newTransaction = new Transaction(/*Convert.ToInt32(transactionElement.Element("ID").Value), */transactionElement.Sum,
                                transactionElement.Date, transactionElement.Name,
                                transactionElement.Tag == null ? null : TagsGlobals.SearchOrAddNewTag(transactionElement.Tag.Name, false, AddTag),
                                transactionElement.Category == null ? null : CategoriesGlobals.SearchOrAddNewCategory(transactionElement.Category.Name, false, AddCategory),
                                Utils.CategoryOrTagToSKColor(), transactionElement.HasCategory,
                                transactionElement.HasCarConsumption, RepetitiveInterval: transactionElement.RepetitiveInterval);
                        }
                        if (newTransaction.HasCarConsumption)
                        {
                            newTransaction.Kilometers = transactionElement.Kilometers;
                            newTransaction.Liters = transactionElement.Liters;
                            newTransaction.Price = transactionElement.Price;
                            newTransaction.Unit = transactionElement.Unit;
                        }
                        unverifiedTransactions.Add(newTransaction);

                    }

                    foreach (Month month in Globals.MonthsList)
                    {
                        foreach (Transaction tran in month.Transactions)
                        {
                            foreach (Transaction unverifiedTran in unverifiedTransactions.Reverse<Transaction>())
                            {
                                if (MoneyGlobals.TransactionsMatch(unverifiedTran, tran))
                                {
                                    unverifiedTransactions.Remove(unverifiedTran);
                                    tran.Cars.Add(item);
                                    item.Transactions.Add(tran);
                                    break;
                                }
                            }

                            if (unverifiedTransactions.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void SerializeCars()
        {
            //Write
            XmlSerializer ser = new XmlSerializer(typeof(List<Car>));
            TextWriter reader = new StreamWriter(Globals.CarsFilePath);
            ser.Serialize(reader, CarsGlobals.CarsList);
            reader.Close();
        }

        /// <summary>
        /// Used To Reload cars after some tag was deleted to don't add that tag again from XML
        /// </summary>
        /// <param name="AddTag"></param>
        public static void ReLoadCars(bool AddTag = false, bool AddCategory = false)
		{
            DeserializeCars(AddTag, AddCategory);
            RefreshCars();
		}

        [Obsolete]
        private static void LoadCars(bool AddTag = true, bool AddCategory = true)
        {
            XUserDocument = XDocument.Load(Globals.CarsFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                CarsGlobals.CarsList = new List<Car>();
                //load them
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        //one car at a time
                        int ID = Convert.ToInt32(item.Element("ID").Value);
                        string plate = item.Element("Plate").Value;
                        string brand = item.Element("Brand").Value;
                        string model = item.Element("Model").Value;
                        int year = Convert.ToInt32(item.Element("Year").Value);
                        int TagID = Convert.ToInt32(item.Element("TagID").Value);
                        string TagName = item.Element("TagName").Value;
                        Tag tag = TagsGlobals.SearchTag(TagID, TagName);
                        Car car = new Car(ID, plate, brand, model, year, tag, new List<Transaction>(), new List<CarObject>(), new List<Repair>());
						if (item.Element("CarTransactions").HasElements)
						{
                            if (MoneyGlobals.TransactionsList != null && MoneyGlobals.TransactionsList.Count > 0)
                            {
                                XElement carTransactions = item.Element("CarTransactions");
                                List<Transaction> unverifiedTransactions = new List<Transaction>();
								foreach (XElement transactionElement in carTransactions.Elements())
								{
                                    Transaction newTransaction = null;
                                    if (transactionElement.Element("Color").Value != "")
                                    {
                                        newTransaction = new Transaction(/*Convert.ToInt32(transactionElement.Element("ID").Value), */Utils.ConvertStringToDouble(transactionElement.Element("Sum").Value),
                                            Globals.GetXMLDate(transactionElement.Element("Date").Value), transactionElement.Element("Name").Value,
                                            TagsGlobals.SearchOrAddNewTag(transactionElement.Element("TagName").Value, false, AddTag), CategoriesGlobals.SearchOrAddNewCategory(transactionElement.Element("CategoryName").Value, false, AddCategory)
                                            , SKColor.Parse(transactionElement.Element("Color").Value), Convert.ToBoolean(transactionElement.Element("HasCategory").Value),
                                            Convert.ToBoolean(transactionElement.Element("HasConsumption").Value), RepetitiveInterval: transactionElement.Element("Interval").Value);
                                    }
                                    else
                                    {
                                        newTransaction = new Transaction(/*Convert.ToInt32(transactionElement.Element("ID").Value), */Utils.ConvertStringToDouble(transactionElement.Element("Sum").Value),
                                            Globals.GetXMLDate(transactionElement.Element("Date").Value), transactionElement.Element("Name").Value,
                                            TagsGlobals.SearchOrAddNewTag(transactionElement.Element("TagName").Value, false, AddTag), CategoriesGlobals.SearchOrAddNewCategory(transactionElement.Element("CategoryName").Value, false, AddCategory),
                                            Utils.CategoryOrTagToSKColor(), Convert.ToBoolean(transactionElement.Element("HasCategory").Value), Convert.ToBoolean(transactionElement.Element("HasConsumption").Value), RepetitiveInterval: transactionElement.Element("Interval").Value);
                                    }
                                    if (newTransaction.HasCarConsumption)
                                    {
                                        newTransaction.Kilometers = Utils.ConvertStringToDouble(transactionElement.Element("Kilometers").Value);
                                        newTransaction.Liters = Utils.ConvertStringToDouble(transactionElement.Element("Liters").Value);
                                        newTransaction.Price = Utils.ConvertStringToDouble(transactionElement.Element("Price").Value);
                                        newTransaction.Unit = (Unit)Utils.ConvertStringToInt(transactionElement.Element("Unit").Value);
                                    }
                                    unverifiedTransactions.Add(newTransaction);

                                }

                                foreach (Month month in Globals.MonthsList)
                                {
                                    foreach (Transaction tran in month.Transactions)
                                    {
                                        foreach (Transaction unverifiedTran in unverifiedTransactions.Reverse<Transaction>())
                                        {
                                            if (MoneyGlobals.TransactionsMatch(unverifiedTran, tran))
                                            {
                                                unverifiedTransactions.Remove(unverifiedTran);
                                                tran.Cars.Add(car);
                                                car.Transactions.Add(tran);
                                                break;
                                            }
                                        }

                                        if (unverifiedTransactions.Count == 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        LoadCarObjects(car, item);
                        LoadCarRepairs(car, item);
                        CarsGlobals.CarsList.Add(car);
                    }
                }
                XUserDocument.Save(Globals.CarsFilePath);
            }
        }

        private static void LoadCarObjects(Car car, XElement xElementCar)
        {         
            if (car.CarObjects == null)
	        {
                car.CarObjects = new List<CarObject>();
	        }
            //load them
            foreach (XElement item in xElementCar.Element("CarObjects").Elements())
            {
                if (item.HasElements)
                {
                    //one carobject at a time
                    int ID = Convert.ToInt32(item.Element("ID").Value);
                    string name = item.Element("Name").Value;
                    DateTime beginDate = Globals.GetXMLDate(item.Element("BeginDate").Value);
                    DateTime endDate = Globals.GetXMLDate(item.Element("EndDate").Value);
                    car.CarObjects.Add(new CarObject(ID, name, beginDate, endDate, car));
                }
            }
        }

        private static void LoadCarRepairs(Car car, XElement xElementCar)
        {
            if (car.Repairs == null)
            {
                car.Repairs = new List<Repair>();
            }
            //load them
            foreach (XElement item in xElementCar.Element("Repairs").Elements())
            {
                if (item.HasElements)
                {
                    //one carobject at a time
                    int ID = Convert.ToInt32(item.Element("ID").Value);
                    string name = item.Element("Name").Value;
                    string description = item.Element("Description").Value;
                    DateTime date = Globals.GetXMLDate(item.Element("Date").Value);
                    double kilometers = Utils.ConvertStringToDouble(item.Element("Kilometers").Value);
                    Unit unit = (Unit)Utils.ConvertStringToDouble(item.Element("Unit").Value);
                    double sum = Utils.ConvertStringToDouble(item.Element("Sum").Value);
                    car.Repairs.Add(new Repair(ID, name, description, date, kilometers, unit, sum, car));
                   }
            }
        }

        public static void RefreshCars()
        {
            XUserDocument = XDocument.Load(Globals.CarsFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                XElement Cars = XUserDocument.Root;
                Cars.RemoveAll();
                foreach (Car car in CarsGlobals.CarsList)
                {
					XElement carObjects = new XElement("CarObjects");
					if (car.CarObjects != null && car.CarObjects.Count > 0)
					{
						foreach (CarObject carObject in car.CarObjects)
						{
							carObjects.Add(new XElement("CarObject", new XElement("ID", carObject.ID), new XElement("Name", carObject.Name), new XElement("BeginDate", Globals.SetXMLDate(carObject.BeginDate)), new XElement("EndDate", Globals.SetXMLDate(carObject.EndDate))));
						}
					}

                    XElement repairs = new XElement("Repairs");
                    if (car.Repairs != null && car.Repairs.Count > 0)
                    {
                        foreach (Repair repair in car.Repairs)
                        {
                            repairs.Add(new XElement("Repair", new XElement("ID", repair.ID), new XElement("Name", repair.Name), new XElement("Description", repair.Description), new XElement("Date", Globals.SetXMLDate(repair.Date)), new XElement("Kilometers", repair.Kilometers)
                                , new XElement("Unit", (int)repair.Unit), new XElement("Sum", repair.Sum)));
                        }
                    }

                    XElement carTransactions = new XElement("CarTransactions");
                    if (car.Transactions != null && car.Transactions.Count > 0)
					{
						foreach (Transaction transaction in car.Transactions)
						{
                            string color = transaction.Color.ToString() != SKColor.Empty.ToString() ? transaction.Color.ToString() : "";
                            carTransactions.Add(new XElement("Transaction", new XElement("ID", transaction.ID.ToString()), new XElement("Sum", transaction.Sum.ToString("#,##0.00"))
                                , new XElement("Date", Globals.SetXMLDate(transaction.Date)), new XElement("Name", transaction.Name)
                                , new XElement("TagName", transaction.Tag == null ? "" : transaction.Tag.Name), new XElement("CategoryName", transaction.Category == null ? "" : transaction.Category.Name), new XElement("Color", color)
                                , new XElement("HasCategory", transaction.HasCategory), new XElement("HasConsumption", transaction.HasCarConsumption)
                                , new XElement("Kilometers", transaction.Kilometers), new XElement("Liters", transaction.Liters), new XElement("Price", transaction.Price), new XElement("Unit", (int)UnitType.Unit), new XElement("Interval", transaction.RepetitiveInterval)));
                        }
					}
                    Cars.Add(new XElement("Car", new XElement("ID", car.ID), new XElement("Plate", car.Plate), new XElement("Brand", car.Brand),
                        new XElement("Model", car.Model), new XElement("Year", car.Year), new XElement("TagID", car.Tag == null ? -1 : car.Tag.ID), new XElement("TagName", car.Tag == null ? "" : car.Tag.Name), carTransactions, carObjects, repairs));
                }
                XUserDocument.Save(Globals.CarsFilePath);
            }
        }

        public static void ReLoadUnitType()
		{
            XUserDocument = XDocument.Load(Globals.CarsFilePath);
            if (XUserDocument != null && XUserDocument.Root != null && XUserDocument.Root.Element("UnitType") != null)
            {
                XUserDocument.Root.Element("UnitType").Value = Convert.ToInt32(UnitType.Unit).ToString();
                XUserDocument.Save(Globals.CarsFilePath);
            }
        }

        public static void LoadUnitType()
        {
            XUserDocument = XDocument.Load(Globals.CarsFilePath);
            if (XUserDocument != null && XUserDocument.Root != null && XUserDocument.Root.Element("UnitType") != null)
            {
				if (string.IsNullOrEmpty(XUserDocument.Root.Element("UnitType").Value))
				{
                    UnitType.Unit = Unit.Metric;
                }
				else
				{
                    UnitType.Unit = (Unit)Convert.ToInt32(XUserDocument.Root.Element("UnitType").Value);
				}
				if (UnitType.Unit == Unit.UKImperial)
				{
                    UnitType.Kilometers = 1.60934;
                    UnitType.Liters = 4.54609;
                }
				else if (UnitType.Unit == Unit.USImperial)
				{
                    UnitType.Kilometers = 1.60934;
                    UnitType.Liters = 3.78541;
                }
                else if (UnitType.Unit == Unit.Metric)
                {
                    UnitType.Kilometers = 1;
                    UnitType.Liters = 1;
                }
            }
            UnitType.SystemUnits.Clear();
            UnitType.SystemUnits.Add(new UnitSystem(Unit.Metric, 1, 1));
            UnitType.SystemUnits.Add(new UnitSystem(Unit.UKImperial, 1.60934, 4.54609));
            UnitType.SystemUnits.Add(new UnitSystem(Unit.USImperial, 1.60934, 3.78541));
        }

        #endregion

        #region Tags Part

        public static void LoadTagsData()
        {
            Globals.CreateLoadDataFile("Tags");

            LoadDataFromXML(Globals.TagsFilePath, "Tags");
            XUserDocument = XDocument.Load(Globals.TagsFilePath);

            DeserializeTags();
        }

        [Obsolete]
        public static void LoadTags()
        {
            XUserDocument = XDocument.Load(Globals.TagsFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                TagsGlobals.TagsList = new List<Tag>();
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        //one tag at a time
                        int ID = Convert.ToInt32(item.Element("ID").Value);
                        string name = item.Element("Name").Value;
                        SKColor color = SKColor.Empty;
						if (!string.IsNullOrEmpty(item.Element("Color").Value))
						{
                            color = SKColor.Parse(item.Element("Color").Value);
						}
                        Tag tag = new Tag(ID, name, color, new List<string>(), new List<string>());

                        //cars plates
                        if (item.Element("TagCars") != null)
						{
                            foreach (XElement car in item.Element("TagCars").Elements())
							{
                                tag.CarsFromXML.Add(car.Value);
							}
                        }

                        //notes titles
                        if (item.Element("TagNotes") != null)
                        {
                            foreach (XElement note in item.Element("TagNotes").Elements())
                            {
                                tag.NotesFromXML.Add(note.Value);
                            }
                        }

                        TagsGlobals.TagsListAdd(tag);
                    }
                }
                XUserDocument.Save(Globals.TagsFilePath);
            }
        }

        public static void DeserializeTags()
		{
            //Read
            XmlSerializer ser = new XmlSerializer(typeof(List<Tag>));
            TextReader reader = new StreamReader(Globals.TagsFilePath);
			List<Tag> tempTags = ser.Deserialize(reader) as List<Tag>;
			reader.Close();
			foreach (Tag item in tempTags)
			{
                TagsGlobals.TagsListAdd(item);
            }
		}

        public static void SerializeTags()
        {
            //Write
            XmlSerializer ser = new XmlSerializer(typeof(List<Tag>));
            TextWriter reader = new StreamWriter(Globals.TagsFilePath);
            ser.Serialize(reader, TagsGlobals.TagsList);
            reader.Close();
        }

        public static void RefreshTags()
		{
            XUserDocument = XDocument.Load(Globals.TagsFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                XElement Tags = XUserDocument.Root;
                string TagColor;
                Tags.RemoveAll();
                foreach (Tag tag in TagsGlobals.TagsList)
                {
                    tag.CarsFromXML.Clear();
                    tag.NotesFromXML.Clear();
                    TagColor = tag.Color.ToString() != SKColor.Empty.ToString() ? tag.Color.ToString() : "";
                    XElement notesElement = new XElement("TagNotes", "");
                    XElement carsElement = new XElement("TagCars", "");
                    foreach (Note note in tag.Notes)
					{
                        notesElement.Add(new XElement("NoteTitle", note.Title));
                        tag.NotesFromXML.Add(note.Title);
                    }
                    foreach (Car car in tag.Cars)
                    {
                        carsElement.Add(new XElement("CarPlate", car.Plate));
                        tag.CarsFromXML.Add(car.Plate);
                    }
                    Tags.Add(new XElement("Tag", new XElement("ID", tag.ID), new XElement("Name", tag.Name), new XElement("Color", TagColor), notesElement, carsElement));
                }
                AssignTags();
                XUserDocument.Save(Globals.TagsFilePath);
            }
        }

        public static void AssignTags()
		{
			//Assign car to tag if tag has car as autoadd
			if (CarsGlobals.CarsList.Count != 0 || TagsGlobals.TagsList.Count != 0)
			{
			    foreach (Car car in CarsGlobals.CarsList)
			    {
                    foreach (Tag tag in TagsGlobals.TagsList)
                    {
                        if (tag.CarsFromXML.Contains(car.Plate) && !CarsGlobals.CarInList(tag.Cars, car))
                        {
                            tag.Cars.Add(car);
                        }
                    }
			    }
			}

            //Assign note to tag if tag has note as autoadd
            if (NotesGlobals.NotesList.Count != 0 || TagsGlobals.TagsList.Count != 0)
            {
                foreach (Note note in NotesGlobals.NotesList)
                {
                    foreach (Tag tag in TagsGlobals.TagsList)
                    {
                        if (tag.NotesFromXML.Contains(note.Title) && !NotesGlobals.NoteInList(tag.Notes, note))
                        {
                            tag.Notes.Add(note);
                        }
                    }
                }
            }
        }

        #endregion

        #region Categories Part

        public static void LoadCategoriesData()
		{
            Globals.CreateLoadDataFile("Categories");

            LoadDataFromXML(Globals.CategoriesFilePath, "Categories");
            XUserDocument = XDocument.Load(Globals.CategoriesFilePath);

            DeserializeCategories();
        }

        [Obsolete]
        public static void LoadCategories()
        {
            XUserDocument = XDocument.Load(Globals.CategoriesFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                CategoriesGlobals.CategoriesList = new List<Category>();
                foreach (XElement item in XUserDocument.Root.Elements())
                {
                    if (item.HasElements)
                    {
                        string name = item.Element("Name").Value;
                        double charityPercentage = 0;
                        SKColor color = SKColor.Empty;

                        if (item.Element("CharityPercentage") != null && !string.IsNullOrEmpty(item.Element("CharityPercentage").Value))
                        {
                            charityPercentage = Convert.ToDouble(item.Element("CharityPercentage").Value);
                        }

                        if (!string.IsNullOrEmpty(item.Element("Color").Value))
                        {
                            color = SKColor.Parse(item.Element("Color").Value);
                        }
                        Category category = new Category(name, charityPercentage, color);
                        //load checkers
                        int i = 0;
                        foreach (XElement element in item.Element("Checkers").Elements())
                        {
                            CategoriesGlobals.SetCategoryCheckers(category, i, element.Value == "true" ? true : false);
                            i++;
                        }

                        CategoriesGlobals.CategoriesListAdd(category);
                    }
                }
                XUserDocument.Save(Globals.CategoriesFilePath);
            }
        }

        public static void DeserializeCategories()
        {
            //Read
            XmlSerializer ser = new XmlSerializer(typeof(List<Category>));
            TextReader reader = new StreamReader(Globals.CategoriesFilePath);
            List<Category> tempCategories = ser.Deserialize(reader) as List<Category>;
            reader.Close();
            foreach (Category item in tempCategories)
            {
                //load checkers
				for (int i = 0; i < item.Checkers.Count; i++)
				{
                    CategoriesGlobals.SetCategoryCheckers(item, i, item.Checkers[i].State);
                }
                CategoriesGlobals.CategoriesListAdd(item);
            }
        }

        public static void SerializeCategories()
        {
            //Write
            XmlSerializer ser = new XmlSerializer(typeof(List<Category>));
            TextWriter reader = new StreamWriter(Globals.CategoriesFilePath);
            ser.Serialize(reader, CategoriesGlobals.CategoriesList);
            reader.Close();
        }

        public static void RefreshCategories()
        {
            XUserDocument = XDocument.Load(Globals.CategoriesFilePath);
            if (XUserDocument != null && XUserDocument.Root != null)
            {
                XElement Categories = XUserDocument.Root;
                string CategoryColor;
                Categories.RemoveAll();
                foreach (Category category in CategoriesGlobals.CategoriesList)
                {
                    XElement IncIncome = new XElement("IncIncome", CategoriesGlobals.GetCategoryCheckersValue(category, 0));
                    XElement IncExpense = new XElement("IncExpense", CategoriesGlobals.GetCategoryCheckersValue(category, 1));
                    XElement IncCharity = new XElement("IncCharity", CategoriesGlobals.GetCategoryCheckersValue(category, 2));
                    XElement ExcIncome = new XElement("ExcIncome", CategoriesGlobals.GetCategoryCheckersValue(category, 3));
                    XElement ExcExpense = new XElement("ExcExpense", CategoriesGlobals.GetCategoryCheckersValue(category, 4));
                    XElement ExcCharity = new XElement("ExcCharity", CategoriesGlobals.GetCategoryCheckersValue(category, 5));
                    XElement Checkers = new XElement("Checkers", IncIncome, IncExpense, IncCharity, ExcIncome, ExcExpense, ExcCharity);

                    CategoryColor = category.Color.ToString() != SKColor.Empty.ToString() ? category.Color.ToString() : "";
                    Categories.Add(new XElement("Category", new XElement("Name", category.Name), new XElement("CharityPercentage", category.CharityPercentage), new XElement("Color", CategoryColor), Checkers));
                }
                XUserDocument.Save(Globals.CategoriesFilePath);
            }
        }

        #endregion

        #endregion
    }
}