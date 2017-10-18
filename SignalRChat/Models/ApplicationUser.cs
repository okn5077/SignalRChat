using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;

namespace SignalRChat.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public virtual ApplicationUser Representative { get; set; }
        public ICollection<Connection> Connections { get; set; }
        public Guid RepId { get; set; }
        public ICollection<ConversationLog> ConversationLogs { get; set; }
    }
}