using DZ_Lesson_5;
using DZ_Lesson_5.Exceptions;

Console.WriteLine("Добро пожаловать! \r\nЗапущено базовое интерактивное меню будущего бота!");
string menu = $"Введите команду: /start, /help, /info, /exit";
menu += $"\r\nКоманды для работы с задачами: /addtask, /showtasks, /removetask";
Console.WriteLine($"\r\n{menu}");

string userName = "";
string input = "";
List<string> tasks = new List<string>();

try
{
    Commands commands = new Commands();

    Console.WriteLine("Введите максимально допустимое количество задач");
    int maxCountTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    commands.MaxCountTasks = maxCountTasks;

    Console.WriteLine("Введите максимально допустимую длину задачи");
    int maxLengthTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    commands.MaxLengthTasks = maxLengthTasks;

    do
    {
        try
        {
            input = Console.ReadLine().Trim() ?? "";
            string[] commandText = input.Split(new char[] { ' ' });
            if (commandText.Length == 0) continue;

            switch (commandText[0])
            {
                case "/start":
                    commands.CommandStart(menu);
                    commands.UserName = userName;
                    break;
                case "/help":
                    commands.CommandHelp(menu);
                    break;
                case "/info":
                    commands.CommandInfo(menu);
                    break;
                case "/echo":
                    commands.CommandEcho(menu, input, commandText);
                    break;
                case "/addtask":
                    commands.CommandAddtask(tasks);
                    break;
                case "/showtasks":
                    commands.CommandShowtasks(tasks);
                    break;
                case "/removetask":
                    commands.CommandRemovetask(tasks);
                    break;
                case "/exit":
                    break;
                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (TaskCountLimitException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (TaskLengthLimitException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (DuplicateTaskException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла непредвиденная ошибка: Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
        }
    }
    while (!input.Contains("/exit"));

    Console.WriteLine($"\r\n{userName}, работа бота завершена. До свидания!");
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла критическая ошибка: Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
}
