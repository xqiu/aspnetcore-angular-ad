using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class CodeModel
    {
        public string ModuleName { get; set; }
        public string ContextName { get; set; }
        public string ClassName { get; set; }
        public List<FieldModel> Fields { get; set; }

        public CodeModel()
        {
            Fields = new List<FieldModel>();
        }

        public string FirstLetterLowerCasedClassName {
            get
            {
                return ClassName.Substring(0, 1).ToLowerInvariant() + ClassName.Substring(1);
            }
        }

        public int FieldStatCount()
        {
            var count = Fields.Count(x => x.IsStat);
            return count;
        }
    }

    public class FieldModel
    {
        public string FieldName { get; set; }
        public bool IsNumber { get; set; }
        public bool IsBool { get; set; }
        public bool IsString { get; set; }
        public bool IsDate { get; set; }
        public bool IsYearMonth { get; set; }

        /// <summary>
        /// Is needed for Input
        /// </summary>
        public bool IsInput { get; set; } 

        /// <summary>
        /// Is needed for Stat
        /// </summary>
        public bool IsStat { get; set; }

        /// <summary>
        /// Is needed for Sum in table footer
        /// </summary>
        public bool IsSum { get; set; }

        /// <summary>
        /// Is the value auto caculated
        /// </summary>
        public bool IsAuto { get; set; }

        public string FirstLetterLowerCasedClassName
        {
            get
            {
                return FieldName.Substring(0, 1).ToLowerInvariant() + FieldName.Substring(1);
            }
        }
    }

}
