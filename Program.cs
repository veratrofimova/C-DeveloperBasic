using DZ_Lesson_5;
using DZ_Lesson_5.DAL;
using DZ_Lesson_5.DAL.Exceptions;

Console.WriteLine("Добро пожаловать! \r\nЗапущено базовое интерактивное меню будущего бота!");
string menu = $"Доступные команды: /start, /exit";

Console.WriteLine($"\r\n{menu}");

ToDoUser currentUser = null;
string input = "";
List<ToDoItem> tasks = new List<ToDoItem>();

try
{
    Commands commands = new Commands();

    Console.WriteLine("\r\nВведите максимально допустимое количество задач");
    int maxCountTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    commands.MaxCountTasks = maxCountTasks;

    Console.WriteLine("Введите максимально допустимую длину задачи");
    int maxLengthTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    commands.MaxLengthTasks = maxLengthTasks;

    do
    {
        try
        {
            Console.WriteLine($"\r\nВведите команду");
            input = Console.ReadLine().Trim() ?? "";
            string[] commandText = input.Split(new char[] { ' ' });
            if (commandText.Length == 0) continue;

            if (currentUser == null && commandText[0] != "/start")
            {
                Console.WriteLine("Сначала выполните команду /start");
                continue;
            }

            switch (commandText[0])
            {
                case "/start":
                    commands.CommandStart(menu, ref currentUser);
                    break;
                case "/help":
                    commands.CommandHelp(menu, currentUser);
                    break;
                case "/info":
                    commands.CommandInfo(menu, currentUser);
                    break;
                case "/echo":
                    commands.CommandEcho(menu, input, commandText, currentUser);
                    break;
                case "/addtask":
                    commands.CommandAddtask(tasks, currentUser);
                    break;
                case "/showtasks":
                    commands.CommandShowtasks(tasks, currentUser);
                    break;
                case "/showalltasks":
                    commands.CommandShowAllTasks(tasks, currentUser);
                    break;
                case "/removetask":
                    commands.CommandRemovetask(tasks, currentUser);
                    break;
                case "/completetask":
                    commands.CommandCompleteTask(tasks, commandText, currentUser);
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

    Console.WriteLine($"\r\n{(currentUser?.TelegramUserName ?? "Пользователь")}, работа бота завершена. До свидания!");
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла критическая ошибка: Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
}
