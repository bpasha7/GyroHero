
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Mob
{
    public class Person : ContentPage
    {
        public delegate void RentManipulate(/*RentItem*/RentModel rentItem);
        public RentManipulate AddNewRentInfo { get; set; }
        #region Client controls
        private Entry _clientName;
        #endregion
        #region Time controls
        private Label _timeLbl;
        private Xamarin.Forms.TimePicker _time;
        private Picker _times;
        private Image _customImg;
        private Switch _customProperty;
        #endregion
        #region Phone controls
        private Switch _phoneExist;
        private Entry _phoneNumber;
        private Image _phoneImg;
        #endregion
        #region Price controls
        private Label _priceLbl;
        private Entry _price;
        #endregion
        #region Documents controls
        private Button _isPassport;
        private Button _isStud;
        private Button _isDriverLicense;
        private Dictionary<string, Button> DocButtons;
        private Entry _documentSerial;
        private Entry _documentNumber;
        private string _selectedDocumentType = "РФ";
        #endregion
        #region Layouts
        private StackLayout PriceInfoLayer;
        private StackLayout DateInfoLayer;
        #endregion
        private List<PriceInfo> _prices;

        private PriceInfo _selectedPrice;

        private Image _submitBtn;
        private bool isValidInfo()
        {
            if (_clientName.Text.TrimEnd() == "")
                return false;
            else if (_time.Time == new TimeSpan(0, 0, 0) && _selectedPrice == null)
                return false;
            else if (_price.Text == "")
                return false;
            else if (_phoneNumber.Text.Length != 12 && _phoneNumber.Text.Length != 7 && _phoneExist.IsToggled)
                return false;
            else
                return true;

        }

        public Person(List<PriceInfo> prices)
        {
            try
            {
                _prices = prices;
                this.Title = "Новый клиент";
                _submitBtn = new Image { Source = "checked.png", BackgroundColor = Color.Transparent };
                _submitBtn.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = async (v, o) =>
                    {
                        if (!isValidInfo())
                        {
                            await DisplayAlert("Недостаточно информации", "Проверьте, все ли данные заполнены.", "OK");
                            return;
                        }


                        var currentPrice = _selectedPrice == null ? new PriceInfo { Name = "Без прайса", Id = -1, Time = _time.Time, Price = Convert.ToDecimal(_price.Text), Vehicle = _prices[0].Vehicle } : _selectedPrice;
                        var clientInfo = new ClientInfo { Name = _clientName.Text.TrimEnd(), Phone = _phoneNumber.Text, DocumentType = _selectedDocumentType, Number = _documentNumber.Text, Serial = _documentSerial.Text };
                        AddNewRentInfo(new RentModel(clientInfo, currentPrice));
                        await Navigation.PopToRootAsync();

                    }
                });
                ///имя клиента
                _clientName = new Entry
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    TextColor = Color.Black, Text = "",
                    Placeholder = "Введите имя клиента",
                    FontSize = 20,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                _clientName.Unfocused += _clientName_Unfocused;
#if DEBUG
                _clientName.Text = "sdfdsfs";
#endif
                ///время
                _timeLbl = new Label { Text = "Время: ", FontSize = 18, VerticalTextAlignment = TextAlignment.Center };
                _time = new Xamarin.Forms.TimePicker { Format = @"HH:mm", Time = new TimeSpan(0, 0, 0), IsVisible = false };
                _times = new Picker { Title = "Выберите время", VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Center };
                foreach (var item in _prices)
                {
                    _times.Items.Add(item.Name);
                }
                _times.SelectedIndexChanged += _times_SelectedIndexChanged;
                _customImg = new Image { Scale = 0.5, Opacity = 20, Source = "edit.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                _customProperty = new Switch { IsToggled = false, HorizontalOptions = LayoutOptions.EndAndExpand };
                _customProperty.PropertyChanged += _customProperty_PropertyChanged;

                ///Телефон клиента
                _phoneNumber = new Entry { WidthRequest = 230, IsEnabled = false, TextColor = Color.Black, Text = "", Placeholder = "Добавить  телефон", FontSize = 18, Keyboard = Keyboard.Telephone, };
                _phoneNumber.TextChanged += _phoneNumber_TextChanged;
                _phoneExist = new Switch { IsToggled = false, HorizontalOptions = LayoutOptions.EndAndExpand };
                _phoneExist.PropertyChanged += _phoneExist_PropertyChanged;
                _phoneImg = new Image { Scale = 0.5, Opacity = 0.2, Source = "smartphone.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                ///Документ
                _isDriverLicense = new Button { Opacity = 0.2, HorizontalOptions = LayoutOptions.CenterAndExpand, HeightRequest = 20, WidthRequest = 60, Text = "ВУ", BackgroundColor = Color.LightPink, TextColor = Color.DarkRed };
                _isDriverLicense.Clicked += _isDriverLicense_Clicked;
                _isPassport = new Button { Opacity = 1, HorizontalOptions = LayoutOptions.CenterAndExpand, WidthRequest = 45, Text = "РФ", BackgroundColor = Color.DarkRed, TextColor = Color.White };
                _isPassport.Clicked += _isPassport_Clicked;
                _isStud = new Button { Opacity = 0.2, HorizontalOptions = LayoutOptions.CenterAndExpand, HeightRequest = 30, WidthRequest = 65, Text = "СБ", BackgroundColor = Color.MediumBlue, TextColor = Color.White };
                _isStud.Clicked += _isStud_Clicked;
                DocButtons = new Dictionary<string, Button>();
                DocButtons.Add("РФ", _isPassport);
                DocButtons.Add("ВУ", _isDriverLicense);
                DocButtons.Add("СБ", _isStud);
                _documentNumber = new Entry { Placeholder = " Номер ", Keyboard = Keyboard.Numeric };
                _documentSerial = new Entry { Placeholder = "Серия", Keyboard = Keyboard.Numeric };
                ///Цена
                _priceLbl = new Label { Text = "Внесено: ", FontSize = 18, VerticalTextAlignment = TextAlignment.Center };
                _price = new Entry { WidthRequest = 90, Placeholder = "Сумма", Keyboard = Keyboard.Numeric };

                var ClientLayer = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(15,15,15,0 ),
                    Children = { _clientName}
                };

                DateInfoLayer = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(10),
                    Children = { _timeLbl, _times, _time, _customProperty, _customImg }
                };
                var PhoneInfoLayer = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(10),
                    Children = { _phoneNumber, _phoneExist, _phoneImg }
                };
                PriceInfoLayer = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(10),
                    Children = { _priceLbl, _price }
                };
                var DocumentLayer = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(10),
                    Children = { _isPassport, _isDriverLicense, _isStud }
                };
                var DocumentNumberLayer = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(5),
                    Children = { _documentSerial, _documentNumber }
                };

                var SmbLayer = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(10),
                    Children = { _submitBtn }
                };
                
                Content = new StackLayout
                {
                    Children = {
                    ClientLayer,
                    DateInfoLayer,
                    PhoneInfoLayer,
                    PriceInfoLayer,
                    DocumentLayer,
                    DocumentNumberLayer,
                    SmbLayer
            }
                };

            }
            catch (Exception ex)
            {
                App.Database.SaveError(new Mob.Dto.Error { Date = DateTime.Now, Invoker = this.GetType().Name, Message = ex.Message });
            }
        }

        private void _clientName_Unfocused(object sender, FocusEventArgs e)
        {
            _clientName.Text = UppercaseWords(_clientName.Text);
        }

        private void _isStud_Clicked(object sender, EventArgs e)
        {
            CheckDocumentType("СБ");
        }

        private void _isPassport_Clicked(object sender, EventArgs e)
        {
            CheckDocumentType("РФ");
        }

        private void CheckDocumentType(string Type)
        {
            _selectedDocumentType = Type;
            DocButtons.Where(w => w.Key == Type).SingleOrDefault().Value.Opacity = 1;
            foreach (var item in DocButtons.Where(w => w.Key != Type))
            {
                item.Value.Opacity = 0.2;
            }
        }

        static string UppercaseWords(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        private void _isDriverLicense_Clicked(object sender, EventArgs e)
        {
            CheckDocumentType("ВУ");
        }

        private void _customProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_customProperty.IsToggled)
            {
                _times.IsVisible = false;
                _times.SelectedItem = null;
                _time.IsVisible = true;
                _customImg.Opacity = 1;
                _price.IsEnabled = true;
                _price.Text = "";
            }
            else
            {
                _times.IsVisible = true;
                _times.SelectedItem = null;
                _time.IsVisible = false;
                _time.Time = new TimeSpan(0, 0, 0, 0);
                _customImg.Opacity = 0.2;
                _price.IsEnabled = false;
                _selectedPrice = null;
            }
        }

        private void _times_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_times.SelectedItem == null)
                return;
            var selectedTime = _times.SelectedItem.ToString();
            _selectedPrice = _prices.Where(p => p.Name == selectedTime).FirstOrDefault();
            if (_selectedPrice != null)
                _price.Text = $"{_selectedPrice.Price:c0}";
        }

        private void _phoneExist_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_phoneExist.IsToggled)
            {
                _phoneNumber.IsEnabled = true;
                _phoneImg.Opacity = 1;
                _phoneNumber.Text = "+7";
#if DEBUG
                _phoneNumber.Text = "+79607606042";
#endif
            }
            else
            {
                _phoneNumber.IsEnabled = false;
                _phoneImg.Opacity = 0.2;
                _phoneNumber.Text = "";
            }
        }

        private void _phoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_phoneNumber.Text.Length != 12 && _phoneNumber.Text.Length != 7)
                _phoneNumber.TextColor = Color.Red;
            else
                _phoneNumber.TextColor = Color.Green;
        }
    }
}