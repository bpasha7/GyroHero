using Mob.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Mob
{
    public class Menu : MasterDetailPage
    {
        private Image _timersInfoImg;
        private Label _timersLbl;

        private Image _statInfoImg;
        private Label _StatLbl;

        private Image _reportImg;
        private Label _reportLbl;

        private Image _pricesImg;
        private Label _pricesLbl;

        private Image _settingsImg;
        private Label _settingsLbl;

        private Image _bugReportImg;
        private Label _bugReportLbl;

        private NavigationPage _cur;

        private void SetGesture(bool state)
        {
            this.IsGestureEnabled = state;
        }
        protected override void OnAppearing()
        {
            if (_bugReportLbl != null)
            {
                var countError = App.Database.ErrorCount();
                if(countError>0)
                    _bugReportLbl.Text = $"Отправить ошибки ({countError})";
                else
                {
                    _bugReportLbl.Text = $"Отправить ошибки";
                    _bugReportLbl.IsEnabled = false;
                }
            }
            base.OnAppearing();
        }

        public Menu(NavigationPage cur)
        {
            try
            {
                Title = "Меню";
                _cur = cur;
                Detail = _cur;
                _timersInfoImg = new Image { Source = "time.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                _timersLbl = new Label { TextColor = Color.Black, Text = "Таймеры", FontSize = 16, VerticalTextAlignment = TextAlignment.Center };
                _timersLbl.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = (v, o) =>
                    {
                        try
                        {
                            Detail = _cur;
                            this.IsGestureEnabled = false;
                            this.IsGestureEnabled = true;
                        }
                        catch (Exception ex)
                        {
                            App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = $"{this.GetType().Name}->Timers", Message = ex.Message });

                        }
                    }
                });
                ///
                _statInfoImg = new Image { Source = "barchart.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                _StatLbl = new Label { TextColor = Color.Black, Text = "Статистика ", FontSize = 16, VerticalTextAlignment = TextAlignment.Center };
                _StatLbl.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = (v, o) =>
                   {
                       try
                       {
                           Detail = new NavigationPage(new StatsPage() /*{ SetGestureState = SetGesture }*/);
                           this.IsGestureEnabled = false;
                           this.IsGestureEnabled = true;
                       }
                       catch (Exception ex)
                       {
                           App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = $"{this.GetType().Name}->Stats", Message = ex.Message });

                       }
                   }
                });
                /// 
                _reportImg = new Image { Source = "email.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                _reportLbl = new Label { TextColor = Color.Black, Text = "Отчитаться ", FontSize = 16, VerticalTextAlignment = TextAlignment.Center };
                _reportLbl.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = (v, o) =>
                    {
                        try
                        {
                            var report = new EmailReport(DateTime.Now);
                            report.SendAsync(false);
                            this.IsGestureEnabled = false;
                            this.IsGestureEnabled = true;
                        }
                        catch (Exception ex)
                        {
                            App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = $"{this.GetType().Name}->Reports", Message = ex.Message });
                        }
                    }
                });

                ///
                //_pricesImg = new Image { Source = "ruble.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                //_pricesLbl = new Label { TextColor = Color.Black, Text = "Цены ", FontSize = 16, VerticalTextAlignment = TextAlignment.Center };
                ///
                _settingsImg = new Image { Source = "user.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                _settingsLbl = new Label { TextColor = Color.Black, Text = "Профиль ", FontSize = 16, VerticalTextAlignment = TextAlignment.Center };
                _settingsLbl.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = (v, o) =>
                    {
                        try
                        {
                            Detail = new NavigationPage(new UserPage());
                            this.IsGestureEnabled = false;
                            this.IsGestureEnabled = true;
                        }
                        catch (Exception ex)
                        {
                            App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = $"{this.GetType().Name}->Settings", Message = ex.Message });

                        }
                    }
                });

                ///
                _bugReportImg = new Image { Source = "bug.png", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
                _bugReportLbl = new Label { TextColor = Color.Black, /*Text = $"Отправить ошибки программы({App.Database.ErrorCount()}) ",*/ FontSize = 16, VerticalTextAlignment = TextAlignment.Center };
                _bugReportLbl.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    TappedCallback = (v, o) =>
                    {
                        try
                        {
                            var report = new EmailReport(DateTime.Now);
                            App.Toast("Отправляется");
                            report.SendAsync(true);
                            this.IsGestureEnabled = false;
                            this.IsGestureEnabled = true;
                        }
                        catch (Exception ex)
                        {
                            App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = $"{this.GetType().Name}->Reports", Message = ex.Message });
                        }
                    }
                });

                StackLayout TimersLayout = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(30, 80, 20, 0),
                    Children = { _timersInfoImg, _timersLbl }
                };

                StackLayout StatLayout = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(30, 20, 20, 0),
                    Children = { _statInfoImg, _StatLbl }
                };
                StackLayout ReportLayout = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(30, 20, 20, 0),
                    Children = { _reportImg, _reportLbl }
                };

                StackLayout SettingsLayout = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(30, 20, 20, 0),
                    Children = { _settingsImg, _settingsLbl }
                };

                StackLayout BugReportLayout = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(30, 40, 20, 0),
                    Children = { _bugReportImg, _bugReportLbl }
                };
                var content = new StackLayout
                {
                    Children = {
                    TimersLayout,
                    StatLayout,
                    ReportLayout,
                    //PriceLayout,
                    SettingsLayout,
                    BugReportLayout
                }

                };
                this.Master = new ContentPage
                {
                    Title = "Options",
                    Content = content,
                    Icon = "hamburger.png"
                };
            }
            catch (Exception ex)
            {
                App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = this.GetType().Name, Message = ex.Message });
            }
        }
    }
}
