using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Mob.Contents;
using Xamarin.Forms;

namespace Mob
{
    public partial class App : Application
    {
        public delegate void DoNotification(string title, string message);
        public delegate bool DoAlert(string Title, string Message, string PositiveBtn, string NegativeBtn, string PositiveToast, string NegativeToast);
        public delegate void DoToast(string Satatment);
        public delegate void DoProgress(bool run);
        public delegate void ServiceManager(string Msg);
        public static DoProgress Progress { get; set; }
        public static DoNotification _doNotification;
        public static DoAlert _doAlert;
        public static DoToast Toast;
        public static string DoNotify
        {
            set
            {
                _doNotification("GyroHero прокат", value);
            }
        }

       // public static StoreRents()

        public static Task MainTask { get; set; }

        public static ServiceManager StartService;
        public static ServiceManager StopService;

        static Repository database;
        public static Repository Database
        {
            get
            {
                if (database == null)
                {
                    database = new Repository();
                }
                return database;
            }

        }

        public App(DoNotification doNotification, DoAlert doAlert, DoToast doToast/*, DoProgress progress, ServiceManager start, ServiceManager stop*/)
        {
            try
            {
                
                InitializeComponent();
                _doNotification = doNotification;
                _doAlert = doAlert;
                Toast = doToast;
                var main = new NavigationPage(new RentInfo(doNotification));
                var master = new Menu(main);
                master.Detail = main;
                MainPage = master;
            }
            catch (Exception ex)
            {
                _doNotification("GyroHero прокат", ex.Message);
            }
        }


        protected override void OnStart()
        {
            _doNotification("GyroHero прокат", "Привет, сегодня у нас будет много клиентов:)");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
