using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tasks.Context;
using Tasks.Models;

namespace Tasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HandleTasksController : ControllerBase
    {
        private readonly DbConnect _db;
        public HandleTasksController(DbConnect db)
        {
            _db = db;
        }

        public IEnumerable<TasksList> AllTasks
        {
            get
            {
                return _db.Tasks.Include("Author").Include("Members").ToList();
            }
        }

        public IEnumerable<Tags> AllTags
        {
            get
            {
                return _db.Tags.ToList();
            }
        }

        #region GET
        // Get all my task list
        [HttpGet]
        public IEnumerable<TasksList> Get()
        {
            var userId = User.Claims.ToList()[0].Value;
            return AllTasks.Where(x => x.Author.Id == userId).ToList();
        }

        // Get all task list where are a member
        [HttpGet("Member")]
        public IEnumerable<TasksList> TasksMember()
        {
            var userId = User.Claims.ToList()[0].Value;
            return AllTasks.Where(x => x.Members.Any(m => m.Id == userId)).ToList();
        }

        // Get task by id
        [HttpGet("GetTask/{id}")]
        public ActionResult GetTask(int id)
        {
            var userId = User.Claims.ToList()[0].Value;
            var task = AllTasks.FirstOrDefault(x => x.Id == id && x.Author.Id == userId || x.Members.Any(m => m.Id == userId));
            if (task != null)
                return Ok(task);

            return Ok(null);
        }

        // Get all tags from task by task id
        [HttpGet("Tags/{id}")]
        public IEnumerable<Tags> GetTags(int id)
        {
            var userId = User.Claims.ToList()[0].Value;
            var list = AllTasks.FirstOrDefault(x => x.Id == id);
            if (list != null)
            {
                var tags = AllTags.Where(x => x.TaskId == list.Id).ToList();
                return tags;
            }

            return null;
        }

        // Search by word
        [HttpGet("SearchTasks/{key}")]
        public IEnumerable<TasksList> Search(string key) {

            List<TasksList> result = new List<TasksList>();
            var userId = User.Claims.ToList()[0].Value;
            key = key.ToLower();
            var list = AllTasks.Where(x => x.Author.Id == userId || x.Members.Any(m => m.Id == userId)).ToList();
            if(list.Count > 0)
            {
                List<Tags> tagsList = new List<Tags>();
                if(AllTags.Count() > 0 && list.Count > 0)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var taskId = list[i].Id;
                        var tag = AllTags.FirstOrDefault(x => x.TaskId == taskId);
                        if (tag != null)
                            tagsList.Add(tag);
                    }
                }
                var foundTasks = list.Where(x => x.Name.ToLower().Contains(key) || x.Description.ToLower().Contains(key)).ToList();
                for (var i = 0; i < foundTasks.Count; i++)
                    result.Add(foundTasks[i]);
                if(tagsList.Count() > 0)
                {
                    var foundTags = tagsList.Where(x => x.Text.ToLower().Contains(key)).ToList();
                    for(var i = 0;i < foundTags.Count; i++)
                    {
                        var id = foundTags[i].TaskId;
                        if (result.Find(x => x.Id == id).Equals(null))
                            result.Add(list.FirstOrDefault(x => x.Id == id));
                    }
                }
            }

            return result;
        }
        #endregion

        #region POST
        // Post new task to tasklist
        [HttpPost("NewTask")]
        public IActionResult NewTask(TasksList model)
        {
            var userId = User.Claims.ToList()[0].Value;
            model.Author = _db.Users.FirstOrDefault(x => x.Id == userId);
            _db.Tasks.Add(model);

            if (_db.SaveChanges() > 0)
                return Ok(true);

            return Ok(false);
        }

        // Post new tag to task
        [HttpPost("NewTag")]
        public IActionResult NewTask(Tags model)
        {
            var userId = User.Claims.ToList()[0].Value;
            var task = AllTasks.FirstOrDefault(x => x.Id == model.TaskId && x.Author.Id == userId);
            if (task != null)
            {
                _db.Tags.Add(model);
                if (_db.SaveChanges() > 0)
                    return Ok(true);
            }

            return Ok(false);
        }
        #endregion

        #region PUT
        // PUT updatera task
        [HttpPut("UpdateTask/{id}")]
        public ActionResult UpdateTaskList(int id, TasksList model)
        {
            var task = AllTasks.FirstOrDefault(x => x.Id == id);
            var userId = User.Claims.ToList()[0].Value;
            if (task != null && task.Author.Id == userId)
            {
                task.Name = model.Name;
                task.Description = model.Description;
                if (_db.SaveChanges() > 0)
                    return Ok(true);
            }

            return Ok(false);
        }

        // PUT add user to list
        [HttpPut("Task/{id}/Member/{user_id}")]
        public ActionResult NewMembers(int id, string user_id)
        {

            var task = AllTasks.FirstOrDefault(x => x.Id == id);
            var userId = User.Claims.ToList()[0].Value;
            if (task != null && task.Author.Id == userId)
            {
                var member = _db.Users.FirstOrDefault(x => x.Id == user_id);
                if (member != null)
                {
                    task.Members.Add(member);
                    if (_db.SaveChanges() > 0)
                        return Ok(true);
                }
            }
            return Ok(false);
        }

        //PUT update task is done
        [HttpPut("TaskDone/{taskId}")]
        public ActionResult TaskIsDone(int taskId)
        {
            var userId = User.Claims.ToList()[0].Value;
            var task = AllTasks.FirstOrDefault(x => x.Id == taskId && x.Author.Id == userId);
            if (task != null)
            {
                task.Done = true;
                if (_db.SaveChanges() > 0)
                    return Ok(true);
            }

            return Ok(false);
        }

        //PUT update task's tag is done
        [HttpPut("TagDone/{tagId}")]
        public ActionResult TaskTagIsDone(int tagId)
        {
            var userId = User.Claims.ToList()[0].Value;
            var tag = AllTags.FirstOrDefault(x => x.Id == tagId);
            if (tag != null)
            {
                var task = AllTasks.FirstOrDefault(x => x.Id == tag.TaskId && x.Author.Id == userId);
                if (task != null)
                {
                    tag.Done = true;
                    if (_db.SaveChanges() > 0)
                        return Ok(true);
                }
            }
            return Ok(false);
        }
        #endregion

        #region DELETE
        // DELETE delete task from task list
        [HttpDelete("DeleteTask/{id}")]
        public ActionResult DeleteTask(int id)
        {
            var userId = User.Claims.ToList()[0].Value;
            var task = AllTasks.FirstOrDefault(x => x.Id == id && x.Author.Id == userId);
            if (task != null)
            {
                var tags = AllTags.Where(x => x.TaskId == task.Id).ToList();
                if (tags.Count > 0)
                {
                    for (var i = 0; i < tags.Count; i++)
                        _db.Tags.Remove(tags[i]);
                }

                _db.Tasks.Remove(task);
                if (_db.SaveChanges() > 0)
                    return Ok(true);
            }
            return Ok(false);
        }

        // DELETE delete task from task list
        [HttpDelete("DeleteTag/{id}")]
        public ActionResult DeleteTasksTag(int id)
        {
            var tag = AllTags.FirstOrDefault(x => x.Id == id);
            if (tag != null)
            {
                var userId = User.Claims.ToList()[0].Value;
                var task = AllTasks.FirstOrDefault(x => x.Id == tag.TaskId && x.Author.Id == userId);
                if (task != null)
                {
                    _db.Tags.Remove(tag);
                    if (_db.SaveChanges() > 0)
                        return Ok(true);
                }
            }
            return Ok(false);
        }
        #endregion
    }
}
