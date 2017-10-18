using SignalRChat.DAL;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRChat.Utilities
{
    public class DbUtils : IDisposable
    {
        private static DatabaseContext _context;
        private static DatabaseContext context
        {
            get
            {
                if (_context == null)
                {
                    _context = new DatabaseContext();
                }
                return _context;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void DisconnectUsersPreviousConnections(string userName, ConnectionType connectionType)
        {
            var connections = context.Connections.Where(c => c.UserName == userName).ToList();
            foreach (var conn in connections)
            {
                conn.Connected = ConnectionType.Disconnected;
            }
            context.SaveChanges();
        }

        public Connection GetUserConnection(string connId)
        {
            Connection conn = new Connection();
            conn = context.Connections.FirstOrDefault(c => c.ConnectionID == connId);
            return conn;
        }

        public List<Connection> GetOnlineUsers()
        {
            List<Connection> connList = new List<Connection>();
            connList = context.Connections.Where(c => c.Connected == ConnectionType.Online).ToList();
            return connList;
        }

        public List<Connection> GetConnectedUsers()
        {
            List<Connection> connList = new List<Connection>();
            connList = context.Connections.Where(c => c.Connected == ConnectionType.Connected).ToList();
            return connList;
        }
    }
}