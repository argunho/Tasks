using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tasks.Context;
using Tasks.Models;

namespace Tasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HandleUsersController : ControllerBase
    {
        private readonly DbConnect _db;

        public HandleUsersController(DbConnect  db)
        {
            _db = db;
        }

        public IEnumerable<Users> AllUsers
        {
            get
            {
                return _db.Users.ToList();
            }
        }

        // Get list of users
        [HttpGet]
        public IEnumerable<Users> Get()
        {
            return AllUsers;
        }

        // Delete from tasks members
        [HttpDelete("DeleteMemberFromTask/{id}/Member/{userId}")]
        public ActionResult Delete(int id, string userId)
        {
            var user = AllUsers.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                var task = _db.Tasks.Include("Members").FirstOrDefault(x => x.Members.Any(m => m.Id == user.Id) && x.Id == id);

                if (task != null)
                    task.Members.Remove(user);

                if (_db.SaveChanges() > 0)
                    return Ok(true);
            }
            return Ok(false);
        }

        // Delete user from systemet
        [HttpDelete("DeleteAccount")]
        public ActionResult Delete()
        {
            var userId = User.Claims.ToList()[0].Value;
            var user = AllUsers.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                var list = _db.Tasks.Include("Members").Where(x => x.Author.Id == userId).ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    _db.Tasks.Remove(list[i]);
                    if (_db.SaveChanges() == 0)
                        return Ok(false);
                }

                var member = _db.Tasks.Include("Members").Where(x => x.Members.Any(m => m.Id == user.Id)).ToList();
                if (member.Count > 0)
                {
                   for (var i = 0; i < member.Count; i++)
                    {
                        list[i].Members.Remove(user);
                        if (_db.SaveChanges() == 0)
                            return Ok(false);
                    }
                }
 

                _db.Users.Remove(user);
                if (_db.SaveChanges() > 0)
                    return Ok(true);
            }
            return Ok(false);
        }
    }
}