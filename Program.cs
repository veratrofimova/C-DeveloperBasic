Console.WriteLine("Добро пожаловать! \r\nЗапущено базовое интерактивное меню будущего бота!");
string menu = $"Введите команду: /start, /help, /info, /exit";

Console.WriteLine($"\r\n{menu}");

string userName = "";
string input = "";
do
{
    input = Console.ReadLine().Trim();
    string[] commandText = input.Split(new char[] { ' ' });

    switch (commandText[0])
    {
        case "/start":
            Console.WriteLine("\r\nКак к Вам обращаться? Введите Ваше имя:");
            userName = Console.ReadLine();

            Console.WriteLine($"\r\nВам теперь доступна команда /echo Текст\r\n");

            menu += " /echo Текст";
            Console.WriteLine($"{userName}, {menu}");
            break;

        case "/help":
            Console.WriteLine($"\r\nКраткая справочная информация о том, как пользоваться программой\r\n");
            Console.WriteLine($"{userName}, {menu}");
            break;

        case "/info":
            Console.WriteLine($"\r\nВерсия программы 1.0, создана 20.09.2024 года\r\n");
            Console.WriteLine($"{userName}, {menu}");
            break;

        case "/echo":
            Console.WriteLine($"\r\n{ userName}, Вы ввели: {input.Replace(commandText[0], "").Trim()}\r\n");
            Console.WriteLine($"{userName}, {menu}");
            break;

        case "/exit":
            break;
    }
}
while (!input.Contains("/exit"));

Console.WriteLine($"\r\n{userName}, работа бота завершена. До свидания!");
