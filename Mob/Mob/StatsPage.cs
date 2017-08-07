using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Mob
{
    public class RentItemCell : ViewCell
    {
        public RentItemCell()
        {
            var time = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontSize = 18,
                FontAttributes = Xamarin.Forms.FontAttributes.Bold,
                TextColor = Color.Black
            };
            time.SetBinding(Label.TextProperty, new Binding("Time", stringFormat: "{0:HH:mm:ss}"));
            var money = new Label {  VerticalOptions = LayoutOptions.CenterAndExpand};
            money.SetBinding(Label.TextProperty, new Binding("Payment", stringFormat: "{0}₽"));
            var type = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontSize = 12,
                TextColor = Color.Black
            };
            type.SetBinding(Label.TextProperty, "Type");

            var aboutLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { time, type }
            };

            var infoLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(10),
                Children = { aboutLayout, money }
            };
            View = infoLayout;

        }
    }
    public class StatsPage : ContentPage
    {
        ListView listView;
        private ObservableCollection<RentStringFormats> _rentList;
        decimal sum = 0;
        private Image _reportImg;
        private Label sumLbl;
        private DatePicker _date;

        public delegate void SetGesture(bool state);

        public SetGesture SetGestureState { set; get; }

        private void LoadStats(DateTime dateTime)
        {
            var items = App.Database.GetRents(dateTime);
            sum = 0;
            _rentList.Clear();
            foreach (var item in items)
            {
                _rentList.Add(new RentStringFormats(item));
                sum += item.Payment;
            }
            sumLbl.Text = $"Всего: {sum}₽";
            _reportImg.IsEnabled = true;

        }
        public StatsPage()
        {
            try
            {
                this.Title = "Статистика";
                sumLbl = new Label
                {
                    TextColor = Color.Green,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };
                _reportImg = new Image { IsEnabled = false, Source = "email.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                listView = new ListView();
                _rentList = new ObservableCollection<RentStringFormats>(); LoadStats(DateTime.Now.Date);
                listView.ItemsSource = _rentList;
                listView.ItemTemplate = new DataTemplate(typeof(RentItemCell));
                listView.RowHeight = 60;
                listView.ItemTapped += async (sender, e) =>
                {
                    RentStringFormats item = (RentStringFormats)e.Item;
                    await DisplayAlert("Информация", $"{item.ClientName}\nВ прокате {item.RentTime}\nПолучено {item.Payment:c}\n", "OK");
                    ((ListView)sender).SelectedItem = null;
                };
                var dateLbl = new Label { Text = "Дата: ", FontSize = 18, VerticalTextAlignment = TextAlignment.Center };
                _date = new DatePicker { Format = @"dd-MM-yyyy" };
                _date.DateSelected += _date_DateSelected;
                _date.Date = DateTime.Now.Date;
                _reportImg.IsVisible = _date.Date == DateTime.Now.Date ? false : true;
                _reportImg.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = async (v, o) =>
                    {
                        try
                        {
                            var ans = await DisplayAlert("Подтвердите", $"Отправить отчет за {_date.Date.ToString("D")}", "Да", "Нет");
                            if (ans == true)
                            {
                                var report = new EmailReport(_date.Date);
                                report.SendAsync(false);
                            }
                            else
                            {
                                App.Toast("Отмена");
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                });
                var dateLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(5, 0, 5, 0),
                    Children = { dateLbl, _date, _reportImg }
                };

                StackLayout viewLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(10, 10, 10, 0),
                    Children = { dateLayout, listView, sumLbl }
                };

                Content = viewLayout;
            }
            catch (Exception ex)
            {
                App.Database.SaveError(new Mob.Dto.Error { Date = DateTime.Now, Invoker = this.GetType().Name, Message = ex.Message });
            }
        }
        private void _date_DateSelected(object sender, DateChangedEventArgs e)
        {
            LoadStats(_date.Date);
            _reportImg.IsVisible = _date.Date == DateTime.Now.Date ? false : true;
            _reportImg.IsVisible = _rentList.Count == 0 ? false : true;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}