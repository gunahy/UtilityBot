using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using UtilityBot.Models;

namespace UtilityBot.Controllers
{
    public class CallbackQueryController
    {
        private readonly ITelegramBotClient _telegramBotClient;  
        private readonly UserSessionManager _userSessionManager;

        public CallbackQueryController(ITelegramBotClient telegramBotClient, UserSessionManager userSessionManager)
        {
            _telegramBotClient = telegramBotClient;
            _userSessionManager = userSessionManager;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var session = _userSessionManager.GetSession(callbackQuery.From.Id);
            if (callbackQuery.Data == "count_characters")
            {
                session.SetMode(UserSession.Mode.CountCharacters);
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Отправьте текстовое сообщение, и я посчитаю количество символов.",
                    cancellationToken: cancellationToken);
            }
            else if (callbackQuery.Data == "sum_numbers")
            {
                session.SetMode(UserSession.Mode.SumNumbers);
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Отправьте набор чисел, разеленных пробелом, и я посчитаю их сумму",
                    cancellationToken: cancellationToken);
            }
        }
    }
}
