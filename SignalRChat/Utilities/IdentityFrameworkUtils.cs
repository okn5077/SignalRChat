using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SignalRChat.DAL;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace SignalRChat.Utilities
{
    public class IdentityFrameworkUtils : IDisposable
    {

        private static UserManager<ApplicationUser> _userManager;
        private static UserManager<ApplicationUser> userManager
        {
            get
            {
                if (_userManager == null)
                {
                    UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);
                    _userManager = new UserManager<ApplicationUser>(userStore);
                }
                return _userManager;
            }
        }

        private static IdentityContext _context;
        private static IdentityContext context
        {
            get
            {
                if (_context == null)
                {
                    _context = new IdentityContext();
                }
                return _context;
            }
        }

        public void SaveConversation(ConversationLog log, String msgFrom)
        {
            var from = context.Users
                .Include(u => u.ConversationLogs)
                .SingleOrDefault(u => u.UserName == msgFrom);

            from.ConversationLogs.Add(log);
            context.SaveChanges();
        }

        public void SaveConnection(Connection conn, string userName)
        {
            var user = context.Users
                .Include(u => u.Connections)
                .SingleOrDefault(u => u.UserName == userName);
            user.Connections.Add(conn);
            context.SaveChanges();
        }

        public List<Connection> GetUserConnectionInfo(string userName)
        {
            List<Connection> connList = new List<Connection>();
            connList = context.Users.Include(u => u.Connections).FirstOrDefault(u => u.UserName == userName).Connections.Where(c => c.Connected == ConnectionType.Online).ToList();
            return connList;
        }

        public IEnumerable<string> GetCustomerRepresentative(string userName)
        {
            ApplicationUser user = userManager.FindByName(userName);
            ApplicationUser rep = userManager.FindById(user.RepId.ToString());
            List<string> userList = new List<string>();
            if (rep != null)
                userList.Add(rep.UserName);
            return userList;
        }

        public IEnumerable<string> GetRepresentativesUsers(string representativeName)
        {
            ApplicationUser rep = userManager.FindByName(representativeName);
            return context.Users.Where(u => u.Representative.Id == rep.Id).Select(u => u.UserName).ToList();
        }

        public ApplicationUser GetUser(string userName)
        {
            return userManager.FindByName(userName);
        }

        public UserType GetUserType(string userName)
        {
            ApplicationUser user = userManager.FindByName(userName);
            if (userManager.IsInRole(user.Id, "Representative"))
            {
                return UserType.Representative;
            }
            else
                return UserType.User;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}