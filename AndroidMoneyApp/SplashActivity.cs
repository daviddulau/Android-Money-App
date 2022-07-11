using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;

namespace ManyManager
{
    [Activity(Theme = "@style/AppTheme", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        public static ProgressBar ProgressBar;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            SetContentView(Resource.Layout.LoadingApp);
            //ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            //ProgressBar.Progress = 100;
            base.OnResume();
            //Task startupWork = new Task(() => { SimulateStartup(); });
            //ProgressBar.SetProgress(33, true);
            //startupWork.Start();
            SimulateStartup();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        // Simulates background work that happens behind the splash screen
        void SimulateStartup()
        {
            //ProgressBar.SetProgress(66, true);
            //Task.Delay(1000);
            Thread.Sleep(500);
            //ProgressBar.SetProgress(80, true);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}