using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMisWeb.Data;
using MyMisWeb.Models;

namespace MyMisWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MisUserController : MisBaseController
    {
        public MisUserController(MyMisContext context) : base(context)
        {
        }

        [HttpGet("[action]")]
        public IActionResult GetUsers()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return NotFound("You are not a user.");
            }

            if (currentUser.IsAdmin && currentUser.IsActive && !currentUser.Deleted)
            {
                var list = _context.MisUsers.Where(b=>!b.Deleted).Include(b => b.Center).ToList();
                foreach (var user in list)
                {
                    user.ModifyRights = _context.ModifyRights.Where(b => b.MisUserID == user.MisUserID).ToList();
                }
                return Ok(list);
            }
            return NotFound("You are not allowed.");
        }

        [HttpPost("[action]")]
        public IActionResult Create([FromBody] MisUser misUser)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                var newUser = new MisUser()
                {
                    Name = misUser.Name,
                    IdentityName = misUser.IdentityName,
                    IsActive = misUser.IsActive,
                    IsAdmin = misUser.IsAdmin,
                    CenterID = misUser.CenterID
                };
                newUser.Center = _context.Centers.SingleOrDefault(b => b.CenterID == misUser.CenterID && b.Deleted==false);
                if(newUser.Center == null)
                {
                    return BadRequest();
                }
                _context.MisUsers.Add(newUser);
                _context.SaveChanges();

                foreach (var modifyRight in misUser.ModifyRights)
                {
                    var newModifyRight = new ModifyRight()
                    {
                        MisUserID = newUser.MisUserID,
                        CenterID = modifyRight.CenterID,
                        CanAdmin = modifyRight.CanAdmin,
                        CanRead = modifyRight.CanRead,
                        CanWrite = modifyRight.CanWrite,
                    };
                    _context.ModifyRights.Add(newModifyRight);
                }
                _context.SaveChanges();

                newUser.ModifyRights = _context.ModifyRights.Where(b => b.ModifyRightID == newUser.MisUserID).ToList();

                return Ok(newUser);
            }
            return BadRequest();
        }

        // 
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] MisUser misUser)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                var dbMisUser = _context.MisUsers.SingleOrDefault(b => b.MisUserID == misUser.MisUserID && b.Deleted==false);
                if (dbMisUser != null)
                {
                    dbMisUser.Name = misUser.Name;
                    dbMisUser.IdentityName = misUser.IdentityName;
                    dbMisUser.IsActive = misUser.IsActive;
                    dbMisUser.IsAdmin = misUser.IsAdmin;
                    dbMisUser.CenterID = misUser.CenterID;
                    _context.Entry(dbMisUser).State = EntityState.Modified;
                    _context.SaveChanges();

                    RemoveUserModifyRights(dbMisUser);

                    foreach (var modifyRight in misUser.ModifyRights)
                    {
                        var newRight = new ModifyRight()
                        {
                            MisUserID = misUser.MisUserID,
                            CenterID = modifyRight.CenterID,
                            CanAdmin = modifyRight.CanAdmin,
                            CanRead = modifyRight.CanRead,
                            CanWrite = modifyRight.CanWrite
                        };
                        _context.ModifyRights.Add(newRight);
                    }
                    _context.SaveChanges();

                    dbMisUser.Center = _context.Centers.SingleOrDefault(b => b.CenterID == dbMisUser.CenterID && b.Deleted == false);

                    dbMisUser.ModifyRights = _context.ModifyRights.Where(b => b.ModifyRightID == dbMisUser.MisUserID).ToList();

                    return Ok(dbMisUser);
                }
            }
            return BadRequest();
        }

        private void RemoveUserModifyRights(MisUser misUser)
        {
            _context.Database.ExecuteSqlCommand(
                "DELETE from ModifyRight where MisUserID={0}",
                parameters: misUser.MisUserID);
        }

        [HttpPost("[action]")]
        public IActionResult Delete([FromBody] MisUser misUser)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                var dbMisUser = _context.MisUsers.SingleOrDefault(b => b.MisUserID == misUser.MisUserID);
                if (dbMisUser != null)
                {
                    dbMisUser.Deleted = true;
                    dbMisUser.DeletedAt = DateTime.Now;
                    dbMisUser.DeletedBy = User.Identity.Name;

                    _context.Entry(dbMisUser).State = EntityState.Modified;
                    _context.SaveChanges();
                    return NoContent();
                }
            }
            return BadRequest();
        }

        [HttpPost("[action]")]
        public IActionResult HasAccess([FromBody] AccessMode mode)
        {
            if(mode.Name == "useradmin")
            {
                if (IsCurrentUserActiveGlobalAdmin())
                {
                    return Ok();
                }
            }
            if (mode.Name == "user")
            {
                if (IsCurrentUserActive())
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}
