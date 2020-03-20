using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcPoint.Models
{
    public class CardLevel
    {
        [Key]
        public int CL_ID { get; set; }
        public string CL_LevelName { get; set; }
        public string CL_NeedPoint { get; set; }
        public Double CL_Point { get; set; }
        public Double CL_Percent { get; set; }
    }
}