using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using UtilityBot.Models;

namespace UtilityBot.Controllers
{
    public class TextMessangerController
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly UserSessionManager _userSessionManager;

        public TextMessangerController(ITelegramBotClient telegramBotClient, UserSessionManager userSessionManager)
        {
            _telegramBotClient = telegramBotClient;
            _userSessionManager = userSessionManager;
        }

        public async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var session = _userSessionManager.GetSession(message.From.Id);
            switch (session.SelectedMode)
            {
                case UserSession.Mode.CountCharacters:
                    int characterCount = message.Text.Length;
                    await _telegramBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Длинна сообщения: {characterCount} символов",
                        cancellationToken: cancellationToken);
                    break;
                case UserSession.Mode.SumNumbers:
                    var numbers = message.Text
                        .Split(' ')
                        .Where(x => int.TryParse(x, out _))
                        .Select(int.Parse);
                    var sum = numbers.Sum();

                    await _telegramBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Сумма введенных чисел равна: {sum}",
                        cancellationToken: cancellationToken);
                    break;
                default:
                    await _telegramBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Пожалуйста, выберите действие в главном меню.",
                        cancellationToken: cancellationToken);
                    break;

            }
        }
    }
}
