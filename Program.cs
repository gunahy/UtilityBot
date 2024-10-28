using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using UtilityBot.Controllers;
using UtilityBot.Models;

namespace UtilityBot
{
    internal class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) => ConfigureServices(services, context.Configuration))
                .UseConsoleLifetime()
                .Build();
            Console.WriteLine("Starting Service");
            await host.RunAsync();
            Console.WriteLine("Service stopped");
        }

        static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var token = configuration["TelegramBotToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("Токен бота отсутствует");
            }
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(token));
            services.AddSingleton<UserSessionManager>();
            services.AddTransient<TextMessangerController>();
            services.AddTransient<CallbackQueryController>();
            services.AddHostedService<Bot>();
        }
    }
}
