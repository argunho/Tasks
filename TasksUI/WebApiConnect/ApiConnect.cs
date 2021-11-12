   using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Tasks.Repositories;

namespace TasksUI.WebApiConnect
{
    public class ApiConnect
    {
        public HttpClient Initial()
        {
            var token = "";
            if (HelpRepository.loc.Exists("hashToken"))
                token = HelpRepository.loc.Get<string>("hashToken");

            var api = new HttpClient();
            api.BaseAddress = new Uri("https://localhost:44380/");
            api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return api;
        }
    }
}
