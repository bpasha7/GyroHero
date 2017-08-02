
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Mob
{
    public class RentEnd : ContentPage
    {

        Button _submit;
        Label _limit;
        Label _time;
        public Entry Extra;
        public RentEnd(RentItem rentInfo)
        {
            this.Title = "Оформление доплаты";

            _limit = new Label { Text = $"{rentInfo.RentPrice.Time}" };
            _time = new Label { Text = $"Перекатано {rentInfo.Overtime}" };
            Extra = new Entry { Placeholder = "Сумма" };
            _submit = new Button { Text = "OK" };
            _submit.Clicked += _submit_Clicked;

            Content = new StackLayout
            {
                Children = {
                    _limit,
                    _time,
                    Extra,
                    _submit
                }
            };
        }

        private void _submit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
}
