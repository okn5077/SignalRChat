using Microsoft.AspNet.Identity.EntityFramework;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SignalRChat.DAL
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext() : base("Data Source=192.168.100.28;Initial Catalog=SignalRDemo;User ID=bbs;Password=bbs")
        {
            Configuration.LazyLoadingEnabled = false;
        }
    }
}