using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.Identity;
using SignalRChat.Models;
using SignalRChat.Utilities;

namespace SignalRChat.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {

        public override System.Threading.Tasks.Task OnConnected()
        {
            string userName = Context.User.Identity.Name;

            IEnumerable<string> users = GetAvailableUserList(userName, out UserType userType);
            Dictionary<string, string> onlineUsers = GetOnlineAvailableUserList(users);
            InformAll(userName, onlineUsers, userType, ConnectionType.Connected);
            SaveUserConnection(userName, ConnectionType.Connected);
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.GetUserName();

            IEnumerable<string> users = GetAvailableUserList(userName, out UserType userType);
            Dictionary<string, string> onlineUsers = GetOnlineAvailableUserList(users);
            InformAll(userName, onlineUsers, userType, ConnectionType.Disconnected);
            SaveUserConnection(userName, ConnectionType.Disconnected);
            return base.OnDisconnected(stopCalled);
        }

        [HubMethodName("goOnline")]
        public void GoOnline()
        {
            string userName = Context.User.Identity.Name;
            IEnumerable<string> users = GetAvailableUserList(userName, out UserType userType);
            Dictionary<string, string> onlineUsers = GetOnlineAvailableUserList(users);
            InformAll(userName, onlineUsers, userType, ConnectionType.Online);
            SaveUserConnection(userName, ConnectionType.Online);

            Dictionary<string, string> connectedUsers = GetConnectedAvailableUserList(users);
            if (userType.Equals(UserType.Representative))
                InformCustomers(userName, connectedUsers, ConnectionType.Online);
        }

        [HubMethodName("goOffline")]
        public void GoOffline()
        {
            string userName = Context.User.Identity.Name;
            IEnumerable<string> users = GetAvailableUserList(userName, out UserType userType);
            Dictionary<string, string> onlineUsers = GetOnlineAvailableUserList(users);
            InformUsers(userName, onlineUsers, ConnectionType.Connected, userType);
            SaveUserConnection(userName, ConnectionType.Connected);

            Dictionary<string, string> connectedUsers = GetConnectedAvailableUserList(users);
            if (userType.Equals(UserType.Representative))
                InformCustomers(userName, connectedUsers, ConnectionType.Connected);
        }

        [HubMethodName("privatemessage")]
        public void SendPrivateMessage(String msgFrom, String msg, String msgTo)
        {
            List<Connection> connList = new List<Connection>();
            using (IdentityFrameworkUtils utils = new IdentityFrameworkUtils())
                connList = utils.GetUserConnectionInfo(msgTo);
            if (connList.Count > 0)
            {
                Clients.Caller.receiveMessage(msgFrom, msg, msgTo, true);
                Clients.Client(connList.FirstOrDefault().ConnectionID).receiveMessage(msgFrom, msg, msgTo, false);
                using (IdentityFrameworkUtils utils = new IdentityFrameworkUtils())
                {
                    ConversationLog log = new ConversationLog
                    {
                        ClientIp = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null,
                        CreatedOn = DateTime.Now,
                        FromUser = msgFrom,
                        ToUser = msgTo,
                        Message = msg
                    };
                    utils.SaveConversation(log, msgFrom);
                }
            }
            else
            {
                //ERROR
            }
        }

        private void InformAll(string userName, Dictionary<string, string> onlineUsers, UserType userType, ConnectionType connType)
        {
            InformSelf(onlineUsers, userName, userType);
            InformUsers(userName, onlineUsers, connType, userType);
        }

        private void InformUsers(string userName, Dictionary<string, string> onlineUsers, ConnectionType connectionType, UserType type)
        {
            foreach (var user in onlineUsers)
            {
                Clients.Client(user.Value).informUser(userName, (int)connectionType, (int)type);
            }
        }

        private void InformCustomers(string userName, Dictionary<string, string> connectedUsers, ConnectionType connectionType)
        {
            foreach (var user in connectedUsers)
            {
                Clients.Client(user.Value).informCustomer((int)connectionType);
            }
        }

        private void InformSelf(Dictionary<string, string> onlineUsers, string selfName, UserType type)
        {
            Clients.Caller.informSelf(onlineUsers.Keys.ToArray(), selfName, (int)type);
        }

        private IEnumerable<string> GetAvailableUserList(string userName, out UserType type)
        {
            using (IdentityFrameworkUtils utils = new IdentityFrameworkUtils())
            {
                type = utils.GetUserType(userName);
                IEnumerable<string> list = new List<string>();

                switch (type)
                {
                    case UserType.User:
                        list = utils.GetCustomerRepresentative(userName);
                        break;
                    case UserType.Representative:
                        list = utils.GetRepresentativesUsers(userName);
                        break;
                    default:
                        break;
                }
                return list;
            }
        }

        private Dictionary<string, string> GetOnlineAvailableUserList(IEnumerable<string> users)
        {
            Dictionary<string, string> onlineList = new Dictionary<string, string>();
            List<Connection> onlineUsers = new List<Connection>();
            using (DbUtils dbUtils = new DbUtils())
                onlineUsers = dbUtils.GetOnlineUsers();
            foreach (var item in users)
            {
                if (onlineUsers.Where(o => o.UserName == item).Count() > 0)
                    onlineList.Add(onlineUsers.FirstOrDefault(t => t.UserName == item).UserName, onlineUsers.FirstOrDefault(t => t.UserName == item).ConnectionID);
            }
            return onlineList;
        }

        private Dictionary<string, string> GetConnectedAvailableUserList(IEnumerable<string> users)
        {
            Dictionary<string, string> onlineList = new Dictionary<string, string>();
            List<Connection> connectedUsers = new List<Connection>();
            using (DbUtils dbUtils = new DbUtils())
                connectedUsers = dbUtils.GetConnectedUsers();
            foreach (var item in users)
            {
                if (connectedUsers.Where(o => o.UserName == item).Count() > 0)
                    onlineList.Add(connectedUsers.FirstOrDefault(t => t.UserName == item).UserName, connectedUsers.FirstOrDefault(t => t.UserName == item).ConnectionID);
            }
            return onlineList;
        }

        private void SaveUserConnection(string userName, ConnectionType connectionType)
        {
            using (DbUtils dbUtils = new DbUtils())
                dbUtils.DisconnectUsersPreviousConnections(userName, connectionType);

            Connection connection = new Connection
            {
                ConnectionID = Context.ConnectionId,
                UserAgent = Context.Request.Headers["User-Agent"],
                Connected = connectionType,
                UserName = userName
            };
            using (IdentityFrameworkUtils utils = new IdentityFrameworkUtils())
                utils.SaveConnection(connection, userName);
        }

    }
}