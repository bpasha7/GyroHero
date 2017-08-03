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
using System.Threading.Tasks;
using System.Threading;

namespace Mob.Droid
{
    [Service]
    public class BackgroundTaskService : Service
    {
        CancellationTokenSource _cts = new CancellationTokenSource();

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            Task.Run(() => {
                try
                {
                    PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
                    PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
                    wl.Acquire();
                    var counter = new TimerTask();
                    counter.RunTimer(_cts, startId).Wait();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        var message = new CancelledMessage();
                        //Device.BeginInvokeOnMainThread(
                        //    () => MessagingCenter.Send(message, "CancelledMessage")
                        //);
                    }
                }

            }, _cts.Token);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }
            base.OnDestroy();
        }

    }
}