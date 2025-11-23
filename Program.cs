using DZ_Lessons.Core.Services;
using DZ_Lessons.DAL;
using DZ_Lessons.Infrastructure.DataAccess;
using DZ_Lessons.TelegramBot;
using Otus.ToDoList.ConsoleBot;

Console.WriteLine("Добро пожаловать! \r\nЗапущено базовое интерактивное меню будущего бота!");

try
{
    var userRepository = new InMemoryUserRepository();
    var toDoRepository = new InMemoryToDoRepository();

    var userService = new UserService(userRepository);
    var toDoService = new ToDoService(toDoRepository);
    var toDoReportService = new ToDoReportService(toDoService);

    Console.WriteLine("\r\nВведите максимально допустимое количество задач");
    int maxCountTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    toDoService.MaxCountTasks = maxCountTasks;

    Console.WriteLine("Введите максимально допустимую длину задачи");
    int maxLengthTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    toDoService.MaxLengthTasks = maxLengthTasks;

    CancellationTokenSource tokenSource = new CancellationTokenSource();
    var botClient = new ConsoleBotClient();
    var updateHandler = new UpdateHandler(userService, toDoService, toDoReportService, tokenSource.Token);

    MessageEventHandler startEventHandler = (message) =>
    {
        Console.WriteLine($"Началась обработка сообщения '{message}'");
    };

    MessageEventHandler completedEventHandler = (message) =>
    {
        Console.WriteLine($"Закончилась обработка сообщения '{message}'");
    };

    try
    {
        updateHandler.OnHandleUpdateStarted += startEventHandler;
        updateHandler.OnHandleUpdateCompleted += completedEventHandler;

        botClient.StartReceiving(updateHandler, tokenSource.Token);
    }
    finally
    {
        updateHandler.OnHandleUpdateStarted -= startEventHandler;
        updateHandler.OnHandleUpdateCompleted -= completedEventHandler;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла критическая ошибка: Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
}