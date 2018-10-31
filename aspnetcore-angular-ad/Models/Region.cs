using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyMisWeb.Models
{
    public class Region
    {
        public int RegionID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [StringLength(50)]
        public string DeletedBy { get; set; }

        public virtual ICollection<Center> Centers { get; set; }
    }
}