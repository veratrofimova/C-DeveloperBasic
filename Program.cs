using DZ_Lessons.DAL;
using DZ_Lessons.Infrastracture;
using Otus.ToDoList.ConsoleBot;

Console.WriteLine("Добро пожаловать! \r\nЗапущено базовое интерактивное меню будущего бота!");

try
{
    var userService = new UserService();
    var toDoService = new ToDoService();

    Console.WriteLine("\r\nВведите максимально допустимое количество задач");
    int maxCountTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    toDoService.MaxCountTasks = maxCountTasks;

    Console.WriteLine("Введите максимально допустимую длину задачи");
    int maxLengthTasks = (new ParseAndValidate()).ParseAndValidateInt(Console.ReadLine(), 1, 100);
    toDoService.MaxLengthTasks = maxLengthTasks;

    var botClient = new ConsoleBotClient();
    var updateHandler = new UpdateHandler(userService, toDoService);

    botClient.StartReceiving(updateHandler);
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла критическая ошибка: Type: {ex.GetType().Name}, Message: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
}
