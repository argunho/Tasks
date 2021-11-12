using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tasks.Models;
using Tasks.Repositories;
using TasksUI.WebApiConnect;

namespace TasksUI.Controllers
{
    [Authorize]
    public class ActionsController : Controller
    {
        private const string _apiT = "api/HandleTasks";
        private const string _apiU = "api/HandleUsers";

        #region GET
        // Get all tasks for user where ia author or member
        [Route("Tasks")]
        [Route("Tasks/member")]
        public async Task<IActionResult> Tasks()
        {
            var url = (Request.Path.Value.ToString().ToLower().IndexOf("member") != -1) ? "/Member" : "";
            List<TasksList> tasks = new List<TasksList>();
            HttpResponseMessage res = await Api().GetAsync(_apiT + url);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                tasks = JsonConvert.DeserializeObject<List<TasksList>>(result);
            }

            return View(tasks);
        }

        // Get one task by id
        [Route("ViewTask/{id}")]
        public async Task<IActionResult> ViewTask(int id)
        {
            var tasks = new TasksList();
            HttpResponseMessage res = await Api().GetAsync(_apiT + "/GetTask/" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                tasks = JsonConvert.DeserializeObject<TasksList>(result);
            }

            return View(tasks);
        }

        // Find tasks or tags by search key word
        [Route("SearchResult")]
        public async Task<IActionResult> Tasks(string key)
        {
            List<TasksList> tasks = new List<TasksList>();
            HttpResponseMessage res = await Api().GetAsync(_apiT + "/SearchTasks/" + key);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                tasks = JsonConvert.DeserializeObject<List<TasksList>>(result);
            }
            return View(tasks);
        }
        #endregion

        #region POST
        // Page for new task
        [Route("NewTask")]
        public ActionResult NewTask()
        { 
            return View();      
        }        
        
        // Send new task data
        [HttpPost]
        public async Task<IActionResult> NewTask(TasksList model) {

            HttpResponseMessage res = await Api().PostAsync(_apiT + "/NewTask", Content(model));
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }

        // Page for new tasks tag
        [Route("NewTaskTag/{taskId}")]
        public ActionResult NewTaskTag()
        {
            return View();
        }

        // Send new tasks tag data
        [HttpPost]
        public async Task<IActionResult> NewTaskTag(Tags model)
        {
            HttpResponseMessage res = await Api().PostAsync(_apiT + "/NewTag", Content(model));
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }
        #endregion

        #region PUT
        // Page for edit a task
        [Route("EditTask/{id}")]
        public async Task<ActionResult> EditTask(int id)
        {
            var tasks = new TasksList();
            HttpResponseMessage res = await Api().GetAsync(_apiT + "/GetTask/" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                tasks = JsonConvert.DeserializeObject<TasksList>(result);
            }

            return View(tasks);
        }

        // Change task data 
        [HttpPut]
        public async Task<IActionResult> EditTask(TasksList model)
        {
            HttpResponseMessage res = await Api().PutAsync(_apiT + "/UpdateTask/" + model.Id, Content(model));
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }

        // Make task done
        [HttpPut]
        public async Task<IActionResult> DoneTask(int id)
        {
            HttpResponseMessage res = await Api().PutAsync(_apiT + "/TaskDone/" + id, null);
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }

        // Page with users list for invite a user to task
        [Route("InviteUsersToTask/{taskId}")]
        public async Task<IActionResult> Users(int taskId)
        {
            List<Users> users = new List<Users>();
            TasksList task = new TasksList();
            HttpResponseMessage res = await Api().GetAsync(_apiU + "");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<Users>>(result);

                HttpResponseMessage taskResult = await Api().GetAsync(_apiT + "/GetTask/" + taskId);
                if (taskResult.IsSuccessStatusCode)
                {
                    var resultTask = taskResult.Content.ReadAsStringAsync().Result;
                    task = JsonConvert.DeserializeObject<TasksList>(resultTask);
                }
            }

            var list = Tuple.Create(users, task);
            return View(list);
        }

        // Invite a user
        [HttpPut]
        public async Task<IActionResult> InviteUserToTask(int id, string userId)
        {
            HttpResponseMessage res = await Api().PutAsync(_apiT + "/Task/" + id + "/Member/" + userId, null);
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }

        // Make task's tag done
        [HttpPut]
        public async Task<IActionResult> DoneTaskTag(int id)
        {
            HttpResponseMessage res = await Api().PutAsync(_apiT + "/TagDone/" + id, null);
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }
        #endregion

        #region DELETE
        // Delete task
        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            HttpResponseMessage res = await Api().DeleteAsync(_apiT + "/DeleteTask/" + id);
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }        
        
        // Delete invited user from task
        [HttpDelete]
        public async Task<IActionResult> DeleteUserFromTask(int id, string userId)
        {
            HttpResponseMessage res = await Api().DeleteAsync(_apiU + "/DeleteMemberFromTask/" + id + "/Member/" + userId);
            if (res.IsSuccessStatusCode)
               return Ok(true);

            return Ok(false);
        }
     
        // Remove task's tag
        [HttpDelete]
        public async Task<IActionResult> DeleteTaskTag(int id)
        {
            HttpResponseMessage res = await Api().DeleteAsync(_apiT + "/DeleteTag/" + id);
            if (res.IsSuccessStatusCode)
                return Ok(true);

            return Ok(false);
        }

        // Delete inloggad users account
        [HttpDelete]
        public async Task<IActionResult> RemoveAccount() {
            HttpResponseMessage res = await Api().DeleteAsync(_apiU + "/DeleteAccount");
            if (res.IsSuccessStatusCode)
            {
                HelpRepository.loc.Clear();
                return RedirectToAction("Index", "Home");
            }

            return Ok(false);
        }        
        #endregion

        #region Helpers
        public dynamic Api() {
            ApiConnect _api = new ApiConnect();
            HttpClient http = _api.Initial();
            return http;
        }

        public dynamic Content(object model) {
            var list = JsonConvert.SerializeObject(model);
            var buffer = System.Text.Encoding.UTF8.GetBytes(list);
            var content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }
        #endregion
    }
}