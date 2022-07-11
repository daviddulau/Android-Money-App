using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using static ManyManager.Resource;
using System;
using Android;
using Android.Content.PM;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using System.Collections.Generic;
using Android.Text.Style;
using System.Linq;
using SkiaSharp;
using System.Globalization;
using Android.Gms.Common;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Gms.Common.Apis;
using PerpetualEngine.Storage;
using Android.Text;
using AndroidX.Core.App;
using System.IO;
using System.Timers;

namespace ManyManager
{
    [Activity(Label = "Many Manager", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, GoogleApiClient.IOnConnectionFailedListener
    {
        #region All Forms With Events For Main Menues Part

        #region MainMenu

        public void MainMenu_Events()
        {
            try
            {
                XML.LoadUserData();
                Globals.ReLoadNotesAndCars();
            }
            catch (System.Exception ex)
            {
                Utils.ShowAlert("", ex.Message);
            }

            // Set up an intent so that tapping the notifications returns to this app:
            Intent intent = new Intent(this, typeof(MainActivity));

            // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

            // Instantiate the builder and set notification elements:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Globals.Context)
                .SetContentTitle("Sample Notification")
                .SetContentText("Hello World! This is my first notification!")
                .SetSmallIcon(Drawable.Add);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager = GetSystemService(NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = Resource.Layout.MainMenu;

            notificationManager.Notify(notificationId, notification);

            Globals.btnMoneyPart = FindViewById<Button>(Id.btnMoneyPart);
            Globals.btnNotesPart = FindViewById<Button>(Id.btnNotesPart);
            Globals.btnCarsPart = FindViewById<Button>(Id.btnCarsPart);
            Globals.btnTagsPart = FindViewById<Button>(Id.btnTagsPart);
            Globals.btnCategoriesPart = FindViewById<Button>(Id.btnCategoriesPart);
            Globals.btnQuickAddTransaction = FindViewById<Google.Android.Material.FloatingActionButton.FloatingActionButton>(Id.btnQuickAddTransaction);

            Globals.btnMoneyPart.Click -= BtnMainMenuNavigation_Click;
            Globals.btnMoneyPart.Click += BtnMainMenuNavigation_Click;

            Globals.btnNotesPart.Click -= BtnMainMenuNavigation_Click;
            Globals.btnNotesPart.Click += BtnMainMenuNavigation_Click;

            Globals.btnCarsPart.Click -= BtnMainMenuNavigation_Click;
            Globals.btnCarsPart.Click += BtnMainMenuNavigation_Click;

            Globals.btnTagsPart.Click -= BtnMainMenuNavigation_Click;
            Globals.btnTagsPart.Click += BtnMainMenuNavigation_Click;

            Globals.btnCategoriesPart.Click -= BtnMainMenuNavigation_Click;
            Globals.btnCategoriesPart.Click += BtnMainMenuNavigation_Click;

            Globals.btnQuickAddTransaction.Click -= BtnMainMenuNavigation_Click;
            Globals.btnQuickAddTransaction.Click += BtnMainMenuNavigation_Click;
        }

        private void BtnMainMenuNavigation_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn == Globals.btnMoneyPart)
                {
                    SetContentView(Globals.YearlyStats);
                }
                else if (btn == Globals.btnNotesPart)
                {
                    SetContentView(Globals.NotesMenu);
                }
                else if (btn == Globals.btnCarsPart)
                {
                    SetContentView(Globals.CarsMenu);
                }
                else if (btn == Globals.btnTagsPart)
                {
                    SetContentView(Globals.TagsMenu);
                }
                else if (btn == Globals.btnCategoriesPart)
                {
                    SetContentView(Globals.CategoriesMenu);
                }
            }
            else if (sender == Globals.btnQuickAddTransaction)
            {
                SetContentView(Globals.TransactionMenu);
            }
        }

        #endregion

        #endregion

        #region All Forms With Events For Accounts Part

        #region AccountsMenu

        public void AccountsMenu_Events()
        {
            try
            {
                RequestPermissions();
                XML.LoadAccountsData();
            }
            catch (System.Exception ex)
            {

            }

            Globals.imgAddNewAccount = FindViewById<ImageView>(Id.imgAddNewAccount);

            Globals.grdAccounts = FindViewById<GridView>(Id.grdAccounts);

            Globals.imgAddNewAccount.Click -= ImgAddNewAccount_Click;
            Globals.imgAddNewAccount.Click += ImgAddNewAccount_Click;

            Globals.grdAccounts.Adapter = new AccountsAdapter(Globals.Activity, AccountsGlobals.AccountsList);

            AccountsGlobals.NewAccountWasAdded = false;
        }

        private void ImgAddNewAccount_Click(object sender, EventArgs e)
        {
            SetContentView(Globals.AccountMenu);
            Globals.PreviousForm = Globals.AccountsMenu;
        }

        #endregion

        #region AccountMenu

        public void AccountMenu_Events()
        {
            Globals.txtAccountName = FindViewById<EditText>(Id.txtAccountName);

            Globals.btnAddOrModifyAccount = FindViewById<Button>(Id.btnAddOrModifyAccount);

            Globals.btnAddOrModifyAccount.Click -= BtnAddOrModifyAccount_Click;
            Globals.btnAddOrModifyAccount.Click += BtnAddOrModifyAccount_Click;

            if (AccountsGlobals.NewAccountWasAdded)
            {
                Globals.txtAccountName.Text = AccountsGlobals.SelectedAccount.Name;
            }
            else
            {
                Globals.txtAccountName.Text = string.Empty;
            }

            Globals.btnAddOrModifyAccount.Text = AccountsGlobals.NewAccountWasAdded ? "Modify" : "Add New Account";

            ShowInput(Globals.txtAccountName);
        }

        private void BtnAddOrModifyAccount_Click(object sender, EventArgs e)
        {
            string name = Globals.txtAccountName.Text.ToUpper();
            if (!AccountsGlobals.NewAccountWasAdded)
            {
                //add new

                if (string.IsNullOrWhiteSpace(name))
                {
                    ShowInput(Globals.txtAccountName);
                    Utils.ShowAlert("", "Account Name Can't Be Empty");
                    return;
                }
                //check if name exists already - should be unique
                if (-1 != AccountsGlobals.AccountsList.FindIndex(x => x.Name.ToUpper() == name))
                {
                    ShowInput(Globals.txtAccountName);
                    Utils.ShowAlert("", "Account Name Already Exists");
                    return;
                }

                AccountsGlobals.AccountsList.Add(new Account(name, Path.Combine(Globals.InternalPath, name, name + ".xml")));
                XML.RefreshAccounts();
                Utils.ShowToast("Account: " + name + " Was Added!");
                AccountsGlobals.NewAccountWasAdded = true;
                SetContentView(Globals.PreviousForm);
            }
            else
            {
                bool modified = false;
                bool nameModified = false;
                if (string.IsNullOrWhiteSpace(name))
                {
                    ShowInput(Globals.txtAccountName);
                    Utils.ShowAlert("", "Account Name Can't Be Empty");
                    return;
                }

                if (name != AccountsGlobals.SelectedAccount.Name)
                {
                    //modify filename and path acordingly to new name
                    modified = true;
                    nameModified = true;
                    //check if name exists already - should be unique
                    if (-1 != AccountsGlobals.AccountsList.FindIndex(x => x.Name == name))
                    {
                        ShowInput(Globals.txtAccountName);
                        Utils.ShowAlert("", "Account Name Already Exists");
                        return;
                    }
                }

                if (modified)
                {
                    int index = AccountsGlobals.AccountIndexInList(AccountsGlobals.AccountsList, AccountsGlobals.SelectedAccount);
                    if (index != -1)
                    {
                        AccountsGlobals.AccountsList[index] = AccountsGlobals.SelectedAccount;
                        if (nameModified)
                        {
                            XML.RefreshAccount(AccountsGlobals.AccountsList[index], name);
                        }
                        XML.RefreshAccounts();
                        Utils.ShowToast("Account: " + name + " Was Modified!");
                        AccountsGlobals.NewAccountWasAdded = true;
                        SetContentView(Globals.PreviousForm);
                    }
                }
                else
                {
                    SetContentView(Globals.PreviousForm);
                }
            }
        }

        #endregion

        #endregion

        #region All Forms With Events For Settings Part

        #region Google Login

        const int RC_SIGN_IN = 9001;

        GoogleApiClient mGoogleApiClient;

        public void UpdateUI(bool isSignedIn)
        {
            if (isSignedIn)
            {
                if (Globals.CurrentForm == Globals.ChangeLogin)
                {
                    FindViewById(Id.GoogleSignInButton).Visibility = ViewStates.Gone;
                    FindViewById(Id.LayoutGoogleSignOutDisconnect).Visibility = ViewStates.Visible;
                }
            }
            else
            {
                Utils.ShowToast("Signed Out Google");
                if (Globals.CurrentForm == Globals.ChangeLogin)
                {
                    FindViewById(Id.GoogleSignInButton).Visibility = ViewStates.Visible;
                    FindViewById(Id.LayoutGoogleSignOutDisconnect).Visibility = ViewStates.Gone;
                }
            }
        }

        private void GoogleLoginStart()
        {
            var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
                var result = opr.Get() as GoogleSignInResult;
                HandleSignInResult(result);
            }
            else
            {
                Globals.ShowProgressDialog();
                opr.SetResultCallback(new SignInResultCallback());
            }
        }

        public void GoogleSign(object sender, EventArgs e)
        {
            if (sender == Globals.GoogleSignInButton)
            {
                SignInButton_Click();
            }
            else if (sender is Button btn)
            {
                if (btn == Globals.GoogleSignOutButton)
                {
                    SignOutButton_Click();
                }
                else if (btn == Globals.GoogleDisconnectButton)
                {
                    DisconnectButton_Click();
                }
            }
        }

