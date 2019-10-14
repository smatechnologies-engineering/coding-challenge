using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SelfDrivingCar.Entities;

namespace SelfDrivingCar.Client
{
    public class SelfDrivingCarRestClient : ISelfDrivingCarService
    {
        private const string ApiUrl = "https://smachallenge.azurewebsites.net";
        private readonly HttpClient _client;

        public Token Token { get; set; }

        public SelfDrivingCarRestClient(int timeoutSeconds = 20)
        {
            _client = new HttpClient { Timeout = new TimeSpan(0, 0, timeoutSeconds) };
        }

        public SelfDrivingCarRestClient(HttpClient client)
        {
            _client = client;
        }

        public CarAction DoAction(CarAction action)
        {
            return Post<CarAction, CarAction>("api/caractions", action);
        }

        public Car GetCar()
        {
            return Get<Car>("api/car");
        }

        public Road GetRoad()
        {
            return Get<Road>("api/road");
        }

        public Token Register(TokenRequest request)
        {
            Token = Post<Token, TokenRequest>("api/tokens", request);
            return Token;
        }

        private T Get<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Get, uri);
            var response = SendRequest<T>(request);
            return response;
        }

        private T Post<T, TK>(string uri, TK content)
        {
            var request = CreateRequest(HttpMethod.Post, uri, content);
            var response = SendRequest<T>(request);
            return response;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null)
        {
            var url = $"{ApiUrl.TrimEnd('/')}/{uri}";
            var request = new HttpRequestMessage(method, url);
            if (Token != null)
                request.Headers.Add("Authorization", new List<string> { $"Basic {Token.Id}" });
            if (content == null) return request;
            var body = JsonConvert.SerializeObject(content,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return request;
        }

        private T SendRequest<T>(HttpRequestMessage request)
        {
            var response = _client.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var serializer = new JsonSerializer();
            var reader = new JsonTextReader(new StringReader(responseContent));
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage error;
                try
                {
                    error = serializer.Deserialize<ErrorMessage>(reader);
                }
                catch
                {
                    throw new Exception("An unhandled exception has occurred.");
                }
                throw new Exception(error.Error);
            }
            var deserializedObject = serializer.Deserialize<T>(reader);
            return deserializedObject;
        }
    }
}
