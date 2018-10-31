using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMisWeb.Models
{
    public class Center
    {
        public int CenterID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        public int RegionID { get; set; }
        public virtual Region Region { get; set; }

        public bool DoNotTrackMonthStatus { get; set; }

        public bool IsRegionHQ { get; set; }

        [StringLength(50)]
        public string NotificationEmail { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [StringLength(50)]
        public string DeletedBy { get; set; }

        internal string GetDictKey()
        {
            return string.Format("{0}_{1}", RegionID, Name);
        }
        internal static string GetDictKey(int regionID, string name)
        {
            return string.Format("{0}_{1}", regionID, name);
        }
    }

}