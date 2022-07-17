using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebSocketServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                WriteRequestParam(context);
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("WebSocket Connected");
                    
                    await ReceiveMessage(webSocket, async (result, buffer) =>
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            Console.WriteLine("Message Received");
                            return;
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine("Received close message");
                            return;
                        }
                    });
                }
                else
                {
                    Console.WriteLine("Hello from the 2nd request delegate.");
                    await next();
                }
            });

            app.Run(async context =>
            {
                Console.WriteLine("Hello from the 3rd request delegate.");
                await context.Response.WriteAsync("Hello from the 3rd request delegate.");
            });
        }

        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine("Request Method: " + context.Request.Method);
            Console.WriteLine("Request Protocol: " + context.Request.Protocol);

            if (context.Request.Headers != null)
            {
                foreach (var h in context.Request.Headers)
                {
                    Console.WriteLine("---> " + h.Key + " : " + h.Value);
                }
            }
        }

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
