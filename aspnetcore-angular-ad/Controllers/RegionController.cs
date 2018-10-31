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
    public class RegionController : MisBaseController
    {
        public RegionController(MyMisContext context) : base(context)
        {
        }

        //api/Region
        [HttpGet("[action]")]
        public IActionResult GetWritableRegions()
        {
            return GetMyRegions(true);
        }

        //api/Region
        [HttpGet("[action]")]
        public IActionResult GetReadableRegions()
        {
            return GetMyRegions(false);
        }

        private IActionResult GetMyRegions(bool needWriteAccess)
        {
            if (IsCurrentUserActiveGlobalAdmin())
            {
                var allRegions = _context.Regions.Where(b => !b.Deleted)
                              .Select(p => new Region
                              {
                                  RegionID = p.RegionID,
                                  Name = p.Name,
                                  Deleted = p.Deleted,
                                  Centers = p.Centers.Where(a => !a.Deleted).ToList()
                              })
                              .ToList();
                return Ok(allRegions);
            }

            var user = GetCurrentUser();
            if (user == null)
            {
                return BadRequest();
            }

            var regions = _context.Regions.Where(b => !b.Deleted)
                          .Select(p => new Region
                          {
                              RegionID = p.RegionID,
                              Name = p.Name,
                              Deleted = p.Deleted
                          })
                          .ToList();

            foreach (var myregion in regions)
            {
                if (needWriteAccess)
                {
                    myregion.Centers =
                        (from center in _context.Centers
                         join region in _context.Regions on center.RegionID equals region.RegionID
                         join right in _context.ModifyRights on center.CenterID equals right.CenterID
                         where right.MisUserID == user.MisUserID &&
                         right.CanWrite == true && region.RegionID == myregion.RegionID
                         select center).ToList();
                }
                else
                {
                    myregion.Centers =
                        (from center in _context.Centers
                         join region in _context.Regions on center.RegionID equals region.RegionID
                         join right in _context.ModifyRights on center.CenterID equals right.CenterID
                         where right.MisUserID == user.MisUserID &&
                         right.CanRead == true && region.RegionID == myregion.RegionID
                         select center).ToList();
                }
            }

            return Ok(regions);
        }

        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Region region)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin() )
            {
                _context.Regions.Add(region);
                _context.SaveChanges();
                return Ok(region);
            }
            return BadRequest();
        }

        // 
        [HttpPost("[action]")]
        public IActionResult Update([FromBody, Bind("RegionID, Name")] Region region)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                var dbRegion = _context.Regions.SingleOrDefault(b=>b.RegionID == region.RegionID && !b.Deleted);
                if(dbRegion != null)
                {
                    dbRegion.Name = region.Name;
                    _context.Entry(dbRegion).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(region);
                }
            }
            return BadRequest();
        }

        [HttpPost("[action]")]
        public IActionResult Delete([FromBody] Region region)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                var dbRegion = _context.Regions.Include(b=>b.Centers).SingleOrDefault(b => b.RegionID == region.RegionID);
                if (dbRegion != null)
                {
                    dbRegion.Deleted = true;
                    dbRegion.DeletedAt = DateTime.Now;
                    dbRegion.DeletedBy = User.Identity.Name;

                    _context.Entry(dbRegion).State = EntityState.Modified;
                    _context.SaveChanges();
                    return NoContent();
                }
            }
            return BadRequest();
        }

    }
}
