using SignalRChat.Models;
using System.Data.Entity;

namespace SignalRChat.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ConversationLog> ConversationLogs { get; set; }
        public DbSet<Connection> Connections { get; set; }

        public DatabaseContext() : base("Data Source=192.168.100.28;Initial Catalog=SignalRDemo;User ID=bbs;Password=bbs")
        {
            Configuration.LazyLoadingEnabled = false;
        }
    }
}