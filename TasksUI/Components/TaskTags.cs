using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tasks.Models;
using TasksUI.WebApiConnect;

namespace TasksUI.Components
{
    public class TaskTags : ViewComponent
    {

        // Find all tags by task id
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            ApiConnect _api = new ApiConnect();
            List<Tags> tags = new List<Tags>();
            HttpClient http = _api.Initial();
            HttpResponseMessage res = await http.GetAsync("api/HandleTasks/Tags/" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                tags = JsonConvert.DeserializeObject<List<Tags>>(result);
            }
            return View(tags);
        }
    }
}
