using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Timers;
using static ManyManager.Resource;
using Android.Gms.Common;
using Android.Util;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Gms.Common.Apis;
using PerpetualEngine.Storage;
using System.Threading;
using Android.OS;
using Android.Runtime;
using System.Linq;
using System.IO;
using Android.Webkit;
using Android.Hardware.Camera2;
using Android.Renderscripts;

namespace ManyManager
{

	public static class Globals
	{
		public static ProgressDialog ProgressDialog;

		public static void ShowProgressDialog()
		{
			if (ProgressDialog == null)
			{
				ProgressDialog = new ProgressDialog(Context);
				ProgressDialog.SetMessage("Loading…");
				ProgressDialog.Indeterminate = true;
			}
			//display progress in another thread
			//ProgressDialog.Show(Context, string.Empty, "Loading...", true);
		}

		public static void HideProgressDialog()
		{
			if (ProgressDialog != null)
			{
				ProgressDialog.Dismiss();
			}
		}

		public static void GetContentView()
		{
			exitApp += Convert.ToInt32(CurrentForm == AccountsMenu);
			if (ParentForm == AccountsMenu && CurrentForm != AccountsMenu) exitApp = 0;
			if (CurrentForm == Settings)
			{
				Activity.SetContentView(MainMenu);
			}
			else if (CurrentForm == CarPropertiesMenu && !CarsGlobals.NewCarWasAdded)
			{
				Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Context);
				alert.SetMessage("Are You Sure You Don't Want To Save The New Car?");
				alert.SetPositiveButton("Yes", (senderAlert, args) =>
				{
					Activity.SetContentView(CarsMenu);
				});
				alert.SetNegativeButton("No", (senderAlert, args) =>
				{ });
				Dialog dialog = alert.Create();
				dialog.Show();
			}
			else if (ParentForm == AccountsMenu)
			{
				if (exitApp >= 1)
				{
					Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Context);
					alert.SetMessage("Do You Want To Exit App?");
					alert.SetPositiveButton("Yes", (senderAlert, args) =>
					{
						Activity.Finish();
					});
					alert.SetNegativeButton("No", (senderAlert, args) =>
					{ });
					Dialog dialog = alert.Create();
					dialog.Show();
				}
				else
				{
					Activity.SetContentView(ParentForm);
				}
			}
			else if (ParentForm == MainMenu)
			{
				//when backpress in tagsmenu, carsmenu, notesmenu from transactionmenu forms
				if (PreviousForm == TransactionMenu)
				{
					Activity.SetContentView(PreviousForm);
				}
				else if (CurrentForm == NotesMenu)
				{
					if (NotesGlobals.NotesEditState)
					{
						(grdNotes.Adapter as NotesAdapter).EditState();
						(grdNotes.Adapter as NotesAdapter).RevertChanges();
						(grdNotes.Adapter as NotesAdapter).UpdateAdapter();
					}
					else
					{
						Activity.SetContentView(ParentForm);
					}
				}
				else if (CurrentForm == TagsMenu)
				{
					if (TagsGlobals.TagsEditState)
					{
						(grdTags.Adapter as TagsAdapter).EditState();
						(grdTags.Adapter as TagsAdapter).RevertChanges();
						(grdTags.Adapter as TagsAdapter).UpdateAdapter();
					}
					else
					{
						Activity.SetContentView(ParentForm);
					}
				}
				else if (CurrentForm == CarsMenu)
				{
					if (CarsGlobals.CarsEditState)
					{
						(grdCars.Adapter as CarsAdapter).EditState();
						(grdCars.Adapter as CarsAdapter).RevertChanges();
						(grdCars.Adapter as CarsAdapter).UpdateAdapter();
					}
					else
					{
						Activity.SetContentView(ParentForm);
					}
				}
				else if (CurrentForm == CategoriesMenu)
				{
					if (CategoriesGlobals.CategoryEditState)
					{
						(grdCategories.Adapter as CategoriesAdapter).EditState();
						(grdCategories.Adapter as CategoriesAdapter).RevertChanges();
						(grdCategories.Adapter as CategoriesAdapter).UpdateAdapter();
					}
					else
					{
						Activity.SetContentView(ParentForm);
					}
				}
				else
				{
					Activity.SetContentView(MainMenu);
				}
			}
			else if (ParentForm == CarsMenu)
			{
				if (CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Tag && TagsGlobals.TagsEditState)
				{
					TagsGlobals.TagsEditState = false;
					(grdCarStuffs.Adapter as TagsAdapter).RevertChanges();
					(grdCarStuffs.Adapter as TagsAdapter).UpdateAdapter();
				}
				else
				{
					Activity.SetContentView(ParentForm);
				}
			}
			else if (ParentForm == YearlyStats)
			{
				if (CurrentForm == TransactionsMenu && MoneyGlobals.TransactionsEditState)
				{
					(grdTransactions.Adapter as TransactionsAdapter).EditState();
					(grdTransactions.Adapter as TransactionsAdapter).RevertChanges();
					(grdTransactions.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);
				}
				else
				{
					Activity.SetContentView(YearlyStats);
				}
			}
			else
			{
				Activity.SetContentView(ParentForm);
			}
		}

		#region Public Forms

		/// <summary>
		/// Global Activity
		/// </summary>
		public static MainActivity Activity;
		/// <summary>
		/// Global Context
		/// </summary>
		public static Context Context;

		public static int TagsMenuParent;
		public static int SpecificForm;
		public static int ParentForm;
		public static int CurrentForm;
		public static int PreviousForm;
		public static int CurrentTheme;

		public static int PauseForm;

		#endregion

		#region Layouts

		//Accounts
		public static int AccountsMenu;
		public static int AccountMenu;
		public static int GridViewAccountItem;
		//Loading & MainMenues
		public static int AutoLogin;
		public static int MainMenu;
		public static int LoadingApp;
		//Settings
		public static int Settings;
		public static int ChangeCategoryOrTagColor;
		public static int ChangeCharityPercentage;
		public static int ChangeLogin;
		public static int ChangeUnitsType;
		//Money
		public static int YearlyStats;
		public static int TransactionsMenu;
		public static int TransactionMenu;
		public static int GridViewTransactionItem;
		public static int GridViewSKColorImageItem;
		public static int Filters;
		public static int Sorts;
		public static int GridViewFiltersItem;
		//Notes
		public static int NotesMenu;
		public static int NoteMenu;
		public static int NoteSettings;
		public static int MiniNoteMenu;
		public static int GridViewMiniNoteItem;
		public static int GridViewNoteItem;
		//Cars
		public static int CarsMenu;
		public static int CarMenu;
		public static int GridViewCarItem;
		public static int GridViewCarObjectItem;
		public static int CarObject;
		public static int CarConsumption;
		public static int CarPropertiesMenu;
		public static int GridViewRepairItem;
		public static int RepairMenu;
		//Tags
		public static int TagsMenu;
		public static int TagMenu;
		public static int GridViewTagItem;
		//Categories
		public static int CategoriesMenu;
		public static int CategoryMenu;
		public static int GridViewCategoryItem;

		#endregion

		#region Global Variables

		public static System.Timers.Timer timer;
		public static int timeElapsed = 0;

		/// <summary>
		/// Used to don't trigger stuff from events of checkall checkboxes in adapters
		/// </summary>
		public static bool doNothing = false;

		public static View currentInput;

		public static bool returnFromPause = false;

		public static string UserName;

		public static bool dataLoaded = false;
		public static int exitApp = 0;
		public static bool internalStorage = false;
		public static bool inSwitchChanged = false;
		public static SKColor background;
		public static SKColor textColor;

		public static bool ProcessedRepetitive = false;

		public static bool formLoading = false;

		public static bool AddMiniNote = false;

		//used to bind transactions with notes & cars & tags
		public static bool TagToAddInTransaction = false;
		public static bool NoteToAddInTransaction = false;
		public static bool CarToAddInTransaction = false;
		public static Tag SelectedTagForTransaction;
		public static List<Car> SelectedCarsForTransaction = new List<Car>();
		public static List<Note> SelectedNotesForTransaction = new List<Note>();

		//used to bind categories with colors
		public static bool ColorToAddInCategory = false;
		public static SKColor SelectedColorForCategory;

		//used to bind tag with notes & cars & colors
		public static bool ColorToAddInTag = false;
		public static bool NoteToAddInTag = false;
		public static bool CarToAddInTag = false;
		public static int SelectedColorIndex = -1;
		public static SKColor SelectedColorForTag;
		public static List<Car> SelectedCarsForTag = new List<Car>();
		public static List<Note> SelectedNotesForTag = new List<Note>();

		/// <summary>
		/// use with phone login
		/// </summary>
		public static bool LoggedIn;
		public static KeyguardManager KeyguardManager;

		public static bool SetPIN;
		public static SimpleStorage CacheStorage;

		public static int Active = 0;

		#endregion

		#region Accounts Stuff

		#region AccountsMenu

		public static ImageView imgAddNewAccount;

		public static GridView grdAccounts;

		#endregion

		#region AccountMenu

		public static EditText txtAccountName;

		public static Button btnAddOrModifyAccount;

		#endregion

		#region GridViewAccounts Form

		public static Button btnGridLoginAccount;

		public static ImageView imgModifyAccount;
		public static ImageView imgRemoveAccount;

		#endregion

		#endregion

		#region MainMenus Stuff

		#region MainMenu Form

		public static Button btnMoneyPart;
		public static Button btnNotesPart;
		public static Button btnCarsPart;
		public static Button btnTagsPart;
		public static Button btnCategoriesPart;

		public static Google.Android.Material.FloatingActionButton.FloatingActionButton btnQuickAddTransaction;

		#endregion

		#region AutoLogin

		public static TextView txtShowPIN;

		public static ImageView btnNumericRemoveOne;

		public static Button btnNumeric0;
		public static Button btnNumeric1;
		public static Button btnNumeric2;
		public static Button btnNumeric3;
		public static Button btnNumeric4;
		public static Button btnNumeric5;
		public static Button btnNumeric6;
		public static Button btnNumeric7;
		public static Button btnNumeric8;
		public static Button btnNumeric9;

		#endregion

		#endregion

		#region Settings Stuff

		#region Settings Form

		public static Switch externalSwitch;

		public static Button btnExportToCSV;
		public static Button btnImportToCSV;
		public static Button btnGoToChangeCharity;
		public static Button btnGoToChangeCategoryOrTagColor;
		public static Button btnGoToChangeLogin;
		public static Button btnGoToChangeUnitsType;

		#endregion

		#region ChangeCategoryOrTagColor Form

		public static Button btnSetCategoryOrTagColor;

		public static Spinner spinnerChooseCategoryOrTagColor;
		public static Spinner spinnerChooseCategoryOrTagTypeColor;

		public static GridView grdSKColorImages;

		public static TextView sKColorSelectedName;

		public static EditText sKColorFiltering;

		public static ImageView sKColorSelectedImage;

		#endregion

		#region ChangeCharityPercentage Form

		public static Button btnNewZecAdd;

		public static EditText txtNewZec;

		public static Spinner spinnerZecPeriodChangeChooser;

		public static GridView grdYearZecPercentage;

		#endregion

		#region ChangeLogin

		public static Switch swtSetPIN;
		public static Switch swtSetLocalSecurity;

		public static SignInButton GoogleSignInButton;
		public static Button GoogleSignOutButton;
		public static Button GoogleDisconnectButton;

		#endregion

		#region ChangeUnitsType

		public static Button btnChangeUnits;

		public static Spinner spiUnitsType;

		#endregion

		#endregion

		#region Money Stuff

		#region Properties For Logic Money Calculations

		public static double Income;
		public static double Expense;
		public static double ExpenseWith;
		/// <summary>
		/// Charity which was given. Payed each month.
		/// </summary>
		public static double GivenCharity;
		public static double CharityWith;

		public static double CurrentMonthProfit;
		public static double CurrentMonthIncome;
		public static double CurrentMonthCharity;

		public static List<Month> MonthsList;

		/// <summary>
		/// Total Needed
		/// </summary>
		public static double TotalCharity;
		public static double TotalRatio;
		public static double TotalProfit;
		public static double TotalIncome;
		public static double TotalExpense;
		/// <summary>
		/// Left to pay from Total, after givenCharity
		/// </summary>
		public static double LeftCharity;
		public static double MonthlyIncome;
		public static double MonthlyExpense;
		public static double DailyIncome;
		public static double DailyExpense;
		public static int TotalDays;

		#endregion

		#region Public Properties Money

		public static Java.IO.File AccountsDataFile;
		public static string AccountsDataFilePath;

		public static Java.IO.File YearFile;
		public static string YearFilePath;

		public static Java.IO.File CategoriesFile;
		public static string CategoriesFilePath;

		public static Java.IO.File TagsFile;
		public static string TagsFilePath;

		public static Java.IO.File CarsFile;
		public static string CarsFilePath;

		public static Java.IO.File FiltersFile;
		public static string FiltersFilePath;

		public static Java.IO.File NotesFile;
		public static string NotesFilePath;
		//todo remove this
		public static Java.IO.File UserDataFile;
		public static string UserDataFilePath;

		public static string InternalPath;

		/// <summary>
		/// Used for block first wrong selection of spinner in TransactionsMenu
		/// </summary>
		public static bool blockedCategoryOrTag;

		//SKColors
		public static List<CTColor> SKCategoriesOrTagColor;
		public static Dictionary<string, SKColor> SKStringColors;
		public static List<SKColor> AllSKColors;
		public static SKColor SKDefaultCategoriesColor = SKColors.Empty;

		#endregion

		#region Public Methods

		public static bool ProcessRepetitive(Transaction item)
		{
			string[] intervals = item.RepetitiveInterval.Split(";");

			//calculate if it's time to add a new transaction
			//if there's time - add new transaction with this repetitive setting and remove from old transaction the repetitive setting
			DateTime nextTime = item.Date;
			int interval1 = 0;
			if (intervals[1] != string.Empty)
			{
				interval1 = Convert.ToInt32(intervals[1]);
			}

			bool process = false;
			switch (intervals[0])
			{
				case "Interval Of":
					switch (intervals[3])
					{
						case "Days":
							nextTime = nextTime.AddDays(interval1);
							break;
						case "Months":
							nextTime = nextTime.AddMonths(interval1);
							break;
						case "Years":
							nextTime = nextTime.AddYears(interval1);
							break;
					}
					if (DateTime.Now.ToOADate() >= nextTime.ToOADate())
					{
						process = true;
					}
					break;
				case "On Each Day Of Week":
					nextTime = GetNextWeekOfMonth(nextTime, intervals[2]);

					if (DateTime.Now.ToOADate() >= nextTime.ToOADate())
					{
						process = true;
					}
					break;
				case "On Each Week Of Month":
					nextTime = GetNextDate(nextTime, DayOfWeek.Monday);

					if (DateTime.Now.ToOADate() >= nextTime.ToOADate())
					{
						process = CheckWeekNumberToProcess(nextTime, intervals[2]);
					}
					break;
				case "On Each Day Of Month":
					//Go to the next month to start verifications from there
					int month = nextTime.Month;
					while (month == nextTime.Month)
					{
						nextTime = GetNextWeekOfMonth(nextTime, intervals[3]);
					}

					if (DateTime.Now.ToOADate() >= nextTime.ToOADate())
					{
						process = CheckWeekNumberToProcess(nextTime, intervals[2]);
					}
					break;
			}
			if (process)
			{
				string repetitiveInterval = string.Empty;
				if (!string.IsNullOrWhiteSpace(intervals[4]))
				{
					bool IsRepetitive = true;
					switch (intervals[4])
					{
						case "Once":
							repetitiveInterval = string.Empty;
							IsRepetitive = false;
							break;
						case "2 Times":
							intervals[4] = "Once";
							break;
						case "3 Times":
							intervals[4] = "2 Times";
							break;
						case "4 Times":
							intervals[4] = "3 Times";
							break;
					}
					if (intervals[4] == "Until I Stop It")
					{
						repetitiveInterval = item.RepetitiveInterval;
					}
					else if (IsRepetitive)
					{
						for (int i = 0; i <= 4; i++)
						{
							repetitiveInterval += intervals[i] + ";";
						}
					}
					ProcessedRepetitive = true;
					Transaction transaction = new Transaction(item.Sum, nextTime, item.Name, item.Tag, item.Category, item.Color, item.HasCategory, item.HasCarConsumption, item.Kilometers, item.Liters, item.Price, item.Unit, repetitiveInterval);
					item.RepetitiveInterval = string.Empty;
					MoneyGlobals.TransactionsList.Add(transaction);
					if (!string.IsNullOrWhiteSpace(transaction.RepetitiveInterval))
					{
						ProcessedRepetitive = ProcessRepetitive(transaction);
					}
				}
			}
			return ProcessedRepetitive;
		}

		public static void ProcessRepetitives()
		{
			ProcessedRepetitive = false;
			List<Transaction> temp = MoneyGlobals.TransactionsList.FindAll(x => !string.IsNullOrWhiteSpace(x.RepetitiveInterval));

			foreach (Transaction item in temp)
			{
				ProcessedRepetitive = ProcessRepetitive(item);
			}

			if (ProcessedRepetitive)
			{
				XML.SaveYears();
			}
		}

		public static bool CheckWeekNumberToProcess(DateTime nextTime, string week)
		{
			bool process = false;
			int weekNumber = GetWeekNumberOfMonth(nextTime);
			switch (week)
			{
				case "First":
					if (weekNumber == 1)
					{
						process = true;
					}
					break;
				case "Second":
					if (weekNumber == 2)
					{
						process = true;
					}
					break;
				case "Third":
					if (weekNumber == 3)
					{
						process = true;
					}
					break;
				case "Last":
					if (weekNumber == GetWeeksOfMonth(nextTime))
					{
						process = true;
					}
					break;
			}
			return process;
		}

		public static DateTime GetNextDate(DateTime nextTime, DayOfWeek dayOfWeek = DayOfWeek.Monday)
		{
			if (nextTime.DayOfWeek == dayOfWeek)
			{
				return nextTime.AddDays(7);
			}
			while (nextTime.DayOfWeek != dayOfWeek)
			{
				nextTime = nextTime.AddDays(1);
			}
			return nextTime;
		}

		public static DateTime GetNextWeekOfMonth(DateTime nextTime, string day)
		{
			switch (day)
			{
				case "Monday":
					nextTime = GetNextDate(nextTime, DayOfWeek.Monday);
					break;
				case "Tuesday":
					nextTime = GetNextDate(nextTime, DayOfWeek.Tuesday);
					break;
				case "Wednesday":
					nextTime = GetNextDate(nextTime, DayOfWeek.Wednesday);
					break;
				case "Thursday":
					nextTime = GetNextDate(nextTime, DayOfWeek.Thursday);
					break;
				case "Friday":
					nextTime = GetNextDate(nextTime, DayOfWeek.Friday);
					break;
				case "Saturday":
					nextTime = GetNextDate(nextTime, DayOfWeek.Saturday);
					break;
				case "Sunday":
					nextTime = GetNextDate(nextTime, DayOfWeek.Sunday);
					break;
			}
			return nextTime;
		}

		public static int GetWeekNumberOfMonth(DateTime date)
		{
			date = date.Date;
			DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
			DateTime firstMonthMonday = firstMonthDay.AddDays((date.DayOfWeek + 7 - firstMonthDay.DayOfWeek) % 7);
			if (firstMonthMonday > date)
			{
				firstMonthDay = firstMonthDay.AddMonths(-1);
				firstMonthMonday = firstMonthDay.AddDays((date.DayOfWeek + 7 - firstMonthDay.DayOfWeek) % 7);
			}
			return (date - firstMonthMonday).Days / 7 + 1;
		}

		public static int GetWeeksOfMonth(DateTime date)
		{
			//extract the month
			int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
			DateTime firstOfMonth = new DateTime(date.Year, date.Month, 1);
			//days of week starts by default as Sunday = 0
			int firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
			return (firstDayOfMonth + daysInMonth) / 7;
		}

		public static DateTime GetXMLDate(string date)
		{
			DateTime result = new DateTime();
			if (string.IsNullOrWhiteSpace(date))
			{
				result = DateTime.Now;
			}
			else
			{
				//process date from XML like:2020Y10M11D
				result = new DateTime(Convert.ToInt32(date.Substring(0, date.IndexOf("Y"))), Convert.ToInt32(date.Substring(date.IndexOf("Y") + 1, date.IndexOf("M") - date.IndexOf("Y") - 1)), Convert.ToInt32(date.Substring(date.IndexOf("M") + 1, date.IndexOf("D") - date.IndexOf("M") - 1)));
			}
			return result;
		}

		public static string SetXMLDate(DateTime date)
		{
			//process date to XML like:2020Y10M11D
			return date.Year + "Y" + date.Month + "M" + date.Day + "D";
		}

		public static string GetColorName(SKColor color)
		{
			if (color == SKColor.Empty)
			{
				return string.Empty;
			}
			var record = SKStringColors.Where(p => p.Value == color).Select(p => new { Key = p.Key, Value = p.Value }).FirstOrDefault();
			if (record != null)
			{
				return record.Key;
			}
			else
			{
				return string.Empty;
			}
		}

		public static SKColor GetColor(string color)
		{
			if (string.IsNullOrWhiteSpace(color))
			{
				return SKColor.Empty;
			}
			var record = SKStringColors.Where(p => p.Key == color).Select(p => new { Key = p.Key, Value = p.Value }).FirstOrDefault();
			if (record != null)
			{
				return record.Value;
			}
			else
			{
				return SKColor.Empty;
			}
		}

		#endregion

		#region All Money Forms & Controls

		#region Header Form

		public static Button btnTransactionsMenu;
		public static Button btnYearly;
		public static Button btnAddTransaction;

		#endregion

		#region YearlyStats Form

		public static TextView txtRatio;
		public static TextView txtIncome;
		public static TextView txtExpense;
		public static TextView txtProfit;
		public static TextView txtZec;
		public static TextView txtTotalDays;
		public static TextView txtLeftZec;
		public static TextView txtYearlyMonthlyIncome;
		public static TextView txtYearlyMonthlyExpense;
		public static TextView txtDailyIncome;
		public static TextView txtDailyExpense;

		public static LinearLayout layoutTextStats;

		#endregion

		#region TransactionMenu Form

		public static Button btnModifyTransaction;
		public static Button btnAddAnotherTransaction;
		public static Button btnAddAddTransaction;
		public static Button btnAddDate;
		public static Button btnSeeTags;
		public static Button btnAddToNote;
		public static Button btnAddToCar;
		public static Button btnHasCarConsumption;
		public static Button btnRepetitiveTransaction;
		public static Button btnAdvancedTransactionSettings;

		public static TextView txtChoosenNoteToAdd;
		public static TextView textviewLitersAdded;
		public static TextView textviewKilometersRan;
		public static TextView textviewPriceAdded;
		public static TextView txtChoosenCarToAdd;

		public static EditText txtAddSum;
		public static EditText txtAddDate;
		public static EditText txtInterval;

		public static EditText txtAddLiters;
		public static EditText txtAddKilometersRan;
		public static EditText txtAddPrice;

		public static AutoCompleteTextView txtAddName;
		public static AutoCompleteTextView txtAddTag;

		public static Spinner spiCategory;
		public static Spinner spinnerInterval;
		public static Spinner spinnerSetIntervalTime;
		public static Spinner spinnerEachInterval;
		public static Spinner spinnerRepeatFor;

		public static LinearLayout layoutCarConsumption;
		public static LinearLayout layoutRepetitiveTransaction;
		public static LinearLayout layoutAddTransaction;
		public static LinearLayout layoutModifyTransaction;
		public static LinearLayout layoutAdvancedTransactionSettings;

		public static CheckBox chkHasCarConsumption;

		public static CheckBox chkRepetitiveTransaction;

		#endregion

		#region Filters Form

		public static Button btnFilterThisMonth;
		public static Button btnFilterLastMonth;
		public static Button btnFilterLast3Months;
		public static Button btnFilterThisYear;
		public static Button btnFilterLastYear;
		public static Button btnFilterAllTime;
		public static Button btnFilterListDisplay;
		public static Button btnFilterChartDisplay;
		public static Button btnFilterGridDisplay;
		public static Button btnFilterFilter;
		public static Button btnNewFilterAdd;
		public static Button btnNewFilter;

		public static ImageButton btnFilterBeginDate;
		public static ImageButton btnFilterEndDate;

		public static ImageView imgCloseNewFilter;

		public static TextView txtFilterBeginDate;
		public static TextView txtFilterEndDate;

		public static EditText txtFilterMinSum;
		public static EditText txtFilterMaxSum;
		public static EditText txtNewFilterName;

		public static LinearLayout layoutFilterDates;
		public static LinearLayout layoutFilters;
		public static LinearLayout layoutShowTransactions;

		public static LinearLayout layoutFilterDefinedDates;
		public static LinearLayout layoutNewFilter;

		public static Spinner spinnerCategoryOrTagFilter;
		public static Spinner spinnerCategoryAndTagFilterChooser;

		public static GridView grdUserFilters;

		#endregion

		#region Sorts Form

		public static Button btnSortApply;
		public static Spinner spinnerTransactionsSortBy;

		#endregion

		#region TransactionsMenu Form

		public static GridView grdTransactions;

		public static Button btnTransactionsFilter;
		public static Button btnTransactionsClearFilter;
		public static Button btnTransactionsSort;

		public static TextView tvwTransactionsNoData;
		public static TextView txtTransactionsSort;
		public static TextView txtTransactionsAppliedFilter;

		public static RelativeLayout layoutGridTransactions;
		public static RelativeLayout layoutCharts;

		public static ExpandableListView elvGridTransactions;

		public static ImageView imgRemoveAllTransactions;

		public static CheckBox chkAllTransactions;

		public static Spinner chooseCategoryOrTag;
		public static Spinner chooseDisplayChart;

		public static Android.Widget.Switch showAllCategoriesOrTags;


		#endregion

		#endregion

		#region Add/Remove/Refresh XML

		public static Month GetMonthFromTransaction(Transaction transaction)
		{
			if (transaction == null)
			{
				Utils.ShowAlert("", "Transaction Doesn't Exist");
				return null;
			}
			//get Transaction
			Month month = GetMonthFromDate(transaction.Date);

			//check if there is this Transaction again
			if (month != null && month.Transactions != null)
			{
				foreach (Transaction item in month.Transactions)
				{
					if (transaction.Date.Month == month.month && transaction.Date.Year == month.year)
					{
						return month;
					}
				}
			}
			return null;
		}

		public static Month GetMonthFromDate(DateTime date)
		{
			if (MonthsList != null)
			{
				Month month = MonthsList.Find(x => x.month == date.Month && x.year == date.Year);
				if (month == null)
				{
					month = new Month();
					month.month = date.Month;
					month.year = date.Year;
					MonthsList.Add(month);
				}
				return month;
			}
			return null;
		}

		public static void RemoveMonth(DateTime date)
		{
			Month month = MonthsList.Find(x => x.month == date.Month && x.year == date.Year);
			if (month != null)
			{
				MonthsList.Remove(month);
			}
		}

		public static void AddSum(Month month, Transaction transaction)
		{
			if (month == null)
			{
				Utils.ShowAlert("", "No Valid Month To Add Sum");
				return;
			}
			month.Transactions.Add(transaction);
			MoneyGlobals.TransactionsList.Add(transaction);
			RefreshMonthWithNewTransaction(month);
		}

		public static void ReLoadNotesAndCars()
		{
			//reload Notes and mininotes which could contain transactions
			XML.ReLoadNotes();

			//reload Cars which could contain missing tags
			XML.ReLoadCars();
		}

		/// <summary>
		/// After added or modified a transaction, the month has to be refreshed and saved in XML
		/// </summary>
		/// <param name="month"></param>
		public static void RefreshMonthWithNewTransaction(Month month)
		{
			RefreshMonth(month, true);
			XML.SaveMonth(month);
		}

		public static void RefreshYear(DateTime? currentYearDate = null)
		{
			if (currentYearDate == null)
			{
				currentYearDate = DateTime.Now;
			}
			//refresh each month
			if (MonthsList.Count > 0)
			{
				List<Month> currentYear = GetYearMonths(currentYearDate.Value);
				//reset year stats
				TotalIncome = 0;
				TotalExpense = 0;
				LeftCharity = 0;
				TotalProfit = 0;
				MonthlyIncome = 0;
				MonthlyExpense = 0;
				DailyIncome = 0;
				DailyExpense = 0;
				TotalDays = TimeSpan.FromDays(currentYearDate.Value.ToOADate()).Days - TimeSpan.FromDays(Utils.ConvertStringToDate("1.1." + currentYearDate.Value.Year).ToOADate()).Days + 1;
				foreach (Month item in currentYear)
				{
					RefreshMonth(item);
				}
				//refresh global/monthly/daily stats
				RefreshYearCharity();
				TotalRatio = TotalIncome / (TotalExpense + GivenCharity);

				MonthlyIncome = TotalIncome / DateTime.Now.Month;
				MonthlyExpense = TotalExpense / DateTime.Now.Month;

				DailyIncome = TotalIncome / TotalDays;
				DailyExpense = TotalExpense / TotalDays;
			}
		}

		private static void RefreshYearCharity()
		{
			TotalCharity = 0;
			GivenCharity = 0;
			List<Month> months = GetYearMonths(DateTime.Now);
			foreach (Month month in months)
			{
				TotalCharity += month.Income * month.CharityPercentage / 100;
				foreach (Transaction transaction in month.Transactions.FindAll(x=>x.Category != null && CategoriesGlobals.IncomeCategories.Contains(x.Category) && x.Category.CharityPercentage != month.CharityPercentage))
				{
					if (CategoriesGlobals.IncomeCategories.Contains(transaction.Category))
					{
						TotalCharity -= transaction.Sum * month.CharityPercentage / 100;
						TotalCharity += transaction.Sum * transaction.Category.CharityPercentage / 100;
					}
				}
				GivenCharity += month.Charity;
			}
			LeftCharity = TotalCharity - GivenCharity;
		}

		private static List<Month> GetYearMonths(DateTime year)
		{
			return MonthsList.FindAll(x => x.year == year.Year);
		}

		/// <summary>
		/// Updates Month calculations based on Transactions
		/// </summary>
		/// <param name="month"></param>
		/// <param name="updateMonthlyDaily"></param>
		public static void RefreshMonth(Month month, bool updateMonthlyDaily = false)
		{
			//get all year data
			List<Month> currentYear = MonthsList.Count > 12 ? GetYearMonths(DateTime.Now) : MonthsList;

			//reset month stats
			month.Expense = 0;
			month.Income = 0;
			month.Profit = 0;
			month.Charity = 0;

			//remove data month from global
			GivenCharity = 0;
			TotalIncome -= month.Income;
			TotalExpense -= month.Expense;
			TotalProfit -= month.Income + month.Expense + month.Charity;

			//refresh month data
			foreach (Transaction transaction in month.Transactions)
			{
				if (transaction.Category != null)
				{
					foreach (Checker checker in transaction.Category.Checkers)
					{
						if (checker.State)
						{
							if (checker.Name.StartsWith("Inc") && !checker.Name.StartsWith("Exc"))
							{
								if (checker.Name.EndsWith("Income"))
								{
									month.Income += transaction.Sum;
								}
								else if (checker.Name.EndsWith("Expense"))
								{
									month.Expense += transaction.Sum;
								}
								else if (checker.Name.EndsWith("Charity"))
								{
									month.Charity += transaction.Sum;
								}
							}
							else
							{
								if (checker.Name.EndsWith("Income"))
								{
									month.Income -= transaction.Sum;
								}
								else if (checker.Name.EndsWith("Expense"))
								{
									month.Expense -= transaction.Sum;
								}
								else if (checker.Name.EndsWith("Charity"))
								{
									month.Charity -= transaction.Sum;
								}
							}
						}
					}
				}
			}
			month.Profit = month.Income - month.Expense - month.Charity;

			//add data to yearly stats
			TotalIncome += month.Income;
			TotalExpense += month.Expense;
			GivenCharity += month.Charity;
			TotalProfit += month.Income - month.Expense - month.Charity;
			month.Ratio = month.Income / (month.Expense + month.Charity);

			if (updateMonthlyDaily)
			{
				//refresh global/monthly/daily stats
				MonthlyIncome = TotalIncome / currentYear.Count;
				MonthlyExpense = TotalExpense / currentYear.Count;

				DailyIncome = TotalIncome / TotalDays;
				DailyExpense = TotalExpense / TotalDays;
			}
		}

		public static void CreateLoadDataFile(string fileName)
		{
			try
			{
				Java.IO.File dir = Context.GetExternalFilesDir(null);
				InternalPath = dir.Path;
				if (!dir.Exists())
				{
					dir.Mkdirs();
				}
				//load local
				string path = Path.Combine(InternalPath, fileName + ".xml");
				dir = new Java.IO.File(path);
				switch (fileName)
				{
					case "Accounts":
						AccountsDataFile = new Java.IO.File(path);
						AccountsDataFilePath = path;
						if (!AccountsDataFile.Exists())
						{
							AccountsDataFile.CreateNewFile();
						}
						break;
					case "Filters":
						FiltersFile = new Java.IO.File(path);
						FiltersFilePath = path;
						if (!FiltersFile.Exists())
						{
							FiltersFile.CreateNewFile();
						}
						break;
					case "Notes":
						NotesFile = new Java.IO.File(path);
						NotesFilePath = path;
						if (!NotesFile.Exists())
						{
							NotesFile.CreateNewFile();
						}
						break;
					case "Cars":
						CarsFile = new Java.IO.File(path);
						CarsFilePath = path;
						if (!CarsFile.Exists())
						{
							CarsFile.CreateNewFile();
						}
						break;
					case "Tags":
						TagsFile = new Java.IO.File(path);
						TagsFilePath = path;
						if (!TagsFile.Exists())
						{
							TagsFile.CreateNewFile();
						}
						break;
					case "Categories":
						CategoriesFile = new Java.IO.File(path);
						CategoriesFilePath = path;
						if (!CategoriesFile.Exists())
						{
							CategoriesFile.CreateNewFile();
						}
						break;
				}
			}
			catch
			{
			}
		}

		#endregion

		#endregion

		#region Notes Stuff

		#region All Notes Forms & Controls

		#region Notes Top Layout


		#endregion

		#region GridViewNotes Form

		public static CheckBox chkGridNotesSelectionCheck;

		public static TextView txtGridNotesDescription;
		public static TextView txtGridNotesLastModified;

		public static ImageView imgGridNotesRemove;

		#endregion

		#region NotesMenu Form

		public static HorizontalScrollView layoutAddNewNote;

		public static ImageView imgAddNewNote;
		public static ImageView imgAddNewGoal;
		public static ImageView imgAddNewShopList;
		public static ImageView imgDoneNotes;
		public static ImageView imgRemoveNote;

		public static CheckBox chkNoteCheckAll;

		public static TextView txtNoNotes;

		public static GridView grdNotes;

		#endregion

		#region NoteMenu Form

		public static TextView txtNoteTotalSum;
		public static TextView txtNoteTitle;
		public static TextView txtNoteDescription;
		public static TextView textviewGoalLeft;
		public static TextView textviewGoalTotal;

		public static ProgressBar pbGoalProgress;

		public static Button btnNoteSettings;

		public static CheckBox chkAllMiniNotes;

		public static ImageView imgAddNewMiniNote;
		public static ImageView imgRemoveAllMiniNotes;

		public static GridView grdMiniNotes;

		public static LinearLayout LayoutGoalProgress;

		#endregion

		#region NoteSettings Form

		public static EditText txtNoteSettingsTitle;
		public static EditText txtNoteSettingsDescription;
		public static EditText txtNoteSettingsGoalTotal;

		public static TextView txtNoteSettingsLastModified;
		public static TextView txtNoteSettingsCreatedDate;
		public static TextView txtviewNoteSettingsGoalSum;

		public static Button btnChangeNoteSettings;

		public static CheckBox chkHasTotal;
		public static CheckBox chkHasMiniNotes;

		#endregion

		#region MiniNote Menu Form

		public static LinearLayout layoutMiniNoteSum;
		public static LinearLayout layoutAddMiniNote;

		public static EditText txtMiniNoteSum;
		public static EditText txtMiniNoteDescription;

		public static TextView textviewMiniNoteAlert;

		public static Button btnModifyMiniNote;
		public static Button btnAddMiniNote;
		public static Button btnAddAnotherMiniNote;

		#endregion

		#endregion

		#endregion

		#region Cars Stuff

		#region All Cars Forms & Controls

		#region CarsMenu

		public static LinearLayout layoutAddNewCar;

		public static ImageView imgAddNewCar;
		public static ImageView imgDoneCars;
		public static ImageView imgRemoveCar;

		public static CheckBox chkCarCheckAll;

		public static TextView txtNoCars;

		public static GridView grdCars;

		#endregion

		#region CarMenu

		public static TextView txtCarProperties;
		public static TextView txtNoCarObjectsOrRepairs;
		public static TextView txtNoCarStuffs;

		public static Button btnCarProperties;
		public static Button btnCarConsumption;
		public static Button btnShowCarObjects;
		public static Button btnCarRepairHistory;
		public static Button btnCarTransactions;
		public static Button btnCarTags;

		public static ImageView imgAddNewObject;

		public static GridView grdCarObjectsOrRepairs;
		public static GridView grdCarStuffs;

		public static LinearLayout layoutCarCarObjectsRepairs;
		public static LinearLayout layoutCarStuffs;

		#endregion

		#region CarObject

		public static EditText txtCarObjectName;
		public static EditText txtCarObjectBeginDate;
		public static EditText txtCarObjectEndDate;

		public static Button btnAddOrModifyCarObject;
		public static Button btnCarObjectBeginDate;
		public static Button btnCarObjectEndDate;

		#endregion

		#region CarConsumption

		public static GridView grdCarConsumption;

		public static Button btnConsumFilter;
		public static Button btnConsumClearFilter;
		public static Button btnConsumSort;

		public static TextView txtCarConsumptionFilters;
		public static TextView txtCarConsumptionSorts;
		public static TextView txtTotalLiters;
		public static TextView txtTotalKilometersRan;
		public static TextView txtMediumConsumption;
		public static TextView txtMediumPricePerLiter;
		public static TextView txtMediumPricePerKilometer;
		public static TextView txtSince;

		#endregion

		#region CarPropertiesMenu

		public static EditText txtAddCarBrand;
		public static EditText txtAddCarModel;
		public static EditText txtAddCarYear;
		public static EditText txtAddCarPlate;

		public static Button btnAddOrModifyCarProperties;

		#endregion

		#region RepairMenu

		public static Button btnCarRepairDate;
		public static Button btnAddOrModifyCarRepair;

		public static TextView textviewCarRepairName;
		public static TextView textviewCarRepairDescription;
		public static TextView textviewCarRepairKilometers;
		public static TextView textviewCarRepairSum;

		public static EditText txtCarRepairName;
		public static EditText txtCarRepairDescription;
		public static EditText txtCarRepairDate;
		public static EditText txtCarRepairKilometers;
		public static EditText txtCarRepairSum;

		#endregion

		#endregion

		#endregion

		#region Tags Stuff

		#region All Tags Forms & Controls

		#region TagsMenu

		public static LinearLayout layoutAddNewTag;

		public static ImageView imgAddNewTag;
		public static ImageView imgDoneTags;
		public static ImageView imgRemoveTag;

		public static CheckBox chkTagsCheckAll;

		public static TextView txtNoTags;

		/// <summary>
		/// Shared with grdCarTransactionsOrTags in TagsAdapter
		/// </summary>
		public static GridView grdTags;

		#endregion

		#region TagMenu

		public static EditText txtTagName;

		public static Button btnSetTagColor;
		public static Button btnAddNoteToTag;
		public static Button btnAddCarToTag;
		public static Button btnAddOrModifyTag;

		public static TextView txtChoosenNotesToTag;
		public static TextView txtChoosenCarsToTag;
		public static TextView txtTagColor;

		#endregion

		#endregion

		#endregion

		#region Categories Stuff

		#region All Categories Forms & Controls

		#region CategoriesMenu

		public static LinearLayout layoutAddNewCategory;

		public static ImageView imgAddNewCategory;
		public static ImageView imgDoneCategories;
		public static ImageView imgRemoveCategory;

		public static CheckBox chkCategoriesCheckAll;

		public static TextView txtNoCategories;

		public static GridView grdCategories;

		#endregion

		#region CategoryMenu

		public static TextView txtCategoryColor;
		public static TextView textviewCurrentPercentage;

		public static EditText txtCategoryName;
		public static EditText txtCustomCharity;

		public static Button btnSetCategoryColor;
		public static Button btnAddCategory;

		public static CheckBox chkIncIncome;
		public static CheckBox chkIncExpense;
		public static CheckBox chkIncCharity;
		public static CheckBox chkExcIncome;
		public static CheckBox chkExcExpense;
		public static CheckBox chkExcCharity;
		public static CheckBox chkStandardCharity;
		public static CheckBox chkCustomCharity;

		public static LinearLayout showCustomCharityCategory;

		#endregion

		#endregion

		#endregion
	}
}
