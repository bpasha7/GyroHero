using Mob.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Mob
{
    public class EmailReport
    {
        private List<Rent> _rentList;
        private DateTime _date;
        private decimal _sum;

        public EmailReport(DateTime Date)
        {
            _date = Date.Date;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Errors">Отправить ошибки</param>
        public void SendAsync(bool Errors)
        {
            try
            {
                if (!Errors)
                {
                    _rentList = App.Database.GetRents(_date);
                    if (_rentList == null || _rentList.Count == 0)
                    {
                        App.Toast("Данные отсутствуют!");
                        return;
                    }
                }
                SmtpClient Smtp = new SmtpClient("smtp.bk.ru", 2525)
                {
                    Credentials = new NetworkCredential("s.ql@bk.ru", "q1852sl")
                };
                MailMessage Message = new MailMessage();
                //Выключаем или включаем SSL 
                Smtp.EnableSsl = true;
                var from = App.Database.GetUserInfo();
                var to = App.Database.GetSettingsByName("ReportEmail")?.Vlaue;
                if( from == null || to == null)
                {
                    App.Toast("Заполните профиль!");
                    return;
                }
                Message.From = new MailAddress("s.ql@bk.ru", from);// от кого
                Message.To.Add(new MailAddress(to));// кому
                Message.IsBodyHtml = true;
                Message.BodyEncoding = System.Text.Encoding.UTF8;
                Message.SubjectEncoding = System.Text.Encoding.UTF8;
                if (Errors)
                {
                    Message.Body = ErrorsReport();
                    App.Database.DeleteAllErrors();
                    Message.Subject = $"Ошибки за {_date.ToString("D")}";
                }
                else
                {
                    Message.Body = ReportHTML();
                    Message.Subject = $"Отчет за {_date.ToString("D")}";
                }
                App.Toast("Отправляется");
                Smtp.SendCompleted += Smtp_SendCompleted;
#if DEBUG
                throw new Exception("Email reports in debug are not available");
#endif
#if !DEBUG
                Smtp.SendAsync(Message, "");               
#endif

                Message.Dispose();
            }
            catch (Exception ex)
            {
                App.Database.SaveError(new Error { Date = DateTime.Now, Invoker = this.GetType().Name, Message = ex.Message });
            }
        }

        private string ErrorsReport()
        {
            var txt = new StringBuilder("");
            foreach (var item in App.Database.GetErrors())
            {
                txt.Append(item.ToString());
            }
            return txt.ToString();
        }

        private string ReportHTML()
        {
            _sum = 0;
            var htmlForm = new StringBuilder("");
            htmlForm.Append("<!DOCTYPE html>");
            htmlForm.Append("<html>");
            htmlForm.Append("<head>");
            htmlForm.Append("<style>");
            htmlForm.Append("table {border-collapse: collapse; width: 100 %;}");
            htmlForm.Append("th, td {text-align: left; padding: 8px;}");
            htmlForm.Append("tr:nth-child(even){background-color: #f2f2f2}");
            htmlForm.Append("th {background-color: #4CAF50;color: white;}");
            htmlForm.Append("</style>");
            htmlForm.Append("</head>");
            htmlForm.Append("<body>");
            htmlForm.Append("<h2>Отчет</h2>");
            htmlForm.Append(TypeReport("G"));
            htmlForm.Append(TypeReport("C"));
            htmlForm.Append($"<p>Итого: {_sum:c}</p>");
            htmlForm.Append("</body>");
            htmlForm.Append("</html>");

            return htmlForm.ToString();
        }

        private string TypeReport(string type)
        {
            var htmlForm = new StringBuilder("");
            string name = "";
            switch (type)
            {
                case "G": name = "гироскутеры"; break;
                case "C": name = "велосипеды"; break;
                default:
                    return "";
            }
            htmlForm.Append($"<h2>Отчет {name}</h2>");
            htmlForm.Append("<table>");
            htmlForm.Append("<tr>");
            htmlForm.Append("<th>Время</th>");
            htmlForm.Append("<th>Откатано</th>");
            htmlForm.Append("<th>Поулчено</th>");
            htmlForm.Append("</tr>");

            decimal sum = 0;

            foreach (var item in _rentList.Where(i => i.Type == type))
            {
                htmlForm.Append($"<tr><td>{item.Time.ToString(@"hh\:mm")}</td>");
                htmlForm.Append($"<td>{item.RentTime}</td>");
                htmlForm.Append($"<td>{item.Payment}</td></tr>");
                sum += item.Payment;
            }

            htmlForm.Append("</table>");
            htmlForm.Append($"<p>Всего: {sum:c}</p>");
            _sum += sum;
            return htmlForm.ToString();
        }

        private void Smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //String token = (string)e.UserState;

            //if (e.Cancelled)
            //{
            //    Console.WriteLine("[{0}] Send canceled.", token);
            //}
            //if (e.Error != null)
            //{
            //    App.Toast(e.Error.ToString());
            //    //Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            //}
            //else
            //{
            //    Console.WriteLine("Message sent.");
            //}
            //mailSent = true;

            App.DoNotify = $"Отчет за {_date.ToString("D")} отправлен!";
            App.Toast("Отчет отправлен!");
        }
    }
}
