using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;
using System.Threading.Tasks;
using static Android.Media.Audiofx.BassBoost;

namespace Mob.Droid
{
    [Activity(Label = "Mob", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        NotificationManager _notificationManager;
        int notificationId = 0;
        AlertDialog.Builder _alert;
        ProgressDialog _progress;

        void StartService()
        {
            var intent = new Intent(this, typeof(BackgroundTaskService));
            //String packageName = Context.GeetPackageName();
            //PowerManager pm = (PowerManager)Context.GetSystemService("POWER_SERVICE");
            //if (pm.isIgnoringBatteryOptimizations(packageName))
            //    intent.SetAction(SettingsACTION_IGNORE_BATTERY_OPTIMIZATION_SETTINGS);
            //else
            //{
            //    intent.setAction(Settings.ACTION_REQUEST_IGNORE_BATTERY_OPTIMIZATIONS);
            //    intent.setData(Uri.parse("package:" + packageName));
            //}
            StartService(intent);
            //MessagingCenter.Subscribe<StartBackgroundTask>(this, "StartBackgroundTask", message  =>
            //{
            //    var intent = new Intent(this, typeof(BackgroundTaskService));
            //    StartService(intent);
            //});
            ////StartService();
            //var mes = new StartBackgroundTask();
            //MessagingCenter.Send(mes, "StartBackgroundTask");
        }

        void StopService()
        {
            MessagingCenter.Subscribe<StopBackgroundTask>(this, "BackgroundTaskService", message => {
                var intent = new Intent(this, typeof(BackgroundTaskService));
                StopService(intent);
            });
        }

        private void ShowProgress(bool run)
        {
            if (!run)
            {
                _progress.Hide();
                return;
            }
            _progress =
            ProgressDialog.Show(this, "Please wait...", "Checking account info...", true);
        }
        private void HideProgress()
        {
            _progress.Hide();
        }

        private void DoToast(string Statement)
        {
            Toast.MakeText(this, Statement, ToastLength.Short).Show();
        }
        private bool DoAlert(string Title, string Message, string PositiveBtn, string NegativeBtn, string PositiveToast, string NegativeToast)
        {
            var dialogResult = false;
            _alert.SetTitle(Title);
            _alert.SetMessage(Message);
            _alert.SetPositiveButton(PositiveBtn, (senderAlert, args) =>
            {
                Toast.MakeText(this, PositiveToast, ToastLength.Short).Show();
                dialogResult = true;
            });
            _alert.SetNegativeButton(NegativeBtn, (senderAlert, args) =>
            {
                Toast.MakeText(this, NegativeToast, ToastLength.Short).Show();
                dialogResult = false;
            });
            _alert.SetCancelable(false);
            _alert.SetIcon(Resource.Drawable.user);
            Dialog dialog = _alert.Create();
            dialog.Show();
            return dialogResult;
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var userSelectedCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = userSelectedCulture;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            //this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            _alert = new AlertDialog.Builder(this);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            _notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;
            LoadApplication(new Mob.App(DoNotification, DoAlert, DoToast/*, ShowProgress, StartService, StopService*/));
            StartService();
            
        }       

        public void DoNotification(string title, string message)
        {
            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetVibrate(new long[] { 500, 500, 500 })
                .SetDefaults(NotificationDefaults.Sound)
                .SetSmallIcon(Resource.Drawable.Noti);

            // Build the notification:
            Notification notification = builder.Build();
            _notificationManager.Notify(notificationId++, notification);
        }


    }
}

