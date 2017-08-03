using System;
using Xamarin.Forms;

namespace Mob
{
    public partial class App : Application
    {
        #region Delegates
        public delegate void DoNotification(string title, string message);
        public delegate void DoToast(string Satatment);
        #endregion
        #region Private members
        /// <summary>
        /// SQLite Database repository
        /// </summary>
        private static Repository database;
        /// <summary>
        /// Main navigation page
        /// </summary>
        private NavigationPage _main;
        /// <summary>
        /// Function which do notification thoght MainActivity
        /// </summary>
        private static DoNotification _doNotification;
        #endregion
        #region Public members
        /// <summary>
        /// Toast
        /// </summary>
        public static DoToast Toast;
        /// <summary>
        /// Send notifucation to user
        /// </summary>
        public static string DoNotify
        {
            set
            {
                _doNotification("GyroHero прокат", value);
            }
        }
        /// <summary>
        /// SQLite Database repository provider
        /// </summary>
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
        #endregion
        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="doNotification">Notification functions</param>
        /// <param name="doToast">Toast function</param>
        public App(DoNotification doNotification, DoToast doToast)
        {
            try
            {
                InitializeComponent();
                _doNotification = doNotification;
                Toast = doToast;
                _main = new NavigationPage(new RentInfo(doNotification));
                var master = new Menu(_main);
                master.Detail = _main;
                MainPage = master;
            }
            catch (Exception ex)
            {
                _doNotification("GyroHero прокат", ex.Message);
            }
        }
        /// <summary>
        /// OnStart event
        /// </summary>
        protected override void OnStart()
        {
            _doNotification("GyroHero прокат", "Привет, сегодня у нас будет много клиентов:)");
        }

    }
}
