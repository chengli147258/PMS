using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcPoint.Models
{
    public class ExchangLog
    {
        /// <summary>
        /// 兑换记录编号
        /// </summary>
        [Key]
        public int EL_ID { get; set; }
        /// <summary>
        /// 店铺编号
        /// </summary>
        public int S_ID { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public int U_ID { get; set; }
        /// <summary>
        /// 会员编号
        /// </summary>
        public int MC_ID { get; set; }
        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MC_CardID { get; set; }
        /// <summary>
        /// 会员姓名
        /// </summary>
        public string MC_Name { get; set; }
        /// <summary>
        /// 礼品编号
        /// </summary>
        public int EG_ID { get; set; }
        /// <summary>
        /// 礼品编码
        /// </summary>
        public string EG_GiftCode { get; set; }
        /// <summary>
        /// 礼品名称
        /// </summary>
        public string EG_GiftName { get; set; }
        /// <summary>
        /// 兑换数量
        /// </summary>
        public int EL_Number { get; set; }
        /// <summary>
        /// 所用积分
        /// </summary>
        public int EL_Point { get; set; }
        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime EL_CreateTime { get; set; }
    }
}