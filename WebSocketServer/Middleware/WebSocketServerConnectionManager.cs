using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketServer.Middleware
{
    public class WebSocketServerConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }

        public string AddSocket(WebSocket socket)
        {
            var connId = Guid.NewGuid().ToString();
            _sockets.TryAdd(connId, socket);
            Console.WriteLine("Connection Added: " + connId);

            return connId;
        }
    }
}