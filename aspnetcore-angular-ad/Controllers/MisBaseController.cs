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
    public class MisBaseController : Controller
    {
        protected readonly MyMisContext _context;

        protected Dictionary<string, Region> _regions = null;
        protected Dictionary<string, Center> _centers = null;
        protected Dictionary<int, ModifyRight> _modifyRights = null;
        private bool _isGlobalAdmin = false;

        public MisBaseController(MyMisContext context)
        {
            _context = context;
        }

        public MisUser GetCurrentUser()
        {
            return _context.MisUsers.SingleOrDefault(x => x.IdentityName == User.Identity.Name && x.Deleted==false);
        }

        public ModifyRight GetUserRightForCenter(MisUser user, int centerID)
        {
            var right = _context.ModifyRights.SingleOrDefault(x => x.CenterID == centerID && x.MisUserID == user.MisUserID);

            if(right == null)
            {
                return new ModifyRight()
                {
                    CanAdmin = false,
                    CanRead = false,
                    CanWrite = false,
                    CenterID = centerID,
                    MisUserID = user.MisUserID,
                    ModifyRightID = 0
                };
            }
            return right;
        }

        protected bool CanCurrentUserRead(int centerID)
        {
            var user = GetCurrentUser();
            if(user == null)
            {
                return false;
            }

            if (!user.IsActive || user.Deleted)
            {
                return false;
            }

            if (user.IsAdmin)
            {
                return true;
            }

            var right = GetUserRightForCenter(user, centerID);
            if (right == null)
            {
                return false;
            }

            return right.CanRead | right.CanAdmin;
        }

        protected bool CanCurrentUserWrite(int centerID)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return false;
            }

            if (!user.IsActive || user.Deleted)
            {
                return false;
            }

            if (user.IsAdmin)
            {
                return true;
            }

            var right = GetUserRightForCenter(user, centerID);
            if (right == null)
            {
                return false;
            }

            return right.CanWrite | right.CanAdmin;
        }

        protected bool CanCurrentUserAdmin(int centerID)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return false;
            }

            if (!user.IsActive || user.Deleted)
            {
                return false;
            }

            if (user.IsAdmin)
            {
                return true;
            }

            var right = GetUserRightForCenter(user, centerID);
            if (right == null)
            {
                return false;
            }

            return right.CanAdmin;
        }

        protected bool IsCurrentUserActiveGlobalAdmin()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return false;
            }
            
            return user.IsAdmin && user.IsActive && !user.Deleted;
        }

        protected bool IsCurrentUserActive()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return false;
            }

            return user.IsActive && !user.Deleted;
        }

        protected void CacheRegionsAndCenters()
        {
            var user = GetCurrentUser();
            _isGlobalAdmin = user.IsAdmin && user.IsActive && !user.Deleted;

            if (_regions == null)
            {
                _regions = new Dictionary<string, Region>();
            }
            if (_centers == null)
            {
                _centers = new Dictionary<string, Center>();
            }
            if (_modifyRights == null)
            {
                _modifyRights = new Dictionary<int, ModifyRight>();
            }

            foreach (var region in _context.Regions)
            {
                _regions.Add(region.Name, region);
            }
            foreach (var center in _context.Centers)
            {
                _centers.Add(center.GetDictKey(), center);
            }

            foreach (var right in _context.ModifyRights.Where(x=>x.MisUserID == user.MisUserID))
            {
                _modifyRights.Add(right.CenterID, right);
            }
        }

        protected bool CanWriteCenterFromCachedRights(int centerID)
        {
            if (_isGlobalAdmin) return true;
            if (_modifyRights.ContainsKey(centerID))
            {
                if(_modifyRights[centerID].CanWrite || _modifyRights[centerID].CanAdmin)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
