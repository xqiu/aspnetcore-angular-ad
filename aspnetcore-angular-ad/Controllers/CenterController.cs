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
    public class CenterController : MisBaseController
    {
        public static List<int> MonthStatusGenerated = new List<int>();  //caching the genreated yearmonth

        public CenterController(MyMisContext context) : base(context)
        {
        }

        //api/Center
        [HttpGet("[action]")]
        public IActionResult GetCenters()
        {
            var Centers = _context.Centers.Where(b => !b.Deleted).ToList();
            return Ok(Centers);
        }

        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Center center)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                _context.Centers.Add(center);
                _context.SaveChanges();
                return Ok(center);
            }
            return BadRequest();
        }

        // 
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Center center)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                var dbCenter = _context.Centers.SingleOrDefault(b => b.CenterID == center.CenterID && !b.Deleted);
                if (dbCenter != null)
                {
                    dbCenter.Name = center.Name;
                    dbCenter.DoNotTrackMonthStatus = center.DoNotTrackMonthStatus;
                    dbCenter.NotificationEmail = center.NotificationEmail;
                    dbCenter.IsRegionHQ = center.IsRegionHQ;
                    _context.Entry(dbCenter).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(center);
                }
            }
            return BadRequest();
        }

        [HttpPost("[action]")]
        public IActionResult Delete([FromBody] Center center)
        {
            if (ModelState.IsValid && IsCurrentUserActiveGlobalAdmin())
            {
                var dbCenter = _context.Centers.SingleOrDefault(b => b.CenterID == center.CenterID);
                if (dbCenter != null)
                {
                    dbCenter.Deleted = true;
                    dbCenter.DeletedAt = DateTime.Now;
                    dbCenter.DeletedBy = User.Identity.Name;
                    _context.Entry(dbCenter).State = EntityState.Modified;
                    _context.SaveChanges();
                    return NoContent();
                }
            }
            return BadRequest();
        }
    }
}