        public void SignInButton_Click()
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, RC_SIGN_IN);
        }

        public void SignOutButton_Click()
        {
            Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(new SignOutResultCallback());
        }

        public void DisconnectButton_Click()
        {
            Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient).SetResultCallback(new SignOutResultCallback());
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
                Utils.ShowToast("Signed in as: " + acct.DisplayName);
                UpdateUI(true);
            }
            else
            {
                // Signed out, show unauthenticated UI.
                UpdateUI(false);
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            // An unresolvable error has occurred and Google APIs (including Sign-In) will not
            // be available.
        }

        #endregion

        #region ChangeLogin

        public void ChangeLogin_Events()
        {
            GoogleLoginStart();
            Globals.GoogleSignInButton = FindViewById<SignInButton>(Id.GoogleSignInButton);
            Globals.GoogleSignOutButton = FindViewById<Button>(Id.GoogleSignOutButton);
            Globals.GoogleDisconnectButton = FindViewById<Button>(Id.GoogleDisconnectButton);

            Globals.swtSetPIN = FindViewById<Switch>(Id.swtSetPIN);
            Globals.swtSetLocalSecurity = FindViewById<Switch>(Id.swtSetLocalSecurity);

            Globals.swtSetPIN.Checked = Globals.CacheStorage.Get<bool>("HasPIN");
            Globals.swtSetPIN.CheckedChange += SwtSetPIN_CheckedChange;

            Globals.swtSetLocalSecurity.Checked = Globals.CacheStorage.Get<bool>("LocalSeciruty");
            Globals.swtSetLocalSecurity.CheckedChange += SwtSetPIN_CheckedChange;

            Globals.GoogleSignInButton.SetSize(SignInButton.SizeStandard);
            Globals.GoogleSignInButton.Click -= GoogleSign;
            Globals.GoogleSignInButton.Click += GoogleSign;

            Globals.GoogleSignOutButton.Click -= GoogleSign;
            Globals.GoogleSignOutButton.Click += GoogleSign;

            Globals.GoogleDisconnectButton.Click -= GoogleSign;
            Globals.GoogleDisconnectButton.Click += GoogleSign;
        }

        private void SwtSetPIN_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (sender is Switch swt)
            {
                if (swt == Globals.swtSetPIN)
                {
                    Globals.CacheStorage.Put("HasPIN", e.IsChecked);
                    if (e.IsChecked)
                    {
                        Globals.SetPIN = true;
                        SetContentView(Globals.AutoLogin);
                        Globals.PreviousForm = Globals.ChangeLogin;
                    }
                    else
                    {
                        Globals.CacheStorage.Put("PIN", string.Empty);
                    }
                }
                else if (swt == Globals.swtSetLocalSecurity)
                {
                    if (e.IsChecked)
                    {
                        Globals.KeyguardManager = (KeyguardManager)GetSystemService(KeyguardService);
                        if (Globals.KeyguardManager != null)
                        {
                            Globals.CacheStorage.Put("LocalSecurity", Globals.KeyguardManager.IsKeyguardSecure);
                        }
                        else
                        {
                            Globals.swtSetLocalSecurity.Checked = false;

                        }
                    }
                    else
                    {
                        Globals.CacheStorage.Put("LocalSecurity", false);
                    }

                }
            }
        }

        #endregion

        #region AutoLogin

        public void AutoLogin_Events()
        {
            Globals.btnNumericRemoveOne = FindViewById<ImageView>(Id.btnNumericRemoveOne);

            Globals.btnNumeric0 = FindViewById<Button>(Id.btnNumeric0);
            Globals.btnNumeric1 = FindViewById<Button>(Id.btnNumeric1);
            Globals.btnNumeric2 = FindViewById<Button>(Id.btnNumeric2);
            Globals.btnNumeric3 = FindViewById<Button>(Id.btnNumeric3);
            Globals.btnNumeric4 = FindViewById<Button>(Id.btnNumeric4);
            Globals.btnNumeric5 = FindViewById<Button>(Id.btnNumeric5);
            Globals.btnNumeric6 = FindViewById<Button>(Id.btnNumeric6);
            Globals.btnNumeric7 = FindViewById<Button>(Id.btnNumeric7);
            Globals.btnNumeric8 = FindViewById<Button>(Id.btnNumeric8);
            Globals.btnNumeric9 = FindViewById<Button>(Id.btnNumeric9);

            Globals.txtShowPIN = FindViewById<TextView>(Id.txtShowPIN);

            Globals.btnNumericRemoveOne.Click -= BtnNumericRemove_Click;
            Globals.btnNumericRemoveOne.Click += BtnNumericRemove_Click;

            Globals.btnNumeric0.Click -= BtnNumeric_Click;
            Globals.btnNumeric0.Click += BtnNumeric_Click;

            Globals.btnNumeric1.Click -= BtnNumeric_Click;
            Globals.btnNumeric1.Click += BtnNumeric_Click;

            Globals.btnNumeric2.Click -= BtnNumeric_Click;
            Globals.btnNumeric2.Click += BtnNumeric_Click;

            Globals.btnNumeric3.Click -= BtnNumeric_Click;
            Globals.btnNumeric3.Click += BtnNumeric_Click;

            Globals.btnNumeric4.Click -= BtnNumeric_Click;
            Globals.btnNumeric4.Click += BtnNumeric_Click;

            Globals.btnNumeric5.Click -= BtnNumeric_Click;
            Globals.btnNumeric5.Click += BtnNumeric_Click;

            Globals.btnNumeric6.Click -= BtnNumeric_Click;
            Globals.btnNumeric6.Click += BtnNumeric_Click;

            Globals.btnNumeric7.Click -= BtnNumeric_Click;
            Globals.btnNumeric7.Click += BtnNumeric_Click;

            Globals.btnNumeric8.Click -= BtnNumeric_Click;
            Globals.btnNumeric8.Click += BtnNumeric_Click;

            Globals.btnNumeric9.Click -= BtnNumeric_Click;
            Globals.btnNumeric9.Click += BtnNumeric_Click;

            Globals.txtShowPIN.Text = string.Empty;
            Globals.txtShowPIN.AfterTextChanged -= TxtShowPIN_AfterTextChanged;
            Globals.txtShowPIN.AfterTextChanged += TxtShowPIN_AfterTextChanged;

        }

        private void TxtShowPIN_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            if (Globals.txtShowPIN.Text.Length == 4)
            {
                if (Globals.SetPIN)
                {
                    Globals.CacheStorage.Put<string>("PIN", Globals.txtShowPIN.Text);
                    Globals.SetPIN = false;
                    Utils.ShowToast("New PIN Saved");
                    SetContentView(Globals.PreviousForm);
                }
                else
                {
                    string password = Globals.CacheStorage.Get<string>("PIN");
                    if (password == null || password == string.Empty || (password != null && password == Globals.txtShowPIN.Text))
                    {
                        Globals.ShowProgressDialog();
                        if (password == null || password == string.Empty)
                        {
                            Globals.CacheStorage.Put<bool>("HasPIN", false);
                            Globals.CacheStorage.Put<string>("PIN", string.Empty);
                        }
                        SetContentView(Globals.AccountsMenu);
                        Globals.HideProgressDialog();
                    }
                    else
                    {
                        Utils.ShowToast("Invalid PIN");
                        Globals.txtShowPIN.Text = string.Empty;
                    }
                }
            }
        }

        private void BtnNumericRemove_Click(object sender, EventArgs e)
        {
            if (sender is ImageView btn)
            {
                if (btn == Globals.btnNumericRemoveOne)
                {
                    if (Globals.txtShowPIN.Text.Length > 0)
                    {
                        Globals.txtShowPIN.Text = Globals.txtShowPIN.Text.Substring(0, Globals.txtShowPIN.Text.Length - 1);
                    }
                }
            }
        }

        private void BtnNumeric_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                Globals.txtShowPIN.Text += btn.Text;
                Globals.btnNumericRemoveOne.Visibility = Globals.txtShowPIN.Text.Length > 0 ? ViewStates.Visible : ViewStates.Invisible;
            }
        }

        #endregion

        #region Settings

        public void Settings_Events()
        {
            Globals.externalSwitch = FindViewById<Switch>(Id.ExternalSwitch);

            Globals.btnExportToCSV = FindViewById<Button>(Id.btnExportToCSV);
            Globals.btnImportToCSV = FindViewById<Button>(Id.btnImportToCSV);
            Globals.btnGoToChangeCharity = FindViewById<Button>(Id.btnGoToChangeCharity);
            Globals.btnGoToChangeCategoryOrTagColor = FindViewById<Button>(Id.btnGoToChangeCategoryOrTagColor);
            Globals.btnGoToChangeLogin = FindViewById<Button>(Id.btnGoToChangeLogin);
            Globals.btnGoToChangeUnitsType = FindViewById<Button>(Id.btnGoToChangeUnitsType);

            Globals.externalSwitch.Checked = Globals.internalStorage;
            Globals.externalSwitch.Text = "Data Store: " + (Globals.internalStorage ? "Internal" : "External");

            Globals.externalSwitch.CheckedChange -= ExternalSwitch_CheckedChange;
            Globals.externalSwitch.CheckedChange += ExternalSwitch_CheckedChange;

            Globals.btnExportToCSV.Click -= btnImportExportToCSV_Click;
            Globals.btnExportToCSV.Click += btnImportExportToCSV_Click;

            Globals.btnImportToCSV.Click -= btnImportExportToCSV_Click;
            Globals.btnImportToCSV.Click += btnImportExportToCSV_Click;

            Globals.btnGoToChangeCharity.Click -= BtnGoToChange_Click;
            Globals.btnGoToChangeCharity.Click += BtnGoToChange_Click;

            Globals.btnGoToChangeCategoryOrTagColor.Click -= BtnGoToChange_Click;
            Globals.btnGoToChangeCategoryOrTagColor.Click += BtnGoToChange_Click;

            Globals.btnGoToChangeLogin.Click -= BtnGoToChange_Click;
            Globals.btnGoToChangeLogin.Click += BtnGoToChange_Click;

            Globals.btnGoToChangeUnitsType.Click -= BtnGoToChange_Click;
            Globals.btnGoToChangeUnitsType.Click += BtnGoToChange_Click;
        }

        private void ExternalSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Globals.externalSwitch.Text = "Data Store: " + (Globals.internalStorage ? "Internal" : "External");

            if (!Globals.inSwitchChanged)
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
                alert.SetTitle("Change Storage Location");
                alert.SetMessage("Are You Sure You Want To Change to " + (!Globals.internalStorage ? "Internal" : "External") + " Storage?");
                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    XML.SaveUserStoragePreference(true);
                    Globals.internalStorage = Globals.externalSwitch.Checked;
                    Globals.externalSwitch.Text = "Data Store: " + (Globals.internalStorage ? "Internal" : "External");
                    Utils.ShowAlert("Success", "Location Changed Successfully!");
                    Globals.inSwitchChanged = false;
                });
                alert.SetNegativeButton("No", (senderAlert, args) =>
                {
                    Globals.externalSwitch.Checked = Globals.internalStorage;
                    Globals.inSwitchChanged = false;
                });
                Dialog dialog = alert.Create();
                dialog.Show();
                Globals.inSwitchChanged = true;
            }
        }

        private void btnImportExportToCSV_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn == Globals.btnImportToCSV)
                {
                    CSV.ImportData();
                }
                else if (btn == Globals.btnExportToCSV)
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
                    alert.SetTitle("Export");
                    alert.SetMessage("Are You Sure You Want To Export Data?");
                    alert.SetPositiveButton("Yes", (senderAlert, args) =>
                    {
                        CSV.ExportData(Globals.MonthsList);
                    });
                    alert.SetNegativeButton("No", (senderAlert, args) =>
                    {
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            }
        }

        private void BtnGoToChange_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn == Globals.btnGoToChangeCharity)
                {
                    SetContentView(Globals.ChangeCharityPercentage);
                }
                else if (btn == Globals.btnGoToChangeCategoryOrTagColor)
                {
                    SetContentView(Globals.ChangeCategoryOrTagColor);
                }
                else if (btn == Globals.btnGoToChangeLogin)
                {
                    SetContentView(Globals.ChangeLogin);
                }
                else if (btn == Globals.btnGoToChangeUnitsType)
                {
                    SetContentView(Globals.ChangeUnitsType);
                }
            }
        }

        #endregion

        #region ChangeCharityPercentage

        private void ChangeCharityPercentage_Events()
        {
            Globals.btnNewZecAdd = FindViewById<Button>(Id.btnNewZecAdd);

            Globals.txtNewZec = FindViewById<EditText>(Id.txtNewZec);

            Globals.spinnerZecPeriodChangeChooser = FindViewById<Spinner>(Id.spinnerZecPeriodChangeChooser);

            Globals.grdYearZecPercentage = FindViewById<GridView>(Id.grdYearZecPercentage);

            string[] adapter = new string[12];
            for (int i = 1; i <= 12; i++)
            {
                DateTime dt = new DateTime(DateTime.Now.Year, i, 1);
                Month month = Globals.GetMonthFromDate(dt);
                adapter[i - 1] = dt.ToString("MMMM", CultureInfo.InvariantCulture) + " : " + month.CharityPercentage + "%";
            }
            ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
            Globals.grdYearZecPercentage.Adapter = arrayAdapter;

            Globals.spinnerZecPeriodChangeChooser.Adapter = StoreControlsData.CharityPeriodChangeChooserAdapter;
            Globals.spinnerZecPeriodChangeChooser.SetSelection(0);

            Globals.btnNewZecAdd.Click -= btnNewZecAdd_Click;
            Globals.btnNewZecAdd.Click += btnNewZecAdd_Click;
        }

        private void btnNewZecAdd_Click(object sender, EventArgs e)
        {
            double result = 0;
            if (!string.IsNullOrWhiteSpace(Globals.txtNewZec.Text) && double.TryParse(Globals.txtNewZec.Text, out result) && result >= 0)
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
                alert.SetTitle("Charity Percentage Change");
                alert.SetMessage("Are You Sure You Want To Change Charity Percentage To " + result + "% ?");
                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    Month month;
                    DateTime dt;
                    switch (Globals.spinnerZecPeriodChangeChooser.SelectedItem.ToString())
                    {
                        case "Last Month":
                            month = Globals.GetMonthFromDate(DateTime.Now.AddMonths(-1));
                            month.CharityPercentage = result;
                            Globals.RefreshYear();
                            XML.SaveMonth(month);
                            break;
                        case "Current Month":
                            month = Globals.GetMonthFromDate(DateTime.Now);
                            month.CharityPercentage = result;
                            Globals.RefreshYear();
                            XML.SaveMonth(month);
                            break;
                        case "Next Month":
                            month = Globals.GetMonthFromDate(DateTime.Now.AddMonths(1));
                            month.CharityPercentage = result;
                            Globals.RefreshYear();
                            XML.SaveMonth(month);
                            break;
                        case "Until End Of The Year":
                            for (int i = DateTime.Now.Month; i <= 12; i++)
                            {
                                dt = new DateTime(DateTime.Now.Year, i, 1);
                                month = Globals.GetMonthFromDate(dt);
                                month.CharityPercentage = result;
                                Globals.RefreshYear();
                                XML.SaveMonth(month);
                            }
                            break;
                        case "Entire Year":
                            for (int i = 1; i <= 12; i++)
                            {
                                dt = new DateTime(DateTime.Now.Year, i, 1);
                                month = Globals.GetMonthFromDate(dt);
                                month.CharityPercentage = result;
                                Globals.RefreshYear();
                                XML.SaveMonth(month);
                            }
                            break;
                        case "From Begin Of Year Until Now":
                            for (int i = 1; i <= DateTime.Now.Month; i++)
                            {
                                dt = new DateTime(DateTime.Now.Year, i, 1);
                                month = Globals.GetMonthFromDate(dt);
                                month.CharityPercentage = result;
                                Globals.RefreshYear();
                                XML.SaveMonth(month);
                            }
                            break;
                        default:
                            Utils.ShowAlert("Alert", "Can't Change. Something Went Wrong!");
                            break;
                    }
                    //refresh list with monthly percentages
                    string[] adapter = new string[12];
                    for (int i = 1; i <= 12; i++)
                    {
                        DateTime date = new DateTime(DateTime.Now.Year, i, 1);
                        Month mth = Globals.GetMonthFromDate(date);
                        adapter[i - 1] = date.ToString("MMMM", CultureInfo.InvariantCulture) + " : " + mth.CharityPercentage + "%";
                    }
                    ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, adapter);
                    Globals.grdYearZecPercentage.Adapter = arrayAdapter;

                    switch (Globals.spinnerZecPeriodChangeChooser.SelectedItem.ToString())
                    {
                        case "Last Month":
                            Utils.ShowAlert("Information", "Changed For Next Month");
                            break;
                        case "Current Month":
                            Utils.ShowAlert("Information", "Changed For Current Month");
                            break;
                        case "Next Month":
                            Utils.ShowAlert("Information", "Changed For Next Month");
                            break;
                        case "Until End Of The Year":
                            Utils.ShowAlert("Information", "Changed Until End Of The Year");
                            break;
                        case "Entire Year":
                            Utils.ShowAlert("Information", "Changed For Entire Year");
                            break;
                        case "From Begin Of Year Until Now":
                            Utils.ShowAlert("Information", "Changed From Begin Of Year Until Now");
                            break;
                    }
                });
                alert.SetNegativeButton("No", (senderAlert, args) =>
                {
                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                Utils.ShowAlert("No Number", "Charity Percentage Cannot Be Empty");
            }
            HideInput();
        }

        #endregion

        #region ChangeCategoryOrTagColor

        private void ChangeCategoryOrTagColor_Events()
        {
            Globals.btnSetCategoryOrTagColor = FindViewById<Button>(Id.btnSetCategoryOrTagColor);

            Globals.spinnerChooseCategoryOrTagTypeColor = FindViewById<Spinner>(Id.spinnerChooseCategoryOrTagTypeColor);
            Globals.spinnerChooseCategoryOrTagColor = FindViewById<Spinner>(Id.spinnerChooseCategoryOrTagColor);

            Globals.sKColorSelectedImage = FindViewById<ImageView>(Id.SKColorSelectedImage);

            Globals.sKColorFiltering = FindViewById<EditText>(Id.SKColorFiltering);

            Globals.sKColorSelectedName = FindViewById<TextView>(Id.SKColorSelectedName);

            Globals.grdSKColorImages = FindViewById<GridView>(Id.grdSKColorImages);

            Globals.btnSetCategoryOrTagColor.Click -= btnSetCategoryOrTagColor_Click;
            Globals.btnSetCategoryOrTagColor.Click += btnSetCategoryOrTagColor_Click;

            Globals.sKColorFiltering.TextChanged -= SKColorFiltering_TextChanged;
            Globals.sKColorFiltering.TextChanged += SKColorFiltering_TextChanged;

            Globals.spinnerChooseCategoryOrTagTypeColor.Adapter = StoreControlsData.CategoriesOrTagsAdapter;
            Globals.spinnerChooseCategoryOrTagTypeColor.SetSelection(0);
            Globals.spinnerChooseCategoryOrTagTypeColor.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
                List<string> listAdapter = new List<string>();
                switch (Globals.spinnerChooseCategoryOrTagTypeColor.SelectedItem.ToString())
                {
                    case "Categories":
                        listAdapter.AddRange(CategoriesGlobals.CategoriesNameList);
                        break;
                    case "Tags":
                        listAdapter.AddRange(TagsGlobals.TagsNameList);
                        break;
                }
                ArrayAdapter<string> spinnerArrayAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, listAdapter);
                spinnerArrayAdapter.SetDropDownViewResource(Drawable.spinner_item);
                Globals.spinnerChooseCategoryOrTagColor.Adapter = spinnerArrayAdapter;
                if (Globals.SelectedColorIndex != -1)
                {
                    Globals.spinnerChooseCategoryOrTagColor.SetSelection(Globals.SelectedColorIndex);
                    Globals.SelectedColorIndex = -1;
                }
            };

            Globals.spinnerChooseCategoryOrTagColor.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
                if (Globals.SKCategoriesOrTagColor.Count > 0)
                {
                    SelectColorInGrid(Globals.spinnerChooseCategoryOrTagColor.SelectedItem.ToString());
                }
            };

            Globals.ShowProgressDialog();

            Globals.grdSKColorImages.Adapter = new SKColorImageAdapter(Globals.Activity);

            Globals.HideProgressDialog();
        }

        /// <summary>
        /// Used to set selection in the grid of ChangeCategoryOrTagColor
        /// </summary>
        public void SelectColorInGrid(string name)
        {
            if (CTColor.GetColor(name) == SKColor.Empty)
            {
                //it will crash if it will enter at -1
                //(Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SetSelection(-1);
                //Globals.grdSKColorImages.SetSelectionFromTop(0, 0);
            }
            else
            {
                CTColor item = CTColor.GetElement(name);
                (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SetSelection(Globals.AllSKColors.IndexOf(item.Color));
                Globals.grdSKColorImages.SetSelectionFromTop((Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SelectedPosition, Globals.grdSKColorImages.Height / 2);
            }
        }

        private void btnSetCategoryOrTagColor_Click(object sender, EventArgs e)
        {
            try
            {
                SKColor colorValue = SKColor.Empty;
                string colorKey = "";
                if (Globals.PreviousForm == Globals.TagMenu && Globals.spinnerChooseCategoryOrTagColor.SelectedItem == null)
                {
                    Globals.SelectedColorForTag = (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SKColor;
                    SetContentView(Globals.PreviousForm);
                }
                else
                {
                    string selectedCategoryOrTagItem = Globals.spinnerChooseCategoryOrTagColor.SelectedItem.ToString();
                    bool HasCategory = CategoriesGlobals.CategoriesNameList.Contains(selectedCategoryOrTagItem);
                    colorValue = (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SKColor;
                    colorKey = (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SKColorString;
                    if (colorKey == null && colorValue == SKColor.Empty)
                    {
                        Utils.ShowToast("Please Select a Color");
                        return;
                    }
                    if ((Globals.SKCategoriesOrTagColor.Count == 0 && colorKey != null) || CTColor.ContainsKey(selectedCategoryOrTagItem) || CTColor.GetColor(selectedCategoryOrTagItem) != colorValue)
                    {
                        Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
                        alert.SetTitle("Change Color");
                        alert.SetMessage("Do You Want To Change Color for " + selectedCategoryOrTagItem + " to " + colorKey);
                        alert.SetPositiveButton("Yes", (senderAlert, args) =>
                        {
                            if (CTColor.ContainsKey(selectedCategoryOrTagItem))
                            {
                                CTColor.Remove(selectedCategoryOrTagItem);
                            }
                            CTColor.Add(selectedCategoryOrTagItem, colorValue, HasCategory);

                            if (Globals.PreviousForm == Globals.TagMenu)
                            {
                                Globals.SelectedColorForTag = (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SKColor;
                            }
                            else if (Globals.PreviousForm == Globals.CategoryMenu)
                            {
                                Globals.SelectedColorForCategory = (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SKColor;
                            }

							//todo change color in XML for all transactions with this category/tag
							if (HasCategory)
							{
							    foreach (Transaction item in MoneyGlobals.TransactionsList.Where(x=>x.Category != null))
							    {
                                    item.Color = Utils.CategoryOrTagToSKColor(item.Category.Name);
							    }
							}
							else
							{
                                foreach (Transaction item in MoneyGlobals.TransactionsList.Where(x => x.Tag != null))
                                {
                                    item.Color = Utils.CategoryOrTagToSKColor(item.Tag.Name);
                                }
                            }
                            XML.SaveYears();

                            Utils.ShowToast("Color Was Changed!");
                            SetContentView(Globals.PreviousForm);
                        });
                        alert.SetNegativeButton("No", (senderAlert, args) =>
                        {
                            if (Globals.PreviousForm == Globals.TagMenu || Globals.PreviousForm == Globals.CategoryMenu)
                            {
                                SetContentView(Globals.PreviousForm);
                            }
                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                    }
                }
            }
            catch (System.Exception ex) { }
        }

        private void SKColorFiltering_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).Filtering(e.Text.ToString());
        }

        #endregion

        #region ChangeUnitsType

        public void ChangeUnitsType_Events()
        {
            Globals.btnChangeUnits = FindViewById<Button>(Id.btnChangeUnits);

            Globals.spiUnitsType = FindViewById<Spinner>(Id.spiUnitsType);

            Globals.spiUnitsType.Adapter = StoreControlsData.UnitsTypeChangeAdapter;
            Globals.spiUnitsType.SetSelection((int)UnitType.Unit);

            Globals.btnChangeUnits.Click -= BtnChangeUnits_Click;
            Globals.btnChangeUnits.Click += BtnChangeUnits_Click;
        }

        private void BtnChangeUnits_Click(object sender, EventArgs e)
        {
            if (UnitType.Unit != (Unit)Globals.spiUnitsType.SelectedItemPosition)
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
                alert.SetTitle("");
                alert.SetMessage("By Doing This Will Convert All Your Existing Data To " + UnitType.GetUnitName((Unit)Globals.spiUnitsType.SelectedItemPosition) + ".Continue ?");
                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    UnitType.ChangeUnitType((Unit)Globals.spiUnitsType.SelectedItemPosition);
                    UnitType.ConvertToUnit((Unit)Globals.spiUnitsType.SelectedItemPosition);
                    XML.ReLoadUnitType();
                    Utils.ShowToast("Changed to " + UnitType.GetUnitName(UnitType.Unit) + " Units System");
                    SetContentView(Globals.ParentForm);
                });
                alert.SetNegativeButton("No", (senderAlert, args) =>
                {
                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        #endregion

        #endregion

        #region All Forms Controls With Events For Money Part

        #region YearlyStats

        public void YearlyStats_Events()
        {
            Globals.txtRatio = FindViewById<TextView>(Id.txtRatio);
            Globals.txtIncome = FindViewById<TextView>(Id.txtIncome);
            Globals.txtExpense = FindViewById<TextView>(Id.txtExpense);
            Globals.txtProfit = FindViewById<TextView>(Id.txtProfit);
            Globals.txtZec = FindViewById<TextView>(Id.txtZec);
            Globals.txtTotalDays = FindViewById<TextView>(Id.txtTotalDays);
            Globals.txtLeftZec = FindViewById<TextView>(Id.txtLeftZec);
            Globals.txtYearlyMonthlyIncome = FindViewById<TextView>(Id.txtYearlyMonthlyIncome);
            Globals.txtYearlyMonthlyExpense = FindViewById<TextView>(Id.txtYearlyMonthlyExpense);
            Globals.txtDailyIncome = FindViewById<TextView>(Id.txtDailyIncome);
            Globals.txtDailyExpense = FindViewById<TextView>(Id.txtDailyExpense);

            Globals.layoutTextStats = FindViewById<LinearLayout>(Id.layoutTextStats);

            YearlyStats_Populate();
        }

        public void YearlyStats_Populate()
        {
            //recalculate all year
            Globals.RefreshYear();

            //populate globals
            Globals.txtRatio.Text = "Ratio: " + Globals.TotalRatio.ToString("#,##0.0000");
            Globals.txtIncome.Text = "Income: " + Globals.TotalIncome.ToString("#,##0.00");
            Globals.txtExpense.Text = "Expense: " + Globals.TotalExpense.ToString("#,##0.00");
            Globals.txtProfit.Text = "Profit: " + Globals.TotalProfit.ToString("#,##0.00");
            Globals.txtZec.Text = "Charity: " + Globals.GivenCharity.ToString("#,##0.00");
            Globals.txtTotalDays.Text = "TotalDays: " + Globals.TotalDays.ToString("#,##0.00");
            Globals.txtLeftZec.Text = "Left Charity: " + Globals.LeftCharity.ToString("#,##0.00");
            Globals.txtYearlyMonthlyIncome.Text = "Income: " + Globals.MonthlyIncome.ToString("#,##0.00");
            Globals.txtYearlyMonthlyExpense.Text = "Expense: " + Globals.MonthlyExpense.ToString("#,##0.00");
            Globals.txtDailyIncome.Text = "Income: " + Globals.DailyIncome.ToString("#,##0.00");
            Globals.txtDailyExpense.Text = "Expense: " + Globals.DailyExpense.ToString("#,##0.00");
        }

        #endregion

        #region Filters

        public void Filters_Events()
        {
            Globals.btnFilterThisMonth = FindViewById<Button>(Id.btnFilterThisMonth);
            Globals.btnFilterLastMonth = FindViewById<Button>(Id.btnFilterLastMonth);
            Globals.btnFilterLast3Months = FindViewById<Button>(Id.btnFilterLast3Months);
            Globals.btnFilterThisYear = FindViewById<Button>(Id.btnFilterThisYear);
            Globals.btnFilterLastYear = FindViewById<Button>(Id.btnFilterLastYear);
            Globals.btnFilterAllTime = FindViewById<Button>(Id.btnFilterAllTime);
            Globals.btnFilterListDisplay = FindViewById<Button>(Id.btnFilterListDisplay);
            Globals.btnFilterChartDisplay = FindViewById<Button>(Id.btnFilterChartDisplay);
            Globals.btnFilterGridDisplay = FindViewById<Button>(Id.btnFilterGridDisplay);
            Globals.btnFilterFilter = FindViewById<Button>(Id.btnFilterFilter);
            Globals.btnNewFilterAdd = FindViewById<Button>(Id.btnNewFilterAdd);
            Globals.btnNewFilter = FindViewById<Button>(Id.btnNewFilter);

            Globals.btnFilterBeginDate = FindViewById<ImageButton>(Id.btnFilterBeginDate);
            Globals.btnFilterEndDate = FindViewById<ImageButton>(Id.btnFilterEndDate);

            Globals.imgCloseNewFilter = FindViewById<ImageView>(Id.imgCloseNewFilter);

            Globals.txtFilterBeginDate = FindViewById<TextView>(Id.txtFilterBeginDate);
            Globals.txtFilterEndDate = FindViewById<TextView>(Id.txtFilterEndDate);

            Globals.txtFilterMaxSum = FindViewById<EditText>(Id.txtFilterMaxSum);
            Globals.txtFilterMinSum = FindViewById<EditText>(Id.txtFilterMinSum);
            Globals.txtNewFilterName = FindViewById<EditText>(Id.txtNewFilterName);

            Globals.layoutFilterDates = FindViewById<LinearLayout>(Id.LayoutFilterDates);
            Globals.layoutFilterDefinedDates = FindViewById<LinearLayout>(Id.LayoutFilterDefinedDates);
            Globals.layoutFilters = FindViewById<LinearLayout>(Id.LayoutFilters);
            Globals.layoutShowTransactions = FindViewById<LinearLayout>(Id.LayoutShowTransactions);
            Globals.layoutNewFilter = FindViewById<LinearLayout>(Id.LayoutNewFilter);

            Globals.showAllCategoriesOrTags = FindViewById<Switch>(Id.ShowAllCategoriesOrTags);

            Globals.grdUserFilters = FindViewById<GridView>(Id.grdUserFilters);

            Globals.spinnerCategoryOrTagFilter = FindViewById<Spinner>(Id.spinnerCategoryOrTagFilter);
            Globals.spinnerCategoryAndTagFilterChooser = FindViewById<Spinner>(Id.spinnerCategoryAndTagFilterChooser);

            Globals.showAllCategoriesOrTags.Checked = Filtering.ShowAllCategoriesOrTags;
            Globals.showAllCategoriesOrTags.CheckedChange -= ShowAllCategoriesOrTags_CheckedChange;
            Globals.showAllCategoriesOrTags.CheckedChange += ShowAllCategoriesOrTags_CheckedChange;

            Globals.spinnerCategoryAndTagFilterChooser.Adapter = StoreControlsData.SpinnerCategoryOrTagFilterChooserAdapter;
            Globals.spinnerCategoryAndTagFilterChooser.ItemSelected -= CategoryAndTagFilterChooser_ItemSelected;
            Globals.spinnerCategoryAndTagFilterChooser.ItemSelected += CategoryAndTagFilterChooser_ItemSelected;

            Globals.spinnerCategoryOrTagFilter.Adapter = StoreControlsData.SpinnerCategoryOrTagFilterAdapter;
            Globals.spinnerCategoryOrTagFilter.ItemSelected -= CategoryOrTagFilter_ItemSelected;
            Globals.spinnerCategoryOrTagFilter.ItemSelected += CategoryOrTagFilter_ItemSelected;

            Filtering.SpinnerToSet = -1;

            //get spinner to selected category or tag if one
            if (Filtering.CategoryOrTagFilter != "No Category;None")
            {
                Filtering.SetFilterSpinners();
            }
            else
            {
                Globals.spinnerCategoryAndTagFilterChooser.SetSelection(0);
                Globals.spinnerCategoryOrTagFilter.SetSelection(0);
            }

            if (Filtering.FilterList.Count > 5)
            {
                Globals.ShowProgressDialog();
            }
            Globals.grdUserFilters.Adapter = new UserFiltersAdapter(this, Filtering.FilterList);

            Globals.HideProgressDialog();

            Globals.txtFilterEndDate.Text = Filtering.EndDate.ToShortDateString();
            Globals.txtFilterBeginDate.Text = Filtering.BeginDate.ToShortDateString();

            Globals.btnFilterFilter.Click -= btnFilterFilter_Click;
            Globals.btnFilterFilter.Click += btnFilterFilter_Click;

            Globals.txtFilterMaxSum.Text = Filtering.MaxSum;
            Globals.txtFilterMaxSum.TextChanged -= txtFilterMaxSum_TextChanged;
            Globals.txtFilterMaxSum.TextChanged += txtFilterMaxSum_TextChanged;

            Globals.txtFilterMinSum.Text = Filtering.MinSum;
            Globals.txtFilterMinSum.TextChanged -= txtFilterMinSum_TextChanged;
            Globals.txtFilterMinSum.TextChanged += txtFilterMinSum_TextChanged;

            Globals.btnFilterThisMonth.Click -= BtnFilterMonth_Click;
            Globals.btnFilterThisMonth.Click += BtnFilterMonth_Click;

            Globals.btnFilterLastMonth.Click -= BtnFilterMonth_Click;
            Globals.btnFilterLastMonth.Click += BtnFilterMonth_Click;

            Globals.btnFilterLast3Months.Click -= BtnFilterMonth_Click;
            Globals.btnFilterLast3Months.Click += BtnFilterMonth_Click;

            Globals.btnFilterThisYear.Click -= BtnFilterMonth_Click;
            Globals.btnFilterThisYear.Click += BtnFilterMonth_Click;

            Globals.btnFilterLastYear.Click -= BtnFilterMonth_Click;
            Globals.btnFilterLastYear.Click += BtnFilterMonth_Click;

            Globals.btnFilterAllTime.Click -= BtnFilterMonth_Click;
            Globals.btnFilterAllTime.Click += BtnFilterMonth_Click;

            Globals.btnFilterListDisplay.Click -= FilterDisplayTransactionsBy_Click;
            Globals.btnFilterListDisplay.Click += FilterDisplayTransactionsBy_Click;

            Globals.btnFilterChartDisplay.Click -= FilterDisplayTransactionsBy_Click;
            Globals.btnFilterChartDisplay.Click += FilterDisplayTransactionsBy_Click;

            Globals.btnFilterGridDisplay.Click -= FilterDisplayTransactionsBy_Click;
            Globals.btnFilterGridDisplay.Click += FilterDisplayTransactionsBy_Click;

            Globals.btnFilterBeginDate.Click -= btnFilteryBeginDate_Click;
            Globals.btnFilterBeginDate.Click += btnFilteryBeginDate_Click;

            Globals.btnFilterEndDate.Click -= btnFilterEndDate_Click;
            Globals.btnFilterEndDate.Click += btnFilterEndDate_Click;

            Globals.imgCloseNewFilter.Click -= imgCloseNewFilter_Click;
            Globals.imgCloseNewFilter.Click += imgCloseNewFilter_Click;

            Globals.btnNewFilterAdd.Click -= btnNewFilterAdd_Click;
            Globals.btnNewFilterAdd.Click += btnNewFilterAdd_Click;

            Globals.btnNewFilter.Click -= btnNewFilter_Click;
            Globals.btnNewFilter.Click += btnNewFilter_Click;
        }

        private void CategoryOrTagFilter_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            Filtering.CategoryOrTagFilter = Globals.spinnerCategoryAndTagFilterChooser.SelectedItem.ToString() + ";" + Globals.spinnerCategoryOrTagFilter.SelectedItem.ToString();
        }

        private void CategoryAndTagFilterChooser_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            List<string> listAdapter = new List<string>();
            Globals.spinnerCategoryOrTagFilter.Visibility = ViewStates.Visible;
            switch (Globals.spinnerCategoryAndTagFilterChooser.SelectedItem.ToString())
            {
                case "No Category":
                    listAdapter.Add("None");
                    Globals.spinnerCategoryOrTagFilter.Visibility = ViewStates.Invisible;
                    break;
                case "Categories":
                    listAdapter.AddRange(CategoriesGlobals.CategoriesNameList);
                    break;
                case "Tags":
                    listAdapter.AddRange(TagsGlobals.TagsNameList);
                    break;
            }
            ArrayAdapter<string> spinnerArrayAdapter = new ArrayAdapter<string>(Globals.Context, Drawable.spinner_item, listAdapter);
            spinnerArrayAdapter.SetDropDownViewResource(Drawable.spinner_item);
            Globals.spinnerCategoryOrTagFilter.Adapter = spinnerArrayAdapter;
            Globals.spinnerCategoryOrTagFilter.SetSelection(Filtering.SpinnerToSet > -1 ? Filtering.SpinnerToSet : 0);
        }

        private void ShowAllCategoriesOrTags_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            Filtering.ShowAllCategoriesOrTags = e.IsChecked;
        }

        private void btnFilterFilter_Click(object sender, EventArgs e)
        {
            MoneyGlobals.GetSelectedCategoryOrTagFilter();
            MoneyGlobals.GetSelectedDatesFilter();
            MoneyGlobals.SetSumFilter();
            SetContentView(Globals.PreviousForm);
        }

        private void txtFilterMaxSum_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            Filtering.MaxSum = Globals.txtFilterMaxSum.Text;
            MoneyGlobals.SetSumFilter();
        }

        private void txtFilterMinSum_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            Filtering.MinSum = Globals.txtFilterMinSum.Text;
            MoneyGlobals.SetSumFilter();
        }

        private void BtnFilterMonth_Click(object sender, EventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            DateTime today = DateTime.Today;
            DateTime month = new DateTime(today.Year, today.Month, 1);
            Button btn = sender as Button;
            if (btn == Globals.btnFilterAllTime || sender.ToString() == "AllTime")
            {
                DateTime begin = DateTime.MaxValue;
                DateTime end = DateTime.MinValue;
                foreach (Transaction item in MoneyGlobals.TransactionsList)
                {
                    end = item.Date > end ? item.Date : end;
                    begin = item.Date < begin ? item.Date : begin;
                }
                Filtering.BeginDate = begin;
                Filtering.EndDate = end;
                if (sender.ToString() != "AllTime")
                {
                    Globals.txtFilterBeginDate.Text = begin.ToShortDateString();
                    Globals.txtFilterEndDate.Text = end.ToShortDateString();
                }
                //display the type of filter selected
                MoneyGlobals.SetSelectedDatesFilter(false, AllTime: true);
            }
            else if (btn == Globals.btnFilterLast3Months || sender.ToString() == "Last3Months")
            {
                if (sender.ToString() != "Last3Months")
                {
                    Globals.txtFilterBeginDate.Text = month.AddMonths(-3).ToShortDateString();
                    Globals.txtFilterEndDate.Text = month.AddDays(-1).ToShortDateString();
                }
                //display the type of filter selected
                MoneyGlobals.SetSelectedDatesFilter(false, Last3Months: true);
            }
            else if (btn == Globals.btnFilterLastMonth || sender.ToString() == "LastMonth")
            {
                if (sender.ToString() != "LastMonth")
                {
                    Globals.txtFilterBeginDate.Text = month.AddMonths(-1).ToShortDateString();
                    Globals.txtFilterEndDate.Text = month.AddDays(-1).ToShortDateString();
                }
                //display the type of filter selected
                MoneyGlobals.SetSelectedDatesFilter(false, true);
            }
            else if (btn == Globals.btnFilterThisMonth || sender.ToString() == "ThisMonth")
            {
                if (sender.ToString() != "ThisMonth")
                {
                    Globals.txtFilterBeginDate.Text = month.ToShortDateString();
                    Globals.txtFilterEndDate.Text = month.AddMonths(1).AddDays(-1).ToShortDateString();
                }
                MoneyGlobals.SetSelectedDatesFilter();
            }
            else if (btn == Globals.btnFilterThisYear || sender.ToString() == "ThisYear")
            {
                if (sender.ToString() != "ThisYear")
                {
                    month = new DateTime(today.Year, 1, 1);
                    Globals.txtFilterBeginDate.Text = month.ToShortDateString();
                    month = new DateTime(today.Year, 12, 31);
                    Globals.txtFilterEndDate.Text = month.ToShortDateString();
                }
                MoneyGlobals.SetSelectedDatesFilter(false, ThisYear: true);
            }
            else if (btn == Globals.btnFilterLastYear || sender.ToString() == "LastYear")
            {
                if (sender.ToString() != "LastYear")
                {
                    month = new DateTime(today.AddYears(-1).Year, 1, 1);
                    Globals.txtFilterBeginDate.Text = month.ToShortDateString();
                    month = new DateTime(today.AddYears(-1).Year, 12, 31);
                    Globals.txtFilterEndDate.Text = month.ToShortDateString();
                }
                MoneyGlobals.SetSelectedDatesFilter(false, LastYear: true);
            }
            MoneyGlobals.GetSelectedCategoryOrTagFilter();
        }

        private void FilterDisplayTransactionsBy_Click(object sender, EventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            if (sender is Button btn)
			{
				if (btn == Globals.btnFilterListDisplay)
				{
                    Filtering.TransactionDisplay = TransactionDisplay.List;
				}
                else if (btn == Globals.btnFilterGridDisplay)
                {
                    Filtering.TransactionDisplay = TransactionDisplay.Grid;
                }
                else if (btn == Globals.btnFilterChartDisplay)
                {
                    Filtering.TransactionDisplay = TransactionDisplay.Chart;
                }
            }
        }

        private void btnFilteryBeginDate_Click(object sender, EventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            Utils.Dpd_DateSet = DpdTransactionsFilterBeginDate_DateSet;
            Utils.DateSet = Globals.txtFilterBeginDate.Text;
            Utils.ShowDatePicker(Filtering.BeginDate);
        }

        private void DpdTransactionsFilterBeginDate_DateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Globals.txtFilterBeginDate.Text = new DateTime(e.Year, e.Month + 1, e.DayOfMonth).ToShortDateString();
            MoneyGlobals.SetSelectedDatesFilter(false, CustomDates: true);
        }

        private void btnFilterEndDate_Click(object sender, EventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            Utils.Dpd_DateSet = DpdTransactionsFilterEndDate_DateSet;
            Utils.DateSet = Globals.txtFilterEndDate.Text;
            Utils.ShowDatePicker(Filtering.EndDate);
        }

        private void DpdTransactionsFilterEndDate_DateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Globals.txtFilterEndDate.Text = new DateTime(e.Year, e.Month + 1, e.DayOfMonth).ToShortDateString();
            MoneyGlobals.SetSelectedDatesFilter(false, CustomDates: true);
        }

        private void imgCloseNewFilter_Click(object sender, EventArgs e)
        {
            Globals.btnNewFilter.Visibility = ViewStates.Visible;
            HideInput();
        }

        private void btnNewFilterAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Globals.txtNewFilterName.Text))
            {
                (Globals.grdUserFilters.Adapter as UserFiltersAdapter).AddFilter(Globals.txtNewFilterName.Text);
            }
            else
            {
                Utils.ShowAlert("No Name", "Filter Name Cannot Be Empty");
            }
            Globals.btnNewFilter.Visibility = ViewStates.Visible;
            HideInput();
        }

        private void btnNewFilter_Click(object sender, EventArgs e)
        {
            MoneyGlobals.ClearUserFilterSelection();
            Globals.btnNewFilter.Visibility = ViewStates.Invisible;
            Globals.txtNewFilterName.RequestFocus();
            ShowInput(Globals.txtNewFilterName);
        }

        #endregion

        #region Sorts

        public void Sorts_Events()
        {
            Globals.btnSortApply = FindViewById<Button>(Id.btnSortApply);
            Globals.spinnerTransactionsSortBy = FindViewById<Spinner>(Id.spinnerTransactionsSortBy);

            Globals.spinnerTransactionsSortBy.Adapter = StoreControlsData.SpinnerTransactionsSortByAdapter;
            Globals.spinnerTransactionsSortBy.SetSelection((int)Sorting.CurrentSort);

            Globals.spinnerTransactionsSortBy.ItemSelected -= SpinnerTransactionsSortBy_ItemSelected;
            Globals.spinnerTransactionsSortBy.ItemSelected += SpinnerTransactionsSortBy_ItemSelected;

            Globals.btnSortApply.Click -= BtnSortApply_Click;
            Globals.btnSortApply.Click += BtnSortApply_Click;
        }

        private void SpinnerTransactionsSortBy_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Sorting.CurrentSort = (SortBy)e.Position;
        }

        private void BtnSortApply_Click(object sender, EventArgs e)
        {
            SetContentView(Globals.PreviousForm);
        }

        #endregion

        #region TransactionsMenu

        public void TransactionsMenu_Events()
        {
            Globals.grdTransactions = FindViewById<GridView>(Id.grdTransactions);

            Globals.btnTransactionsFilter = FindViewById<Button>(Id.btnTransactionsFilter);
            Globals.btnTransactionsClearFilter = FindViewById<Button>(Id.btnTransactionsClearFilter);
            Globals.btnTransactionsSort = FindViewById<Button>(Id.btnTransactionsSort);

            Globals.txtTransactionsAppliedFilter = FindViewById<TextView>(Id.txtTransactionsAppliedFilter);
            Globals.txtTransactionsSort = FindViewById<TextView>(Id.txtTransactionsSort);

            Globals.tvwTransactionsNoData = FindViewById<TextView>(Id.tvwTransactionsNoData);
            Globals.layoutGridTransactions = FindViewById<RelativeLayout>(Id.LayoutGridTransactions);
            Globals.layoutCharts = FindViewById<RelativeLayout>(Id.LayoutCharts);

            Globals.elvGridTransactions = FindViewById<ExpandableListView>(Id.elvGridTransactions);

            Globals.chkAllTransactions = FindViewById<CheckBox>(Id.chkAllTransactions);

            Globals.imgRemoveAllTransactions = FindViewById<ImageView>(Id.imgRemoveAllTransactions);

            Globals.chooseCategoryOrTag = FindViewById<Spinner>(Id.ChooseCategoryOrTag);
            Globals.chooseDisplayChart = FindViewById<Spinner>(Id.ChooseDisplayChart);

            Globals.formLoading = true;
            Globals.chooseDisplayChart.Adapter = StoreControlsData.ChooseDisplayChartAdapter;
            Globals.chooseDisplayChart.SetSelection(0);
            Globals.chooseDisplayChart.ItemSelected -= DisplayChart_ItemSelected;
            Globals.chooseDisplayChart.ItemSelected += DisplayChart_ItemSelected;

            Globals.blockedCategoryOrTag = true;
            Globals.chooseCategoryOrTag.Adapter = StoreControlsData.CategoriesOrTagsAdapter;
            Globals.chooseCategoryOrTag.SetSelection(0);
            Globals.chooseCategoryOrTag.ItemSelected -= CategoryOrTag_ItemSelected;
            Globals.chooseCategoryOrTag.ItemSelected += CategoryOrTag_ItemSelected;

            if (MoneyGlobals.TransactionsList.Count > 15)
            {
                Globals.ShowProgressDialog();
            }

            Globals.HideProgressDialog();

            Globals.chkAllTransactions.Checked = false;
            Globals.chkAllTransactions.Visibility = ViewStates.Invisible;
            Globals.chkAllTransactions.CheckedChange -= ChkRemoveAllTransactions_CheckedChange;
            Globals.chkAllTransactions.CheckedChange += ChkRemoveAllTransactions_CheckedChange;

            Globals.imgRemoveAllTransactions.Visibility = ViewStates.Invisible;
            Globals.imgRemoveAllTransactions.Click -= ImgRemoveAllTransactions_Click;
            Globals.imgRemoveAllTransactions.Click += ImgRemoveAllTransactions_Click;

            Globals.btnTransactionsFilter.Click -= btnTransactionsFilterOrSort_Click;
            Globals.btnTransactionsFilter.Click += btnTransactionsFilterOrSort_Click;

            Globals.btnTransactionsSort.Click -= btnTransactionsFilterOrSort_Click;
            Globals.btnTransactionsSort.Click += btnTransactionsFilterOrSort_Click;

            Globals.btnTransactionsClearFilter.Click -= btnTransactionsClearFilter_Click;
            Globals.btnTransactionsClearFilter.Click += btnTransactionsClearFilter_Click;

            if (Filtering.DateFilter == "All Time")
            {
                BtnFilterMonth_Click("AllTime", new EventArgs());
            }

            //Filters
            Filtering.AllFilters = Filtering.DateFilter;
            if (Filtering.AllFilters != "This Month")
            {
                MoneyGlobals.ClearFilters = true;
            }
            if (Filtering.CategoryOrTagFilter != "No Category;None")
            {
                Filtering.AllFilters += "; " + Filtering.CategoryOrTagFilter;
                MoneyGlobals.ClearFilters = true;
            }
            if (Filtering.SumFilter.Length > 0)
            {
                Filtering.AllFilters += "; " + Filtering.SumFilter;
                MoneyGlobals.ClearFilters = true;
            }
            Globals.btnTransactionsClearFilter.Visibility = MoneyGlobals.ClearFilters ? ViewStates.Visible : ViewStates.Invisible;
            Globals.txtTransactionsAppliedFilter.Text = Filtering.AllFilters;

            //Sorts
            Globals.txtTransactionsSort.Text = Sorting.DisplayCurrentSort;

            TransactionsMenuShowTransactionsBy();
        }

        private void CategoryOrTag_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (!Globals.blockedCategoryOrTag)
            {
                MoneyGlobals.DisplayCategoryOrTag = Globals.chooseCategoryOrTag.SelectedItem.ToString();
                DisplayChart(MoneyGlobals.DisplayCategoryOrTag, MoneyGlobals.DisplayChart);
            }
            Globals.blockedCategoryOrTag = false;
        }

        private void DisplayChart_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) 
        {
            if (!Globals.formLoading)
            {
                MoneyGlobals.DisplayChart = Globals.chooseDisplayChart.SelectedItem.ToString();
                DisplayChart(MoneyGlobals.DisplayCategoryOrTag, MoneyGlobals.DisplayChart);
            }
            Globals.formLoading = false;
        }

        private void ChkRemoveAllTransactions_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
			if (!Globals.doNothing)
			{
                (Globals.grdTransactions.Adapter as TransactionsAdapter).CheckAll(e.IsChecked);
			}
        }

        private void ImgRemoveAllTransactions_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
            alert.SetTitle("Remove?");
            alert.SetMessage("Are you sure you want to remove the transaction" + (MoneyGlobals.DisplayTransactions.FindAll(x => x.Checked).Count > 1 ? "s" : "") + " ?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                (Globals.grdTransactions.Adapter as TransactionsAdapter).RemoveSelected();
                Globals.grdTransactions.Adapter = Globals.grdTransactions.Adapter;
            });
            alert.SetNeutralButton("No", (senderAlert, args) =>
            {
                (Globals.grdTransactions.Adapter as TransactionsAdapter).EditState();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void btnTransactionsFilterOrSort_Click(object sender, EventArgs e)
        {
			if (sender is Button btn)
			{
				if (btn == Globals.btnTransactionsFilter)
				{
                    SetContentView(Globals.Filters);
                    Globals.PreviousForm = Globals.TransactionsMenu;
				}
				else if (btn == Globals.btnTransactionsSort)
				{
                    SetContentView(Globals.Sorts);
                    Globals.PreviousForm = Globals.TransactionsMenu;
                }
			}
        }

        private void btnTransactionsClearFilter_Click(object sender, EventArgs e)
        {
            SetContentView(Globals.Filters);
            Globals.spinnerCategoryOrTagFilter.SetSelection(0);
            MoneyGlobals.GetSelectedCategoryOrTagFilter();
            Filtering.CategoryOrTagFilter = "No Category;None";
            Filtering.SumFilter = "";
            Filtering.DateFilter = "";
            Filtering.AllFilters = "";
            MoneyGlobals.SetSumFilter(true);
			if (sender is Button btn)
			{
				if (btn == Globals.btnTransactionsClearFilter)
				{
                    SetContentView(Globals.TransactionsMenu);
                    Globals.txtTransactionsAppliedFilter.Text = "This Month";
                    BtnFilterMonth_Click("ThisMonth", new EventArgs());
                    TransactionsMenuShowTransactionsBy();
                    Globals.btnTransactionsClearFilter.Visibility = ViewStates.Invisible;
				}
				else if (btn == Globals.btnConsumClearFilter)
				{
                    SetContentView(Globals.CarConsumption);
                    Globals.txtCarConsumptionFilters.Text = "This Month";
                    BtnFilterMonth_Click("ThisMonth", new EventArgs());
                    Globals.btnConsumClearFilter.Visibility = ViewStates.Invisible;
                }
			}

            MoneyGlobals.ClearFilters = false;
        }

        private void TransactionsMenuShowTransactionsBy()
        {
            MoneyGlobals.WhatTransactionTypeToDisplay(Filtering.TransactionDisplay);

            if (Filtering.TransactionDisplay == TransactionDisplay.Chart)
            {
                DisplayChart();
            }
            else if (Filtering.TransactionDisplay == TransactionDisplay.List)
            {
                Globals.ShowProgressDialog();
                //filters
                MoneyGlobals.DisplayTransactions = Filtering.FilterTransactions(MoneyGlobals.TransactionsList, Filtering.BeginDate, Filtering.EndDate);
                //sorts
                MoneyGlobals.DisplayTransactions = Sorting.SortTransactions(MoneyGlobals.DisplayTransactions);
                //Show/Hide no data to display
                MoneyGlobals.WhatTransactionToDisplay();

                Globals.grdTransactions.Adapter = new TransactionsAdapter(this, MoneyGlobals.DisplayTransactions);

                Globals.HideProgressDialog();
            }
			else if (Filtering.TransactionDisplay == TransactionDisplay.Grid)
			{
                List<Month> DisplayMonths = Globals.MonthsList.Where(x=>x.Transactions.Count > 0).ToList();
				if (Sorting.CurrentSort == SortBy.DateLatest)
				{
                    DisplayMonths = DisplayMonths.OrderByDescending(o => o.year).ThenByDescending(o=>o.month).ToList();
                }
				else if (Sorting.CurrentSort == SortBy.DateOldest)
                {
                    DisplayMonths = DisplayMonths.OrderBy(o => o.year).ThenBy(o => o.month).ToList();
                }
                Globals.elvGridTransactions.SetAdapter(new ExpandableDataAdapter(this, DisplayMonths));
            }
        }

        private void DisplayChart(string displayCategoryOrTag = "Categories", string displayChart = "Bar")
        {
            if (MoneyGlobals.TransactionsList != null)
            {
                Globals.ShowProgressDialog();

                DateTime beginDate;
                DateTime endDate;
                DateTime.TryParse(Globals.txtFilterBeginDate.Text, out beginDate);
                if (beginDate.ToOADate() != 0)
                {
                    DateTime.TryParse(Globals.txtFilterEndDate.Text, out endDate);
                    if (endDate.ToOADate() != 0)
                    {

                        Globals.grdTransactions.Visibility = ViewStates.Invisible;
                        Globals.layoutCharts.Visibility = ViewStates.Visible;
                        Globals.elvGridTransactions.Visibility = ViewStates.Invisible;

                        //Filters
                        MoneyGlobals.DisplayTransactions = Filtering.GetTransactionsByDate(beginDate, endDate);
                        MoneyGlobals.DisplayTransactions = Filtering.GetTransactionsByCategoriesAndTags(MoneyGlobals.DisplayTransactions);

                        Dictionary<string, Chart> charts = Data.CreateCharts(MoneyGlobals.DisplayTransactions, displayCategoryOrTag, displayChart);

                        //Show/Hide no data to display
                        MoneyGlobals.WhatTransactionToDisplay(charts.Values.ToList()[0].Entries.Count());

                        FindViewById<ChartView>(Id.Charts).Chart = charts[displayChart];
                    }
                }

                Globals.HideProgressDialog();
            }
        }

        #endregion

        #region MoneyLayoutTop

        public void MoneyLayoutTop_Events(bool showThem = true)
        {
            Globals.btnTransactionsMenu = FindViewById<Button>(Id.btnTransactionsMenu);
            Globals.btnYearly = FindViewById<Button>(Id.btnYearly);
            Globals.btnAddTransaction = FindViewById<Button>(Id.btnAddTransaction);

            //TransactionsMenu form doesn't have button which was replaced with Filter Button
			if (Globals.btnTransactionsMenu != null)
			{
                Globals.btnTransactionsMenu.Visibility = showThem ? Globals.btnTransactionsMenu.Visibility : ViewStates.Invisible;
                Globals.btnTransactionsMenu.Click -= btnMoneyLayoutTop_Click;
                Globals.btnTransactionsMenu.Click += btnMoneyLayoutTop_Click;
			}
            Globals.btnYearly.Visibility = showThem ? Globals.btnYearly.Visibility : ViewStates.Invisible;
            Globals.btnAddTransaction.Visibility = showThem ? Globals.btnAddTransaction.Visibility : ViewStates.Invisible;

            Globals.btnYearly.Click -= btnMoneyLayoutTop_Click;
            Globals.btnYearly.Click += btnMoneyLayoutTop_Click;
            Globals.btnAddTransaction.Click -= btnMoneyLayoutTop_Click;
            Globals.btnAddTransaction.Click += btnMoneyLayoutTop_Click;
        }

        private void btnMoneyLayoutTop_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn == Globals.btnTransactionsMenu)
                {
                    SetContentView(Globals.TransactionsMenu);
                }
                else if (btn == Globals.btnYearly)
                {
                    SetContentView(Globals.YearlyStats);
                }
                else if (btn == Globals.btnAddTransaction)
                {
                    MoneyGlobals.onTransactionMenuForm = true;
                    MoneyGlobals.GlobalTransaction = null;
                    ClearTransactionFlags();
                    SetContentView(Globals.TransactionMenu);
                }
            }
        }

        #endregion

        #region TransactionMenu

        /// <summary>
        /// on true it sets data, on false it gets data
        /// </summary>
        /// <param name="flag"></param>
        public string ProcessRepetitiveInterval(bool flag = true)
		{
			if (!flag || Globals.chkRepetitiveTransaction.Checked)
			{
				switch (Globals.spinnerSetIntervalTime.SelectedItem.ToString())
				{
                    case "Interval Of":
						if (flag)
						{
                            Globals.txtInterval.Visibility = ViewStates.Visible;
                            Globals.spinnerEachInterval.Visibility = ViewStates.Invisible;
                            Globals.spinnerInterval.Visibility = ViewStates.Visible;
                            Globals.spinnerInterval.Adapter = StoreControlsData.SpinnerIntervalIntervalOfAdapter;
                        }
						else
						{
                            return SetInterval(Globals.spinnerSetIntervalTime.SelectedItem.ToString(), Globals.txtInterval.Text, null, Globals.spinnerInterval.SelectedItem.ToString(), Globals.spinnerRepeatFor.SelectedItem.ToString());
						}
                        break;
                    case "On Each Day Of Week":
						if (flag)
						{
                            Globals.txtInterval.Visibility = ViewStates.Invisible;
                            Globals.spinnerEachInterval.Visibility = ViewStates.Visible;
                            Globals.spinnerEachInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachDayOfWeekAdapter;
                            Globals.spinnerInterval.Visibility = ViewStates.Invisible;
                        }
						else
						{
                            return SetInterval(Globals.spinnerSetIntervalTime.SelectedItem.ToString(), null, Globals.spinnerEachInterval.SelectedItem.ToString(), null, Globals.spinnerRepeatFor.SelectedItem.ToString());
                        }
                        break;
                    case "On Each Week Of Month":
						if (flag)
						{
                            Globals.txtInterval.Visibility = ViewStates.Invisible;
                            Globals.spinnerEachInterval.Visibility = ViewStates.Visible;
                            Globals.spinnerEachInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachWeekOfMonthAdapter;
                            Globals.spinnerInterval.Visibility = ViewStates.Invisible;
                        }
						else
						{
                            return SetInterval(Globals.spinnerSetIntervalTime.SelectedItem.ToString(), null, Globals.spinnerEachInterval.SelectedItem.ToString(), null, Globals.spinnerRepeatFor.SelectedItem.ToString());
                        }
                        break;
                    case "On Each Day Of Month":
						if (flag)
						{
                            Globals.txtInterval.Visibility = ViewStates.Invisible;
                            Globals.spinnerEachInterval.Visibility = ViewStates.Visible;
                            Globals.spinnerEachInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachWeekOfMonthAdapter;
                            Globals.spinnerInterval.Visibility = ViewStates.Visible;
                            Globals.spinnerInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachDayOfWeekAdapter;
                        }
						else
						{
                            return SetInterval(Globals.spinnerSetIntervalTime.SelectedItem.ToString(), null, Globals.spinnerEachInterval.SelectedItem.ToString(), Globals.spinnerInterval.SelectedItem.ToString(), Globals.spinnerRepeatFor.SelectedItem.ToString());
                        }
                        break;
                }
			}
            return "";
		}

		private string SetInterval(string v1, string v2, string v3, string v4, string v5)
		{
            string result = string.Empty;
			if (v1 == "Interval Of")
			{
                result = v1 + ";" + v2 + ";;" + v4;
			}
			else if (v1 == "On Each Day Of Week" || v1 == "On Each Week Of Month")
			{
                result = v1 + ";" + ";" + v3 + ";";
			}
			else if (v1 == "On Each Day Of Month")
			{
                result = v1 + ";" + ";" + v3 + ";" + v4;
            }

            result += ";" + v5;

            return result;
		}

        private void PopulateInterval(string interval)
		{
            string[] intervals = interval.Split(";");
            Globals.chkRepetitiveTransaction.Checked = intervals.Length > 1;

			if (Globals.spinnerSetIntervalTime.Adapter == null)
			{
                Globals.spinnerSetIntervalTime.Adapter = StoreControlsData.SpinnerSetIntervalTimeAdapter;
			}
			if (Globals.spinnerRepeatFor.Adapter == null)
			{
                Globals.spinnerRepeatFor.Adapter = StoreControlsData.SpinnerRepeatFor;
            }
            if (!string.IsNullOrWhiteSpace(interval))
            {
                switch (intervals[0])
                {
                    case "Interval Of":
                        Globals.spinnerSetIntervalTime.SetSelection(0);
                        Globals.txtInterval.Text = intervals[1];
                        Globals.spinnerInterval.Adapter = StoreControlsData.SpinnerIntervalIntervalOfAdapter;
                        switch (intervals[2])
                        {
                            case "Days":
                                Globals.spinnerInterval.SetSelection(0);
                                break;
                            case "Months":
                                Globals.spinnerInterval.SetSelection(1);
                                break;
                            case "Years":
                                Globals.spinnerInterval.SetSelection(2);
                                break;
                        }
                        break;
                    case "On Each Day Of Week":
                        Globals.spinnerSetIntervalTime.SetSelection(1);
                        Globals.spinnerEachInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachDayOfWeekAdapter;
                        switch (intervals[2])
                        {
                            case "Monday":
                                Globals.spinnerEachInterval.SetSelection(0);
                                break;
                            case "Tuesday":
                                Globals.spinnerEachInterval.SetSelection(1);
                                break;
                            case "Wednesday":
                                Globals.spinnerEachInterval.SetSelection(2);
                                break;
                            case "Thursday":
                                Globals.spinnerEachInterval.SetSelection(3);
                                break;
                            case "Friday":
                                Globals.spinnerEachInterval.SetSelection(4);
                                break;
                            case "Saturday":
                                Globals.spinnerEachInterval.SetSelection(5);
                                break;
                            case "Sunday":
                                Globals.spinnerEachInterval.SetSelection(6);
                                break;
                        }
                        break;
                    case "On Each Week Of Month":
                        Globals.spinnerSetIntervalTime.SetSelection(2);
                        Globals.spinnerEachInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachWeekOfMonthAdapter;
                        switch (intervals[2])
                        {
                            case "First":
                                Globals.spinnerEachInterval.SetSelection(0);
                                break;
                            case "Second":
                                Globals.spinnerEachInterval.SetSelection(1);
                                break;
                            case "Third":
                                Globals.spinnerEachInterval.SetSelection(2);
                                break;
                            case "Last":
                                Globals.spinnerEachInterval.SetSelection(3);
                                break;
                        }
                        break;
                    case "On Each Day Of Month":
                        Globals.spinnerSetIntervalTime.SetSelection(3);
                        Globals.spinnerEachInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachWeekOfMonthAdapter;
                        switch (intervals[2])
                        {
                            case "First":
                                Globals.spinnerEachInterval.SetSelection(0);
                                break;
                            case "Second":
                                Globals.spinnerEachInterval.SetSelection(1);
                                break;
                            case "Third":
                                Globals.spinnerEachInterval.SetSelection(2);
                                break;
                            case "Last":
                                Globals.spinnerEachInterval.SetSelection(3);
                                break;
                        }
                        Globals.spinnerInterval.Adapter = StoreControlsData.SpinnerIntervalOnEachDayOfWeekAdapter;
                        switch (intervals[3])
                        {
                            case "Monday":
                                Globals.spinnerInterval.SetSelection(0);
                                break;
                            case "Tuesday":
                                Globals.spinnerInterval.SetSelection(1);
                                break;
                            case "Wednesday":
                                Globals.spinnerInterval.SetSelection(2);
                                break;
                            case "Thursday":
                                Globals.spinnerInterval.SetSelection(3);
                                break;
                            case "Friday":
                                Globals.spinnerInterval.SetSelection(4);
                                break;
                            case "Saturday":
                                Globals.spinnerInterval.SetSelection(5);
                                break;
                            case "Sunday":
                                Globals.spinnerInterval.SetSelection(6);
                                break;
                        }
                        break;
                }

                switch (intervals[4])
                {
                    case "Once":
                        Globals.spinnerRepeatFor.SetSelection(0);
                        break;
                    case "2 Times":
                        Globals.spinnerRepeatFor.SetSelection(1);
                        break;
                    case "3 Times":
                        Globals.spinnerRepeatFor.SetSelection(2);
                        break;
                    case "4 Times":
                        Globals.spinnerRepeatFor.SetSelection(3);
                        break;
                    case "Until I Stop It":
                        Globals.spinnerRepeatFor.SetSelection(4);
                        break;
                }
            }
		}

        private void ShowAdvancedSettings(Transaction transaction = null, bool showBtnClicked = false)
		{
            if ((showBtnClicked && Globals.layoutAdvancedTransactionSettings.Visibility != ViewStates.Visible) 
                || transaction != null && (!string.IsNullOrWhiteSpace(transaction.RepetitiveInterval) || transaction.HasCarConsumption || !string.IsNullOrWhiteSpace(Globals.txtChoosenCarToAdd.Text) || !string.IsNullOrWhiteSpace(Globals.txtChoosenNoteToAdd.Text)))
            {
                Globals.layoutAdvancedTransactionSettings.Visibility = ViewStates.Visible;
            }
            else
            {
                Globals.layoutAdvancedTransactionSettings.Visibility = ViewStates.Invisible;
            }
        }

        public void TransactionMenu_Events()
        {
            MoneyGlobals.SetTimeInterval = false;

            Globals.btnSeeTags = FindViewById<Button>(Id.btnSeeTags);
            Globals.txtAddSum = FindViewById<EditText>(Id.txtAddSum);
            Globals.txtAddDate = FindViewById<EditText>(Id.txtAddDate);
            Globals.txtAddLiters = FindViewById<EditText>(Id.txtAddLiters);
            Globals.txtAddKilometersRan = FindViewById<EditText>(Id.txtAddKilometersRan);
            Globals.txtAddPrice = FindViewById<EditText>(Id.txtAddPrice);
            Globals.txtInterval = FindViewById<EditText>(Id.txtInterval);

            Globals.txtChoosenNoteToAdd = FindViewById<TextView>(Id.txtChoosenNoteToAdd);
            Globals.txtChoosenCarToAdd = FindViewById<TextView>(Id.txtChoosenCarToAdd);
            Globals.textviewKilometersRan = FindViewById<TextView>(Id.textviewKilometersRan);
            Globals.textviewLitersAdded = FindViewById<TextView>(Id.textviewLitersAdded);
            Globals.textviewPriceAdded = FindViewById<TextView>(Id.textviewPriceAdded);

            Globals.btnAddToNote = FindViewById<Button>(Id.btnAddToNote);
            Globals.btnAddToCar = FindViewById<Button>(Id.btnAddToCar);
            Globals.btnModifyTransaction = FindViewById<Button>(Id.btnModifyTransaction);
            Globals.btnAddAnotherTransaction = FindViewById<Button>(Id.btnAddAnotherTransaction);
            Globals.btnAddAddTransaction = FindViewById<Button>(Id.btnAddAddTransaction);
            Globals.btnAddDate = FindViewById<Button>(Id.btnAddDate);
            Globals.btnHasCarConsumption = FindViewById<Button>(Id.btnHasCarConsumption);
            Globals.btnRepetitiveTransaction = FindViewById<Button>(Id.btnRepetitiveTransaction);
            Globals.btnAdvancedTransactionSettings = FindViewById<Button>(Id.btnAdvancedTransactionSettings);

            Globals.spiCategory = FindViewById<Spinner>(Id.spiCategory);
            Globals.spinnerInterval = FindViewById<Spinner>(Id.spinnerInterval);
            Globals.spinnerSetIntervalTime = FindViewById<Spinner>(Id.spinnerSetIntervalTime);
            Globals.spinnerEachInterval = FindViewById<Spinner>(Id.spinnerEachInterval);
            Globals.spinnerRepeatFor = FindViewById<Spinner>(Id.spinnerRepeatFor);

            Globals.txtAddName = FindViewById<AutoCompleteTextView>(Id.txtAddName);
            Globals.txtAddTag = FindViewById<AutoCompleteTextView>(Id.txtAddTag);

            Globals.chkHasCarConsumption = FindViewById<CheckBox>(Id.chkHasCarConsumption);
            Globals.chkRepetitiveTransaction = FindViewById<CheckBox>(Id.chkRepetitiveTransaction);

            Globals.layoutCarConsumption = FindViewById<LinearLayout>(Id.LayoutCarConsumption);
            Globals.layoutRepetitiveTransaction = FindViewById<LinearLayout>(Id.LayoutRepetitiveTransaction);
            Globals.layoutAddTransaction = FindViewById<LinearLayout>(Id.LayoutAddTransaction);
            Globals.layoutModifyTransaction = FindViewById<LinearLayout>(Id.LayoutModifyTransaction);
            Globals.layoutAdvancedTransactionSettings = FindViewById<LinearLayout>(Id.LayoutAdvancedTransactionSettings);

            Globals.layoutCarConsumption.Visibility = ViewStates.Invisible;
            Globals.layoutRepetitiveTransaction.Visibility = ViewStates.Invisible;

            MoneyGlobals.DisplayUnitType();

            Globals.spinnerRepeatFor.Adapter = StoreControlsData.SpinnerRepeatFor;

            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, TagsGlobals.TagsNameList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Globals.txtAddTag.Adapter = adapter;

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, MoneyGlobals.TransactionsNameList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Globals.txtAddName.Adapter = adapter;

            Globals.txtAddName.FocusChange -= txtAddName_FocusChange;
			Globals.txtAddName.FocusChange += txtAddName_FocusChange;

            Globals.txtInterval.TextChanged -= TxtInterval_TextChanged;
            Globals.txtInterval.TextChanged += TxtInterval_TextChanged;

            Globals.btnSeeTags.Click -= btnAddNoteOrCarOrTagToTransaction_Click;
            Globals.btnSeeTags.Click += btnAddNoteOrCarOrTagToTransaction_Click;

            Globals.btnAddDate.Click -= btnAddDate_Click;
            Globals.btnAddDate.Click += btnAddDate_Click;

            Globals.btnModifyTransaction.Click -= btnModify_Click;
            Globals.btnModifyTransaction.Click += btnModify_Click;

            Globals.btnAddAnotherTransaction.Click -= btnAddAddTransaction_Click;
            Globals.btnAddAnotherTransaction.Click += btnAddAddTransaction_Click;

            Globals.btnAddAddTransaction.Click -= btnAddAddTransaction_Click;
            Globals.btnAddAddTransaction.Click += btnAddAddTransaction_Click;

            Globals.btnAddToNote.Click -= btnAddNoteOrCarOrTagToTransaction_Click;
            Globals.btnAddToNote.Click += btnAddNoteOrCarOrTagToTransaction_Click;

            Globals.btnAddToCar.Click -= btnAddNoteOrCarOrTagToTransaction_Click;
            Globals.btnAddToCar.Click += btnAddNoteOrCarOrTagToTransaction_Click;

            Globals.btnHasCarConsumption.Click -= BtnHasCarConsumption_Click;
            Globals.btnHasCarConsumption.Click += BtnHasCarConsumption_Click;

            Globals.btnRepetitiveTransaction.Click -= BtnRepetitiveTransaction_Click;
            Globals.btnRepetitiveTransaction.Click += BtnRepetitiveTransaction_Click;

            Globals.chkHasCarConsumption.CheckedChange -= ChkHasCarConsumptionInTransaction_CheckedChange;
            Globals.chkHasCarConsumption.CheckedChange += ChkHasCarConsumptionInTransaction_CheckedChange;

            Globals.chkRepetitiveTransaction.CheckedChange -= ChkRepetitiveTransaction_CheckedChange;
			Globals.chkRepetitiveTransaction.CheckedChange += ChkRepetitiveTransaction_CheckedChange;

            Globals.btnAdvancedTransactionSettings.Click -= BtnAdvancedTransactionSettings_Click;
            Globals.btnAdvancedTransactionSettings.Click += BtnAdvancedTransactionSettings_Click;

            Globals.ShowProgressDialog();

            List<string> temp = new List<string>(CategoriesGlobals.CategoriesNameList);
            if (CategoriesGlobals.CategoriesNameList.Count == 0)
            {
                temp.Add("NO CATEGORY!");
				if (MoneyGlobals.GlobalTransaction == null)
				{
                    Utils.ShowAlert("", "Please Insert First A Category");
                    SetContentView(Globals.ParentForm);
                    return;
				}
            }
            Globals.spiCategory.Adapter = new ArrayAdapter(this, Drawable.spinner_item, temp);

            Globals.spinnerInterval.Adapter = StoreControlsData.SpinnerIntervalIntervalOfAdapter;
			Globals.spinnerInterval.ItemSelected -= SpinnerInterval_ItemSelected;
            Globals.spinnerInterval.ItemSelected += SpinnerInterval_ItemSelected;

            Globals.spinnerEachInterval.ItemSelected -= SpinnerEachInterval_ItemSelected;
			Globals.spinnerEachInterval.ItemSelected += SpinnerEachInterval_ItemSelected;

            ClearTransactionMenuTextBoxes();

            if (!MoneyGlobals.IsCopiedTransaction && !Globals.returnFromPause && !Globals.NoteToAddInTransaction && 
                !Globals.CarToAddInTransaction && !Globals.TagToAddInTransaction)
            {
				if (MoneyGlobals.GlobalTransaction == null)
				{
                    ClearTransactionFlags();

                    Globals.layoutAddTransaction.Visibility = ViewStates.Visible;
                    Globals.layoutModifyTransaction.Visibility = ViewStates.Invisible;
                    Globals.layoutAdvancedTransactionSettings.Visibility = ViewStates.Invisible;

                    Globals.txtAddDate.Text = DateTime.Now.ToShortDateString();

                    ShowInput(Globals.txtAddName);
				}
				else
				{
                    Globals.layoutAddTransaction.Visibility = ViewStates.Invisible;
                    Globals.layoutModifyTransaction.Visibility = ViewStates.Visible;

					//populate with selected transaction
                    Globals.spiCategory.SetSelection(CategoriesGlobals.CategoryIndexInList(CategoriesGlobals.CategoriesList, MoneyGlobals.GlobalTransaction.Category));
                    Globals.txtAddName.Text = MoneyGlobals.GlobalTransaction.Name;
                    Globals.txtAddSum.Text = Utils.ConvertDoubleToString(MoneyGlobals.GlobalTransaction.Sum);
                    Globals.txtAddTag.Text = MoneyGlobals.GlobalTransaction.Tag?.Name;
                    Globals.txtAddDate.Text = MoneyGlobals.GlobalTransaction.Date.ToShortDateString();
                    Globals.txtAddKilometersRan.Text = MoneyGlobals.GlobalTransaction.Kilometers > 0 ? MoneyGlobals.GlobalTransaction.Kilometers.ToString("#,##0.00") : "";
                    Globals.txtAddLiters.Text = MoneyGlobals.GlobalTransaction.Liters > 0 ? MoneyGlobals.GlobalTransaction.Liters.ToString("#,##0.00") : "";
                    Globals.txtAddPrice.Text = MoneyGlobals.GlobalTransaction.Price > 0 ? MoneyGlobals.GlobalTransaction.Price.ToString("#,##0.00") : "";
                    Globals.chkHasCarConsumption.Checked = MoneyGlobals.GlobalTransaction.HasCarConsumption;
                    Globals.layoutCarConsumption.Visibility = MoneyGlobals.GlobalTransaction.HasCarConsumption ? ViewStates.Visible : ViewStates.Invisible;

                    PopulateInterval(MoneyGlobals.GlobalTransaction.RepetitiveInterval);

                    //check for duplicated notes
                    NotesGlobals.RemoveDuplicated(NotesGlobals.NotesList);
                    //also can already have mininotes
                    foreach (Note note in NotesGlobals.NotesList)
                    {
                        foreach (MiniNote miniNote in note.MiniNotesList)
                        {
                            if (MoneyGlobals.TransactionsMatch(miniNote.Transaction, MoneyGlobals.GlobalTransaction))
                            {
                                note.Checked = true;
                                if (!NotesGlobals.NoteInList(Globals.SelectedNotesForTransaction, note))
                                {
                                    Globals.SelectedNotesForTransaction.Add(note);
                                    Globals.txtChoosenNoteToAdd.Text += note.ID + "." + note.Title + ";";
                                }
                            }
                        }
                    }

                    //check for duplicated cars
                    CarsGlobals.RemoveDuplicated(CarsGlobals.CarsList);
                    //also can already have cars
                    foreach (Car car in CarsGlobals.CarsList)
                    {
                        if (MoneyGlobals.TransactionInList(car.Transactions, MoneyGlobals.GlobalTransaction))
                        {
                            car.Checked = true;
                            Globals.SelectedCarsForTransaction.Add(car);
                            if (!CarsGlobals.CarInList(Globals.SelectedCarsForTransaction, car))
                            {
                                Globals.SelectedCarsForTransaction.Add(car);
                            }
                            Globals.txtChoosenCarToAdd.Text += car.ID + "." + car.Plate + ";";
                        }
                    }
                    ShowAdvancedSettings(MoneyGlobals.GlobalTransaction);
                }
            }
            else
            {
                //here comes a cloned transaction
                if (MoneyGlobals.IsCopiedTransaction && MoneyGlobals.GlobalTransaction != null)
                {
                    MoneyGlobals.PendingTransaction = new Transaction(MoneyGlobals.GlobalTransaction);
                    MoneyGlobals.GlobalTransaction = null;
                }

                if (MoneyGlobals.GlobalTransaction != null)
				{
                    Globals.layoutAddTransaction.Visibility = ViewStates.Invisible;
                    Globals.layoutModifyTransaction.Visibility = ViewStates.Visible;
                    PopulateInterval(MoneyGlobals.GlobalTransaction.RepetitiveInterval);
                    ShowAdvancedSettings(MoneyGlobals.GlobalTransaction);
                }
				else
				{
                    Globals.layoutAddTransaction.Visibility = ViewStates.Visible;
                    Globals.layoutModifyTransaction.Visibility = ViewStates.Invisible;
				}

                Globals.spiCategory.SetSelection(CategoriesGlobals.CategoryIndexInList(CategoriesGlobals.CategoriesList, MoneyGlobals.PendingTransaction.Category));

                //populate with selected transaction
                Globals.txtAddName.Text = MoneyGlobals.PendingTransaction.Name;
                Globals.txtAddSum.Text = MoneyGlobals.PendingTransaction.Sum.ToString() == "0" ? "" : MoneyGlobals.PendingTransaction.Sum.ToString();
                Globals.txtAddTag.Text = MoneyGlobals.PendingTransaction.Tag?.Name;
                if (MoneyGlobals.IsCopiedTransaction)
				{
                    Globals.txtAddDate.Text = DateTime.Now.ToShortDateString();
                }
				else
				{
                    Globals.txtAddDate.Text = MoneyGlobals.PendingTransaction.Date.ToShortDateString();
				}

				if (MoneyGlobals.PendingTransaction.HasCarConsumption)
				{
                    Globals.chkHasCarConsumption.Checked = MoneyGlobals.PendingTransaction.HasCarConsumption;
                    Globals.txtAddKilometersRan.Text = MoneyGlobals.PendingTransaction.Kilometers.ToString("#,##0.0000");
                    Globals.txtAddLiters.Text = MoneyGlobals.PendingTransaction.Liters.ToString("#,##0.00");
                    Globals.txtAddPrice.Text = MoneyGlobals.PendingTransaction.Price.ToString("#,##0.00");
                }

                //check for duplicated notes
                NotesGlobals.RemoveDuplicated(Globals.SelectedNotesForTransaction);

                foreach (Note note in Globals.SelectedNotesForTransaction)
                {
                    if (note.Checked)
                    {
                        Globals.txtChoosenNoteToAdd.Text += note.ID + "." + note.Title + ";";
                    }
                }

                //check for duplicated cars
                CarsGlobals.RemoveDuplicated(Globals.SelectedCarsForTransaction);
                foreach (Car car in Globals.SelectedCarsForTransaction)
                {
                    if (car.Checked)
                    {
                        Globals.txtChoosenCarToAdd.Text += car.ID + "." + car.Plate + ";";
                    }
                }
                ShowAdvancedSettings(MoneyGlobals.PendingTransaction);
            }

            if (Globals.TagToAddInTransaction)
            {
                if (Globals.SelectedTagForTransaction != null)
                {
                    Globals.txtAddTag.Text = Globals.SelectedTagForTransaction.Name;
                }
                else
                {
                    Globals.txtAddTag.Text = string.Empty;
                }
            }

			if (Globals.spinnerSetIntervalTime.Adapter == null)
			{
                Globals.spinnerSetIntervalTime.Adapter = StoreControlsData.SpinnerSetIntervalTimeAdapter;
			}
            Globals.spinnerSetIntervalTime.ItemSelected -= SpinnerSetIntervalTime_ItemSelected;
            Globals.spinnerSetIntervalTime.ItemSelected += SpinnerSetIntervalTime_ItemSelected;

            Globals.TagToAddInTransaction = false;
            Globals.SelectedTagForTransaction = null;

            Globals.HideProgressDialog();
        }

		private void BtnAdvancedTransactionSettings_Click(object sender, EventArgs e)
		{
            ShowAdvancedSettings(showBtnClicked:true);
		}

        private void txtAddName_FocusChange(object sender, View.FocusChangeEventArgs e)
		{
			if (Globals.txtAddName.Text != string.Empty && !Globals.txtAddName.IsFocused && MoneyGlobals.TransactionsList != null && MoneyGlobals.TransactionsList.Count > 0)
			{
                string name = Globals.txtAddName.Text.ToLower();
                List<Transaction> list = MoneyGlobals.TransactionsList.FindAll(x => x.Name.ToLower() == name && x.Tag != null && x.Category != null);
                if (list.Count > 0)
				{
                    Transaction transaction = list[list.Count - 1];
					if (transaction != null)
					{
						if (transaction.Tag != null)
						{
                            Globals.txtAddTag.Text = transaction.Tag.Name;
                            ShowInput(Globals.txtAddSum);
						}
						if (transaction.Category != null)
						{
                            Globals.spiCategory.SetSelection(CategoriesGlobals.CategoryIndexInList(CategoriesGlobals.CategoriesList, transaction.Category));
						}
					}
				}
			}
		}

        private void TxtInterval_TextChanged(object sender, TextChangedEventArgs e)
		{
            MoneyGlobals.RepetitiveInterval = ProcessRepetitiveInterval(false);
        }

		private void SpinnerEachInterval_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
            MoneyGlobals.RepetitiveInterval = ProcessRepetitiveInterval(false);
        }

		private void SpinnerInterval_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
            MoneyGlobals.RepetitiveInterval = ProcessRepetitiveInterval(false);
        }

		private void SpinnerSetIntervalTime_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			if (!MoneyGlobals.SetTimeInterval)
			{
                MoneyGlobals.SetTimeInterval = true;
                return;
            }
            ProcessRepetitiveInterval();
        }

		private void BtnRepetitiveTransaction_Click(object sender, EventArgs e)
		{
			if (Globals.layoutRepetitiveTransaction.Visibility == ViewStates.Visible)
			{
                Globals.layoutCarConsumption.Visibility = ViewStates.Invisible;
                Globals.layoutRepetitiveTransaction.Visibility = ViewStates.Invisible;
            }
			else
			{
                Globals.layoutCarConsumption.Visibility = ViewStates.Invisible;
                Globals.layoutRepetitiveTransaction.Visibility = ViewStates.Visible;
			}

            if (Globals.chkRepetitiveTransaction.Checked)
            {
                Globals.layoutRepetitiveTransaction.Enabled = true;
            }
            else
            {
                Globals.layoutRepetitiveTransaction.Enabled = false;
            }
        }

		private void BtnHasCarConsumption_Click(object sender, EventArgs e)
		{
			if (Globals.layoutCarConsumption.Visibility == ViewStates.Visible)
			{
                Globals.layoutCarConsumption.Visibility = ViewStates.Invisible;
                Globals.layoutRepetitiveTransaction.Visibility = ViewStates.Invisible;
            }
			else
			{
                Globals.layoutCarConsumption.Visibility = ViewStates.Visible;
                Globals.layoutRepetitiveTransaction.Visibility = ViewStates.Invisible;
			}

            if (Globals.chkHasCarConsumption.Checked)
            {
                Globals.layoutCarConsumption.Enabled = true;
                ShowInput(Globals.txtAddLiters);
            }
			else
			{
                Globals.layoutCarConsumption.Enabled = false;
                HideInput();
            }
        }

		private void btnAddDate_Click(object sender, EventArgs e)
        {
            Utils.Dpd_DateSet = DpdAdd_DateSet;
            Utils.DateSet = Globals.txtAddDate.Text;
            Utils.ShowDatePicker(Utils.ConvertStringToDate(Globals.txtAddDate.Text));
        }

        private void DpdAdd_DateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Globals.txtAddDate.Text = new DateTime(e.Year, e.Month + 1, e.DayOfMonth).ToShortDateString();
        }

        private void btnAddAddTransaction_Click(object sender, EventArgs e)
        {
            Globals.ShowProgressDialog();

            if (!MoneyGlobals.AllowedTagName(Globals.txtAddTag.Text))
            {
                Utils.ShowAlert("", "Tag Name is a Category, Already Alocated. Please Choose Other");
                ShowInput(Globals.txtAddTag);
                return;
            }
            else if (string.IsNullOrWhiteSpace(Globals.spiCategory.SelectedItem.ToString()))
            {
                Utils.ShowAlert("", "Please Select A Category");
                Globals.spiCategory.RequestFocus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(Globals.txtAddName.Text))
            {
                Utils.ShowAlert("", "Please Insert A Name");
                ShowInput(Globals.txtAddName);
                return;
            }

            //date add
            DateTime date;
            DateTime.TryParse(Globals.txtAddDate.Text, out date);
            if (date.ToOADate() == 0)
            {
                Utils.ShowAlert("Invalid Date", "Something Went Wrong With Date");
                Globals.txtAddDate.RequestFocus();
                return;
            }
            else if (Globals.chkHasCarConsumption.Checked)
            {
                double temp;
                double.TryParse(Globals.txtAddKilometersRan.Text, out temp);
                if (temp == 0)
                {
                    Utils.ShowAlert("", "Invalid Kilometers Added");
                    ShowInput(Globals.txtAddKilometersRan);
                    return;
                }
                double.TryParse(Globals.txtAddLiters.Text, out temp);
                if (temp == 0)
                {
                    Utils.ShowAlert("", "Invalid Liters Added");
                    ShowInput(Globals.txtAddLiters);
                    return;
                }
            }

            Tag tag = TagsGlobals.SearchOrAddNewTag(Globals.txtAddTag.Text);
            bool added = true;
            if (Globals.spiCategory.SelectedItem != null && !string.IsNullOrWhiteSpace(Globals.spiCategory.SelectedItem.ToString()))
            {
				if (Globals.spiCategory.Count == 1 && Globals.spiCategory.SelectedItem.ToString() == "NO CATEGORY!")
				{
                    Utils.ShowAlert("", "Please Add or Select a Category");
                    return;
				}
                if (!string.IsNullOrWhiteSpace(Globals.txtAddSum.Text))
                {
                    Transaction transaction = null;
                    Category cat = CategoriesGlobals.GetCategory(Globals.spiCategory.SelectedItem.ToString());
                    if (Globals.chkHasCarConsumption.Checked)
                    {
                        transaction = new Transaction(Utils.ConvertStringToDouble(Globals.txtAddSum.Text), date, Globals.txtAddName.Text, tag
                            , cat, cat.Color, tag == null, Globals.chkHasCarConsumption.Checked
                            , Utils.ConvertStringToDouble(Globals.txtAddKilometersRan.Text), Utils.ConvertStringToDouble(Globals.txtAddLiters.Text), Utils.ConvertStringToDouble(Globals.txtAddPrice.Text));
                    }
                    else
                    {
                        transaction = new Transaction(Utils.ConvertStringToDouble(Globals.txtAddSum.Text), date, Globals.txtAddName.Text, tag
                            , cat, cat.Color, tag == null, Globals.chkHasCarConsumption.Checked);
                    }

					if (Globals.chkRepetitiveTransaction.Checked)
					{
                        transaction.RepetitiveInterval = ProcessRepetitiveInterval(false);
					}

                    Globals.AddSum(Globals.GetMonthFromDate(date), transaction);
                    NotesGlobals.AssignTransactionToNote(transaction);
                    CarsGlobals.AssignTransactionToCar(transaction);

                    ClearTransactionFlags();
                }
                else
                {
                    added = false;
                    Utils.ShowAlert("", "Please Insert A Sum");
                    Globals.txtAddSum.RequestFocus();
                }
                if (added)
                {
                    ClearTransactionMenuTextBoxes();
                    if (sender is Button btn && btn.Text != "Add Another")
                    {
                        //go to main activity when finish here
                        SetContentView(Globals.YearlyStats);
                    }
                }
            }
            else
            {
                Utils.ShowAlert("", "Please Select A Category");
            }

            Globals.HideProgressDialog();
        }

        private void btnAddNoteOrCarOrTagToTransaction_Click(object sender, EventArgs e)
        {
            Globals.ShowProgressDialog();

            //Save Modifications
            if (Globals.txtAddSum.Text == string.Empty)
            {
                Globals.txtAddSum.Text = "0";
            }
            Category cat = CategoriesGlobals.GetCategory(Globals.spiCategory.SelectedItem.ToString());
            MoneyGlobals.PendingTransaction = new Transaction(Utils.ConvertStringToDouble(Globals.txtAddSum.Text), Utils.ConvertStringToDate(Globals.txtAddDate.Text), Globals.txtAddName.Text
                , TagsGlobals.SearchOrAddNewTag(Globals.txtAddTag.Text), cat, cat.Color, false
                , Globals.chkHasCarConsumption.Checked, Utils.ConvertStringToDouble(Globals.txtAddKilometersRan.Text), Utils.ConvertStringToDouble(Globals.txtAddLiters.Text), Utils.ConvertStringToDouble(Globals.txtAddPrice.Text), RepetitiveInterval: ProcessRepetitiveInterval(false));

            if (Globals.chkRepetitiveTransaction.Checked)
            {
                MoneyGlobals.PendingTransaction.RepetitiveInterval = ProcessRepetitiveInterval(false);
            }

            if (sender is Button btn)
            {
                if (btn == Globals.btnAddToNote)
                {
                    Globals.SelectedNotesForTransaction = new List<Note>();
                    Globals.NoteToAddInTransaction = true;
                    SetContentView(Globals.NotesMenu);
                    Globals.PreviousForm = Globals.TransactionMenu;
                }
                else if (btn == Globals.btnAddToCar)
                {
                    Globals.SelectedCarsForTransaction = new List<Car>();
                    Globals.CarToAddInTransaction = true;
                    SetContentView(Globals.CarsMenu);
                    Globals.PreviousForm = Globals.TransactionMenu;
                }
                else if (btn == Globals.btnSeeTags)
                {
                    Globals.TagToAddInTransaction = true;
                    int PreviousForm = Globals.CurrentForm;
                    SetContentView(Globals.TagsMenu);
                    (Globals.grdTags.Adapter as TagsAdapter).EditState(Globals.TagToAddInTransaction);

                    string selectedTagName = Globals.txtAddTag.Text;
                    (Globals.grdTags.Adapter as TagsAdapter).CheckAll(false, selectedTagName);

                    Globals.TagsMenuParent = PreviousForm;
                    Globals.PreviousForm = PreviousForm;
                    Globals.ParentForm = PreviousForm;
                }
            }

            Globals.HideProgressDialog();
        }

        private void ChkHasCarConsumptionInTransaction_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Globals.layoutCarConsumption.Enabled = e.IsChecked;
			if (e.IsChecked)
			{
                ShowInput(Globals.txtAddLiters);
			}
			else
			{
                HideInput();
            }
        }

		private void ChkRepetitiveTransaction_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
            Globals.layoutRepetitiveTransaction.Enabled = e.IsChecked;
        }

		private void btnModify_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Globals.txtAddName.Text))
			{
				Utils.ShowAlert("", "Please Insert A Name");
				Globals.txtAddName.RequestFocus();
				return;
			}
			else if (string.IsNullOrWhiteSpace(Globals.txtAddSum.Text))
			{
				Utils.ShowAlert("", "Please Insert A Sum");
				Globals.txtAddSum.RequestFocus();
				return;
			}
			else if (Globals.chkHasCarConsumption.Checked)
			{
				double temp;
				double.TryParse(Globals.txtAddKilometersRan.Text, out temp);
				if (temp == 0)
				{
					Utils.ShowAlert("", "Invalid Kilometers Added");
					ShowInput(Globals.txtAddKilometersRan);
					return;
				}
				double.TryParse(Globals.txtAddLiters.Text, out temp);
				if (temp == 0)
				{
					Utils.ShowAlert("", "Invalid Liters Added");
					ShowInput(Globals.txtAddLiters);
					return;
				}
			}
			bool modified = false;
			//update transaction modifications
			if (MoneyGlobals.GlobalTransaction != null)
			{
				// get month
				Month month = Globals.GetMonthFromTransaction(MoneyGlobals.GlobalTransaction);

				//find transaction
				if (month != null)
				{
					foreach (Transaction transaction in month.Transactions.Reverse<Transaction>())
					{
						if (transaction == MoneyGlobals.GlobalTransaction)
						{
                            // modify transaction
                            Transaction oldTransaction = new Transaction(transaction);
							if (Globals.txtAddName.Text != transaction.Name)
							{
								transaction.Name = Globals.txtAddName.Text;
								modified = true;
							}

							if (!string.IsNullOrWhiteSpace(Globals.txtAddSum.Text))
							{
								if (Globals.spiCategory.SelectedItem != null && transaction.Category != CategoriesGlobals.GetCategory(Globals.spiCategory.SelectedItem.ToString()))
								{
									transaction.Category = CategoriesGlobals.GetCategory(Globals.spiCategory.SelectedItem.ToString());
									modified = true;
								}
								else if (Globals.spiCategory.SelectedItem.ToString() == "NO CATEGORY!" && transaction.Category != null)
								{
                                    transaction.Category = null;
                                    modified = true;
                                }
								if (transaction.Sum != Utils.ConvertStringToDouble(Globals.txtAddSum.Text))
								{
									transaction.Sum = Utils.ConvertStringToDouble(Globals.txtAddSum.Text);
									modified = true;
								}
							}
							DateTime oldDate = transaction.Date;
							if (transaction.Tag != TagsGlobals.SearchOrAddNewTag(Globals.txtAddTag.Text))
							{
								transaction.Tag = TagsGlobals.SearchOrAddNewTag(Globals.txtAddTag.Text);
                                modified = true;
							}
							if (transaction.Date != Utils.ConvertStringToDate(Globals.txtAddDate.Text))
							{
								transaction.Date = Utils.ConvertStringToDate(Globals.txtAddDate.Text);
								modified = true;
							}
							if (transaction.HasCarConsumption != Globals.chkHasCarConsumption.Checked)
							{
								transaction.HasCarConsumption = Globals.chkHasCarConsumption.Checked;
								modified = true;
							}

                            if (transaction.HasCarConsumption)
                            {
                                if (transaction.Kilometers != Utils.ConvertStringToDouble(Globals.txtAddKilometersRan.Text))
                                {
                                    transaction.Kilometers = Utils.ConvertStringToDouble(Globals.txtAddKilometersRan.Text);
                                    modified = true;
                                }
                                if (transaction.Liters != Utils.ConvertStringToDouble(Globals.txtAddLiters.Text))
                                {
                                    transaction.Liters = Utils.ConvertStringToDouble(Globals.txtAddLiters.Text);
                                    modified = true;
                                }
                                if (transaction.Price != Utils.ConvertStringToDouble(Globals.txtAddPrice.Text))
                                {
                                    transaction.Price = Utils.ConvertStringToDouble(Globals.txtAddPrice.Text);
                                    modified = true;
                                }
                            }
							else
							{
                                if (transaction.Kilometers != 0)
                                {
                                    transaction.Kilometers = 0;
                                    modified = true;
                                }
                                if (transaction.Liters != 0)
                                {
                                    transaction.Liters = 0;
                                    modified = true;
                                }
                                if (transaction.Price != 0)
                                {
                                    transaction.Price = 0;
                                    modified = true;
                                }
                            }
							if (Globals.chkRepetitiveTransaction.Checked)
							{
							    if (transaction.RepetitiveInterval != ProcessRepetitiveInterval(false))
							    {
                                    transaction.RepetitiveInterval = ProcessRepetitiveInterval(false);
                                    modified = true;
                                }
							}
                            else if (!string.IsNullOrWhiteSpace(transaction.RepetitiveInterval))
                            {
                                transaction.RepetitiveInterval = string.Empty;
                                modified = true;
                            }

                           Utils.CheckTransactionCategoryFlag(transaction);

                            //add to note and car if needed
                            NotesGlobals.AssignTransactionToNote(transaction);
							CarsGlobals.AssignTransactionToCar(transaction, modified ? oldTransaction : null);

							//check if moved in another month and move from xml month to the correct one
							if (oldDate.Month != transaction.Date.Month || oldDate.Year != transaction.Date.Year)
							{
								Month newMonth = Globals.GetMonthFromDate(transaction.Date);
								newMonth.Transactions.Add(transaction);
								//refresh in XML
								Globals.RefreshMonthWithNewTransaction(newMonth);

								month.Transactions.Remove(transaction);
							}

							//refresh in XML
							Globals.RefreshMonthWithNewTransaction(month);

							//refresh tag in transaction state
							Globals.TagToAddInTransaction = false;

                            break;
						}
					}
				}
			}

			//remove from global instance
			NotesGlobals.CheckAll();
			CarsGlobals.CheckAll();
			TagsGlobals.CheckAll();

			MoneyGlobals.GlobalTransaction = null;

			if (modified)
			{
				XML.LoadYears(true);
			}

			if (Globals.SpecificForm != 0)
			{
				SetContentView(Globals.SpecificForm);
			}
			else
			{
				btnMoneyLayoutTop_Click(Globals.btnTransactionsMenu, new EventArgs());
			}
		}

		public void btnModifyTransaction_Load(object sender)
		{
			MoneyGlobals.GlobalTransaction = MoneyGlobals.ViewToTransaction(sender as ImageView);
			if (MoneyGlobals.GlobalTransaction != null)
			{
				SetContentView(Globals.TransactionMenu);
			}
		}

        public void imgCopyTransaction_Click(object sender)
        {
            MoneyGlobals.GlobalTransaction = MoneyGlobals.ViewToTransaction(sender as ImageView);
            if (MoneyGlobals.GlobalTransaction != null)
            {
                MoneyGlobals.IsCopiedTransaction = true;
                SetContentView(Globals.TransactionMenu);
            }
        }


        public void btnRemoveTransaction_Click(object sender)
		{
			Transaction transaction = MoneyGlobals.ViewToTransaction(sender as ImageView);
			if (transaction != null)
			{
				Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
				alert.SetMessage("Remove this transaction?");
				alert.SetPositiveButton("Yes", (senderAlert, args) =>
				{
					if (Globals.CurrentForm == Globals.TransactionsMenu)
					{
						(Globals.grdTransactions.Adapter as TransactionsAdapter).RemoveItem(transaction);
						(Globals.grdTransactions.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);
						MoneyGlobals.WhatTransactionToDisplay();
					}
					else if (Globals.CurrentForm == Globals.CarMenu)
					{
						(Globals.grdCarStuffs.Adapter as TransactionsAdapter).RemoveItem(transaction);
						(Globals.grdCarStuffs.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);
						MoneyGlobals.WhatTransactionToDisplay();
					}
					else if (Globals.CurrentForm == Globals.CarConsumption)
					{
						(Globals.grdCarConsumption.Adapter as TransactionsAdapter).RemoveItem(transaction);
						(Globals.grdCarConsumption.Adapter as TransactionsAdapter).UpdateAdapter(MoneyGlobals.DisplayTransactions);
						CarsGlobals.WhatCarStuffToDisplay();
					}
					else
					{
						Utils.ShowAlert("", "Remove Transaction Form not implemented");
					}
				});
				alert.SetNegativeButton("No", (senderAlert, args) =>
				{
					MoneyGlobals.WhatTransactionToDisplay();
				});
				Dialog dialog = alert.Create();
				dialog.Show();
			}
		}

		#endregion

		#endregion

		#region All Forms Controls With Events For Notes Part

		#region MiniNoteMenu

		public void MiniNoteMenu_Events()
        {
            Globals.layoutMiniNoteSum = FindViewById<LinearLayout>(Id.LayoutMiniNoteSum);
            Globals.layoutAddMiniNote = FindViewById<LinearLayout>(Id.LayoutAddMiniNote);

            Globals.txtMiniNoteSum = FindViewById<EditText>(Id.txtMiniNoteSum);
            Globals.txtMiniNoteDescription = FindViewById<EditText>(Id.txtMiniNoteDescription);

            Globals.textviewMiniNoteAlert = FindViewById<TextView>(Id.textviewMiniNoteAlert);

            Globals.btnModifyMiniNote = FindViewById<Button>(Id.btnModifyMiniNote);
            Globals.btnAddMiniNote = FindViewById<Button>(Id.btnAddMiniNote);
            Globals.btnAddAnotherMiniNote = FindViewById<Button>(Id.btnAddAnotherMiniNote);

            Globals.txtMiniNoteSum.Text = string.Empty;
            Globals.txtMiniNoteDescription.Text = string.Empty;

            if (NotesGlobals.SelectedNote != null && !NotesGlobals.SelectedNote.HasTotal)
            {
                Globals.layoutMiniNoteSum.Visibility = ViewStates.Invisible;
            }

            if (NotesGlobals.SelectedMiniNote != null && !Globals.AddMiniNote)
            {
                Globals.txtMiniNoteSum.Text = NotesGlobals.SelectedMiniNote.Sum.ToString();
                Globals.txtMiniNoteDescription.Text = NotesGlobals.SelectedMiniNote.Description;

                Globals.layoutAddMiniNote.Visibility = ViewStates.Invisible;
                Globals.btnModifyMiniNote.Visibility = ViewStates.Visible;
                Globals.textviewMiniNoteAlert.Visibility = ViewStates.Invisible;
            }
            else
            {
                Globals.layoutAddMiniNote.Visibility = ViewStates.Visible;
                Globals.btnModifyMiniNote.Visibility = ViewStates.Invisible;
                Globals.textviewMiniNoteAlert.Visibility = ViewStates.Invisible;

                if (Globals.layoutMiniNoteSum.Visibility == ViewStates.Visible)
				{
                    ShowInput(Globals.txtMiniNoteSum);
				}
				else
				{
                    ShowInput(Globals.txtMiniNoteDescription);
                }
            }
            if (NotesGlobals.SelectedMiniNote != null && NotesGlobals.SelectedMiniNote.Transaction != null)
            {
                Globals.txtMiniNoteSum.Focusable = false;
                Globals.txtMiniNoteDescription.Focusable = false;
                Globals.btnModifyMiniNote.Visibility = ViewStates.Invisible;
                Globals.layoutAddMiniNote.Visibility = ViewStates.Invisible;
                Globals.textviewMiniNoteAlert.Visibility = ViewStates.Visible;
            }

            Globals.btnModifyMiniNote.Click -= btnAddModifyMiniNote_Click;
            Globals.btnModifyMiniNote.Click += btnAddModifyMiniNote_Click;

            Globals.btnAddMiniNote.Click -= btnAddModifyMiniNote_Click;
            Globals.btnAddMiniNote.Click += btnAddModifyMiniNote_Click;

            Globals.btnAddAnotherMiniNote.Click -= btnAddModifyMiniNote_Click;
            Globals.btnAddAnotherMiniNote.Click += btnAddModifyMiniNote_Click;
        }

        private void btnAddModifyMiniNote_Click(object sender, EventArgs e)
        {
            double sum = Globals.layoutMiniNoteSum.Visibility == ViewStates.Visible ? Utils.ConvertStringToDouble(Globals.txtMiniNoteSum.Text) : 0;
            if (NotesGlobals.SelectedMiniNote == null || Globals.AddMiniNote)
            {
                NotesGlobals.SelectedNote.MiniNotesList.Add(new MiniNote(NotesGlobals.SelectedNote, Globals.txtMiniNoteDescription.Text, sum));
            }
            else// if ((sender as Button) == Globals.btnModifyMiniNote)
            {
                //modify selected
                NotesGlobals.SelectedMiniNote.Description = Globals.txtMiniNoteDescription.Text;
                NotesGlobals.SelectedMiniNote.Sum = sum;
            }

            XML.RefreshNotes();

			if ((sender as Button) == Globals.btnAddAnotherMiniNote)
			{
                MiniNoteMenu_Events();
                Globals.txtMiniNoteSum.Text = string.Empty;
                Globals.txtMiniNoteDescription.Text = string.Empty;
            }
			else
			{
                Globals.AddMiniNote = false;
                SetContentView(Globals.NoteMenu);
			}
        }

        #endregion

        #region NoteSettings

        public void NoteSettings_Events()
        {
            Globals.txtNoteSettingsTitle = FindViewById<EditText>(Id.txtNoteSettingsTitle);
            Globals.txtNoteSettingsDescription = FindViewById<EditText>(Id.txtNoteSettingsDescription);
            Globals.txtNoteSettingsGoalTotal = FindViewById<EditText>(Id.txtNoteSettingsGoalTotal);

            Globals.txtNoteSettingsCreatedDate = FindViewById<TextView>(Id.txtNoteSettingsCreatedDate);
            Globals.txtNoteSettingsLastModified = FindViewById<TextView>(Id.txtNoteSettingsLastModified);
            Globals.txtviewNoteSettingsGoalSum = FindViewById<TextView>(Id.txtviewNoteSettingsGoalSum);

            Globals.btnChangeNoteSettings = FindViewById<Button>(Id.btnChangeNoteSettings);

            Globals.chkHasTotal = FindViewById<CheckBox>(Id.chkHasTotal);
            Globals.chkHasMiniNotes = FindViewById<CheckBox>(Id.chkHasMiniNotes);

            Globals.txtNoteSettingsTitle.Text = NotesGlobals.SelectedNote.Title;
            Globals.txtNoteSettingsDescription.Text = NotesGlobals.SelectedNote.ShortDescription;
            Globals.txtNoteSettingsLastModified.Text = "Last Modified: " + NotesGlobals.SelectedNote.LastModifiedDate.ToShortDateString();
            Globals.txtNoteSettingsCreatedDate.Text = "Date Created: " + NotesGlobals.SelectedNote.CreatedDate.ToShortDateString();
            Globals.chkHasTotal.Checked = NotesGlobals.SelectedNote.HasTotal;
            Globals.chkHasMiniNotes.Checked = NotesGlobals.SelectedNote.HasMiniNotes;
            if (NotesGlobals.SelectedNote.NoteType == NotesGlobals.NoteType.Goal)
            {
                Globals.txtviewNoteSettingsGoalSum.Visibility = ViewStates.Visible;
                Globals.txtNoteSettingsGoalTotal.Visibility = ViewStates.Visible;
                Globals.txtNoteSettingsGoalTotal.Text = NotesGlobals.SelectedNote.TotalGoal.ToString();
            }

            Globals.btnChangeNoteSettings.Click -= btnChangeNoteSettings_Click;
            Globals.btnChangeNoteSettings.Click += btnChangeNoteSettings_Click;
        }

        private void btnChangeNoteSettings_Click(object sender, EventArgs e)
        {
            bool modified = false;
            if (Globals.txtNoteSettingsTitle.Text != NotesGlobals.SelectedNote.Title)
            {
                foreach (Note item in NotesGlobals.NotesList)
                {
                    if (item.Title == Globals.txtNoteSettingsTitle.Text)
                    {
                        Utils.ShowAlert("", "There's Already A Note With The Same Title. Data Not Saved");
                        SetContentView(Globals.ParentForm);
                        return;
                    }
                }
                NotesGlobals.SelectedNote.Title = Globals.txtNoteSettingsTitle.Text;
                modified = true;
            }
            if (Globals.txtNoteSettingsDescription.Text != NotesGlobals.SelectedNote.ShortDescription)
            {
                NotesGlobals.SelectedNote.ShortDescription = Globals.txtNoteSettingsDescription.Text;
                modified = true;
            }
            if (Globals.chkHasMiniNotes.Checked != NotesGlobals.SelectedNote.HasMiniNotes)
            {
                NotesGlobals.SelectedNote.HasMiniNotes = Globals.chkHasMiniNotes.Checked;
                modified = true;
            }
            if (Globals.txtNoteSettingsGoalTotal.Text != "Total:")
            {
                try
                {
                    double temp = Utils.ConvertStringToDouble(Globals.txtNoteSettingsGoalTotal.Text);
                }
                catch
                {
                    Utils.ConvertStringToDouble(Globals.txtNoteSettingsGoalTotal.Text);
                    return;
                }
                if (Utils.ConvertStringToDouble(Globals.txtNoteSettingsGoalTotal.Text) != NotesGlobals.SelectedNote.TotalGoal)
                {
                    NotesGlobals.SelectedNote.TotalGoal = Utils.ConvertStringToDouble(Globals.txtNoteSettingsGoalTotal.Text);
                    modified = true;
                }
            }
            if (Globals.chkHasTotal.Checked != NotesGlobals.SelectedNote.HasTotal)
            {
                NotesGlobals.SelectedNote.HasTotal = Globals.chkHasTotal.Checked;
                if (!NotesGlobals.SelectedNote.HasTotal)
                {
                    foreach (MiniNote item in NotesGlobals.SelectedNote.MiniNotesList)
                    {
                        item.Sum = 0;
                    }
                }
                modified = true;
            }
            if (modified)
            {
                NotesGlobals.SelectedNote.LastModifiedDate = DateTime.Now;
                Globals.txtNoteSettingsLastModified.Text = "Last Modified: " + NotesGlobals.SelectedNote.LastModifiedDate.ToShortDateString();
                XML.RefreshNotes();
            }
            if (NotesGlobals.SelectedNote.HasMiniNotes)
            {
                SetContentView(Globals.NoteMenu);
            }
            else
            {
                SetContentView(Globals.NotesMenu);
            }
            Utils.ShowToast("Note Saved");
        }

        #endregion

        #region NoteMenu

        public void NoteMenu_Events()
        {
            Globals.txtNoteTotalSum = FindViewById<TextView>(Id.txtviewNoteTotalSum);
            Globals.txtNoteTitle = FindViewById<TextView>(Id.txtNoteTitle);
            Globals.txtNoteDescription = FindViewById<TextView>(Id.txtNoteDescription);
            Globals.textviewGoalLeft = FindViewById<TextView>(Id.textviewGoalLeft);
            Globals.textviewGoalTotal = FindViewById<TextView>(Id.textviewGoalTotal);

            Globals.pbGoalProgress = FindViewById<ProgressBar>(Id.pbGoalProgress);

            Globals.btnNoteSettings = FindViewById<Button>(Id.btnNoteSettings);

            Globals.chkAllMiniNotes = FindViewById<CheckBox>(Id.chkAllMiniNotes);

            Globals.imgRemoveAllMiniNotes = FindViewById<ImageView>(Id.imgRemoveAllMiniNotes);
            Globals.imgAddNewMiniNote = FindViewById<ImageView>(Id.imgAddNewMiniNote);

            Globals.grdMiniNotes = FindViewById<GridView>(Id.grdMiniNotes);

            Globals.LayoutGoalProgress = FindViewById<LinearLayout>(Id.LayoutGoalProgress);

            if (NotesGlobals.SelectedNote != null && NotesGlobals.SelectedNote.MiniNotesList.Count > 5)
            {
                Globals.ShowProgressDialog();
            }

            Globals.grdMiniNotes.Adapter = new MiniNotesAdapter(Globals.Activity, NotesGlobals.SelectedNote.MiniNotesList);

            Globals.HideProgressDialog();

            if (!NotesGlobals.SelectedNote.HasTotal)
            {
                Globals.txtNoteTotalSum.Visibility = ViewStates.Invisible;
            }

            (Globals.grdMiniNotes.Adapter as MiniNotesAdapter).CalculateSum();

            if (string.IsNullOrWhiteSpace(NotesGlobals.SelectedNote.ShortDescription))
            {
                Globals.txtNoteDescription.Visibility = ViewStates.Invisible;
            }

            Globals.txtNoteTitle.Text = NotesGlobals.SelectedNote.Title;
            Globals.txtNoteDescription.Text = NotesGlobals.SelectedNote.ShortDescription.Substring(0, NotesGlobals.SelectedNote.ShortDescription.Length > 35 ? 35 : NotesGlobals.SelectedNote.ShortDescription.Length) + (NotesGlobals.SelectedNote.ShortDescription.Length > 40 ? "..." : "");

            Globals.chkAllMiniNotes.CheckedChange -= chkRemoveAllMiniNotes_CheckedChange;
            Globals.chkAllMiniNotes.CheckedChange += chkRemoveAllMiniNotes_CheckedChange;

            Globals.imgRemoveAllMiniNotes.Click -= imgRemoveAllMiniNotes_Click;
            Globals.imgRemoveAllMiniNotes.Click += imgRemoveAllMiniNotes_Click;

            Globals.imgAddNewMiniNote.Click -= imgAddNewMiniNote_Click;
            Globals.imgAddNewMiniNote.Click += imgAddNewMiniNote_Click;

            Globals.btnNoteSettings.Click -= btnNoteSettings_Click;
            Globals.btnNoteSettings.Click += btnNoteSettings_Click;
        }

        private void btnNoteSettings_Click(object sender, EventArgs e)
        {
            SetContentView(Globals.NoteSettings);
        }

        private void imgAddNewMiniNote_Click(object sender, EventArgs e)
        {
            Globals.AddMiniNote = true;
            SetContentView(Globals.MiniNoteMenu);
        }

        private void imgRemoveAllMiniNotes_Click(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Globals.Context);
            alert.SetMessage("Are You Sure You Want To Remove Selected MiniNote" + (NotesGlobals.SelectedNote.MiniNotesList.FindAll(x => x.Checked == true).Count > 1 ? "s" : "") + "?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                (Globals.grdMiniNotes.Adapter as MiniNotesAdapter).RemoveSelected();
                Globals.chkAllMiniNotes.Checked = false;
            });
            alert.SetNegativeButton("No", (senderAlert, args) =>
            {
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void chkRemoveAllMiniNotes_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
			if (!Globals.doNothing)
			{
                (Globals.grdMiniNotes.Adapter as MiniNotesAdapter).CheckAll(e.IsChecked);
			}
        }

        #endregion

        #region NotesMenu

        public void NotesMenu_Events()
        {
            Globals.layoutAddNewNote = FindViewById<HorizontalScrollView>(Id.LayoutAddNewNote);

            Globals.grdNotes = FindViewById<GridView>(Id.grdNotes);

            Globals.imgAddNewNote = FindViewById<ImageView>(Id.imgAddNewNote);
            Globals.imgAddNewGoal = FindViewById<ImageView>(Id.imgAddNewGoal);
            Globals.imgAddNewShopList = FindViewById<ImageView>(Id.imgAddNewShopList);
            Globals.imgDoneNotes = FindViewById<ImageView>(Id.imgDoneNotes);
            Globals.imgRemoveNote = FindViewById<ImageView>(Id.imgRemoveNote);

            Globals.txtNoNotes = FindViewById<TextView>(Id.txtNoNotes);

            Globals.chkNoteCheckAll = FindViewById<CheckBox>(Id.chkNoteCheckAll);

            Globals.layoutAddNewNote.Visibility = Globals.NoteToAddInTransaction || Globals.NoteToAddInTag ? ViewStates.Invisible : ViewStates.Visible;
            Globals.imgDoneNotes.Visibility = Globals.NoteToAddInTransaction || Globals.NoteToAddInTag ? ViewStates.Visible : ViewStates.Invisible;
            Globals.imgRemoveNote.Visibility = ViewStates.Invisible;
            Globals.chkNoteCheckAll.Visibility = Globals.NoteToAddInTransaction || Globals.NoteToAddInTag ? ViewStates.Visible : ViewStates.Invisible;

            Globals.imgAddNewNote.Click -= imgAddNewNoteOrCarOrTagOrCategory_Click;
            Globals.imgAddNewNote.Click += imgAddNewNoteOrCarOrTagOrCategory_Click;

            Globals.imgAddNewGoal.Click -= imgAddNewNoteOrCarOrTagOrCategory_Click;
            Globals.imgAddNewGoal.Click += imgAddNewNoteOrCarOrTagOrCategory_Click;

            Globals.imgAddNewShopList.Click -= imgAddNewNoteOrCarOrTagOrCategory_Click;
            Globals.imgAddNewShopList.Click += imgAddNewNoteOrCarOrTagOrCategory_Click;

            Globals.imgDoneNotes.Click -= imgDoneNoteOrCarOrTagOrCategory_Click;
            Globals.imgDoneNotes.Click += imgDoneNoteOrCarOrTagOrCategory_Click;

            Globals.imgRemoveNote.Click -= imgRemoveNoteOrCarOrTagOrCategory_Click;
            Globals.imgRemoveNote.Click += imgRemoveNoteOrCarOrTagOrCategory_Click;

            Globals.chkNoteCheckAll.Click -= chkNoteOrCarOrTagOrCategoryCheckAll_Click;
            Globals.chkNoteCheckAll.Click += chkNoteOrCarOrTagOrCategoryCheckAll_Click;

            if (NotesGlobals.NotesList?.Count > 5)
            {
                Globals.ShowProgressDialog();
            }

            //Checker to don't assign to note from multiple tags - same as in NotesAdapter - NoteToAddInTag - OldCode
            Tag choosenTag = TagsGlobals.PendingTag;
			if (TagsGlobals.SelectedTag != null)
			{
                choosenTag = TagsGlobals.SelectedTag;
			}
            if (Globals.ParentForm == Globals.TagMenu)
            {
                foreach (Note note in NotesGlobals.NotesList.Reverse<Note>())
                {
                    foreach (Tag tag in TagsGlobals.TagsList)
                    {
                        if (note.Visible && !TagsGlobals.TagMatch(tag, choosenTag) && NotesGlobals.NoteInList(tag.Notes, note))
                        {
                            NotesGlobals.NotesList.Remove(note);
                            NotesGlobals.NotesList.Add(note);
                            note.Visible = false;
                            break;
                        }
                    }
                }
            }

			//prevent adding transactions to shoplists because it's irrelevant
			if (Globals.ParentForm == Globals.TransactionMenu /*|| Globals.ParentForm == Globals.ModifyTransaction*/)
			{
                NotesGlobals.DisplayNotesList = NotesGlobals.NotesList.FindAll(x => x.NoteType != NotesGlobals.NoteType.ShopList);
            }
			else
			{
                NotesGlobals.DisplayNotesList = NotesGlobals.NotesList;
			}

            Globals.grdNotes.Adapter = new NotesAdapter(Globals.Activity, NotesGlobals.DisplayNotesList);

            NotesGlobals.WhatNoteToDisplay();

            if (NotesGlobals.NotesList.Count > 0)
            {
                Globals.grdNotes.BringToFront();
            }
            else
            {
                Globals.txtNoNotes.BringToFront();
            }

            (Globals.grdNotes.Adapter as NotesAdapter).EditState(/*left empty because notetoaddintransaction or notetoaddintag could come here, else false*/);

            Globals.HideProgressDialog();
        }

        public void imgAddNewNoteOrCarOrTagOrCategory_Click(object sender, EventArgs e)
        {
			if (sender is ImageView img)
			{
				if (img == Globals.imgAddNewGoal)
				{
                    NotesGlobals.NotesList.Add(new Note(DateTime.Now, DateTime.Now, NotesGlobals.AlocateNumber("My Goal"), "", null, true, NotesGlobals.NoteType.Goal, 0, true, new List<MiniNote>()));
                    XML.RefreshNotes();
                    NotesGlobals.DisplayNotesList = NotesGlobals.NotesList;
                    Globals.grdNotes.Adapter = new NotesAdapter(Globals.Activity, NotesGlobals.DisplayNotesList);
                    NotesGlobals.WhatNoteToDisplay();
                }
				else if (img == Globals.imgAddNewNote)
				{
                    NotesGlobals.NotesList.Add(new Note(DateTime.Now, DateTime.Now, NotesGlobals.AlocateNumber("My Note"), "", null, false, NotesGlobals.NoteType.Note, 0, false, new List<MiniNote>()));
                    XML.RefreshNotes();
                    NotesGlobals.DisplayNotesList = NotesGlobals.NotesList;
                    Globals.grdNotes.Adapter = new NotesAdapter(Globals.Activity, NotesGlobals.DisplayNotesList);
                    NotesGlobals.WhatNoteToDisplay();
                }
				else if (img == Globals.imgAddNewShopList)
				{
                    NotesGlobals.NotesList.Add(new Note(DateTime.Now, DateTime.Now, NotesGlobals.AlocateNumber("My Shop List"), "", null, true, NotesGlobals.NoteType.ShopList, 0, true, new List<MiniNote>()));
                    XML.RefreshNotes();
                    NotesGlobals.DisplayNotesList = NotesGlobals.NotesList;
                    Globals.grdNotes.Adapter = new NotesAdapter(Globals.Activity, NotesGlobals.DisplayNotesList);
                    NotesGlobals.WhatNoteToDisplay();
                }
				else if (img == Globals.imgAddNewCar)
                {
                    CarsGlobals.NewCarWasAdded = false;
                    CarsGlobals.AddNewCar = true;
                    SetContentView(Globals.CarMenu);
                }
                else if (img == Globals.imgAddNewTag)
                {
                    TagsGlobals.AddNewTag = true;
                    SetContentView(Globals.TagMenu);
                }
                else if (img == Globals.imgAddNewCategory)
                {
                    CategoriesGlobals.AddNewCategory = true;
                    SetContentView(Globals.CategoryMenu);
                }
            }
        }

        public void imgDoneNoteOrCarOrTagOrCategory_Click(object sender, EventArgs e)
        {
            bool done = false;
            if (Globals.NoteToAddInTransaction)
            {
                (Globals.grdNotes.Adapter as NotesAdapter).NoteToAddInTransaction();

                bool temp1 = Globals.NoteToAddInTransaction;
                bool temp2 = Globals.NoteToAddInTag;
                Globals.NoteToAddInTransaction = false;
                Globals.NoteToAddInTag = false;
                (Globals.grdNotes.Adapter as NotesAdapter).EditState();
                Globals.NoteToAddInTransaction = temp1;
                Globals.NoteToAddInTag = temp2;
                done = true;
            }
			if (Globals.CarToAddInTransaction)
            {
                (Globals.grdCars.Adapter as CarsAdapter).CarToAddInTransaction();
                bool temp1 = Globals.CarToAddInTransaction;
                bool temp2 = Globals.CarToAddInTag;
                Globals.CarToAddInTransaction = false;
                Globals.CarToAddInTag = false;
                (Globals.grdCars.Adapter as CarsAdapter).EditState();
                Globals.CarToAddInTransaction = temp1;
                Globals.CarToAddInTag = temp2;
                done = true;
            }
            if (Globals.TagToAddInTransaction)
            {
                (Globals.grdTags.Adapter as TagsAdapter).TagToAddInTransaction();
                bool temp = Globals.TagToAddInTransaction;
                Globals.TagToAddInTransaction = false;
                (Globals.grdTags.Adapter as TagsAdapter).EditState();
                Globals.TagToAddInTransaction = temp;
                done = true;
            }
            if (Globals.NoteToAddInTag)
            {
                (Globals.grdNotes.Adapter as NotesAdapter).NoteToAddInTag();
                (Globals.grdNotes.Adapter as NotesAdapter).EditState();
                NotesGlobals.VisibleAll();
                done = true;
            }
            if (Globals.CarToAddInTag)
            {
                (Globals.grdCars.Adapter as CarsAdapter).CarToAddInTag();
                (Globals.grdCars.Adapter as CarsAdapter).EditState();
                CarsGlobals.VisibleAll();
                done = true;
            }
            if (!done)
            {
				if (sender is ImageView btn)
				{
					if (btn == Globals.imgDoneNotes)
					{
                        (Globals.grdNotes.Adapter as NotesAdapter).EditState();
                        (Globals.grdNotes.Adapter as NotesAdapter).RevertChanges();
                        (Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
					}
					else if (btn == Globals.imgDoneCars)
					{
                        (Globals.grdCars.Adapter as CarsAdapter).EditState();
                        (Globals.grdCars.Adapter as CarsAdapter).RevertChanges();
                        (Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();
                    }
                    else if (btn == Globals.imgDoneTags)
                    {
                        (Globals.grdTags.Adapter as TagsAdapter).EditState();
                        (Globals.grdTags.Adapter as TagsAdapter).RevertChanges();
                        (Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter();
                    }
                    else if (btn == Globals.imgDoneCategories)
                    {
                        (Globals.grdCategories.Adapter as CategoriesAdapter).EditState();
                        (Globals.grdCategories.Adapter as CategoriesAdapter).RevertChanges();
                        (Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter();
                    }
                }
            }
			else
			{
                SetContentView(Globals.PreviousForm);
            }
        }

        public void imgRemoveNoteOrCarOrTagOrCategory_Click(object sender, EventArgs e)
        {
			if (sender is ImageView btn)
			{
                string message = string.Empty;
                if (btn == Globals.imgRemoveNote)
                {
                    message = "Are You Sure You Want To Remove The Note" + (NotesGlobals.NotesList.FindAll(x => x.Checked).Count > 1 ? "s" : "") + " ?";
                }
                else if (btn == Globals.imgRemoveCar)
				{
                    message = "Are You Sure You Want To Remove The Car" + (CarsGlobals.CarsList.FindAll(x => x.Checked).Count > 1 ? "s" : "") + " ?";
                }
                else if (btn == Globals.imgRemoveTag)
                {
                    message = "Are You Sure You Want To Remove The Tag" + (TagsGlobals.DisplayTagsList.FindAll(x => x.Checked).Count > 1 ? "s" : "") + " ?";
                }
                else if (btn == Globals.imgRemoveCategory)
                {
                    message = "Are You Sure You Want To Remove The Categor" + (CategoriesGlobals.DisplayCategoriesList.FindAll(x => x.Checked).Count > 1 ? "ies" : "y") + " ?";
                }

                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
                alert.SetTitle("Remove?");
                alert.SetMessage(message);
                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
					if (btn == Globals.imgRemoveCar)
					{
                        (Globals.grdCars.Adapter as CarsAdapter).RemoveSelected();
                    }
                    else if (btn == Globals.imgRemoveNote)
					{
                        (Globals.grdNotes.Adapter as NotesAdapter).RemoveSelected();
					}
                    else if (btn == Globals.imgRemoveTag)
                    {
                        (Globals.grdTags.Adapter as TagsAdapter).RemoveSelected();
                    }
                    else if (btn == Globals.imgRemoveCategory)
                    {
                        (Globals.grdCategories.Adapter as CategoriesAdapter).RemoveSelected();
                    }
                });
                alert.SetNeutralButton("No", (senderAlert, args) =>
                {
                    if (btn == Globals.imgRemoveCar)
                    {
                        (Globals.grdCars.Adapter as CarsAdapter).EditState();
                        (Globals.grdCars.Adapter as CarsAdapter).CheckAll();
                        (Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();
                    }
                    else if (btn == Globals.imgRemoveNote)
                    {
                        (Globals.grdNotes.Adapter as NotesAdapter).EditState();
                        (Globals.grdNotes.Adapter as NotesAdapter).CheckAll();
                        (Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
                    }
                    else if (btn == Globals.imgRemoveTag)
                    {
                        (Globals.grdTags.Adapter as TagsAdapter).EditState();
                        (Globals.grdTags.Adapter as TagsAdapter).CheckAll();
                        (Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter();
                    }
                    else if (btn == Globals.imgRemoveCategory)
                    {
                        (Globals.grdCategories.Adapter as CategoriesAdapter).EditState();
                        (Globals.grdCategories.Adapter as CategoriesAdapter).CheckAll();
                        (Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter();
                    }
                });
                Dialog dialog = alert.Create();
                dialog.Show();
			}
        }

        public void chkNoteOrCarOrTagOrCategoryCheckAll_Click(object sender, EventArgs e)
        {
			if (!Globals.doNothing && sender is Button btn)
			{
                CheckBox t = sender as CheckBox;
				if (btn == Globals.chkNoteCheckAll)
				{
                    (Globals.grdNotes.Adapter as NotesAdapter).CheckAll(t.Checked);
				}
				else if (btn == Globals.chkCarCheckAll)
				{
                    (Globals.grdCars.Adapter as CarsAdapter).CheckAll(t.Checked);
                }
                else if (btn == Globals.chkTagsCheckAll)
                {
                    (Globals.grdTags.Adapter as TagsAdapter).CheckAll(t.Checked);
                }
                else if (btn == Globals.chkCategoriesCheckAll)
                {
                    (Globals.grdCategories.Adapter as CategoriesAdapter).CheckAll(t.Checked);
                }
            }
        }

        #endregion

        #endregion

        #region All Forms Controls With Events For Cars Part

        #region CarsMenu

        public void CarsMenu_Events()
        {
            Globals.layoutAddNewCar = FindViewById<LinearLayout>(Id.LayoutAddNewCar);

            Globals.grdCars = FindViewById<GridView>(Id.grdCars);

            Globals.imgAddNewCar = FindViewById<ImageView>(Id.imgAddNewCar);
            Globals.imgAddNewGoal = FindViewById<ImageView>(Id.imgAddNewGoal);
            Globals.imgDoneCars = FindViewById<ImageView>(Id.imgDoneCars);
            Globals.imgRemoveCar = FindViewById<ImageView>(Id.imgRemoveCar);

            Globals.txtNoCars = FindViewById<TextView>(Id.txtNoCars);

            Globals.chkCarCheckAll = FindViewById<CheckBox>(Id.chkCarCheckAll);

            Globals.layoutAddNewCar.Visibility = Globals.CarToAddInTransaction || Globals.CarToAddInTag ? ViewStates.Invisible : ViewStates.Visible;
            Globals.imgDoneCars.Visibility = Globals.CarToAddInTransaction || Globals.CarToAddInTag ? ViewStates.Visible : ViewStates.Invisible;
            Globals.imgRemoveCar.Visibility = ViewStates.Invisible;
            Globals.chkCarCheckAll.Visibility = Globals.CarToAddInTransaction || Globals.CarToAddInTag ? ViewStates.Visible : ViewStates.Invisible;

            Globals.imgAddNewCar.Click -= imgAddNewNoteOrCarOrTagOrCategory_Click;
            Globals.imgAddNewCar.Click += imgAddNewNoteOrCarOrTagOrCategory_Click;

            Globals.imgDoneCars.Click -= imgDoneNoteOrCarOrTagOrCategory_Click;
            Globals.imgDoneCars.Click += imgDoneNoteOrCarOrTagOrCategory_Click;

            Globals.imgRemoveCar.Click -= imgRemoveNoteOrCarOrTagOrCategory_Click;
            Globals.imgRemoveCar.Click += imgRemoveNoteOrCarOrTagOrCategory_Click;

            Globals.chkCarCheckAll.Click -= chkNoteOrCarOrTagOrCategoryCheckAll_Click;
            Globals.chkCarCheckAll.Click += chkNoteOrCarOrTagOrCategoryCheckAll_Click;

			if (CarsGlobals.CarsList?.Count > 5)
			{
                Globals.ShowProgressDialog();
            }

            //Checker to don't assign to car from multiple tags - same as in CarsAdapter - CarToAddInTag - OldCode
            Tag choosenTag = TagsGlobals.PendingTag;
            if (TagsGlobals.SelectedTag != null)
            {
                choosenTag = TagsGlobals.SelectedTag;
            }
            if (Globals.ParentForm == Globals.TagMenu)
            {
                foreach (Car car in CarsGlobals.CarsList.Reverse<Car>())
                {
                    foreach (Tag tag in TagsGlobals.TagsList)
                    {
                        if (car.Visible && !TagsGlobals.TagMatch(tag, choosenTag) && CarsGlobals.CarInList(tag.Cars, car))
                        {
                            CarsGlobals.CarsList.Remove(car);
                            CarsGlobals.CarsList.Add(car);
                            car.Visible = false;
                            break;
                        }
                    }
                }
            }

            Globals.grdCars.Adapter = new CarsAdapter(Globals.Activity, CarsGlobals.CarsList);

            CarsGlobals.WhatCarToDisplay();

            if (CarsGlobals.CarsList.Count > 0)
            {
                Globals.grdCars.BringToFront();
            }
            else
            {
                Globals.txtNoCars.BringToFront();
            }

            (Globals.grdCars.Adapter as CarsAdapter).EditState(/*left empty because cartoaddintransaction or cartoaddintag could come here, else false*/);
            (Globals.grdCars.Adapter as CarsAdapter).UpdateAdapter();

            CarsGlobals.SelectedCar = null;

            Globals.HideProgressDialog();


            CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Transaction;
        }

        #endregion

        #region CarMenu

        public void CarMenu_Events()
		{
            Globals.txtCarProperties = FindViewById<TextView>(Id.txtCarProperties);

            Globals.txtNoCarObjectsOrRepairs = FindViewById<TextView>(Id.txtNoCarObjectsOrRepairs);
            Globals.txtNoCarStuffs = FindViewById<TextView>(Id.txtNoCarStuffs);

            Globals.btnShowCarObjects = FindViewById<Button>(Id.btnShowCarObjects);
            Globals.btnCarTransactions = FindViewById<Button>(Id.btnCarTransactions);
            Globals.btnCarConsumption = FindViewById<Button>(Id.btnCarConsumption);
            Globals.btnCarRepairHistory = FindViewById<Button>(Id.btnCarRepairHistory);
            Globals.btnCarProperties = FindViewById<Button>(Id.btnCarProperties);
            Globals.btnCarTags = FindViewById<Button>(Id.btnCarTags);

            Globals.imgAddNewObject = FindViewById<ImageView>(Id.imgAddNewObject);

            Globals.grdCarObjectsOrRepairs = FindViewById<GridView>(Id.grdCarObjectsOrRepairs);
            Globals.grdCarStuffs = FindViewById<GridView>(Id.grdCarStuffs);

            Globals.layoutCarCarObjectsRepairs = FindViewById<LinearLayout>(Id.layoutCarCarObjects);
            Globals.layoutCarStuffs = FindViewById<LinearLayout>(Id.layoutCarStuffs);

            if (CarsGlobals.AddNewCar && CarsGlobals.SelectedCar == null && Globals.PreviousForm != Globals.CarObject)
			{
                CarsGlobals.SelectedCar = new Car();

                Globals.grdCarObjectsOrRepairs.Adapter = new CarObjectsAdapter(Globals.Activity, CarsGlobals.SelectedCar.CarObjects);

                (Globals.grdCarObjectsOrRepairs.Adapter as CarObjectsAdapter).AddCarObject("ITP");
                (Globals.grdCarObjectsOrRepairs.Adapter as CarObjectsAdapter).AddCarObject("RCA");
                (Globals.grdCarObjectsOrRepairs.Adapter as CarObjectsAdapter).AddCarObject("CASCO");
                (Globals.grdCarObjectsOrRepairs.Adapter as CarObjectsAdapter).UpdateAdapter(CarsGlobals.SelectedCar.CarObjects);

                SetContentView(Globals.CarPropertiesMenu);
                Globals.PreviousForm = Globals.CarMenu;
                CarsGlobals.AddNewCar = false;
            }
            else
			{
                Globals.txtCarProperties.Text = CarsGlobals.SelectedCar.Plate + " " + CarsGlobals.SelectedCar.Brand + " " + CarsGlobals.SelectedCar.Model + " (" + Convert.ToString(CarsGlobals.SelectedCar.Year) + ")";

				if (CarsGlobals.SelectedCar?.CarObjects?.Count > 5)
				{
                    Globals.ShowProgressDialog();
				}

                Globals.grdCarObjectsOrRepairs.Adapter = new CarObjectsAdapter(Globals.Activity, CarsGlobals.SelectedCar.CarObjects);

                Globals.HideProgressDialog();
            }

            if (CarsGlobals.SelectedCar?.Transactions?.Count > 5)
            {
                Globals.ShowProgressDialog();
            }


            Globals.HideProgressDialog();

            Globals.SpecificForm = Globals.CurrentForm;

            Globals.imgAddNewObject.Click -= ImgAddNewObjectOrRepair_Click;
            Globals.imgAddNewObject.Click += ImgAddNewObjectOrRepair_Click;

            Globals.btnShowCarObjects.Click -= BtnShowCarStuff_Click;
            Globals.btnShowCarObjects.Click += BtnShowCarStuff_Click;

            Globals.btnCarConsumption.Click -= BtnShowCarStuff_Click;
            Globals.btnCarConsumption.Click += BtnShowCarStuff_Click;

            Globals.btnCarRepairHistory.Click -= BtnShowCarStuff_Click;
            Globals.btnCarRepairHistory.Click += BtnShowCarStuff_Click;

            Globals.btnCarTransactions.Click -= BtnShowCarStuff_Click;
            Globals.btnCarTransactions.Click += BtnShowCarStuff_Click;

            Globals.btnCarTags.Click -= BtnShowCarStuff_Click;
            Globals.btnCarTags.Click += BtnShowCarStuff_Click;

            Globals.btnCarProperties.Click -= BtnShowCarStuff_Click;
            Globals.btnCarProperties.Click += BtnShowCarStuff_Click;

			if (CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Properties || CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Consumption)
			{
                CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Transaction;
			}

            BtnShowCarStuff_Click(null, null);

            if (Globals.PreviousForm == Globals.CarObject)
			{
                CarsGlobals.SelectedCarObject = null;
			}
			if (Globals.PreviousForm == Globals.RepairMenu)
			{
                CarsGlobals.SelectedCarRepair = null;
			}
        }

		private void BtnShowCarStuff_Click(object sender, EventArgs e)
		{
            Button btn = sender as Button;

			if (btn == Globals.btnShowCarObjects || (btn == null && CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Object))
			{
                CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Object;
                CarsGlobals.WhatCarStuffToDisplay();

                Globals.grdCarObjectsOrRepairs.Adapter = new CarObjectsAdapter(Globals.Activity, CarsGlobals.SelectedCar.CarObjects);
            }
			else if (btn == Globals.btnCarRepairHistory || (btn == null && CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Repair))
			{
                CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Repair;
                CarsGlobals.WhatCarStuffToDisplay();

                Globals.grdCarObjectsOrRepairs.Adapter = new RepairsAdapter(Globals.Activity, CarsGlobals.SelectedCar.Repairs);
			}
			else if (btn == Globals.btnCarTransactions || (btn == null && CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Transaction))
			{
                CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Transaction;
                MoneyGlobals.DisplayTransactions = CarsGlobals.SelectedCar.Transactions;
                MoneyGlobals.DisplayTransactions = Sorting.SortTransactions(MoneyGlobals.DisplayTransactions);
                CarsGlobals.WhatCarStuffToDisplay();

                Globals.grdCarStuffs.Adapter = new TransactionsAdapter(Globals.Activity, MoneyGlobals.DisplayTransactions);
            }
            else if (btn == Globals.btnCarTags || (btn == null && CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Tag))
            {
                CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Tag;
                CarsGlobals.WhatCarStuffToDisplay();

                //used to bind grdTags used in TagsAdapter with current from this Form(CarMenu)
                foreach (Tag tag in TagsGlobals.TagsList)
				{
					if (CarsGlobals.CarInList(tag.Cars, CarsGlobals.SelectedCar) && !TagsGlobals.TagInList(TagsGlobals.DisplayTagsList, tag))
					{
                        TagsGlobals.DisplayTagsList.Add(tag);
					}
				}

                Globals.grdTags = Globals.grdCarStuffs;
                Globals.grdCarStuffs.Adapter = new TagsAdapter(Globals.Activity, TagsGlobals.DisplayTagsList);
            }
            else if (btn == Globals.btnCarConsumption || (btn == null && CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Consumption))
            {
                CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Consumption;
                SetContentView(Globals.CarConsumption);
            }
            else if (btn == Globals.btnCarProperties || (btn == null && CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Properties))
			{
                CarsGlobals.PopulatedAdapter = CarsGlobals.CarElement.Properties;
                SetContentView(Globals.CarPropertiesMenu);
                Globals.PreviousForm = Globals.CarMenu;
            }
		}

		private void ImgAddNewObjectOrRepair_Click(object sender, EventArgs e)
		{
			if (CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Object)
			{
                CarsGlobals.AddNewCarObject = true;
                CarsGlobals.SelectedCarObject = null;
                SetContentView(Globals.CarObject);
			}
			else if (CarsGlobals.PopulatedAdapter == CarsGlobals.CarElement.Repair)
			{
                CarsGlobals.AddNewCarRepair = true;
                CarsGlobals.SelectedCarRepair = null;
                SetContentView(Globals.RepairMenu);
            }
        }

		#endregion

		#region CarObject

		public void CarObject_Events()
		{
            Globals.txtCarObjectName = FindViewById<EditText>(Id.txtCarObjectName);
            Globals.txtCarObjectBeginDate = FindViewById<EditText>(Id.txtCarObjectBeginDate);
            Globals.txtCarObjectEndDate = FindViewById<EditText>(Id.txtCarObjectEndDate);

            Globals.btnAddOrModifyCarObject = FindViewById<Button>(Id.btnAddOrModifyCarObject);
            Globals.btnCarObjectBeginDate = FindViewById<Button>(Id.btnCarObjectBeginDate);
            Globals.btnCarObjectEndDate = FindViewById<Button>(Id.btnCarObjectEndDate);

			if (CarsGlobals.AddNewCarObject && CarsGlobals.SelectedCarObject == null)
			{
                Globals.txtCarObjectName.Text = string.Empty;
                Globals.txtCarObjectBeginDate.Text = DateTime.Now.ToShortDateString();
                Globals.txtCarObjectEndDate.Text = DateTime.Now.AddYears(1).ToShortDateString();
                ShowInput(Globals.txtCarObjectName);
            }
			else
			{
                Globals.txtCarObjectName.Text = CarsGlobals.SelectedCarObject.Name;
                Globals.txtCarObjectBeginDate.Text = CarsGlobals.SelectedCarObject.BeginDate.ToShortDateString();
                Globals.txtCarObjectEndDate.Text = CarsGlobals.SelectedCarObject.EndDate.ToShortDateString();
			}

            Globals.btnAddOrModifyCarObject.Text = CarsGlobals.AddNewCarObject ? "Add" : "Modify";


            Globals.btnAddOrModifyCarObject.Click -= btnAddOrModifyCarObject_Click;
            Globals.btnAddOrModifyCarObject.Click += btnAddOrModifyCarObject_Click;

            Globals.btnCarObjectBeginDate.Click -= BtnCarObjectBeginDate_Click;
			Globals.btnCarObjectBeginDate.Click += BtnCarObjectBeginDate_Click;

            Globals.btnCarObjectEndDate.Click -= BtnCarObjectEndDate_Click;
			Globals.btnCarObjectEndDate.Click += BtnCarObjectEndDate_Click;
        }

		private void BtnCarObjectEndDate_Click(object sender, EventArgs e)
		{
            Utils.Dpd_DateSet = DpdCarObjectEndDate_DateSet;
            Utils.DateSet = Globals.txtCarObjectEndDate.Text;
            Utils.ShowDatePicker(Utils.ConvertStringToDate(Utils.DateSet));
        }

        private void DpdCarObjectEndDate_DateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Globals.txtCarObjectEndDate.Text = new DateTime(e.Year, e.Month + 1, e.DayOfMonth).ToShortDateString();
        }

        private void BtnCarObjectBeginDate_Click(object sender, EventArgs e)
		{
            Utils.Dpd_DateSet = DpdCarObjectBeginDate_DateSet;
            Utils.DateSet = Globals.txtCarObjectBeginDate.Text;
            Utils.ShowDatePicker(Utils.ConvertStringToDate(Utils.DateSet));
        }

        private void DpdCarObjectBeginDate_DateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Globals.txtCarObjectBeginDate.Text = new DateTime(e.Year, e.Month + 1, e.DayOfMonth).ToShortDateString();
        }

        private void btnAddOrModifyCarObject_Click(object sender, EventArgs e)
		{
			if (CarsGlobals.AddNewCarObject)
			{
                string name = Globals.txtCarObjectName.Text;
                DateTime beginDate = Utils.ConvertStringToDate(Globals.txtCarObjectBeginDate.Text);
                DateTime endDate = Utils.ConvertStringToDate(Globals.txtCarObjectEndDate.Text);
                if (name == string.Empty)
                {
                    ShowInput(Globals.txtCarObjectName);
                    Utils.ShowToast("Invalid Name");
                }
                else if (beginDate.ToOADate() > endDate.ToOADate())
                {
                    ShowInput(Globals.txtCarObjectEndDate);
                    Utils.ShowToast("End Date Can't Be Before Begin Date");
                }
				else
				{
                    SetContentView(Globals.CarMenu);
                    Globals.PreviousForm = Globals.CarObject;
                    (Globals.grdCarObjectsOrRepairs.Adapter as CarObjectsAdapter).AddCarObject(name, beginDate, endDate);
                    XML.RefreshCars();
                    Utils.ShowToast(name + " Was Added!");
                }
			}
			else
			{
                bool modified = false;
                //modify
                if (Globals.txtCarObjectName.Text != CarsGlobals.SelectedCarObject.Name)
                {
                    modified = true;
                }
                else if (Globals.txtCarObjectBeginDate.Text != CarsGlobals.SelectedCarObject.BeginDate.ToShortDateString())
                {
                    modified = true;
                }
                else if (Globals.txtCarObjectEndDate.Text != CarsGlobals.SelectedCarObject.EndDate.ToShortDateString())
                {
                    modified = true;
                }
				if (modified)
				{
                    int index = CarsGlobals.CarObjectIndexInList(CarsGlobals.SelectedCar.CarObjects, CarsGlobals.SelectedCarObject);
                    if (index != -1)
				    {
                        CarsGlobals.SelectedCar.CarObjects[index] = CarsGlobals.SelectedCarObject;
                        CarsGlobals.SelectedCar.CarObjects[index].Name = Globals.txtCarObjectName.Text;
                        CarsGlobals.SelectedCar.CarObjects[index].BeginDate = Utils.ConvertStringToDate(Globals.txtCarObjectBeginDate.Text);
                        CarsGlobals.SelectedCar.CarObjects[index].EndDate = Utils.ConvertStringToDate(Globals.txtCarObjectEndDate.Text);
                        XML.RefreshCars();
                        Utils.ShowToast(CarsGlobals.SelectedCar.CarObjects[index].Name + " Was Modified!");
                    }
                }
			}
        }

		#endregion

		#region CarConsumption

        public void CarConsumption_Events()
		{
            Globals.grdCarConsumption = FindViewById<GridView>(Id.grdCarConsumption);

            Globals.tvwTransactionsNoData = FindViewById<TextView>(Id.tvwTransactionsNoData);
            Globals.txtCarConsumptionFilters = FindViewById<TextView>(Id.txtCarConsumptionFilters);
            Globals.txtCarConsumptionSorts = FindViewById<TextView>(Id.txtCarConsumptionSorts);
            Globals.txtTotalLiters = FindViewById<TextView>(Id.txtTotalLiters);
            Globals.txtTotalKilometersRan = FindViewById<TextView>(Id.txtTotalKilometersRan);
            Globals.txtMediumConsumption = FindViewById<TextView>(Id.txtMediumConsumption);
            Globals.txtMediumPricePerLiter = FindViewById<TextView>(Id.txtMediumPricePerLiter);
            Globals.txtMediumPricePerKilometer = FindViewById<TextView>(Id.txtMediumPricePerKilometer);
            Globals.txtSince = FindViewById<TextView>(Id.txtSince);

            Globals.btnConsumFilter = FindViewById<Button>(Id.btnConsumFilter);
            Globals.btnConsumClearFilter = FindViewById<Button>(Id.btnConsumClearFilter);
            Globals.btnConsumSort = FindViewById<Button>(Id.btnConsumSort);

            Globals.btnConsumFilter.Click -= BtnConsumFilterOrSort_Click;
            Globals.btnConsumFilter.Click += BtnConsumFilterOrSort_Click;

            Globals.btnConsumClearFilter.Click -= btnTransactionsClearFilter_Click;
            Globals.btnConsumClearFilter.Click += btnTransactionsClearFilter_Click;

            Globals.btnConsumSort.Click -= BtnConsumFilterOrSort_Click;
            Globals.btnConsumSort.Click += BtnConsumFilterOrSort_Click;

            Globals.SpecificForm = Globals.CurrentForm;

            MoneyGlobals.DisplayTransactions = CarsGlobals.SelectedCar.Transactions.FindAll(x => x.HasCarConsumption);
            MoneyGlobals.DisplayTransactions = Filtering.FilterTransactions(MoneyGlobals.DisplayTransactions, Filtering.BeginDate, Filtering.EndDate);
            MoneyGlobals.DisplayTransactions = Sorting.SortTransactions(MoneyGlobals.DisplayTransactions);

            Filtering.AllFilters = Filtering.DateFilter;
            if (Filtering.AllFilters != "This Month")
            {
                MoneyGlobals.ClearFilters = true;
            }
            if (Filtering.CategoryOrTagFilter != "No Category;None")
            {
                Filtering.AllFilters += "; " + Filtering.CategoryOrTagFilter;
                MoneyGlobals.ClearFilters = true;
            }
            if (Filtering.SumFilter.Length > 0)
            {
                Filtering.AllFilters += "; " + Filtering.SumFilter;
                MoneyGlobals.ClearFilters = true;
            }
            
            Globals.btnConsumClearFilter.Visibility = MoneyGlobals.ClearFilters ? ViewStates.Visible : ViewStates.Invisible;
            Globals.txtCarConsumptionFilters.Text = "Filter: " + Filtering.AllFilters;
            Globals.txtCarConsumptionSorts.Text = "Order By: " + Sorting.DisplayCurrentSort;

            double TotalLiters = 0, TotalKilometersRan = 0, MediumConsumption = 0, MediumPrice = 0, TotalPaid = 0, MediumKilometer = 0;

            foreach (Transaction item in MoneyGlobals.DisplayTransactions)
			{
                TotalLiters += item.Liters;
                TotalKilometersRan += item.Kilometers;
                TotalPaid += item.Price * item.Liters;
            }

            MediumConsumption = TotalLiters / TotalKilometersRan * 100;
            MediumPrice = TotalPaid / TotalLiters;
            MediumKilometer = TotalPaid / TotalKilometersRan;

            Globals.txtTotalLiters.Text = "Total " + UnitType.GetUnitVolumeType() + "s: " + TotalLiters.ToString("#,###,##0.000");
            Globals.txtTotalKilometersRan.Text = "Total " + UnitType.GetUnitLengthType() + "s: " + TotalKilometersRan.ToString("#,###,##0.000");
            Globals.txtMediumConsumption.Text = "Avg. Consumption: " + MediumConsumption.ToString("#,###,##0.000");
            Globals.txtMediumPricePerLiter.Text = "Avg. Price per " + UnitType.GetUnitVolumeType() + " : " + MediumPrice.ToString("#,###,##0.000");
            Globals.txtMediumPricePerKilometer.Text = "Avg. Price per " + UnitType.GetUnitLengthType() + " : " + MediumKilometer.ToString("#,###,##0.000");
            Globals.txtSince.Text = MoneyGlobals.DisplayTransactions.Count > 0 ? "Since: " + MoneyGlobals.DisplayTransactions[MoneyGlobals.DisplayTransactions.Count - 1].Date.ToShortDateString() : "";

			if (MoneyGlobals.DisplayTransactions?.Count > 5)
			{
                Globals.ShowProgressDialog();
			}

            Globals.grdCarConsumption.Adapter = new TransactionsAdapter(this, MoneyGlobals.DisplayTransactions);

            CarsGlobals.WhatCarStuffToDisplay();

            Globals.HideProgressDialog();
        }

		private void BtnConsumFilterOrSort_Click(object sender, EventArgs e)
		{
			if (sender is Button btn)
			{
				if (btn == Globals.btnConsumFilter)
				{
                    SetContentView(Globals.Filters);
                    Globals.PreviousForm = Globals.CarConsumption;
				}
				else if (btn == Globals.btnConsumSort)
				{
                    SetContentView(Globals.Sorts);
                    Globals.PreviousForm = Globals.CarConsumption;
                }
			}
		}

		#endregion

		#region CarProperties

		public void CarProperties_Events()
        {
            Globals.txtAddCarBrand = FindViewById<EditText>(Id.txtAddCarBrand);
            Globals.txtAddCarModel = FindViewById<EditText>(Id.txtAddCarModel);
            Globals.txtAddCarYear = FindViewById<EditText>(Id.txtAddCarYear);
            Globals.txtAddCarPlate = FindViewById<EditText>(Id.txtAddCarPlate);

            Globals.btnAddOrModifyCarProperties = FindViewById<Button>(Id.btnAddOrModifyCarProperties);

            Globals.txtAddCarPlate.Text = CarsGlobals.SelectedCar.Plate;
            Globals.txtAddCarBrand.Text = CarsGlobals.SelectedCar.Brand;
            Globals.txtAddCarModel.Text = CarsGlobals.SelectedCar.Model;
            Globals.txtAddCarYear.Text = Convert.ToString(CarsGlobals.SelectedCar.Year);
			if (Globals.txtAddCarYear.Text.Length != 4)
			{
                Globals.txtAddCarYear.Text = string.Empty;
            }

            Globals.txtAddCarPlate.TextChanged -= TxtAddCar_TextChanged;
            Globals.txtAddCarPlate.TextChanged += TxtAddCar_TextChanged;

            Globals.txtAddCarBrand.TextChanged -= TxtAddCar_TextChanged;
            Globals.txtAddCarBrand.TextChanged += TxtAddCar_TextChanged;

            Globals.txtAddCarModel.TextChanged -= TxtAddCar_TextChanged;
            Globals.txtAddCarModel.TextChanged += TxtAddCar_TextChanged;

            Globals.txtAddCarYear.TextChanged -= TxtAddCar_TextChanged;
            Globals.txtAddCarYear.TextChanged += TxtAddCar_TextChanged;

            Globals.btnAddOrModifyCarProperties.Text = CarsGlobals.NewCarWasAdded ? "Modify" : "Add New Car";
            Globals.btnAddOrModifyCarProperties.Click -= BtnAddOrModifyCarProperties_Click;
            Globals.btnAddOrModifyCarProperties.Click += BtnAddOrModifyCarProperties_Click;

			if (CarsGlobals.SelectedCar != null && CarsGlobals.AddNewCar)
			{
                ShowInput(Globals.txtAddCarPlate);
			}
        }

		private void BtnAddOrModifyCarProperties_Click(object sender, EventArgs e)
		{
            string name = Globals.txtAddCarPlate.Text.ToUpper();
            if (!CarsGlobals.NewCarWasAdded)
            {
                //check if plate exists already - should be unique
                if (-1 != CarsGlobals.CarsList.FindIndex(x => x.Plate == name))
                {
                    ShowInput(Globals.txtAddCarPlate);
                    Utils.ShowAlert("", "Plate Already Exists");
                    return;
                }
                if (name == string.Empty)
                {
                    ShowInput(Globals.txtAddCarPlate);
                    Utils.ShowToast("Invalid Plate");
                }
                else if (Globals.txtAddCarBrand.Text == string.Empty)
                {
                    ShowInput(Globals.txtAddCarBrand);
                    Utils.ShowToast("Invalid Brand");
                }
                else if (Globals.txtAddCarModel.Text == string.Empty)
                {
                    ShowInput(Globals.txtAddCarModel);
                    Utils.ShowToast("Invalid Model");
                }
                else if (Globals.txtAddCarYear.Text.Length != 4)
                {
                    ShowInput(Globals.txtAddCarYear);
                    Utils.ShowToast("Invalid Year");
                }
                else
                {
                    int year = Utils.ConvertStringToInt(Globals.txtAddCarYear.Text);
                    CarsGlobals.CarsList.Add(new Car(name, Globals.txtAddCarBrand.Text, Globals.txtAddCarModel.Text, year, null, new List<Transaction>(), CarsGlobals.SelectedCar.CarObjects, new List<Repair>()));
                    XML.RefreshCars();
                    Utils.ShowToast("Car With Plate: " + name + " Was Added!");
                    CarsGlobals.NewCarWasAdded = true;
                    SetContentView(Globals.PreviousForm);
                }
            }
            else
            {
                bool modified = false;
                //modify
                if (name != CarsGlobals.SelectedCar.Plate)
                {
                    //check if plate exists already - should be unique
                    if (-1 != CarsGlobals.CarsList.FindIndex(x => x.Plate == name))
                    {
                        ShowInput(Globals.txtAddCarPlate);
                        Utils.ShowAlert("", "Plate Already Exists");
                        return;
                    }
                    modified = true;
                }
                else if (Globals.txtAddCarBrand.Text != CarsGlobals.SelectedCar.Brand)
                {
                    modified = true;
                }
                else if (Globals.txtAddCarModel.Text != CarsGlobals.SelectedCar.Model)
                {
                    modified = true;
                }
                else if (Globals.txtAddCarYear.Text != Convert.ToString(CarsGlobals.SelectedCar.Year))
                {
                    modified = true;
                }
                if (modified)
                {
                    int index = CarsGlobals.CarIndexInList(CarsGlobals.CarsList, CarsGlobals.SelectedCar);
                    if (index != -1)
                    {
                        CarsGlobals.CarsList[index] = CarsGlobals.SelectedCar;
                        CarsGlobals.CarsList[index].Brand = Globals.txtAddCarBrand.Text;
                        CarsGlobals.CarsList[index].Model = Globals.txtAddCarModel.Text;
                        CarsGlobals.CarsList[index].Year = Convert.ToInt32(Globals.txtAddCarYear.Text);
                        CarsGlobals.CarsList[index].Plate = name;
                        XML.RefreshCars();
                        Utils.ShowToast("Car With Plate: " + CarsGlobals.SelectedCar.Plate + " Was Modified!");
                        CarsGlobals.NewCarWasAdded = true;
                        SetContentView(Globals.PreviousForm);
                    }
                }
				else
				{
                    SetContentView(Globals.PreviousForm);
                }
            }
		}

		private void TxtAddCar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is EditText btn)
            {
                if (btn == Globals.txtAddCarPlate)
                {
                    CarsGlobals.SelectedCar.Plate = Globals.txtAddCarPlate.Text.ToUpper();
                }
                else if (btn == Globals.txtAddCarBrand)
                {
                    CarsGlobals.SelectedCar.Brand = Globals.txtAddCarBrand.Text;
                }
                else if (btn == Globals.txtAddCarModel)
                {
                    CarsGlobals.SelectedCar.Model = Globals.txtAddCarModel.Text;
                }
                else if (btn == Globals.txtAddCarYear)
                {
                    if (Globals.txtAddCarYear.Text != string.Empty)
                    {
                        CarsGlobals.SelectedCar.Year = Convert.ToInt32(Globals.txtAddCarYear.Text);
                    }
                    else
                    {
                        CarsGlobals.SelectedCar.Year = 0;
                    }
                }
            }
        }

		#endregion

		#region RepairMenu

        public void RepairMenu_Events()
		{
            Globals.btnCarRepairDate = FindViewById<Button>(Id.btnCarRepairDate);
            Globals.btnAddOrModifyCarRepair = FindViewById<Button>(Id.btnAddOrModifyCarRepair);

            Globals.textviewCarRepairName = FindViewById<TextView>(Id.textviewCarRepairName);
            Globals.textviewCarRepairDescription = FindViewById<TextView>(Id.textviewCarRepairDescription);
            Globals.textviewCarRepairKilometers = FindViewById<TextView>(Id.textviewCarRepairKilometers);
            Globals.textviewCarRepairSum = FindViewById<TextView>(Id.textviewCarRepairSum);

            Globals.txtCarRepairName = FindViewById<EditText>(Id.txtCarRepairName);
            Globals.txtCarRepairDescription = FindViewById<EditText>(Id.txtCarRepairDescription);
            Globals.txtCarRepairDate = FindViewById<EditText>(Id.txtCarRepairDate);
            Globals.txtCarRepairKilometers = FindViewById<EditText>(Id.txtCarRepairKilometers);
            Globals.txtCarRepairSum = FindViewById<EditText>(Id.txtCarRepairSum);

            if (CarsGlobals.SelectedCarRepair != null)
			{
                Globals.txtCarRepairName.Text = CarsGlobals.SelectedCarRepair.Name;
                Globals.txtCarRepairDescription.Text = CarsGlobals.SelectedCarRepair.Description;
                Globals.txtCarRepairDate.Text = CarsGlobals.SelectedCarRepair.Date.ToShortDateString();
                Globals.txtCarRepairKilometers.Text = CarsGlobals.SelectedCarRepair.Kilometers.ToString("#,##0.00");
                Globals.txtCarRepairSum.Text = CarsGlobals.SelectedCarRepair.Sum.ToString("#,##0.00");
            }
            else if (CarsGlobals.AddNewCarRepair && CarsGlobals.SelectedCarRepair == null)
            {
                CarsGlobals.SelectedCarRepair = new Repair(CarsGlobals.SelectedCar);
                Globals.txtCarRepairName.Text = string.Empty;
                Globals.txtCarRepairDescription.Text = string.Empty;
                Globals.txtCarRepairDate.Text = DateTime.Now.ToShortDateString();
                Globals.txtCarRepairKilometers.Text = string.Empty;
                Globals.txtCarRepairSum.Text = string.Empty;
            }

            Globals.btnCarRepairDate.Click -= BtnRepairMenu_Click;
            Globals.btnCarRepairDate.Click += BtnRepairMenu_Click;

            Globals.btnAddOrModifyCarRepair.Click -= BtnRepairMenu_Click;
            Globals.btnAddOrModifyCarRepair.Click += BtnRepairMenu_Click;

            Globals.btnAddOrModifyCarRepair.Text = CarsGlobals.AddNewCarRepair ? "Add New Repair" : "Modify";

            if (CarsGlobals.SelectedCarRepair != null && CarsGlobals.AddNewCarRepair)
            {
                ShowInput(Globals.txtCarRepairName);
            }

            Globals.PreviousForm = Globals.CarMenu;
        }

		private void BtnRepairMenu_Click(object sender, EventArgs e)
		{
			if (sender is Button btn)
			{
				if (btn == Globals.btnCarRepairDate)
				{
                    Utils.Dpd_DateSet = DpdRepairDate_DateSet;
                    Utils.DateSet = Globals.txtCarRepairDate.Text;
                    Utils.ShowDatePicker(Utils.ConvertStringToDate(Globals.txtCarRepairDate.Text));
                }
				else if (btn == Globals.btnAddOrModifyCarRepair)
				{
                    //save repair
                    if (CarsGlobals.AddNewCarRepair)
                    {
                        if (string.IsNullOrWhiteSpace(Globals.txtCarRepairKilometers.Text))
                        {
                            ShowInput(Globals.txtCarRepairKilometers);
                            Utils.ShowToast("Please Insert Kilometers");
                            return;
                        }
						else if (Utils.ConvertStringToDouble(Globals.txtCarRepairKilometers.Text, true, "Invalid Kilometers Inserted", -1) == -1)
						{
                            //handled by utils
						}
                        else if (!string.IsNullOrWhiteSpace(Globals.txtCarRepairSum.Text) && Utils.ConvertStringToDouble(Globals.txtCarRepairSum.Text, true, "Invalid Sum Inserted", -1) == -1)
                        {
                            //handled by utils
                        }
                        else
                        {
                            double year = Utils.ConvertStringToDouble(Globals.txtCarRepairKilometers.Text);

                            CarsGlobals.SelectedCar.Repairs.Add(new Repair(Globals.txtCarRepairName.Text, Globals.txtCarRepairDescription.Text, Utils.ConvertStringToDate(Globals.txtCarRepairDate.Text), year, Utils.ConvertStringToDouble(Globals.txtCarRepairSum.Text), CarsGlobals.SelectedCar));
                            XML.RefreshCars();
                            Utils.ShowToast("Repair Was Added!");
                            CarsGlobals.AddNewCarRepair = false;
                            SetContentView(Globals.PreviousForm);
                        }
                    }
                    else
                    {
                        bool modified = false;
                        //modify
                        if (string.IsNullOrWhiteSpace(Globals.txtCarRepairName.Text) && Globals.txtCarRepairName.Text != CarsGlobals.SelectedCarRepair.Name)
                        {
                            modified = true;
                        }
                        else if (Globals.txtCarRepairDescription.Text != CarsGlobals.SelectedCarRepair.Description)
                        {
                            modified = true;
                        }
                        else if (Utils.ConvertStringToDate(Globals.txtCarRepairDate.Text) != CarsGlobals.SelectedCarRepair.Date)
                        {
                            modified = true;
                        }
                        else if (Utils.ConvertStringToDouble(Globals.txtCarRepairSum.Text) != CarsGlobals.SelectedCarRepair.Sum)
                        {
                            modified = true;
                        }
                        else if (!string.IsNullOrWhiteSpace(Globals.txtCarRepairKilometers.Text))
                        {
                            if (Utils.ConvertStringToDouble(Globals.txtCarRepairKilometers.Text) != CarsGlobals.SelectedCarRepair.Kilometers)
                            {
                                modified = true;
                            }
                            else if (Utils.ConvertStringToDouble(Globals.txtCarRepairKilometers.Text, true, "Invalid Kilometers Inserted", -1) == -1)
                            {
                                return;
                            }
                        }
                        else if (Utils.ConvertStringToDouble(Globals.txtCarRepairKilometers.Text, true, "Invalid Kilometers Inserted", -1) == -1)
                        {
                            return;
                        }
                        else if (Utils.ConvertStringToDouble(Globals.txtCarRepairSum.Text, true, "Invalid Sum Inserted", -1) == -1)
                        {
                            return;
                        }
                        if (modified)
                        {
                            int index = CarsGlobals.CarIndexInList(CarsGlobals.SelectedCar.Repairs, CarsGlobals.SelectedCarRepair);
                            if (index != -1)
                            {
                                CarsGlobals.SelectedCar.Repairs[index] = CarsGlobals.SelectedCarRepair;
                                CarsGlobals.SelectedCar.Repairs[index].Name = Globals.txtCarRepairName.Text;
                                CarsGlobals.SelectedCar.Repairs[index].Description = Globals.txtCarRepairDescription.Text;
                                CarsGlobals.SelectedCar.Repairs[index].Date = Utils.ConvertStringToDate(Globals.txtCarRepairDate.Text);
                                CarsGlobals.SelectedCar.Repairs[index].Kilometers = Utils.ConvertStringToDouble(Globals.txtCarRepairKilometers.Text);
                                CarsGlobals.SelectedCar.Repairs[index].Sum = Utils.ConvertStringToDouble(Globals.txtCarRepairSum.Text);
                                XML.RefreshCars();
                                Utils.ShowToast("Repair Was Modified!");
                                CarsGlobals.AddNewCarRepair = false;
                                SetContentView(Globals.PreviousForm);
                            }
                        }
                        else
                        {
                            SetContentView(Globals.PreviousForm);
                        }
                    }
                }
			}
		}

        private void DpdRepairDate_DateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Globals.txtCarRepairDate.Text = new DateTime(e.Year, e.Month + 1, e.DayOfMonth).ToShortDateString();
        }

        #endregion

        #endregion

        #region All Forms Controls With Events For Tags Part

        #region TagsMenu

        public void TagsMenu_Events()
		{
            Globals.layoutAddNewTag = FindViewById<LinearLayout>(Id.LayoutAddNewTag);

            Globals.imgAddNewTag = FindViewById<ImageView>(Id.imgAddNewTag);
            Globals.imgDoneTags = FindViewById<ImageView>(Id.imgDoneTags);
            Globals.imgRemoveTag = FindViewById<ImageView>(Id.imgRemoveTag);

            Globals.txtNoTags = FindViewById<TextView>(Id.txtNoTags);

            Globals.chkTagsCheckAll = FindViewById<CheckBox>(Id.chkTagsCheckAll);

            Globals.layoutAddNewTag.Visibility = Globals.TagToAddInTransaction ? ViewStates.Invisible : ViewStates.Visible;
            Globals.imgDoneTags.Visibility = Globals.TagToAddInTransaction ? ViewStates.Visible : ViewStates.Invisible;
            Globals.imgRemoveTag.Visibility = ViewStates.Invisible;
            Globals.chkTagsCheckAll.Visibility = Globals.TagToAddInTransaction ? ViewStates.Visible : ViewStates.Invisible;

            Globals.grdTags = FindViewById<GridView>(Id.grdTags);

            TagsGlobals.DisplayTagsList = TagsGlobals.TagsList;
            Globals.grdTags.Adapter = new TagsAdapter(Globals.Activity, TagsGlobals.DisplayTagsList);

            TagsGlobals.WhatTagToDisplay();

            if (TagsGlobals.DisplayTagsList.Count > 0)
			{
                Globals.grdTags.BringToFront();
            }
			else
			{
                Globals.txtNoTags.BringToFront();
			}

            Globals.imgAddNewTag.Click -= imgAddNewNoteOrCarOrTagOrCategory_Click;
            Globals.imgAddNewTag.Click += imgAddNewNoteOrCarOrTagOrCategory_Click;

            Globals.imgDoneTags.Click -= imgDoneNoteOrCarOrTagOrCategory_Click;
            Globals.imgDoneTags.Click += imgDoneNoteOrCarOrTagOrCategory_Click;

            Globals.imgRemoveTag.Click -= imgRemoveNoteOrCarOrTagOrCategory_Click;
            Globals.imgRemoveTag.Click += imgRemoveNoteOrCarOrTagOrCategory_Click;

            Globals.chkTagsCheckAll.CheckedChange -= chkNoteOrCarOrTagOrCategoryCheckAll_Click;
            Globals.chkTagsCheckAll.CheckedChange += chkNoteOrCarOrTagOrCategoryCheckAll_Click;
        }

		#endregion

		#region TagMenu

		public void TagMenu_Events()
        {

            Globals.txtTagName = FindViewById<EditText>(Id.txtTagName);

            Globals.btnSetTagColor = FindViewById<Button>(Id.btnSetTagColor);
            Globals.btnAddNoteToTag = FindViewById<Button>(Id.btnAddNoteToTag);
            Globals.btnAddCarToTag = FindViewById<Button>(Id.btnAddCarToTag);
            Globals.btnAddOrModifyTag = FindViewById<Button>(Id.btnAddOrModifyTag);

            Globals.txtChoosenNotesToTag = FindViewById<TextView>(Id.txtChoosenNotesToTag);
            Globals.txtChoosenCarsToTag = FindViewById<TextView>(Id.txtChoosenCarsToTag);
            Globals.txtTagColor = FindViewById<TextView>(Id.txtTagColor);

            Globals.txtTagName.Text = string.Empty;
            Globals.txtChoosenNotesToTag.Text = string.Empty;
            Globals.txtChoosenCarsToTag.Text = string.Empty;
            Globals.txtTagColor.Text = string.Empty;

            if (Globals.SelectedNotesForTag == null)
			{
                Globals.SelectedNotesForTag = new List<Note>();
			}
            if (Globals.SelectedCarsForTag == null)
            {
                Globals.SelectedCarsForTag = new List<Car>();
            }

            if (TagsGlobals.SelectedTag != null && TagsGlobals.PendingTag == null)
			{
                Globals.txtTagName.Text = TagsGlobals.SelectedTag.Name;
                Globals.txtTagColor.Text = Globals.GetColorName(TagsGlobals.SelectedTag.Color);

                Globals.txtChoosenNotesToTag.Text = string.Empty;
                Globals.txtChoosenCarsToTag.Text = string.Empty;

                foreach (Note note in TagsGlobals.SelectedTag.Notes)
				{
                    note.Checked = true;
                    Globals.txtChoosenNotesToTag.Text += note.Title + ";";
				}
				foreach (Car car in TagsGlobals.SelectedTag.Cars)
				{
                    car.Checked = true;
                    Globals.txtChoosenCarsToTag.Text += car.Plate + ";";
				}
				foreach (Note note in TagsGlobals.SelectedTag.Notes)
				{
                    if (!NotesGlobals.NoteInList(Globals.SelectedNotesForTag, note))
                    {
                        Globals.SelectedNotesForTag.Add(note);
                    }
				}
                foreach (Car car in TagsGlobals.SelectedTag.Cars)
                {
                    if (!CarsGlobals.CarInList(Globals.SelectedCarsForTag, car))
                    {
                        Globals.SelectedCarsForTag.Add(car);
                    }
                }
            }
			else
			{
                if (TagsGlobals.PendingTag == null)
                {
                    TagsGlobals.AddNewTag = true;
                    Globals.txtTagName.Text = string.Empty;
                    Globals.txtTagColor.Text = string.Empty;
                    TagsGlobals.PendingTag = new Tag();
                    Globals.btnAddOrModifyTag.Text = "Add New";
                }
                else
                {
                    Globals.txtTagName.Text = TagsGlobals.PendingTag.Name;
                    Globals.txtTagColor.Text = Globals.GetColorName(Globals.SelectedColorForTag);
                    Globals.btnAddOrModifyTag.Text = "Modify";
                    foreach (Note note in Globals.SelectedNotesForTag.Reverse<Note>())
                    {
						if (!NotesGlobals.NoteInList(TagsGlobals.PendingTag.Notes, note))
						{
                            TagsGlobals.PendingTag.Notes.Add(note);
						}
                    }
                    foreach (Car car in Globals.SelectedCarsForTag.Reverse<Car>())
                    {
                        if (!CarsGlobals.CarInList(TagsGlobals.PendingTag.Cars, car))
                        {
                            TagsGlobals.PendingTag.Cars.Add(car);
                        }
                    }
                }
                TagsGlobals.PendingTag.Color = Globals.SelectedColorForTag;
                
                Globals.txtChoosenNotesToTag.Text = string.Empty;
                Globals.txtChoosenCarsToTag.Text = string.Empty;

                foreach (Note note in Globals.SelectedNotesForTag)
                {
					if (note.Checked)
					{
                        Globals.txtChoosenNotesToTag.Text += note.Title + ";";
					}
                }
                foreach (Car car in Globals.SelectedCarsForTag)
                {
					if (car.Checked)
					{
                        Globals.txtChoosenCarsToTag.Text += car.Plate + ";";
					}
                }

                ShowInput(Globals.txtTagName);
			}

            Globals.btnAddCarToTag.Click -= BtnAddCarOrNoteToTag_Click;
            Globals.btnAddCarToTag.Click += BtnAddCarOrNoteToTag_Click;

            Globals.btnAddNoteToTag.Click -= BtnAddCarOrNoteToTag_Click;
            Globals.btnAddNoteToTag.Click += BtnAddCarOrNoteToTag_Click;

            Globals.btnSetTagColor.Click -= BtnSetCategoryOrTagColor_Click;
            Globals.btnSetTagColor.Click += BtnSetCategoryOrTagColor_Click;

            Globals.btnAddOrModifyTag.Click -= BtnAddOrModifyTag_Click;
            Globals.btnAddOrModifyTag.Click += BtnAddOrModifyTag_Click;
        }

        private void BtnAddCarOrNoteToTag_Click(object sender, EventArgs e)
        {
            TagsGlobals.PendingTag = new Tag(Globals.txtTagName.Text, Globals.GetColor(Globals.txtTagColor.Text), Globals.SelectedCarsForTag, Globals.SelectedNotesForTag);
            if (sender is Button btn)
            {
                if (btn == Globals.btnAddNoteToTag)
                {
                    Globals.SelectedNotesForTag = new List<Note>();
                    Globals.NoteToAddInTag = true;
                    SetContentView(Globals.NotesMenu);
                    Globals.PreviousForm = Globals.TagMenu;
                }
                else if (btn == Globals.btnAddCarToTag)
                {
                    Globals.SelectedCarsForTag = new List<Car>();
                    Globals.CarToAddInTag = true;
                    SetContentView(Globals.CarsMenu);
                    Globals.PreviousForm = Globals.TagMenu;
                }
            }
        }

		private void BtnSetCategoryOrTagColor_Click(object sender, EventArgs e)
		{
			if (sender is Button btn)
			{
				if (btn == Globals.btnSetTagColor)
				{
                    if (Globals.btnAddOrModifyTag.Text == "Add New")
                    {
                        Utils.ShowToast("Please Save Tag Before Setting Color");
                    }
                    else
                    {
                        TagsGlobals.PendingTag = new Tag(Globals.txtTagName.Text, Globals.GetColor(Globals.txtTagColor.Text), Globals.SelectedCarsForTag, Globals.SelectedNotesForTag);
                        Globals.SelectedColorForTag = Globals.GetColor(Globals.txtTagColor.Text);
                        Globals.ColorToAddInTag = true;
                        SetContentView(Globals.ChangeCategoryOrTagColor);
                        Globals.PreviousForm = Globals.TagMenu;

                        Globals.spinnerChooseCategoryOrTagTypeColor.SetSelection(1);
                        int index = TagsGlobals.TagsNameList.IndexOf(TagsGlobals.PendingTag.Name);
                        Globals.SelectedColorIndex = index;
                        Globals.spinnerChooseCategoryOrTagColor.SetSelection(index);

                        (Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SKColorName_Click(Globals.txtTagColor.Text, null);
                        SelectColorInGrid(Globals.txtTagName.Text);

                        Globals.spinnerChooseCategoryOrTagTypeColor.Enabled = false;
                        Globals.spinnerChooseCategoryOrTagColor.Enabled = false;
                    }
				}
				else if (btn == Globals.btnSetCategoryColor)
				{
                    if (Globals.btnAddCategory.Text == "Add New")
                    {
                        Utils.ShowToast("Please Save Category Before Setting Color");
                    }
                    else
                    {
						CategoriesGlobals.PendingCategory = new Category(Globals.txtCategoryName.Text, CategoriesGlobals.SelectedCategory.CharityPercentage, Globals.GetColor(Globals.txtCategoryColor.Text), CategoriesGlobals.SelectedCategory.Checkers);
						Globals.SelectedColorForCategory = Globals.GetColor(Globals.txtCategoryColor.Text);
						Globals.ColorToAddInCategory = true;
						SetContentView(Globals.ChangeCategoryOrTagColor);
						Globals.PreviousForm = Globals.CategoryMenu;

						Globals.spinnerChooseCategoryOrTagTypeColor.SetSelection(0);
						int index = CategoriesGlobals.CategoriesNameList.IndexOf(CategoriesGlobals.PendingCategory.Name);
						Globals.SelectedColorIndex = index;
						Globals.spinnerChooseCategoryOrTagColor.SetSelection(index);

						(Globals.grdSKColorImages.Adapter as SKColorImageAdapter).SKColorName_Click(Globals.txtCategoryColor.Text, null);
						SelectColorInGrid(Globals.txtCategoryName.Text);

						Globals.spinnerChooseCategoryOrTagTypeColor.Enabled = false;
						Globals.spinnerChooseCategoryOrTagColor.Enabled = false;
					}
				}
			}
        }

		private void BtnAddOrModifyTag_Click(object sender, EventArgs e)
		{
            string name = Globals.txtTagName.Text.ToUpper();
			if (TagsGlobals.SelectedTag != null)
			{
                //Verify Uniquity
                if (TagsGlobals.SearchTag(-1, name) != TagsGlobals.SelectedTag)
                {
                    ShowInput(Globals.txtTagName);
                    if (-1 != TagsGlobals.TagsList.FindIndex(x => x.Name == name))
                    {
                        Utils.ShowAlert("", "Tag Name Already Exists");
                        return;
                    }
                }

                //don't change editstate because we are maybe in CarMenu Form and we don't have just the grid
                foreach (Tag tag in TagsGlobals.TagsList)
				{
					if (TagsGlobals.TagMatch(tag, TagsGlobals.SelectedTag))
					{
                        TagsGlobals.SelectedTag = tag;
                        break;
					}
				}
                TagsGlobals.SelectedTag.Name = name;
                TagsGlobals.SelectedTag.Color = Globals.GetColor(Globals.txtTagColor.Text);
                TagsGlobals.SelectedTag.Notes.Clear();
                TagsGlobals.SelectedTag.Cars.Clear();
                foreach (Note note in Globals.SelectedNotesForTag.Reverse<Note>())
				{
					if (note.Checked)
					{
                        NotesGlobals.AddNoteToList(TagsGlobals.SelectedTag.Notes, note);
                    }
				}
                foreach (Car car in Globals.SelectedCarsForTag.Reverse<Car>())
                {
                    if (car.Checked)
                    {
                        CarsGlobals.AddCarToList(TagsGlobals.SelectedTag.Cars, car);
                    }
                }
                Utils.ShowToast("Tag Was Modified!");
                TagsGlobals.SelectedTag = null;
            }
			else
			{
                //Verify Uniquity
                if (TagsGlobals.SearchTag(-1, name) != TagsGlobals.PendingTag)
                {
                    ShowInput(Globals.txtTagName);
                    if (-1 != TagsGlobals.TagsList.FindIndex(x => x.Name == name))
                    {
                        Utils.ShowAlert("", "Tag Name Already Exists");
                        return;
                    }
                }

                TagsGlobals.PendingTag.Name = name;
                TagsGlobals.PendingTag.Color = Globals.GetColor(Globals.txtTagColor.Text);
                TagsGlobals.PendingTag.Notes.Clear();
                TagsGlobals.PendingTag.Cars.Clear();
                foreach (Note note in Globals.SelectedNotesForTag.Reverse<Note>())
                {
                    if (note.Checked)
                    {
                        NotesGlobals.AddNoteToList(TagsGlobals.PendingTag.Notes, note);
                    }
                }
                foreach (Car car in Globals.SelectedCarsForTag.Reverse<Car>())
                {
                    if (car.Checked)
                    {
                        CarsGlobals.AddCarToList(TagsGlobals.PendingTag.Cars, car);
                    }
                }
                TagsGlobals.TagsListAdd(TagsGlobals.PendingTag);
                Utils.ShowToast("New Tag Was Added!");
                TagsGlobals.PendingTag = null;
            }

            XML.RefreshTags();

            NotesGlobals.CheckAll();
            CarsGlobals.CheckAll();
            TagsGlobals.CheckAll();

            Globals.NoteToAddInTag = false;
            Globals.CarToAddInTag = false;
            Globals.SelectedColorForTag = SKColor.Empty;
            Globals.SelectedNotesForTag = null;
            Globals.SelectedCarsForTag = null;
            TagsGlobals.SelectedTag = null;
            TagsGlobals.PendingTag = null;
            SetContentView(Globals.ParentForm);
		}

		#endregion

		#endregion

		#region All Forms Controls With Events For Categories Part

		#region CategoriesMenu

        public void CategoriesMenu_Events()
		{

            Globals.layoutAddNewCategory = FindViewById<LinearLayout>(Id.LayoutAddNewCategory);

            Globals.imgAddNewCategory = FindViewById<ImageView>(Id.imgAddNewCategory);
            Globals.imgDoneCategories = FindViewById<ImageView>(Id.imgDoneCategories);
            Globals.imgRemoveCategory = FindViewById<ImageView>(Id.imgRemoveCategories);

            Globals.chkCategoriesCheckAll = FindViewById<CheckBox>(Id.chkCategoriesCheckAll);

            Globals.txtNoCategories = FindViewById<TextView>(Id.txtNoCategories);

            Globals.grdCategories = FindViewById<GridView>(Id.grdCategories);

            CategoriesGlobals.DisplayCategoriesList = CategoriesGlobals.CategoriesList;
            Globals.grdCategories.Adapter = new CategoriesAdapter(Globals.Activity, CategoriesGlobals.DisplayCategoriesList);

            CategoriesGlobals.WhatCategoryToDisplay();

            Globals.imgDoneCategories.Visibility = ViewStates.Invisible;
            Globals.imgRemoveCategory.Visibility = ViewStates.Invisible;
            Globals.chkCategoriesCheckAll.Visibility = ViewStates.Invisible;

            if (CategoriesGlobals.DisplayCategoriesList.Count > 0)
            {
                Globals.grdCategories.BringToFront();
            }
            else
            {
                Globals.txtNoCategories.BringToFront();
            }

            Globals.imgAddNewCategory.Click -= imgAddNewNoteOrCarOrTagOrCategory_Click;
            Globals.imgAddNewCategory.Click += imgAddNewNoteOrCarOrTagOrCategory_Click;

            Globals.imgDoneCategories.Click -= imgDoneNoteOrCarOrTagOrCategory_Click;
            Globals.imgDoneCategories.Click += imgDoneNoteOrCarOrTagOrCategory_Click;

            Globals.imgRemoveCategory.Click -= imgRemoveNoteOrCarOrTagOrCategory_Click;
            Globals.imgRemoveCategory.Click += imgRemoveNoteOrCarOrTagOrCategory_Click;

            Globals.chkCategoriesCheckAll.CheckedChange -= chkNoteOrCarOrTagOrCategoryCheckAll_Click;
            Globals.chkCategoriesCheckAll.CheckedChange += chkNoteOrCarOrTagOrCategoryCheckAll_Click;
        }

        #endregion

        #region CategoryMenu

        public void CategoryMenu_Events()
        {
            Globals.txtCategoryColor = FindViewById<TextView>(Id.textviewCategoryColor);
            Globals.textviewCurrentPercentage = FindViewById<TextView>(Id.textviewCurrentPercentage);

            Globals.txtCategoryName = FindViewById<EditText>(Id.txtCategoryName);
            Globals.txtCustomCharity = FindViewById<EditText>(Id.txtCustomCharity);

            Globals.btnSetCategoryColor = FindViewById<Button>(Id.btnSetCategoryColor);
            Globals.btnAddCategory = FindViewById<Button>(Id.btnAddCategory);

            Globals.chkIncIncome = FindViewById<CheckBox>(Id.chkIncIncome);
            Globals.chkIncExpense = FindViewById<CheckBox>(Id.chkIncExpense);
            Globals.chkIncCharity = FindViewById<CheckBox>(Id.chkIncCharity);
            Globals.chkExcIncome = FindViewById<CheckBox>(Id.chkExcIncome);
            Globals.chkExcExpense = FindViewById<CheckBox>(Id.chkExcExpense);
            Globals.chkExcCharity = FindViewById<CheckBox>(Id.chkExcCharity);
            Globals.chkStandardCharity = FindViewById<CheckBox>(Id.chkStandardCharity);
            Globals.chkCustomCharity = FindViewById<CheckBox>(Id.chkCustomCharity);

            Globals.showCustomCharityCategory = FindViewById<LinearLayout>(Id.ShowCustomCharityCategory);

            Globals.btnSetCategoryColor.Click -= BtnSetCategoryOrTagColor_Click;
            Globals.btnSetCategoryColor.Click += BtnSetCategoryOrTagColor_Click;

            Globals.btnAddCategory.Click -= BtnAddCategory_Click;
			Globals.btnAddCategory.Click += BtnAddCategory_Click;

			Globals.chkIncIncome.Click -= ChkIncExc_Click;
            Globals.chkIncIncome.Click += ChkIncExc_Click;

            Globals.chkIncExpense.Click -= ChkIncExc_Click;
            Globals.chkIncExpense.Click += ChkIncExc_Click;

            Globals.chkIncCharity.Click -= ChkIncExc_Click;
            Globals.chkIncCharity.Click += ChkIncExc_Click;

            Globals.chkExcIncome.Click -= ChkIncExc_Click;
            Globals.chkExcIncome.Click += ChkIncExc_Click;

            Globals.chkExcExpense.Click -= ChkIncExc_Click;
            Globals.chkExcExpense.Click += ChkIncExc_Click;

            Globals.chkExcCharity.Click -= ChkIncExc_Click;
            Globals.chkExcCharity.Click += ChkIncExc_Click;

			Globals.chkStandardCharity.Click -= ChkStandardOrCustomCharity_Click;
            Globals.chkStandardCharity.Click += ChkStandardOrCustomCharity_Click;

            Globals.chkCustomCharity.Click -= ChkStandardOrCustomCharity_Click;
            Globals.chkCustomCharity.Click += ChkStandardOrCustomCharity_Click;

            Globals.showCustomCharityCategory.Visibility = ViewStates.Invisible;

            string percText = "Standard Percentage";
			if (Globals.MonthsList?.Count > 0)
			{
                Month temp = Globals.MonthsList.Find(x => x.month == DateTime.Now.Month && x.year == DateTime.Now.Year);
				if (temp != null)
				{
                    percText = "Current Percentage: " + temp.CharityPercentage.ToString() + "%";
                }
            }
            Globals.textviewCurrentPercentage.Text = percText;
            Globals.txtCustomCharity.Text = string.Empty;
            Globals.txtCategoryName.Text = string.Empty;
            Globals.txtCategoryColor.Text = string.Empty;

            if (CategoriesGlobals.SelectedCategory != null && CategoriesGlobals.PendingCategory == null)
            {
                Globals.txtCategoryName.Text = CategoriesGlobals.SelectedCategory.Name;
				if (CategoriesGlobals.IncomeCategories.Contains(CategoriesGlobals.SelectedCategory))
				{
                    Globals.txtCustomCharity.Text = CategoriesGlobals.SelectedCategory.CharityPercentage.ToString();
				}
                Globals.txtCategoryColor.Text = Globals.GetColorName(CategoriesGlobals.SelectedCategory.Color);
                //load Checkboxes with checkers value
                Globals.chkIncIncome.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.SelectedCategory, 0);
                Globals.chkIncExpense.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.SelectedCategory, 1);
                Globals.chkIncCharity.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.SelectedCategory, 2);
                Globals.chkExcIncome.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.SelectedCategory, 3);
                Globals.chkExcExpense.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.SelectedCategory, 4);
                Globals.chkExcCharity.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.SelectedCategory, 5);

                if (CategoriesGlobals.IncomeCategories.Contains(CategoriesGlobals.SelectedCategory))
                {
                    Month month = Globals.GetMonthFromDate(DateTime.Now);
					if (month != null && month.CharityPercentage != CategoriesGlobals.SelectedCategory.CharityPercentage)
					{
                        Globals.chkCustomCharity.Checked = true;
                        Globals.txtCustomCharity.Text = CategoriesGlobals.SelectedCategory.CharityPercentage.ToString();
					}
                    else
                    {
                        Globals.chkCustomCharity.Checked = false;
                        Globals.txtCustomCharity.Text = string.Empty;
                    }
                }
                Globals.btnAddCategory.Text = "Modify";
            }
            else
            {
                if (CategoriesGlobals.PendingCategory == null)
                {
                    CategoriesGlobals.AddNewCategory = true;
                    CategoriesGlobals.PendingCategory = new Category();
                    Globals.btnAddCategory.Text = "Add New";
                }
                else
                {
                    Globals.txtCategoryName.Text = CategoriesGlobals.PendingCategory.Name;
                    if (CategoriesGlobals.IncomeCategories.Contains(CategoriesGlobals.PendingCategory))
                    {
                        Globals.txtCustomCharity.Text = CategoriesGlobals.PendingCategory.CharityPercentage.ToString();
                    }
                    Globals.txtCategoryColor.Text = Globals.GetColorName(Globals.SelectedColorForCategory);
                    //load Checkboxes with checkers value
                    Globals.chkIncIncome.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.PendingCategory, 0);
                    Globals.chkIncExpense.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.PendingCategory, 1);
                    Globals.chkIncCharity.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.PendingCategory, 2);
                    Globals.chkExcIncome.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.PendingCategory, 3);
                    Globals.chkExcExpense.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.PendingCategory, 4);
                    Globals.chkExcCharity.Checked = CategoriesGlobals.GetCategoryCheckersValue(CategoriesGlobals.PendingCategory, 5);

                    Globals.btnAddCategory.Text = "Modify";
                }
                CategoriesGlobals.PendingCategory.Color = Globals.SelectedColorForCategory;
                ShowInput(Globals.txtCategoryName);
            }

            Globals.showCustomCharityCategory.Visibility = Globals.chkIncIncome.Checked ? ViewStates.Visible : ViewStates.Invisible;
            if (Globals.showCustomCharityCategory.Visibility == ViewStates.Visible && !Globals.chkStandardCharity.Checked && !Globals.chkCustomCharity.Checked)
            {
                Globals.chkStandardCharity.Checked = true;
                Globals.chkCustomCharity.Checked = false;
            }
        }

		private void ChkStandardOrCustomCharity_Click(object sender, EventArgs e)
		{
			if (sender is CheckBox chk)
			{
				if (chk == Globals.chkCustomCharity)
				{
                    Globals.chkStandardCharity.Checked = !Globals.chkCustomCharity.Checked;
					if (Globals.chkCustomCharity.Checked)
					{
                        ShowInput(Globals.txtCustomCharity);
					}
				}
                else if (chk == Globals.chkStandardCharity)
                {
                    Globals.chkCustomCharity.Checked = !Globals.chkStandardCharity.Checked;
                }
            }
		}

		private void ChkIncExc_Click(object sender, EventArgs e)
		{
            Globals.showCustomCharityCategory.Visibility = ViewStates.Invisible;

            if (sender is CheckBox chk)
			{
				if (chk == Globals.chkIncIncome)
				{
					if (chk.Checked)
					{
                        Globals.showCustomCharityCategory.Visibility = ViewStates.Visible;
						if (!Globals.chkStandardCharity.Checked && !Globals.chkCustomCharity.Checked)
						{
                            Globals.chkStandardCharity.Checked = true;
                        }
						if (Globals.chkExcIncome.Checked)
						{
                            Globals.chkExcIncome.Checked = false;
						}
                    }
                }
                else if (chk == Globals.chkIncExpense)
                {
                    if (chk.Checked && Globals.chkExcExpense.Checked)
                    {
                        Globals.chkExcExpense.Checked = false;
                    }
                }
                else if (chk == Globals.chkIncCharity)
                {
                    if (chk.Checked && Globals.chkExcCharity.Checked)
                    {
                        Globals.chkExcCharity.Checked = false;
                    }
                }

                else if (chk == Globals.chkExcIncome)
                {
                    if (chk.Checked && Globals.chkIncIncome.Checked)
                    {
                        Globals.chkIncIncome.Checked = false;
                    }
                }
                else if (chk == Globals.chkExcExpense)
                {
                    if (chk.Checked && Globals.chkIncExpense.Checked)
                    {
                        Globals.chkIncExpense.Checked = false;
                    }
                }
                else if (chk == Globals.chkExcCharity)
                {
                    if (chk.Checked && Globals.chkIncCharity.Checked)
                    {
                        Globals.chkIncCharity.Checked = false;
                    }
                }
            }
		}

		private void BtnAddCategory_Click(object sender, EventArgs e)
		{
            //Verify Uniquity
            string name = Globals.txtCategoryName.Text.ToUpper();
            if (CategoriesGlobals.GetCategory(name) != CategoriesGlobals.SelectedCategory)
			{
                ShowInput(Globals.txtCategoryName);
                if (-1 != CategoriesGlobals.CategoriesList.FindIndex(x => x.Name == name))
                {
                    Utils.ShowAlert("", "Category Name Already Exists");
                    return;
                }
            }

            double percentage = 0;
            //custom category charity
			if (Globals.chkCustomCharity.Checked && !string.IsNullOrWhiteSpace(Globals.txtCustomCharity.Text))
			{
                double.TryParse(Globals.txtCustomCharity.Text, out percentage);
			}
			else
			{
                //standard month charity
                Month month = Globals.GetMonthFromDate(DateTime.Now);
				if (month != null)
				{
                    percentage = month.CharityPercentage;
				}
				else
				{
                    Utils.ShowAlert("", "There is no month added!");
                    return;
                }
			}

            if (CategoriesGlobals.SelectedCategory != null)
            {
                foreach (Category category in CategoriesGlobals.CategoriesList)
                {
                    if (CategoriesGlobals.CategoriesMatch(category, CategoriesGlobals.SelectedCategory))
                    {
                        CategoriesGlobals.SelectedCategory = category;
                        break;
                    }
                }

                //save new values
                CategoriesGlobals.SelectedCategory.Name = name;
                CategoriesGlobals.SelectedCategory.CharityPercentage = percentage;
                CategoriesGlobals.SelectedCategory.Color = Globals.GetColor(Globals.txtCategoryColor.Text);
                CategoriesGlobals.SaveCategoryCheckers(CategoriesGlobals.SelectedCategory);
                Utils.ShowToast("Category Was Modified!");
                CategoriesGlobals.SelectedCategory = null;
            }
            else
            {
                //save new values
                CategoriesGlobals.PendingCategory.Name = name;
                CategoriesGlobals.PendingCategory.CharityPercentage = percentage;
                CategoriesGlobals.PendingCategory.Color = Globals.GetColor(Globals.txtCategoryColor.Text);
                CategoriesGlobals.SaveCategoryCheckers(CategoriesGlobals.PendingCategory);
                CategoriesGlobals.CategoriesListAdd(CategoriesGlobals.PendingCategory);
                Utils.ShowToast("New Category Was Added!");
                CategoriesGlobals.PendingCategory = null; 
            }

            XML.RefreshCategories();

            CategoriesGlobals.CheckAll();

            Globals.SelectedColorForCategory = SKColor.Empty;
            CategoriesGlobals.PendingCategory = null;
            CategoriesGlobals.SelectedCategory = null;
            SetContentView(Globals.ParentForm);
        }

		#endregion

		#endregion

		#region Logic Part

		/// <summary>
		/// Used when going outside of a transaction when it has notes/cars and it doesn't clear itself because of the existing flags
		/// </summary>
		public void ClearTransactionFlags()
		{
            NotesGlobals.CheckAll();
            CarsGlobals.CheckAll();
            TagsGlobals.CheckAll();

            Globals.NoteToAddInTransaction = false;
            Globals.SelectedNotesForTransaction.Clear();
            Globals.CarToAddInTransaction = false;
            Globals.SelectedCarsForTransaction.Clear();
            MoneyGlobals.PendingTransaction = null;
            MoneyGlobals.IsCopiedTransaction = false;
            Globals.TagToAddInTransaction = false;
            Globals.SelectedTagForTransaction = null;
        }

		public void ClearTransactionMenuTextBoxes()
        {
            Globals.txtAddName.Text = "";
            Globals.txtAddSum.Text = "";
            Globals.spiCategory.SetSelection(0, true);
            Globals.txtAddTag.Text = "";
            Globals.txtChoosenNoteToAdd.Text = "";
            Globals.txtChoosenCarToAdd.Text = "";
            Globals.txtAddLiters.Text = "";
            Globals.txtAddKilometersRan.Text = "";
            Globals.txtAddPrice.Text = "";

            //set focus & open keyboard
            ShowInput(Globals.txtAddName);
        }

        public void HideInput()
        {
			if (Globals.currentInput != null)
			{
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(Globals.currentInput.WindowToken, 0);
			}
        }

        public void ShowInput(View view)
        {
            view.RequestFocus();
            InputMethodManager inputMethodManager = this.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(view, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            Globals.currentInput = view;
        }


        #endregion

        #region Logic Part for Visual

        public override void SetTheme(int resid)
        {
            if (Style.AppThemeDark == resid)
            {
                Globals.background = SkiaSharp.SKColor.Parse("171f2c");
                Globals.textColor = SkiaSharp.SKColors.White;
            }
            else
            {
                Globals.background = SkiaSharp.SKColors.White;
                Globals.textColor = SkiaSharp.SKColor.Parse("171f2c");
            }
            Globals.CurrentTheme = resid;
            base.SetTheme(Globals.CurrentTheme);
        }

        protected override void OnStart()
        {
            base.OnStart();
            //todo GoogleLoginStart();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Globals.Active = 0;
            SetTheme(Style.AppThemeDark);

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            //load important money global properties
            Globals.Activity = this;
            Globals.Context = this;
            Globals.ParentForm = Resource.String.YearlyStats;
            Globals.CurrentForm = Resource.String.YearlyStats;

            //load important global properties
            Globals.SKCategoriesOrTagColor = new List<CTColor>();
            Globals.SKStringColors = new Dictionary<string, SKColor>();
            Globals.AllSKColors = new List<SKColor>();

            SKColors temp = new SKColors();
            foreach (var field in typeof(SKColors).GetFields())
            {
                SKColor color = (SKColor)field.GetValue(temp);
                Globals.SKStringColors.Add(field.ToString().Replace("SkiaSharp.SKColor ", ""), color);
                Globals.AllSKColors.Add(color);
            }
            //Load All Forms

            //Accounts
            Globals.AccountsMenu = Resource.Layout.AccountsMenu;
            Globals.AccountMenu = Resource.Layout.AccountMenu;
            Globals.GridViewAccountItem = Resource.Layout.GridViewAccountItem;
            //Main Menu
            Globals.AutoLogin = Resource.Layout.AutoLogin;
            Globals.MainMenu = Resource.Layout.MainMenu;
			//Money
			Globals.YearlyStats = Resource.Layout.YearlyStats;
			Globals.TransactionMenu = Resource.Layout.TransactionMenu;
			//Globals.ModifyTransaction = Resource.Layout.ModifyTransaction;
			Globals.TransactionsMenu = Resource.Layout.TransactionsMenu;
			Globals.Filters = Resource.Layout.Filters;
            Globals.Sorts = Resource.Layout.Sorts;
            Globals.LoadingApp = Resource.Layout.LoadingApp;
			Globals.GridViewTransactionItem = Resource.Layout.GridViewTransactionItem;
			Globals.GridViewFiltersItem = Resource.Layout.GridViewFilterItem;
			//Settings
			Globals.Settings = Resource.Layout.Settings;
			Globals.ChangeCategoryOrTagColor = Resource.Layout.ChangeCategoryOrTagColor;
			Globals.ChangeCharityPercentage = Resource.Layout.ChangeCharityPercentage;
            Globals.ChangeLogin = Resource.Layout.ChangeLogin;
            Globals.ChangeUnitsType = Resource.Layout.ChangeUnitsType;
			//Notes
			Globals.NotesMenu = Resource.Layout.NotesMenu;
			Globals.NoteMenu = Resource.Layout.NoteMenu;
			Globals.NoteSettings = Resource.Layout.NoteSettings;
			Globals.MiniNoteMenu = Resource.Layout.MiniNoteMenu;
			Globals.GridViewMiniNoteItem = Resource.Layout.GridViewMiniNoteItem;
			Globals.GridViewNoteItem = Resource.Layout.GridViewNoteItem;
			//Cars
			Globals.CarsMenu = Resource.Layout.CarsMenu;
			Globals.CarMenu = Resource.Layout.CarMenu;
			Globals.GridViewCarItem = Resource.Layout.GridViewCarItem;
			Globals.GridViewCarObjectItem = Resource.Layout.GridViewCarObjectItem;
			Globals.CarObject = Resource.Layout.CarObject;
            Globals.CarConsumption = Resource.Layout.CarConsumption;
            Globals.CarPropertiesMenu = Resource.Layout.CarPropertiesMenu;
            Globals.RepairMenu = Resource.Layout.RepairMenu;
            Globals.GridViewRepairItem = Resource.Layout.GridViewRepairItem;
            //Tags
            Globals.TagsMenu = Resource.Layout.TagsMenu;
            Globals.TagMenu = Resource.Layout.TagMenu;
            Globals.GridViewTagItem = Resource.Layout.GridViewTagItem;
            //Categories
            Globals.CategoriesMenu = Resource.Layout.CategoriesMenu;
            Globals.CategoryMenu = Resource.Layout.CategoryMenu;
            Globals.GridViewCategoryItem = Resource.Layout.GridViewCategoryItem;

            //SKColorSpinner Settings
            Globals.GridViewSKColorImageItem = Resource.Layout.GridViewSKColorImageItem;

			//Google Login
            //todo google login
			//GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn).RequestEmail().Build();
            //mGoogleApiClient = new GoogleApiClient.Builder(this).EnableAutoManage(this, this).AddApi(Auth.GOOGLE_SIGN_IN_API, gso).Build();

            //Set our view from the "main" layout resource
            SetContentView(Globals.LoadingApp);

            //Set Cache object
            SimpleStorage.SetContext(Globals.Context);
            Globals.CacheStorage = SimpleStorage.EditGroup("Cache");

            if (Globals.MonthsList == null)
            {
                Globals.MonthsList = new List<Month>();
            }
        }

        public void ProceedLoading()
		{
            if (Globals.Active != 0)
            {
                SetContentView(Globals.LoadingApp);
                Globals.Active = 0;
                return;
            }
            //load all data
            else
            {
                if (!Globals.LoggedIn)
                {
                    Globals.LoggedIn = true;
                    bool hasPIN = Globals.CacheStorage.Get<bool>("HasPIN");
                    string PIN = Globals.CacheStorage.Get<string>("PIN");
                    bool localSecurity = Globals.CacheStorage.Get<bool>("LocalSecurity");
                    bool welcome = Globals.CacheStorage.Get<bool>("GoToWelcome");
                    //todo go to welcome if for first time, go to autologin if user wants or direct to mainmenu
                    if (!welcome)
                    {
                        Globals.CacheStorage.Put("GoToWelcome", true);
                    }
                    else if (localSecurity)
                    {
                        Globals.KeyguardManager = (KeyguardManager)GetSystemService(Context.KeyguardService);
                        if (Globals.KeyguardManager.IsKeyguardSecure)
                        {
                            Intent intent = Globals.KeyguardManager.CreateConfirmDeviceCredentialIntent((string)null, (string)null);
                            if (intent != null)
                            {
                                StartActivityForResult(intent, 1);
                            }
                        }
                        else
                        {
                            //use our login if setup or not at all
                            if (hasPIN)
                            {
                                SetContentView(Globals.AutoLogin);
                            }
                            else
                            {
                                SetContentView(Globals.AccountsMenu);
                            }
                        }
                    }
                    else if (hasPIN && !string.IsNullOrWhiteSpace(PIN))
                    {
                        SetContentView(Globals.AutoLogin);
                    }
                    else
                    {
                        SetContentView(Globals.AccountsMenu);
                    }
                }
                else
                {
                    SetContentView(Globals.AccountsMenu);
                }
            }
        }

        public void RequestPermissions()
		{
            if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
			{
				SetContentView(Globals.LoadingApp);
				Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
				alert.SetTitle("We value your privacy!");
				alert.SetMessage("Many Manager regularly saves your data on your device. So please grant access to device storage.");
				alert.SetPositiveButton("Accept", (senderAlert, args) =>
				{
                    if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
                    {
                        Globals.Active++;
                        RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 101);
                    }
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
					{
						Globals.Active++;
						RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, 101);
					}
					Globals.timer = new Timer();
					Globals.timer.Start();
					Globals.timer.Enabled = true;
					Globals.timer.Interval = 1000;
					Globals.timer.Elapsed += T_Elapsed;
					do
					{
						System.Threading.Thread.Sleep(400);
                        if (Globals.timeElapsed > 2000)
						{
							Android.App.AlertDialog.Builder a = new Android.App.AlertDialog.Builder(Globals.Context);
							a.SetTitle("");
							a.SetMessage("App can't work without storing data on your device.");
							a.SetPositiveButton("Ok", (senderA, arg) =>
							{
						        if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Android.Content.PM.Permission.Granted)
						        {
									Globals.timer.Enabled = false;
									Globals.timer.Stop();
									ProceedLoading();
						        }
						        Finish();
					        });
							Dialog d = a.Create();
							d.Show();
							return;
						}
					}
					while (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted);
					Globals.timer.Enabled = false;
					Globals.timer.Stop();
					ProceedLoading();
				});
				alert.SetNegativeButton("Reject", (senderAlert, args) =>
				{
					Android.App.AlertDialog.Builder a = new Android.App.AlertDialog.Builder(Globals.Context);
					a.SetTitle("");
					a.SetMessage("App can't work without storing data on your device.");
					a.SetPositiveButton("Ok", (senderA, arg) =>
					{
						Finish();
					});
					Dialog d = a.Create();
					d.Show();
				});
				Dialog dialog = alert.Create();
				dialog.Show();
			}
			else
			{
				ProceedLoading();
			}
		}

		private void T_Elapsed(object sender, ElapsedEventArgs e)
		{
            Globals.timeElapsed += 1000;
            if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
            {
                Globals.timer.Enabled = false;
                Globals.timer.Stop();
                ProceedLoading();
            }
        }

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
            if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
            {
                Globals.timer.Enabled = false;
                Globals.timer.Stop();
                ProceedLoading();
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnResume()
        {
            base.OnResume();

            RequestPermissions();

            if (Globals.returnFromPause)
			{
                SetContentView(Globals.PauseForm);
                Globals.returnFromPause = false;
                Globals.PauseForm = 0;
            }
            else if (Globals.CurrentForm == Globals.LoadingApp)
            {
                ProceedLoading();
            }
        }

		protected override void OnPause()
		{
			base.OnPause();

            HideInput();

            Globals.returnFromPause = true;

            //save current data
            if (Globals.CurrentForm == Globals.TransactionMenu)
            {
                Category cat = CategoriesGlobals.SearchOrAddNewCategory(Globals.spiCategory.SelectedItem.ToString());
                MoneyGlobals.PendingTransaction = new Transaction(Utils.ConvertStringToDouble(Globals.txtAddSum.Text), Utils.ConvertStringToDate(Globals.txtAddDate.Text), Globals.txtAddName.Text
                , TagsGlobals.SearchOrAddNewTag(Globals.txtAddTag.Text), cat, cat != null ? cat.Color : SKColor.Empty, false
                , Globals.chkHasCarConsumption.Checked, Utils.ConvertStringToDouble(Globals.txtAddKilometersRan.Text), Utils.ConvertStringToDouble(Globals.txtAddLiters.Text), Utils.ConvertStringToDouble(Globals.txtAddPrice.Text));

                if (Globals.chkRepetitiveTransaction.Checked)
                {
                    MoneyGlobals.PendingTransaction.RepetitiveInterval = ProcessRepetitiveInterval(false);
                }
            }


            if (Globals.CurrentForm == Globals.LoadingApp)
			{
                Globals.PauseForm = Globals.AccountsMenu;
            }
			else
			{
                Globals.PauseForm = Globals.CurrentForm;
			}
        }

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                // Challenge completed, proceed with using cipher
                if (resultCode == Result.Ok)
                {
                    //SetContentView(Globals.MainMenu);
                }
                else
                {
                    Globals.LoggedIn = false;
                    // The user canceled or didn’t complete the lock screen
                    // operation. Go to error/cancellation flow.
                }
            }
        }

        public override void OnBackPressed()
        {
            if (Globals.SpecificForm == Globals.CarMenu && Globals.CurrentForm == Globals.CarMenu)
			{
                Globals.SpecificForm = 0;
            }
			else if (Globals.SpecificForm == Globals.CarConsumption && Globals.CurrentForm == Globals.CarConsumption)
			{
                Globals.SpecificForm = 0;
            }
            //clear transactions flags
			if (Globals.CurrentForm == Globals.TransactionMenu /*|| Globals.CurrentForm == Globals.ModifyTransaction*/)
			{
                ClearTransactionFlags();
                ClearTransactionMenuTextBoxes();
            }
            //clear selections and variables from Tag
            if (Globals.CurrentForm == Globals.TagMenu)
            {
                NotesGlobals.CheckAll();
                CarsGlobals.CheckAll();
                TagsGlobals.CheckAll();

                Globals.NoteToAddInTag = false;
                Globals.SelectedNotesForTag.Clear();
                Globals.CarToAddInTag = false;
                Globals.SelectedCarsForTag.Clear();
                TagsGlobals.PendingTag = null;
                TagsGlobals.SelectedTag = null;
            }

            //clear selections and variables from Categories
            if (Globals.CurrentForm == Globals.CategoryMenu)
            {
                CategoriesGlobals.CheckAll();

                CategoriesGlobals.PendingCategory = null;
                CategoriesGlobals.SelectedCategory = null;
            }

            //if (Globals.CurrentForm == Globals.TagsMenu)
            //{
            //             (Globals.grdTags.Adapter as TagsAdapter).EditState(false);
            //         }
            //refresh sorting
            /*else */
            if (Globals.CurrentForm == Globals.TransactionsMenu)
			{
                Sorting.CurrentSort = SortBy.DateLatest;
			}
            Globals.GetContentView();
        }

        public void SetContentView(int i, bool changeTheme = false)
        {
            HideInput();

            //clear transaction flags used for notes, tags, cars etc
            if (!Globals.returnFromPause && /*(*/Globals.CurrentForm == Globals.TransactionMenu /*|| Globals.CurrentForm == Globals.ModifyTransaction)*/)
            {
                if (i != Globals.TagsMenu && i != Globals.NotesMenu && i != Globals.CarsMenu)
                {
                    ClearTransactionFlags();
                }
            }

            //refresh transactions edit state
            MoneyGlobals.TransactionsEditState = MoneyGlobals.TransactionsEditState ? false : MoneyGlobals.TransactionsEditState;

            //refresh Sorting
            //if (Globals.CurrentForm == Globals.TransactionsMenu)
            //{
            //    Sorting.CurrentSort = SortBy.DateLatest;
            //}

            //if already on same form don't reload it
            if (i == Globals.CurrentForm && !changeTheme)
			{
                return;
			}

            //Hide input
            var inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
			if (inputMethodManager.IsAcceptingText)
			{
                inputMethodManager.HideSoftInputFromWindow(Globals.Activity.Window.DecorView.WindowToken, 0);
			}

            //Set parent form to know for back key to which one to go back
            Globals.CurrentForm = i;
            base.SetContentView(i);

            //Money Part
            if (i == Globals.YearlyStats)
            {
                Globals.ParentForm = Globals.MainMenu;
            }
            else if (i == Globals.TransactionsMenu || i == Globals.TransactionMenu || i == Globals.Filters)
            {
                if (i == Globals.TransactionsMenu)
                {
                    Globals.TagsMenuParent = 0;
                    Globals.ParentForm = Globals.YearlyStats;
                }
				else if (i == Globals.TransactionMenu || i == Globals.Filters)
				{
                    if (Globals.ParentForm == Globals.CarMenu)
                    {
                        //came from CarConsumption and we need to go back there
                        Globals.ParentForm = Globals.CarConsumption;
                    }
                    else if (Globals.ParentForm == Globals.CarsMenu)
                    {
                        //came from CarMenu and we need to go back there
                        Globals.ParentForm = Globals.CarMenu;
                    }
                    else
                    {
                        Globals.ParentForm = Globals.TransactionsMenu;
                    }
                }
                //Globals.ParentForm = Globals.YearlyStats;
            }
			else if (i == Globals.Sorts)
			{
                Globals.ParentForm = Globals.TransactionsMenu;
			}
            //else if (/*i == Globals.ModifyTransaction ||*/ i == Globals.Filters)
            //{
            //    if (Globals.ParentForm == Globals.CarMenu)
            //    {
            //        //came from CarConsumption and we need to go back there
            //        Globals.ParentForm = Globals.CarConsumption;
            //    }
            //    else if (Globals.ParentForm == Globals.CarsMenu)
            //    {
            //        //came from CarMenu and we need to go back there
            //        Globals.ParentForm = Globals.CarMenu;
            //    }
            //    else
            //    {
            //        Globals.ParentForm = Globals.TransactionsMenu;
            //    }
            //}
            //Accounts Part
            else if (i == Globals.AccountMenu)
            {
                Globals.ParentForm = Globals.AccountsMenu;
            }
            else if (i == Globals.AccountsMenu)
			{
                Globals.ParentForm = Globals.AccountsMenu;
            }
			//MainMenu Part
            else if (i == Globals.AutoLogin)
			{
                Globals.ParentForm = Globals.MainMenu;
			}
            else if (i == Globals.MainMenu)
            {
                Globals.ParentForm = Globals.AccountsMenu;
            }
            //Settings Part
            else if (i == Globals.Settings)
            {
                Globals.ParentForm = Globals.PreviousForm;
            }
            else if (i == Globals.ChangeCategoryOrTagColor)
            {
                Globals.ParentForm = Globals.PreviousForm;
            }
            else if (i == Globals.ChangeCharityPercentage || i == Globals.ChangeLogin || i == Globals.ChangeUnitsType)
			{
                Globals.ParentForm = Globals.Settings;
			}
            //Notes Part
            else if (i == Globals.NotesMenu || i == Globals.CarsMenu)
            {
                if (Globals.PreviousForm == Globals.TagMenu)
				{
                    Globals.ParentForm = Globals.PreviousForm;
                }
				else
				{
                    Globals.ParentForm = Globals.MainMenu;
				}
            }
            else if (i == Globals.NoteMenu)
            {
                Globals.ParentForm = Globals.NotesMenu;
            }
            else if (i == Globals.MiniNoteMenu)
            {
                Globals.ParentForm = Globals.NoteMenu;
            }
			else if (i == Globals.NoteSettings)
			{
				if (NotesGlobals.SelectedNote != null && NotesGlobals.SelectedNote.HasMiniNotes)
				{
                    Globals.ParentForm = Globals.NoteMenu;
                }
				else
				{
                    Globals.ParentForm = Globals.NotesMenu;
                }
			}
			//Cars Part
            else if (i == Globals.CarMenu)
            {
                Globals.ParentForm = Globals.CarsMenu;
            }
			else if (i == Globals.CarObject || i == Globals.CarConsumption || i == Globals.CarPropertiesMenu || i == Globals.RepairMenu)
			{
                Globals.ParentForm = Globals.CarMenu;
			}
			//Tags
			else if (i == Globals.TagsMenu)
			{
				if (Globals.TagsMenuParent != 0)
				{
                    Globals.ParentForm = Globals.TagsMenuParent;
                }
				else
				{
                    Globals.ParentForm = Globals.MainMenu;
				}
			}
            else if (i == Globals.TagMenu)
            {
                Globals.ParentForm = Globals.TagsMenu;
            }
			//Categories
			else if (i == Globals.CategoriesMenu)
			{
                Globals.ParentForm = Globals.MainMenu;
			}
            else if (i == Globals.CategoryMenu)
            {
                Globals.ParentForm = Globals.CategoriesMenu;
            }

            Globals.PreviousForm = Globals.CurrentForm;

            //Load Events & Stats for MainMenu Form
            if (i == Globals.MainMenu)
            {
                MainMenu_Events();
            }
            //Load Events & Stats for Notes Form
            else if (i == Globals.NotesMenu)
            {
                NotesMenu_Events();
            }
            else if (i == Globals.NoteSettings)
            {
                NoteSettings_Events();
            }
            else if (i == Globals.NoteMenu)
            {
                NoteMenu_Events();
            }
            else if (i == Globals.MiniNoteMenu)
            {
                MiniNoteMenu_Events();
            }
            //Accounts Part
            else if (i == Globals.AccountMenu)
            {
                AccountMenu_Events();
            }
            else if (i == Globals.AccountsMenu)
            {
                AccountsMenu_Events();
            }
            //Events & Stats for Settings Forms
            else if (i == Globals.AutoLogin)
            {
                AutoLogin_Events();
            }
			else if (i == Globals.ChangeLogin)
			{
                ChangeLogin_Events();
            }
            else if (i == Globals.Settings)
            {
                Settings_Events();
            }
            else if (i == Globals.ChangeCharityPercentage)
            {
                ChangeCharityPercentage_Events();
            }
            else if (i == Globals.ChangeCategoryOrTagColor)
            {
                ChangeCategoryOrTagColor_Events();
            }
            else if (i == Globals.ChangeUnitsType)
            {
                ChangeUnitsType_Events();
            }
            //Load Events & Stats for Money Form
            else if (i == Globals.YearlyStats)
            {
                MoneyLayoutTop_Events();
                YearlyStats_Events();
            }
            else if (i == Globals.TransactionsMenu)
            {
                MoneyLayoutTop_Events();
                TransactionsMenu_Events();
            }
            else if (i == Globals.Filters)
			{
                Filters_Events();
            }
            else if (i == Globals.Sorts)
            {
                Sorts_Events();
            }
            else if (i == Globals.TransactionMenu)
            {
                MoneyLayoutTop_Events();
                TransactionMenu_Events();
            }
			//Events & Stats for Cars Part
			else if (i == Globals.CarsMenu)
			{
                CarsMenu_Events();
            }
            else if (i == Globals.CarMenu)
            {
                CarMenu_Events();
            }
			else if (i == Globals.CarObject)
			{
                CarObject_Events();
			}
			else if (i == Globals.CarConsumption)
			{
                CarConsumption_Events();
			}
            else if (i == Globals.CarPropertiesMenu)
            {
                CarProperties_Events();
            }
            else if (i == Globals.RepairMenu)
            {
                RepairMenu_Events();
            }
            //Events & Stats for Tags Part
            else if (i == Globals.TagsMenu)
            {
                TagsMenu_Events();
            }
            else if (i == Globals.TagMenu)
            {
                TagMenu_Events();
            }
            //Events & Stats for Categories Part
            else if (i == Globals.CategoriesMenu)
            {
                CategoriesMenu_Events();
            }
            else if (i == Globals.CategoryMenu)
            {
                CategoryMenu_Events();
            }
        }

        public override void Finish()
        {
            HideInput();
            base.Finish();
        }

		public override void FinishActivity(int requestCode)
		{
            HideInput();
            base.FinishActivity(requestCode);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
        {
            bool toReturn;
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            toReturn = base.OnCreateOptionsMenu(menu);
            //change menuitem colors
            for (int i = 0; i < menu.Size(); i++)
            {
                IMenuItem item = menu.GetItem(i);
                Android.Text.SpannableString spanString = new Android.Text.SpannableString(menu.GetItem(i).ToString());
                spanString.SetSpan(new ForegroundColorSpan(Android.Graphics.Color.White), 0, spanString.Length(), 0);
                item.SetTitle(spanString);
                if (i == 1)
                {
                    item.SetChecked(Globals.CurrentTheme == Resource.Style.AppThemeDark);
                }
            }
            menu.GetItem(1).SetCheckable(true);
            return toReturn;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Id.menuItem1:
                    {
                        SetContentView(Globals.Settings);
                        return true;
                    }
                case Id.menuItem2:
                    {
                        if (!item.IsChecked)
                        {
                            SetTheme(Style.AppThemeDark);
                            item.SetChecked(true);
                        }
                        else
                        {
                            SetTheme(Style.AppTheme);
                            item.SetChecked(false);
                        }
                        SetContentView(Globals.CurrentForm, changeTheme:true);
                        return true;
                    }
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}