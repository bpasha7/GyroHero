using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Mob.Requests
{
    public static class GyroServer
    {
        private static readonly string _server = "http://gyro.snouwer.ru/";

        private static readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Регистрация проката на сервере
        /// </summary>
        /// <param name="rent"></param>
        async public static void SendOrder(RentModel rent)
        {
            var values = new Dictionary<string, string>
                {
                   { "worker_id", App.Database.GetSettingsByName("Id").Vlaue},
                   { "client_name", rent.Client.Name },
                   { "total", rent.RentPrice.Price.ToString() }
                };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://gyro.snouwer.ru/orders", content);

            var responseString = await response.Content.ReadAsStringAsync();
        }

        async public static void SendUserData()
        {
            var values = new Dictionary<string, string>
                {
                   { "name", App.Database.GetSettingsByName("UserName").Vlaue},
                   { "location", App.Database.GetSettingsByName("Location").Vlaue}
                };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://gyro.snouwer.ru/workers", content);

            var responseString = await response.Content.ReadAsStringAsync();

            var mdUser = new UserSettings { Name = "UserMD", Vlaue = "" };

            App.Database.SaveUserSettings(mdUser);
        }
    }
}
