using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBot.Controllers;

namespace UtilityBot
{
    public class Bot : BackgroundService
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly ILogger<Bot> _logger;
        private readonly TextMessangerController _textMessangerController;
        private readonly CallbackQueryController _callbackQueryController;

        public Bot(ITelegramBotClient telegramClient, ILogger<Bot> logger, TextMessangerController textMessangerController, CallbackQueryController callbackQueryController)
        {
            _telegramClient = telegramClient;
            _logger = logger;
            _textMessangerController = textMessangerController;
            _callbackQueryController = callbackQueryController;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                },
                cancellationToken: stoppingToken);
            _logger.LogInformation("Bot started");
            await Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
                {
                    if (update.Message.Text == "/start")
                    {
                        await ShowMainMenu(update.Message.Chat.Id, cancellationToken);
                    }
                    else
                    {
                        await _textMessangerController.HandleMessageAsync(update.Message, cancellationToken);
                    }
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    await _callbackQueryController.HandleCallbackAsync(update.CallbackQuery, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки сообщения");
            }
        }



        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            string errorMessage;
            var apiRequestException = exception as ApiRequestException;

            if (apiRequestException != null)
            {
                errorMessage = $"Ошибка Telegram Api:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}";
            }
            else
            {
                errorMessage = exception.ToString();
            }

            Console.WriteLine(errorMessage);
            Console.WriteLine("Waiting 10 seconds before retry");

            await Task.Delay(10000, cancellationToken);

        }

        private async Task ShowMainMenu(long chatId, CancellationToken cancellationToken)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Подсчитать количество символов", "count_characters"),
                    InlineKeyboardButton.WithCallbackData("Сумма чисел", "sum_numbers")
                }
            });

            await _telegramClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выберите действие:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }
    }
}
