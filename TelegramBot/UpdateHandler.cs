using DZ_Lessons.Core.DataAccess;
using DZ_Lessons.Core.Entities;
using DZ_Lessons.Core.Services;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using static DZ_Lessons.Core.Enum.ToDoItemStateEnum;

namespace DZ_Lessons.TelegramBot
{
    public delegate void MessageEventHandler(string message);

    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toReportService;
        private readonly CancellationToken _token;
        private string _menu;

        public event MessageEventHandler OnHandleUpdateStarted;
        public event MessageEventHandler OnHandleUpdateCompleted;

        public UpdateHandler(
            IUserService userService, 
            IToDoService toDoService, 
            IToDoReportService toReportService,
            CancellationToken token)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toReportService = toReportService;
            _token = token;
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            _menu = "краткая справочная информация о том, как пользоваться программой"
                + $"\r\nДоступные команды:"
                + $"\r\n/start - начать работу с ботом"
                + $"\r\n/help - показать справку"
                + $"\r\n/info - информация о программе"
                + $"\r\n/addtask [описание] - добавить новую задачу"
                + $"\r\n/showtasks - показать активные задачи"
                + $"\r\n/showalltasks - показать все задачи (активные и завершенные)"
                + $"\r\n/report - статистика по задачам"
                + $"\r\n/find [имя] - поиск задач"
                + $"\r\n/removetask [номер] - удалить задачу по номеру"
                + $"\r\n/completetask [ID] - завершить задачу по ID"
                + $"\r\n/exit - выйти из программы";
        }

        private async Task HandleStartCommand(ITelegramBotClient botClient, Update update, string[] commandText)
        {
            var telegramUserId = update.Message.From.Id;
            var telegramUserName = update.Message.From.Username ?? "Пользователь";

            var currentUser = await _userService.GetUser(telegramUserId);
            if (currentUser == null)
            {
                currentUser = await _userService.RegisterUser(telegramUserId, telegramUserName, _token);
               await botClient.SendMessage(update.Message.Chat, $"Добро пожаловать, {telegramUserName}! Вы успешно зарегистрированы.", _token);
            }

            await botClient.SendMessage(update.Message.Chat, $"{telegramUserName}, {_menu}", _token);
        }

        private async Task HandleHelpCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            string userName = currentUser?.TelegramUserName ?? "Пользователь";
            await botClient.SendMessage(update.Message.Chat, $"{userName}, {_menu}", _token);
        }

        private async Task HandleInfoCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            string info = "Версия программы 1.0, создана 20.11.2025 года";
            string userName = currentUser?.TelegramUserName ?? "Пользователь";
            await botClient.SendMessage(update.Message.Chat, $"{info}\r\n{userName}, {_menu}", _token);
        }

        private async Task HandleAddTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                await botClient.SendMessage(update.Message.Chat, "Не указано описание задачи. Использование: /addtask [описание]", _token);
                return;
            }

            string taskName = string.Join(" ", commandText.Skip(1));
            var newTask = await _toDoService.Add(currentUser, taskName);
            await botClient.SendMessage(update.Message.Chat, $"Задача '{taskName}' добавлена с ID: {newTask.Id}", _token);
        }

        private async Task HandleShowTasksCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            var activeTasks = await _toDoService.GetActiveByUserId(currentUser.UserId);

            if (activeTasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, "У вас нет активных задач. Добавьте задачу по команде /addtask", _token);
                return;
            }

            var tasksList = "Список активных задач:\r\n";
            for (int i = 0; i < activeTasks.Count; i++)
            {
                var task = activeTasks[i];
                tasksList += $"{i}. {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}\r\n";
            }

            await botClient.SendMessage(update.Message.Chat, tasksList, _token);
        }

        private async Task HandleShowAllTasksCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            var allTasks = await _toDoService.GetAllByUserId(currentUser.UserId);

            if (allTasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, "У вас нет задач. Добавьте задачу по команде /addtask", _token);
                return;
            }

            var tasksList = "Список всех задач:\r\n";
            for (int i = 0; i < allTasks.Count; i++)
            {
                var task = allTasks[i];
                string state = task.State == ToDoItemState.Active ? "(Active)" : "(Completed)";
                tasksList += $"{i}. {state} {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}\r\n";
            }

            await botClient.SendMessage(update.Message.Chat, tasksList, _token);
        }

        private async Task HandleRemoveTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                await botClient.SendMessage(update.Message.Chat, "Не указан номер задачи. Использование: /removetask [номер]", _token);
                return;
            }

            if (!int.TryParse(commandText[1], out int taskIndex))
            {
                await botClient.SendMessage(update.Message.Chat, "Неверный формат номера задачи", _token);
                return;
            }

            var allTasks = await _toDoService.GetAllByUserId(currentUser.UserId);
            if (taskIndex < 0 || taskIndex >= allTasks.Count)
            {
                await botClient.SendMessage(update.Message.Chat, $"Задача с номером {taskIndex} не найдена", _token);
                return;
            }

            var taskToRemove = allTasks[taskIndex];
            await _toDoService.Delete(taskToRemove.Id);
            await botClient.SendMessage(update.Message.Chat, $"Задача '{taskToRemove.Name}' удалена", _token);
        }

        private async Task HandleCompleteTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                await botClient.SendMessage(update.Message.Chat, "Не указан ID задачи. Использование: /completetask [ID]", _token);
                return;
            }

            if (!Guid.TryParse(commandText[1], out Guid taskId))
            {
                await botClient.SendMessage(update.Message.Chat, "Неверный формат ID задачи", _token);
                return;
            }

            var allTasks = await _toDoService.GetAllByUserId(currentUser.UserId);
            var task = allTasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Задача с ID {taskId} не найдена", _token);
                return;
            }

            if (task.State == ToDoItemState.Completed)
            {
                await botClient.SendMessage(update.Message.Chat, $"Задача '{task.Name}' уже завершена", _token);
                return;
            }

            await _toDoService.MarkCompleted(taskId);
            await botClient.SendMessage(update.Message.Chat, $"Задача '{task.Name}' завершена", _token);
        }

        private async Task HandleReportTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            var stats = await _toReportService.GetUserStats(currentUser.UserId);

            var statistics = $"Статистика по задачам на {stats.generatedAt}. " +
                $"Всего: {stats.total}; Завершенных: {stats.completed}; Активных: {stats.active}";

            await botClient.SendMessage(update.Message.Chat, statistics, _token);
        }

        private async Task HandleFindTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                await botClient.SendMessage(update.Message.Chat, "Не указано условие поиска. Повторите команду и добавьте условие поиска", _token);
                return;
            }

            string findText = string.Join(" ", commandText.Skip(1));

            var tasks = await _toDoService.Find(currentUser, findText);

            if (tasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, "Задачи не найдены. Измените поиск", _token);
                return;
            }

            var tasksList = $"Задачи, которые начинаются на \"{findText}\":\r\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                tasksList += $"{i}. {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}\r\n";
            }

            await botClient.SendMessage(update.Message.Chat, tasksList, _token);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            string input = update.Message.Text.Trim();

            try
            {
                OnHandleUpdateStarted?.Invoke(input);

                string[] commandText = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (commandText.Length == 0) return;

                var telegramUserId = update.Message.From.Id;
                var currentUser = await _userService.GetUser(telegramUserId);

                if (currentUser == null && commandText[0] != "/start" && commandText[0] != "/help" && commandText[0] != "/info")
                {
                    await botClient.SendMessage(update.Message.Chat, "Сначала выполните команду /start", _token);
                    return;
                }

                switch (commandText[0])
                {
                    case "/start":
                        await HandleStartCommand(botClient, update, commandText);
                        break;
                    case "/help":
                        await HandleHelpCommand(botClient, update, currentUser);
                        break;
                    case "/info":
                        await HandleInfoCommand(botClient, update, currentUser);
                        break;
                    case "/addtask":
                        await HandleAddTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/showtasks":
                        await HandleShowTasksCommand(botClient, update, currentUser);
                        break;
                    case "/showalltasks":
                        await HandleShowAllTasksCommand(botClient, update, currentUser);
                        break;
                    case "/removetask":
                        await HandleRemoveTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/completetask":
                        await HandleCompleteTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/report":
                        await HandleReportTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/find":
                        await HandleFindTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/exit":
                        break;
                    default:
                        await botClient.SendMessage(update.Message.Chat, "Неизвестная команда", _token);
                        break;
                }
            }
            catch (Exception ex)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ошибка: {ex.Message}", _token);
            }
            finally
            {
                OnHandleUpdateCompleted?.Invoke(input);
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            await Task.CompletedTask;
        }
    }
}
