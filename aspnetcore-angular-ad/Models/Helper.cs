using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyMisWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace MyMisWeb.Models
{
    public class Helper
    {
        public static string EnsureMaxLength(string input, int max)
        {
            if(input == null)
            {
                return input;
            }
            if(input.Length <= max)
            {
                return input;
            }
            return input.Substring(0, max);
        }

        public static string ConverCenterName(string center)
        {
            //if need to convert names
            return center;
        }

        public static string ConverRegionName(string region)
        {
            //if need to convert names
            return region;
        }

        internal static string ConverCenterNameForRegionHQ(string centerName, int regionID, MyMisContext context)
        {
            //if need to convert names
            return centerName;
        }
    }
}
