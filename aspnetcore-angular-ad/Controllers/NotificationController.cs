using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MyMisWeb.Data;
using MyMisWeb.Models;

namespace MyMisWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationController : MisBaseController
    {
        public static List<int> MonthStatusGenerated = new List<int>();  //caching the genreated yearmonth

        private readonly IEmailConfiguration _emailConfig;
        private readonly IEmailService _emailService;

        public NotificationController(MyMisContext context, IEmailConfiguration emailConfig, IEmailService emailService) : base(context)
        {
            _emailConfig = emailConfig;
            _emailService = emailService;
        }

        //api/GetEmailTemplates
        [HttpGet("[action]")]
        public IActionResult GetEmailTemplates()
        {
            if(!IsCurrentUserActiveGlobalAdmin())
            {
                return BadRequest();
            }

            var allRecords = _context.EmailTemplates.ToList();
            return Ok(allRecords);
        }

        [HttpPost("[action]")]
        public IActionResult CreateEmailTemplate([FromBody] EmailTemplate record)
        {
            if (!IsCurrentUserActiveGlobalAdmin())
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var newRecord = new EmailTemplate()
                {
                    Subject = record.Subject,
                    Message = record.Message,
                    IsForMonthStatusNotification = record.IsForMonthStatusNotification,
                };

                _context.EmailTemplates.Add(newRecord);
                _context.SaveChanges();
                return Ok(newRecord);
            }
            return BadRequest();
        }

        [HttpPost("[action]")]
        public IActionResult DeleteEmailTemplate([FromBody] EmailTemplate record)
        {
            if (!IsCurrentUserActiveGlobalAdmin())
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var dbEmailTemplate = _context.EmailTemplates.SingleOrDefault(b => b.EmailTemplateID == record.EmailTemplateID);
                if (dbEmailTemplate != null)
                {
                    _context.EmailTemplates.Remove(dbEmailTemplate);
                    _context.SaveChanges();
                    return NoContent();
                }
            }
            return BadRequest();
        }

        [HttpPost("[action]")]
        public IActionResult UpdateEmailTemplate([FromBody] EmailTemplate record)
        {
            if (!IsCurrentUserActiveGlobalAdmin())
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var dbEmailTemplate = _context.EmailTemplates.SingleOrDefault(b => b.EmailTemplateID == record.EmailTemplateID);

                dbEmailTemplate.Subject = record.Subject;
                dbEmailTemplate.Message = record.Message;
                dbEmailTemplate.IsForMonthStatusNotification = record.IsForMonthStatusNotification;

                _context.Entry(dbEmailTemplate).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(dbEmailTemplate);
            }
            return BadRequest();
        }
    }
}
