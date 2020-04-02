using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace SignalRCore
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSignalR((routes) =>
            {
                routes.MapHub<StringHub>("/aas");
            });
        }
    }

    //------------------ this is hub for receiving and sending string ----------------------
    public interface IMessage
    {
        //This method name is important
        //client name need to match this name
        //[Client code] connection.On<string>("ServerReply", ...)
        Task ServerReply(string currentTime);
    }

    public class StringHub : Hub<IMessage>
    {
        [HubMethodName("PostMessage")]
        public Task SendMessage(string message)
        {
            Console.WriteLine($"received something - {message}");

            //return to sender this message
            return Clients.All.ServerReply($"Received your message, you sent: {message}");
        }
    }


}
