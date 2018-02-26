using Newtonsoft.Json;
using ProjectAutolom.Model;
using ProjectAutolom.Model.Out;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAutolom.Data
{
    class Core
    {
      
        
        private static Core instance;

        public static ObservableCollection<PathList> pathList = new ObservableCollection<PathList>();
        public static List<int> FavoriteListID = new List<int>();
        public static bool isLotAdded = false;

        private Core() { }

        public static Core GetInstance()
        {
            if (instance == null)
                instance = new Core();
            return instance;
        }

        #region Регистрация Async

        public static async Task<RegistrationOut> RegistrationAsync(string number)
        {
            var content = new RegistrationOut();
            string buffer = String.Empty;
            string json = JsonConvert.SerializeObject(new { phone = number });

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var result = await client.PostAsync(DOMAIN + "user", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                var user = new { phone = "", userkey = 0 };
                var userRoot = new { user };
                content.Code = JsonConvert.DeserializeAnonymousType(buffer, userRoot).user.userkey;
            }
            else
            {
                var error = new { type = "", message = "", code = "" };
                var errorRoot = new { error };
                content.Error = JsonConvert.DeserializeAnonymousType(buffer, errorRoot).error.message;
            }

            //var content = new RegistrationOut();
            //string buffer = String.Empty;
            //string json = JsonConvert.SerializeObject(new { phone = number });
            //byte[] body = Encoding.UTF8.GetBytes(json);

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://178.172.161.36/user");

            //request.ContentType = "application/json";
            //request.Method = "POST";

            //using (Stream stream = await request.GetRequestStreamAsync())
            //    stream.Write(body, 0, body.Length);

            //using (var response = await request.GetResponseAsync().ConfigureAwait(false))
            //{
            //    using (var streamReader = new StreamReader(response.GetResponseStream()))
            //    {
            //        buffer = await streamReader.ReadToEndAsync();
            //    }
            //}

            //if (Validation(buffer))
            //{
            //    var user = new { phone = "", userkey = 0 };
            //    var userRoot = new { user };
            //    content.Code = JsonConvert.DeserializeAnonymousType(buffer, userRoot).user.userkey;
            //}
            //else
            //{
            //    var error = new { type = "", message = "", code = "" };
            //    var errorRoot = new { error };
            //    content.Error = JsonConvert.DeserializeAnonymousType(buffer, errorRoot).error.message;
            //}

            return await Task.FromResult(content);
        }

        #endregion

        #region Авторизация Async

        public static async Task<AuthorizationOut> AuthorizationAsync(string number)
        {
            var content = new AuthorizationOut();
            string buffer = String.Empty;
            string json = JsonConvert.SerializeObject(new { phone = number });

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var result = await client.PostAsync(DOMAIN + "user/login", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                var user = new { phone = "", userkey = 0 };
                var userRoot = new { user };
                content.Code = JsonConvert.DeserializeAnonymousType(buffer, userRoot).user.userkey;
            }
            else
            {
                var error = new { type = "", message = "", code = "" };
                var errorRoot = new { error };
                content.Error = JsonConvert.DeserializeAnonymousType(buffer, errorRoot).error.message;
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Подтверждение пользователя Async

        public static async Task<ApproveOut> ApproveAsync(string number, int approveCode)
        {
            var content = new ApproveOut();
            string buffer = String.Empty;
            string json = JsonConvert.SerializeObject(new { phone = number, userkey = approveCode });

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var result = await client.PostAsync(DOMAIN + "user/approve", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                content = JsonConvert.DeserializeObject<ApproveOut>(buffer);
            }
            else
            {
                var error = new { type = "", message = "", code = "" };
                var errorRoot = new { error };
                content.Error = JsonConvert.DeserializeAnonymousType(buffer, errorRoot).error.message;
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Получение пользователя по токену Async

        public static async Task<User> GetUserByTokenAsync(string userToken)
        {
            var content = new User();
            string buffer = String.Empty;

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(DOMAIN + "user");

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                var user = new User();
                var contentRoot = new { user };
                content = JsonConvert.DeserializeAnonymousType(buffer, contentRoot).user;
            }
            else
            {
                var error = new { type = "", message = "", code = "" };
                var errorRoot = new { error };
                content.Error = JsonConvert.DeserializeAnonymousType(buffer, errorRoot).error.message;
            }

            return content;
        }

        #endregion

        #region Добавление лота Async

        public static async Task<MessageOut> AddLotAsync(string userToken, LotModelUpload model)
        {
            var content = new MessageOut();
            string buffer = String.Empty;
            string json = JsonConvert.SerializeObject(model);

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.PostAsync(DOMAIN + "lot", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                content.Message = "Успешно добавлено!";
            }

            return await Task.FromResult(content);
        }
        #endregion

        #region Получение активных лотов Async

        public static async Task<ObservableCollection<LotModelDownload>> 
            GetLotsAsync(string userToken, int start, int offset)
        {
            var content = new ObservableCollection<LotModelDownload>();
            string buffer = String.Empty;

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(string.Format(DOMAIN + "lot/{0}/{1}/active", start, offset));

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                List<Root> get = JsonConvert.DeserializeObject<List<Root>>(buffer);

                foreach (var item in get)
                {
                    //Кастомизация полей
                    item.LotModeOut.YearoFissue += " г.в.";
                    item.LotModeOut.StartPrice = item.LotModeOut.StartPrice + " " + item.LotModeOut.Currency; //SetCurrencyByCountry(item.LotModeOut.RegistratiOnCountry);
                    item.LotModeOut.RegistratiOnCountry = RenameCountry(item.LotModeOut.RegistratiOnCountry);

                    item.LotModeOut.Coordinates = item.LotModeOut.Coordinates.Replace('.', ',');

                    if (item.LotModeOut.CountRates != null)
                        item.LotModeOut.CountRates += SetEndOfWord(int.Parse(item.LotModeOut.CountRates));

                    content.Add(item.LotModeOut);
                }
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Получение неактивных лотов

        public static async Task<ObservableCollection<LotModelDownload>>
            GetInactiveLotsAsync(string userToken, int start, int offset)
        {
            var content = new ObservableCollection<LotModelDownload>();
            string buffer = String.Empty;

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(string.Format(DOMAIN + "lot/{0}/{1}/inactive", start, offset));

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                List<Root> get = JsonConvert.DeserializeObject<List<Root>>(buffer);

                foreach (var item in get)
                {
                    //Кастомизация полей
                    item.LotModeOut.YearoFissue += " г.в.";
                    item.LotModeOut.StartPrice = item.LotModeOut.StartPrice + " " + item.LotModeOut.Currency;
                    item.LotModeOut.RegistratiOnCountry = RenameCountry(item.LotModeOut.RegistratiOnCountry);

                    item.LotModeOut.Coordinates = item.LotModeOut.Coordinates.Replace('.', ',');

                    if (item.LotModeOut.CountRates != null)
                        item.LotModeOut.CountRates += SetEndOfWord(int.Parse(item.LotModeOut.CountRates));

                    content.Add(item.LotModeOut);
                }
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Получение выигрышных лотов

        public static async Task<ObservableCollection<LotModelDownload>>
            GetWonLotsAsync(string userToken)
        {
            var content = new ObservableCollection<LotModelDownload>();
            string buffer = String.Empty;

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(DOMAIN + "user/win");

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                List<Root> get = JsonConvert.DeserializeObject<List<Root>>(buffer);

                foreach (var item in get)
                {
                    //Кастомизация полей
                    item.LotModeOut.YearoFissue += " г.в.";
                    item.LotModeOut.StartPrice = item.LotModeOut.StartPrice + " " + item.LotModeOut.Currency;
                    item.LotModeOut.RegistratiOnCountry = RenameCountry(item.LotModeOut.RegistratiOnCountry);

                    item.LotModeOut.Coordinates = item.LotModeOut.Coordinates.Replace('.', ',');

                    if (item.LotModeOut.CountRates != null)
                        item.LotModeOut.CountRates += SetEndOfWord(int.Parse(item.LotModeOut.CountRates));

                    content.Add(item.LotModeOut);
                }
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Получение избранных лотов Async 

        public static async Task<ObservableCollection<LotModelDownload>> GetFavouriteLotsAsync(string userToken)
        {
            var content = new ObservableCollection<LotModelDownload>();
            string buffer = String.Empty;
            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(DOMAIN + "user/favourite/active");

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                List<Root> get = JsonConvert.DeserializeObject<List<Root>>(buffer);

                FavoriteListID.Clear();

                foreach (var item in get)
                {
                    //Кастомизация полей
                    item.LotModeOut.YearoFissue += " г.в.";
                    item.LotModeOut.StartPrice = item.LotModeOut.StartPrice + " " + item.LotModeOut.Currency;
                    item.LotModeOut.RegistratiOnCountry = RenameCountry(item.LotModeOut.RegistratiOnCountry);

                    item.LotModeOut.Coordinates = item.LotModeOut.Coordinates.Replace('.', ',');

                    if (item.LotModeOut.CountRates != null)
                        item.LotModeOut.CountRates += SetEndOfWord(int.Parse(item.LotModeOut.CountRates));

                    content.Add(item.LotModeOut);
                    FavoriteListID.Add(item.LotModeOut.Id);
                }
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Получение лота по ID юзера Async

        public static async Task<ObservableCollection<LotModelDownload>> GetLotByUserIDAsync(string userToken, int userID)
        {
            var content = new ObservableCollection<LotModelDownload>();

            string buffer = String.Empty;
            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(string.Format(DOMAIN + "user/{0}/lot", userID));

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                List<Root> get = JsonConvert.DeserializeObject<List<Root>>(buffer);

                foreach (var item in get)
                {
                    //Кастомизация полей
                    item.LotModeOut.YearoFissue += " г.в.";
                    item.LotModeOut.StartPrice = item.LotModeOut.StartPrice + " " + item.LotModeOut.Currency;
                    item.LotModeOut.RegistratiOnCountry = RenameCountry(item.LotModeOut.RegistratiOnCountry);

                    item.LotModeOut.Coordinates = item.LotModeOut.Coordinates.Replace('.', ',');

                    if (item.LotModeOut.CountRates != null)
                        item.LotModeOut.CountRates += SetEndOfWord(int.Parse(item.LotModeOut.CountRates));

                    content.Add(item.LotModeOut);
                }

            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Добавление ставки Async

        public static async Task<MessageOut> AddRate(int value, int idLot, string userToken, string anonym)
        {
            var content = new MessageOut();

            string buffer = String.Empty;
            string json = JsonConvert.SerializeObject(new { value = value, idlot = idLot, anonim = anonym });

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.PostAsync(DOMAIN + "rate", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                content.Message = "Успешно!";
            }
            else
            {
                var error = new { type = "", message = "", code = "" };
                var errorRoot = new { error };
                content.Error = JsonConvert.DeserializeAnonymousType(buffer, errorRoot).error.message;
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Получение ставок по ID лота Async

        public static async Task<ObservableCollection<RateDownload>> GetAllRateByLotIDAsync(string userToken, int idLot)
        {
            var content = new ObservableCollection<RateDownload>();

            string buffer = String.Empty;
            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(string.Format(DOMAIN + "lot/{0}/rate", idLot));

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                var doc = JsonConvert.DeserializeObject<List<RootRate>>(buffer);

                foreach (var item in doc)
                    content.Add(item.RateDownload);
            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Получение ставок по ID юзера

        public static async Task<List<RateDownload>> GetLotRateByUserIDAsync(string userToken, int idUser)
        {
            var content = new List<RateDownload>();
            string buffer = String.Empty;
            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(DOMAIN + "user/rate");

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                var doc = JsonConvert.DeserializeObject<List<RootRate>>(buffer);

                foreach (var item in doc)
                    content.Add(item.RateDownload);

            }

            return await Task.FromResult(content);
        }

        #endregion

        #region Добавление в избранное Async

        public static async Task<MessageOut> AddFavourite(int idLot, string userToken)
        {
            var content = new MessageOut();
            string buffer = String.Empty;
            string json = JsonConvert.SerializeObject(new { idlot = idLot });

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.PostAsync(DOMAIN + "lot/favourite", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                content.Message = "Успешно!";
            }
            else
            {
                var error = new { type = "", message = "", code = "" };
                var errorRoot = new { error };
                content.Error = JsonConvert.DeserializeAnonymousType(buffer, errorRoot).error.message;
            }

            return await Task.FromResult(content);
        }


        #endregion

        #region Удаление избранного лота по ID лота

        public static async Task DeleteFromFavouriteLotByIDAsync(string userToken, int lotID)
        {
            var content = new List<LotModelDownload>();
            string buffer = String.Empty;
            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.GetAsync(string.Format(DOMAIN + "lot/favourite/{0}", lotID));

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }
        }

        #endregion

        #region Фильтрация лота

        public static async Task<ObservableCollection<LotModelDownload>> GetFilteredLots(string userToken, FilterModel model)
        {
            var content = new ObservableCollection<LotModelDownload>();
            var send = new { autotype = model.AutoType, model = model.Model, guarantee = model.Guarantee,
                yearofissue = model.YearoFissue, generalstate = model.GeneralState};
            
            string buffer = String.Empty;
            string json = JsonConvert.SerializeObject(send);

            Uri baseAddress = new Uri(DOMAIN);
            CookieContainer cookieContainer = new CookieContainer();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie(NAME, userToken));

                    var result = await client.PostAsync(DOMAIN + "lot/filter", httpContent);

                    result.EnsureSuccessStatusCode();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent con = result.Content;
                        buffer = await con.ReadAsStringAsync();
                    }
                }
            }

            if (Validation(buffer))
            {
                List<Root> get = JsonConvert.DeserializeObject<List<Root>>(buffer);

                foreach (var item in get)
                {
                    //Кастомизация полей
                    item.LotModeOut.YearoFissue += " г.в.";
                    item.LotModeOut.StartPrice = item.LotModeOut.StartPrice + " " + item.LotModeOut.Currency;
                    item.LotModeOut.RegistratiOnCountry = RenameCountry(item.LotModeOut.RegistratiOnCountry);

                    item.LotModeOut.Coordinates = item.LotModeOut.Coordinates.Replace('.', ',');

                    if (item.LotModeOut.CountRates != null)
                        item.LotModeOut.CountRates += SetEndOfWord(int.Parse(item.LotModeOut.CountRates));

                    content.Add(item.LotModeOut);
                }                  
            }
            return await Task.FromResult(content);
        }

        #endregion

        #region Обработка ошибок

        public static bool Validation(string item)
        {
            try
            {
                var error = new { type = "", message = "", code = "" };
                var errorRoot = new { error };
                string temp = JsonConvert.DeserializeAnonymousType(item, errorRoot).error.message;

                if (temp == "")
                    return true;
            }
            catch (Exception)
            {
                return true;
            }

            return false;
        }

        #endregion

        private static string RenameCountry(string countryName)
        {
            string buffer = "";

            switch (countryName)
            {
                case "Беларусь":

                    buffer = "РБ";

                    break;

                case "Россия":

                    buffer = "РФ";

                    break;

                case "Украина":

                    buffer = "УК";

                    break;

                case "Литва":

                    buffer = "ЛТ";

                    break;

                case "Латвия":

                    buffer = "ЛВ";

                    break;

                case "Польша":

                    buffer = "ПЛ";

                    break;
                default:
                    buffer = countryName;
                    break;
            }

            return buffer;
        }

        public static string SetCurrencyByCountry(string countryName)
        {
            string buffer = "";

            switch (countryName)
            {
                case "РБ":

                    buffer = " Бел. р.";

                    break;

                case "РФ":

                    buffer = " ₽";

                    break;

                case "УА":

                    buffer = " Гривн.";

                    break;

                case "ЛВ":

                    buffer = " €";

                    break;

                case "ЛТ":

                    buffer = " €";

                    break;

                case "ПЛ":

                    buffer = " П. Злоты";

                    break;
                default:
                    buffer = " $";
                    break;
            }

            return buffer;
        }

        private static string SetEndOfWord(int value)
        {
            value = Math.Abs(value) % 100;

            if (value > 10 && value < 15)
                return " ставок";

            value %= 10;
            if (value == 1)
                return " ставка";

            if (value > 1 && value < 5)
                return " ставки";

            return " ставок";            
        }

    }
}
