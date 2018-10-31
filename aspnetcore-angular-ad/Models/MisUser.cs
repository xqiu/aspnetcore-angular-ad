using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyMisWeb.Models
{
    public class MisUser
    {
        public int MisUserID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string IdentityName { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int? CenterID { get; set; }
        public virtual Center Center { get; set; }

        public virtual ICollection<ModifyRight> ModifyRights { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [StringLength(50)]
        public string DeletedBy { get; set; }

        public DateTime Created { get; set; }
        [StringLength(50, MinimumLength = 1)]
        public string CreatedBy { get; set; }
    }

    public class AccessMode
    {
        public string Name { get; set; }
    }

}