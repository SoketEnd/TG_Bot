using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TG_Bot
{

    class Program
    {
        private static string token { get; set; } = "7432274321:AAGxGF2d76TFTioF5aPxwogtwg7w5hJbXoc";

        private static TelegramBotClient client;

        private static string currentFunction = string.Empty;

        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);

            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }
        //Счёт символов в строке 
        private static string CalculateLength(string str)
        {
            string strWithoutSpaces = str.Replace(" ", string.Empty);
            int length = strWithoutSpaces.Length;
            return $"В вашем сообщении {length} символов.";
        }
        //Счёт чисел
        private static string CalculateSum(string str)
        {
            
            try
            {
                int sum = str.Split(' ').Select(int.Parse).Sum();
                return $"Сумма чисел: {sum}.";
            }
            catch
            {
                return "Ошибка: введите корректные числа через пробел.";
            }
        }
        //Обработка сообщений 
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg?.Text != null)
            {
                if (msg.Text == "/start")
                {
                    currentFunction = string.Empty; // Сбрасываем текущую функцию при старте
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Подсчитать символы", "Вычислить сумму чисел" }
                    })
                    {
                        ResizeKeyboard = true
                    };

                    await client.SendTextMessageAsync(msg.Chat.Id, "Выберите действие:", replyMarkup: replyKeyboard);
                }
                else if (currentFunction == string.Empty)
                {
                    if (msg.Text == "Подсчитать символы")
                    {
                        currentFunction = "lenght";
                        await client.SendTextMessageAsync(msg.Chat.Id, "Отправьте текст для подсчета символов.");
                    }
                    else if (msg.Text == "Вычислить сумму чисел")
                    {
                        currentFunction = "sum";
                        await client.SendTextMessageAsync(msg.Chat.Id, "Отправьте числа через пробел для вычисления суммы.");
                    }
                    else
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пожалуйста, выберите одно из действий из меню.");
                    }
                }
                else if (currentFunction == "lenght")
                {
                    string response = CalculateLength(msg.Text);
                    await client.SendTextMessageAsync(msg.Chat.Id, response, replyToMessageId: msg.MessageId);
                    currentFunction = string.Empty;
                }
                else if (currentFunction == "sum")
                {
                    string response = CalculateSum(msg.Text);
                    await client.SendTextMessageAsync(msg.Chat.Id, response, replyToMessageId: msg.MessageId);
                }
            }
        }
    }
}
