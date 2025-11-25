using DZ_Lessons.Infrastracture.Interface;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using static DZ_Lessons.DAL.Enum.ToDoItemStateEnum;

namespace DZ_Lessons.Infrastracture
{
    public class UpdateHandler: IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private string _menu;

        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            _menu = "краткая справочная информация о том, как пользоваться программой"
                + $"\r\nДоступные команды:"
                + $"\r\n/start - начать работу с ботом"
                + $"\r\n/help - показать справку"
                + $"\r\n/info - информация о программе"
                + $"\r\n/addtask [текст] - добавить новую задачу"
                + $"\r\n/showtasks - показать активные задачи"
                + $"\r\n/showalltasks - показать все задачи (активные и завершенные)"
                + $"\r\n/removetask [номер] - удалить задачу по номеру"
                + $"\r\n/completetask [ID] - завершить задачу по ID"
                + $"\r\n/exit - выйти из программы";
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                string input = update.Message.Text.Trim();
                string[] commandText = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (commandText.Length == 0) return;

                var telegramUserId = update.Message.From.Id;
                var currentUser = _userService.GetUser(telegramUserId);

                if (currentUser == null && commandText[0] != "/start" && commandText[0] != "/help" && commandText[0] != "/info")
                {
                    botClient.SendMessage(update.Message.Chat, "Сначала выполните команду /start");
                    return;
                }

                switch (commandText[0])
                {
                    case "/start":
                        HandleStartCommand(botClient, update, commandText);
                        break;
                    case "/help":
                        HandleHelpCommand(botClient, update, currentUser);
                        break;
                    case "/info":
                        HandleInfoCommand(botClient, update, currentUser);
                        break;
                    case "/addtask":
                        HandleAddTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/showtasks":
                        HandleShowTasksCommand(botClient, update, currentUser);
                        break;
                    case "/showalltasks":
                        HandleShowAllTasksCommand(botClient, update, currentUser);
                        break;
                    case "/removetask":
                        HandleRemoveTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/completetask":
                        HandleCompleteTaskCommand(botClient, update, commandText, currentUser);
                        break;
                    case "/exit":
                        break;
                    default:
                        botClient.SendMessage(update.Message.Chat, "Неизвестная команда");
                        break;
                }
            }
            catch (Exception ex)
            {
                botClient.SendMessage(update.Message.Chat, $"Ошибка: {ex.Message}");
            }
        }

        private void HandleStartCommand(ITelegramBotClient botClient, Update update, string[] commandText)
        {
            var telegramUserId = update.Message.From.Id;
            var telegramUserName = update.Message.From.Username ?? "Пользователь";

            var currentUser = _userService.GetUser(telegramUserId);
            if (currentUser == null)
            {
                currentUser = _userService.RegisterUser(telegramUserId, telegramUserName);
                botClient.SendMessage(update.Message.Chat, $"Добро пожаловать, {telegramUserName}! Вы успешно зарегистрированы.");
            }

            botClient.SendMessage(update.Message.Chat, $"{currentUser.TelegramUserName}, {_menu}");
        }

        private void HandleHelpCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            string userName = currentUser?.TelegramUserName ?? "Пользователь";
            botClient.SendMessage(update.Message.Chat, $"{userName}, {_menu}");
        }

        private void HandleInfoCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            string info = "Версия программы 1.0, создана 20.11.2025 года";
            string userName = currentUser?.TelegramUserName ?? "Пользователь";
            botClient.SendMessage(update.Message.Chat, $"{info}\r\n{userName}, {_menu}");
        }

        private void HandleAddTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                botClient.SendMessage(update.Message.Chat, "Не указано описание задачи. Использование: /addtask [описание]");
                return;
            }

            string taskName = string.Join(" ", commandText.Skip(1));
            var newTask = _toDoService.Add(currentUser, taskName);
            botClient.SendMessage(update.Message.Chat, $"Задача '{taskName}' добавлена с ID: {newTask.Id}");
        }

        private void HandleShowTasksCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            var activeTasks = _toDoService.GetActiveByUserId(currentUser.UserId);

            if (activeTasks.Count == 0)
            {
                botClient.SendMessage(update.Message.Chat, "У вас нет активных задач. Добавьте задачу по команде /addtask");
                return;
            }

            var tasksList = "Список активных задач:\r\n";
            for (int i = 0; i < activeTasks.Count; i++)
            {
                var task = activeTasks[i];
                tasksList += $"{i}. {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}\r\n";
            }

            botClient.SendMessage(update.Message.Chat, tasksList);
        }

        private void HandleShowAllTasksCommand(ITelegramBotClient botClient, Update update, ToDoUser currentUser)
        {
            var allTasks = _toDoService.GetAllByUserId(currentUser.UserId);

            if (allTasks.Count == 0)
            {
                botClient.SendMessage(update.Message.Chat, "У вас нет задач. Добавьте задачу по команде /addtask");
                return;
            }

            var tasksList = "Список всех задач:\r\n";
            for (int i = 0; i < allTasks.Count; i++)
            {
                var task = allTasks[i];
                string state = task.State == ToDoItemState.Active ? "(Active)" : "(Completed)";
                tasksList += $"{i}. {state} {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}\r\n";
            }

            botClient.SendMessage(update.Message.Chat, tasksList);
        }

        private void HandleRemoveTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                botClient.SendMessage(update.Message.Chat, "Не указан номер задачи. Использование: /removetask [номер]");
                return;
            }

            if (!int.TryParse(commandText[1], out int taskIndex))
            {
                botClient.SendMessage(update.Message.Chat, "Неверный формат номера задачи");
                return;
            }

            var allTasks = _toDoService.GetAllByUserId(currentUser.UserId);
            if (taskIndex < 0 || taskIndex >= allTasks.Count)
            {
                botClient.SendMessage(update.Message.Chat, $"Задача с номером {taskIndex} не найдена");
                return;
            }

            var taskToRemove = allTasks[taskIndex];
            _toDoService.Delete(taskToRemove.Id);
            botClient.SendMessage(update.Message.Chat, $"Задача '{taskToRemove.Name}' удалена");
        }

        private void HandleCompleteTaskCommand(ITelegramBotClient botClient, Update update, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                botClient.SendMessage(update.Message.Chat, "Не указан ID задачи. Использование: /completetask [ID]");
                return;
            }

            if (!Guid.TryParse(commandText[1], out Guid taskId))
            {
                botClient.SendMessage(update.Message.Chat, "Неверный формат ID задачи");
                return;
            }

            var allTasks = _toDoService.GetAllByUserId(currentUser.UserId);
            var task = allTasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                botClient.SendMessage(update.Message.Chat, $"Задача с ID {taskId} не найдена");
                return;
            }

            if (task.State == ToDoItemState.Completed)
            {
                botClient.SendMessage(update.Message.Chat, $"Задача '{task.Name}' уже завершена");
                return;
            }

            _toDoService.MarkCompleted(taskId);
            botClient.SendMessage(update.Message.Chat, $"Задача '{task.Name}' завершена");
        }
    }
}
