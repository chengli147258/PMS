using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcPoint.Models
{
    public class MemCard
    {
        [Key]
        public int  MC_ID{ get; set; }
        public int S_ID { get; set; }
        public string MC_CardID { get; set; }
        public string MC_Password { get; set; }
        public string MC_Name { get; set; }
        public int MC_Sex { get; set; }
        public string MC_Mobile { get; set; }
        public string MC_Photo { get; set; }
        public int CL_ID { get; set; }
        public int MC_Birthday_Month { get; set; }
        public int MC_Birthday_Day { get; set; }
        public byte MC_BirthdayType { get; set; }
        public bool MC_IsPast { get; set; }
        public DateTime? MC_PastTime { get; set; }
        public int MC_Point { get; set; }
        public Single MC_Money { get; set; }
        public Single MC_TotalMoney { get; set; }
        public int MC_TotalCount { get; set; }
        public int MC_OverCount { get; set; }
        public int MC_State { get; set; }
        public bool MC_IsPointAuto { get; set; }
        public int MC_RefererID { get; set; }
        public string MC_RefererCard { get; set; }
        public string MC_RefererName { get; set; }
        public DateTime? MC_CreateTime { get; set; }
        public CardLevel CardLevel { get; set; }
    }
}