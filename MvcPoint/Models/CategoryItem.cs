using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcPoint.Models
{
    public class CategoryItem
    {
        [Key]
        public string C_Category { get; set; }
        public int CI_ID { get; set; }
        public string CI_Name { get; set; } 
    }
}