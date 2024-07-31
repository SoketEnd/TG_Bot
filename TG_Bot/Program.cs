using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TG_Bot
{
    // Интерфейс для обработки сообщений
    public interface IMessageHandler
    {
        void HandleMessage(object sender, MessageEventArgs e);
    }
    // Интерфейс для команд бота
    public interface IBotCommand
    {
        string Execute(string input);
    }

    // Команда для подсчета символов
    public class CalculateLength : IBotCommand
    {
        public string Execute(string input)
        {
            string strWithoutSpaces = input.Replace(" ", string.Empty);
            int length = strWithoutSpaces.Length;
            return $"В вашем сообщении {length} символов.";
        }
    }
    // Команда для подсчета суммы 
    public class CalculateSum : IBotCommand
    {
        public string Execute(string input)
        {
            try
            {
                int sum = input.Split(' ').Select(int.Parse).Sum();
                return $"Сумма чисел: {sum}.";
            }
            catch
            {
                return "Ошибка: введите корректные числа через пробел.";
            }
        }
    }
    // Класс для обработки сообщений
    public class BotMessageHandler : IMessageHandler
    {
        private TelegramBotClient _client;
        private string _currentFunction;
        private IBotCommand _currentCommand;

        public BotMessageHandler(TelegramBotClient client)
        {
            _client = client;
            _currentFunction = string.Empty;
        }

        public async void HandleMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg?.Text != null)
            {
                if (msg.Text == "/start")
                {
                    _currentFunction = string.Empty;
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Подсчитать символы", "Вычислить сумму чисел" }
                    })
                    {
                        ResizeKeyboard = true
                    };

                    await _client.SendTextMessageAsync(msg.Chat.Id, "Выберите действие:", replyMarkup: replyKeyboard);
                }
                else if (_currentFunction == string.Empty)
                {
                    if (msg.Text == "Подсчитать символы")
                    {
                        _currentFunction = "length";
                        _currentCommand = new CalculateLength();
                        await _client.SendTextMessageAsync(msg.Chat.Id, "Отправьте текст для подсчета символов.");
                    }
                    else if (msg.Text == "Вычислить сумму чисел")
                    {
                        _currentFunction = "sum";
                        _currentCommand = new CalculateSum();
                        await _client.SendTextMessageAsync(msg.Chat.Id, "Отправьте числа через пробел для вычисления суммы.");
                    }
                    else
                    {
                        await _client.SendTextMessageAsync(msg.Chat.Id, "Пожалуйста, выберите одно из действий из меню.");
                    }
                }
                else
                {
                    string response = _currentCommand.Execute(msg.Text);
                    await _client.SendTextMessageAsync(msg.Chat.Id, response, replyToMessageId: msg.MessageId);
                    _currentFunction = string.Empty;
                }
            }
        }
    }
    class Program
    {
        private static string token { get; set; } = "7432274321:AAGxGF2d76TFTioF5aPxwogtwg7w5hJbXoc";

        static void Main(string[] args)
        {
            var client = new TelegramBotClient(token);

            var messageHandler = new BotMessageHandler(client);

            client.OnMessage += messageHandler.HandleMessage;

            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }
    }
}
