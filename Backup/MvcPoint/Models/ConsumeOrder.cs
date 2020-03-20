using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcPoint.Models
{
    public class ConsumeOrder
    {
        [Key]
        public int CO_ID { get; set; }
        public int S_ID { get; set; }
        public int U_ID { get; set; }
        public string CO_OrderCode { get; set; }
        public int CO_OrderType { get; set; }
        public int MC_ID { get; set; }
        public string MC_CardID { get; set; }
        public int EG_ID { get; set; }
        public float CO_TotalMoney { get; set; }
        public float CO_DiscountMoney { get; set; }
        public int CO_GavePoint { get; set; }
        public float CO_Recash { get; set; }
        public string CO_Remark { get; set; }
        public DateTime? CO_CreateTime { get; set; }
    }
}