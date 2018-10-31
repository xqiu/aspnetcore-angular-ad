using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMisWeb.Models
{
    public class ModifyRight
    {
        public int ModifyRightID { get; set; }
        
        public int CenterID { get; set; }

        public int MisUserID { get; set; }

        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanAdmin { get; set; }
    }
}