using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMisWeb.Models;

namespace MyMisWeb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MyMisContext context)
        {
            context.Database.EnsureCreated();

            // Look for any data to see if it's been seeded.
            if (context.MisUsers.Any())
            {
                return;   // DB has been seeded
            }

            var regions = new Region[]
            {
                new Region{Name="International"},
                new Region{Name="US"},
            };
            foreach (var s in regions)
            {
                context.Regions.Add(s);
            }
            context.SaveChanges();

            foreach (var region in regions)
            {
                switch (region.Name)
                {
                    case "International":
                        var hqs = new Center[]
                        {
                            new Center{Name="HQ", Region=region, DoNotTrackMonthStatus=true},
                        };
                        foreach (var s in hqs)
                        {
                            context.Centers.Add(s);
                        }
                        break;
                    case "US":
                        var usCenters = new Center[]
                        {
                            new Center{Name="Seattle", Region=region},
                        };
                        foreach (var s in usCenters)
                        {
                            context.Centers.Add(s);
                        }
                        break;
                }
            }
            context.SaveChanges();

            var users = new MisUser[]
            {
                new MisUser{Name="test2", IdentityName="live.com#xxxx@outlook.com", IsActive= true, IsAdmin=true },
            };
            foreach (var s in users)
            {
                context.MisUsers.Add(s);
            }
            context.SaveChanges();

            context.SaveChanges();

            var emailTemplate = new EmailTemplate
            {
                IsForMonthStatusNotification = true,
                Subject = @"[%CenterName%] %YearMonth% Notice",
                Message = @"Test message"
            };
            context.EmailTemplates.Add(emailTemplate);
            context.SaveChanges();
        }
    }
}
