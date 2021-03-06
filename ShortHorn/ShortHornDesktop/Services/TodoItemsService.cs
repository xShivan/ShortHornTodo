﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShortHorn.DataTransferObjects;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShortHorn.Desktop.Services
{
    public class TodoItemsService : BaseService
    {
        /// <summary>
        /// The API token used for user authentication.
        /// </summary>
        private string apiUserToken;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="apiBaseUrl">The root URL of API.</param>
        /// <param name="apiUserToken">The API token for authorization.</param>
        public TodoItemsService(string apiBaseUrl, string apiUserToken)
            : base(apiBaseUrl)
        {
            this.apiUserToken = apiUserToken;
        }

        /// <summary>
        /// Fetches all items belonging to a list from API.
        /// </summary>
        /// <param name="listId">The ID of the list to fetch items from.</param>
        /// <returns></returns>
        public async Task<List<TodoItemDTO>> GetAllItems(int listId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.apiBaseUrl);
                this.resetClient(client);

                HttpResponseMessage response = await client.PostAsJsonAsync("api/items/getbylist", new BaseDTO()
                {
                    Id = listId,
                    Token = this.apiUserToken
                });

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsAsync<List<TodoItemDTO>>();
                else
                    return null;
            }
        }

        /// <summary>
        /// Creates a new todo item and saves it at remote server.
        /// </summary>
        /// <param name="parentListId">ID of parent list.</param>
        /// <param name="title"></param>
        /// <param name="details"></param>
        /// <param name="isFinished"></param>
        /// <param name="isFavourite"></param>
        /// <param name="dateFinish"></param>
        /// <returns>True if operation's successful, false otherwise.</returns>
        public async Task<bool> CreateItem(int parentListId, string title, string details = null, bool isFinished = false, bool isFavourite = false, DateTime? dateFinish = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.apiBaseUrl);
                this.resetClient(client);

                HttpResponseMessage response = await client.PostAsJsonAsync("api/items", new TodoItemDTO()
                {
                    Name = title,
                    ParentListId = parentListId,
                    Token = this.apiUserToken,
                    IsFinished = isFinished,
                    IsFavourite = isFavourite,
                    Details = details,
                    DateFinish = dateFinish
                });

                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Updates todo item details and saves changes at remote server.
        /// </summary>
        /// <param name="item">TodoItem model.</param>
        /// <returns>True if operation's successful, false otherwise.</returns>
        public async Task<bool> UpdateItem(TodoItemDTO item)
        {
            item.Token = AppState.ApiLoginToken;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.apiBaseUrl);
                this.resetClient(client);

                HttpResponseMessage response = await client.PutAsJsonAsync("api/items", item);

                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }

        public async Task<string> FetchWeather(DateTime date, string city, string country)
        {
            string uri = "http://api.openweathermap.org/data/2.5/forecast/daily?q=" + city + "," + country + "&cnt=14&mode=json";
            var httpClient = new HttpClient();
            var weatherJSON = await httpClient.GetStringAsync(new Uri(uri));
            JObject d = JObject.Parse(weatherJSON);
            var list = d["list"];

            var rawDates = from item in list
                        select (int)item["dt"];

            int properRawDate = 0;
            foreach (int rawDate in rawDates)
            {
                DateTime checkDate = new DateTime(1970, 1, 1).AddSeconds(rawDate);
                if (checkDate.Day == date.Day)
                {
                    properRawDate = rawDate;
                    break;
                }
            }

            JToken weatherInformation = (from item in list
                                            where (int)item["dt"] == properRawDate
                                            select item).First();
            return weatherInformation["weather"].First["main"].ToString();

        }
    }
}
