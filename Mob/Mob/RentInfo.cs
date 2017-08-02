using Mob.Dto;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mob
{

    public delegate void RentManipulate(RentItem rent);
    public delegate string Navi(RentEnd page);
    public delegate Task<bool> ConfirmActionResult(string text);
    public delegate Task<string> InputBox(string text);

    public class RentStringFormats
    {
        public string Id { get; set; }
        public string RentTime { get; set; }
        public decimal Payment { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string ClientName { get; set; }

        public RentStringFormats(Rent rent)
        {
            Id = rent.Id.ToString();
            RentTime = rent.RentTime.ToString(@"hh\:mm");
            Payment = rent.Payment;//.ToString();
            Date = rent.Date.ToString();
            Time = rent.Time.ToString(@"hh\:mm");
            switch (rent.Type)
            {

                case "G": Type = "Гироскутер"; break;
                case "C": Type = "Велосипед"; break;
                default:
                    Type = "";
                    break;
            }
            ClientName = rent.ClientName;
        }

    }

    public class Rent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public TimeSpan RentTime { get; set; }
        public string ClientName { get; set; }
        public decimal Payment { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Type { get; set; }

    }

    public class ClientInfo
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string DocumentType { get; set; }
        public string Serial { get; set; }
        public string Number { get; set; }
    }

    public class RentItem
    {
        public string ID { get; set; }
        public string ClientName { get { return Client.Name; } }
        public string Limit { get { return RentPrice.Time.Ticks.ToString(); } }
        public string Time { get { return RentPrice.Time.ToString(); } }
        public TimeSpan Overtime { get; set; }
        public PriceInfo RentPrice { get; set; }
        public ClientInfo Client { get; set; }
        public RentManipulate Delete { get; set; }
        public ConfirmActionResult Confirm { get; set; }
        public Navi Navigate { get; set; }
        public InputBox Test { get; set; }
        public RentItem(ClientInfo client, PriceInfo rentPrice)
        {
            Client = client;
            RentPrice = rentPrice;
        }
    }

    public class RentInfo : TabbedPage
    {

        private Mob.App.DoNotification _doNotification;
        public RentInfo(Mob.App.DoNotification doNotification)
        {
            try
            {
                _doNotification = doNotification;
                this.Title = "Клиенты";
                this.BarBackgroundColor = Color.Violet;
                this.BarBackgroundColor = Color.WhiteSmoke;
                this.BarTextColor = Color.Black;
                //this.ToolbarItems.Add(new ToolbarItem("test", "adduser.png", async() => { await this.ScaleTo(0.8, 50, Easing.Linear); await Task.Delay(100); await this.ScaleTo(1, 50, Easing.Linear); }));
                this.ItemsSource = new RentInfoTab[]
                {
                new RentInfoTab("Gyro", 1, "wheel.png" ),
                new RentInfoTab("Cycle", 2, "cycle.png" )
                };
                this.ItemTemplate = new DataTemplate(() =>
                {
                    return new RentTabContent();
                });
            }
            catch (Exception ex)
            {
                App.Database.SaveError(new Mob.Dto.Error { Date = DateTime.Now, Invoker = this.GetType().Name, Message = ex.Message });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }

    class RentInfoTab
    {
        public RentInfoTab(string name, int number, string icon)
        {
            this.Name = name;
            this.Number = number;
            this.Icon = icon;
        }
        public string Name { private set; get; }
        public int Number { private set; get; }
        public string Icon { private set; get; }
    }

    public class RentTabContent : ContentPage
    {
        private ObservableCollection<RentItem> _rentList;
        private int _lastId = 0;

        protected override void OnDisappearing()
        {
            //base.OnDisappearing();
        }
        private List<PriceInfo> _gyroPrices;
        private List<PriceInfo> _cyclePrices;
        private ListView listView;
        public async Task<bool> ConfirmActionResult(string text)
        {
            return await DisplayAlert("Подтвердите", text, "Да", "Нет");
        }

        public Task<string> InputBox(string text)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();

            var lblTitle = new Label { Text = text, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var lblMessage = new Label { Text = "Доплата", FontSize = 8 };
            var txtInput = new Entry { Keyboard = Keyboard.Numeric };

            var btnOk = new Button
            {
                Text = "Принять",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = txtInput.Text;
                await Navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await Navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };



            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            // var PaymentLayout

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, txtInput, lblMessage, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            Navigation.PushModalAsync(page);
            // open keyboard
            txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }

        //private ObservableCollection<RentItem> RestoreRents()
        //{

        //}

        public void AddNewRent(RentItem rentItem)
        {
            rentItem.Delete = DeleteRent;
            rentItem.Navigate = Navigate;
            rentItem.Confirm = ConfirmActionResult;
            rentItem.Test = InputBox;
            rentItem.ID = (++_lastId).ToString();
            _rentList.Add(rentItem);
            
            _rentList.Move(_rentList.Count - 1, 0);
            //_rentList = new ObservableCollection<RentItem>(
            //_rentList = new ObservableCollection<RentItem>(_rentList.OrderBy(r => r.RentPrice.Time));
        }

        public void DeleteRent(RentItem rent)
        {
            try
            {
                _rentList.Remove(rent);
            }
            catch (Exception ex)
            {
                DisplayAlert("Ошибка", "Не могу удалить:(", "OK");
            }
        }

        protected override void OnBindingContextChanged()
        {
            switch (this.Title)
            {
                case "Gyro":
                    _gyroPrices = App.Database.GetPrices("G");
                    break;
                case "Cycle":
                    _cyclePrices = App.Database.GetPrices("C");
                    break;
                default: break;
            }
        }

        public RentTabContent()
        {

            try
            {
                this.SetBinding(ContentPage.TitleProperty, "Name");
                this.SetBinding(ContentPage.IconProperty, "Icon");
                this.WidthRequest = 100;
                this.ToolbarItems.Clear();
                this.BackgroundColor = Color.Azure;
                ///==================================================
                listView = new ListView();
                _rentList = new ObservableCollection<RentItem>();
                listView.ItemsSource = _rentList;
                listView.RowHeight = 80;
                listView.BackgroundColor = Color.White;
                listView.ItemTemplate = new DataTemplate(typeof(ListItemCell));
                listView.ItemTapped += async (sender, e) =>
                {
                    RentItem item = (RentItem)e.Item;
                    await DisplayAlert("Информация", $"{item.ClientName}\n" +
                        $"Документ: {item.Client.DocumentType} серия {item.Client.Serial} номер {item.Client.Number}\n" +
                        $"Всего времени {item.RentPrice.Time}\n" +
                        $"Внесено {item.RentPrice.Price:c}", "OK");
                    ((ListView)sender).SelectedItem = null;
                };
                ///==================================================
                var btn = new Button { Text = "Новый", BackgroundColor = Color.White, TextColor = Color.Green };
                btn.Clicked += async (sender, args) =>
                    {
                        var p = this.Title == "Gyro" ? new Person(_gyroPrices) : new Person(_cyclePrices);
                        p.AddNewRentInfo = AddNewRent;
                        await Navigation.PushAsync(p);
                    };
                ///
                StackLayout viewLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(5, 10, 5, 0),
                    Children = { listView, btn }
                };
                Content = viewLayout;
            }
            catch (Exception ex)
            {
                App.Database.SaveError(new Mob.Dto.Error { Date = DateTime.Now, Invoker = this.GetType().Name, Message = ex.Message });
            }
        }

        public string Navigate(RentEnd page)
        {
            Navigation.PushModalAsync(page);
            return page.Extra.Text;
        }
    }

    public class ListItemCell : ViewCell
    {
        #region Controls
        private TimeSpan _limit;

        private TimeSpan _time;

        public int Diff { get { return (_limit - _time).Minutes; } } 

        private Label _titleLabel;

        private Label _descLabel;

        public string RentId { get; set; }

        private Image _button;

        private Editor T;
        private Editor ID;

        #endregion

        #region Menu
        MenuItem _deleteAction;
        MenuItem _callAction;
        MenuItem _completeAction;
        #endregion
        bool isNotify = false;
        private bool _inProgress;

        void StopReceivedMessages()
        {
            MessagingCenter.Unsubscribe<TickMessage>(this, "TickMessage");
            _inProgress = false;
        }

        void HandleReceivedMessages()
        {

            MessagingCenter.Subscribe<TickMessage>(this, "TickMessage", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _time = _time.Add(new TimeSpan(0, 0, 1));
                    if (_time > _limit)
                    {
                        _descLabel.TextColor = Color.Red;
                        _descLabel.Text = $"+{_time - _limit}";
                        if (!isNotify)
                        {
                            App.DoNotify = $"У {_titleLabel.Text} закончилось время!";
                            isNotify = true;
                        }
                    }
                    else
                    {
                        _descLabel.TextColor = Color.Green;
                        _descLabel.Text = $"{_limit - _time }";
                    }
                });
                _inProgress = true;
            });
        }

        public ListItemCell()
        {
            try
            {
                _titleLabel = new Label
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    FontSize = 25,
                    FontAttributes = Xamarin.Forms.FontAttributes.Bold,
                    TextColor = Color.Black
                };
                ID = new Editor { IsVisible = false };
                ID.SetBinding(Editor.TextProperty, "ID");
                T = new Editor { IsVisible = false };
                T.SetBinding(Editor.TextProperty, "Limit");
                _titleLabel.SetBinding(Label.TextProperty, "ClientName");
                _descLabel = new Label
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    FontSize = 12,
                    TextColor = Color.Black
                };
                _descLabel.SetBinding(Label.TextProperty, "Time");
                StackLayout viewLayoutItem = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Orientation = StackOrientation.Vertical,
                    Children = { _titleLabel, _descLabel }
                };
                _time = new TimeSpan(0, 0, 0, 0);
                _button = new Image
                {
                    Source = "play.png",
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                _button.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = (v, o) =>
                    {
                        _button.Source = _inProgress ? "play.png" : "stop.png";
                        if (_inProgress)
                        {
                            StopReceivedMessages();
                        }
                        else
                        {
                            _limit = new TimeSpan(Convert.ToInt64(T.Text));
                            HandleReceivedMessages();
                        }
                    },
                    NumberOfTapsRequired = 1
                }
            );
                _completeAction = new MenuItem { Icon = "thumb.png", IsDestructive = true, Text = "Завершить" };
                _completeAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                _completeAction.Clicked += async (sender, e) =>
                {
                    try
                    {
                        var mi = ((MenuItem)sender);
                        var item = (RentItem)mi.CommandParameter;
                        if (_time.TotalSeconds == 0)
                        {
                            App.Toast("Нельзя завершить!");
                            return;
                        }
                        var ans = await item.Confirm($"Завершить?");
                        if (ans != true)
                            return;
                        if (_inProgress)
                            StopReceivedMessages();
                            //_cts?.Cancel();
                        _button.Source = "stop.png";
                        var rent = new Rent
                        {
                            Date = DateTime.Now.Date,
                            Time = DateTime.Now.TimeOfDay,
                            Payment = item.RentPrice.Price,
                            Type = item.RentPrice.Vehicle,
                            ClientName = item.Client.Name,
                            RentTime = _time
                        };
                        _inProgress = false;
                        if (_time > _limit)
                        {
                            var overtime = _time - _limit;
                            //var overPay = Convert.ToDecimal(overtime.TotalMinutes / item.RentPrice.Time.TotalMinutes * (double)item.RentPrice.Price);
                            var res = await item.Test($"Необходимо доплатить за {overtime}");
                            if (res == null)
                                return;
                            if (res != "")
                            {
                                rent.Payment += Convert.ToDecimal(res);
                            }
                        }
                        App.Database.SaveRent(rent);
                        item.Delete(item);
                        App.Toast($"+{rent.Payment:c0}");
                    }
                    catch (Exception ex)
                    {
                        App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = this.GetType().Name, Message = ex.Message });
                    }
                };

                _callAction = new MenuItem { Icon = "phone.png", IsDestructive = true, Text = "Вызвать" };
                _callAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                _callAction.Clicked +=
                    async (sender, e) =>
                    {
                        var mi = ((MenuItem)sender);
                        var item = (RentItem)mi.CommandParameter;
                        if (item.Client.Phone == "")
                        {
                            App.Toast("Номер отсутствует!");
                            return;
                        }
                        var ans = await item.Confirm($"Вызвать {item.ClientName}?");
                        if (ans == true)
                        {
                            Device.OpenUri(new Uri($"tel:{item.Client.Phone}"));
                        }
                        else
                        {
                            App.Toast("Отмена");
                        }
                    };
                _deleteAction = new MenuItem { Icon = "garbage.png", IsDestructive = true, Text = "Удалить" };
                _deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                _deleteAction.Clicked +=
                    async (sender, e) =>
                    {
                        var mi = ((MenuItem)sender);
                        var item = (RentItem)mi.CommandParameter;
                        if (_time.TotalSeconds != 0)
                        {
                            App.Toast("Нельзя удалить!");
                            return;
                        }
                        var ans = await item.Confirm($"Удалить?");
                        if (ans != true)
                            return;
                        //_cts?.Cancel();
                        _inProgress = false;
                        item.Delete(item);
                        App.Toast("Удалено");
                    };
                ContextActions.Add(_callAction);
                ContextActions.Add(_deleteAction);
                ContextActions.Add(_completeAction);
                StackLayout viewLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(25, 10, 25, 15),
                    Children = { viewLayoutItem, _button, T, ID }
                };
                View = viewLayout;
            }
            catch (Exception ex)
            {
                App.Database.SaveError(new Mob.Dto.Error
                {
                    Date = DateTime.Now,
                    Invoker = this.GetType().Name,
                    Message = ex.Message
                });
            }
        }
    }
}