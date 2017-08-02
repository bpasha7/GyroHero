using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mob
{
    public class TimerTask
    {
        public async Task RunTimer(CancellationTokenSource _cts, int startId/*, TimeSpan _time, TimeSpan _limit, Label _descLabel, Label _titleLabel*/)
        {
            await Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        if (_cts != null)
                            _cts.Token.ThrowIfCancellationRequested();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var message = new TickMessage();
                            Device.BeginInvokeOnMainThread(() => {
                                MessagingCenter.Send<TickMessage>(message, $"TickMessage");
                            });
                        });
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }
    }
}
