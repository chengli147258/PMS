using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MvcPoint.Models
{
    public class PointContext:DbContext
    {
        public DbSet<CardLevel> CardLevels {get;set;}
        public DbSet<MemCard> MemCards { get; set; }
        public DbSet<Shop> Shop { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<CategoryItem> CategoryItems { get; set; }
        public DbSet<ExchangGift> ExchangGift { get; set; }
        public DbSet<TransferLog> TransferLog { get; set; }
        public DbSet<ConsumeOrder> ConsumeOrder { get; set; }
        public DbSet<ExchangLog> ExchangLogs { get; set; }
    }
}