using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;
using System.Text.Unicode;
using System.Text;
using System.Text.Encodings.Web;

namespace GlassZebra.Controllers
{
    public class WebSocketHandler
    {
        private static List<WebSocket> Sockets = new List<WebSocket>();
        private static List<WebSocket> ListeningSockets = new List<WebSocket>();

        internal static async Task HandleAsync(HttpContext context, Func<Task> next)
        {
            if (context.Request.Path.Value.StartsWith("/ws"))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Sockets.Add(webSocket);

                    if (context.Request.Path.Value.Contains("listen"))
                        ListeningSockets.Add(webSocket);

                    await ListenSocket(context, webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next();
            }
                
        }

        private static async Task ListenSocket(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                foreach (var socket in ListeningSockets)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            Sockets.Remove(webSocket);
            if (ListeningSockets.Contains(webSocket))
                ListeningSockets.Remove(webSocket);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        
        public static async Task BroadcastUpdate()
        {
            var payload = Encoding.UTF8.GetBytes("update");
            foreach (var socket in ListeningSockets)
            {
                await socket.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
