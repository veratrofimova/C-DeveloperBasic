using DZ_Lesson_5.DAL;
using DZ_Lesson_5.DAL.Exceptions;
using static DZ_Lesson_5.DAL.Enum.ToDoItemStateEnum;

namespace DZ_Lesson_5
{
    public class Commands
    {
        public int MaxCountTasks { get; set; }
        public int MaxLengthTasks { get; set; }

        public void CommandStart(string menu, ref ToDoUser currentUser)
        {
            Console.WriteLine("\r\nКак к Вам обращаться? Введите Ваше имя:");
            string userName = Console.ReadLine();
            currentUser = new ToDoUser(userName);

            Console.WriteLine($"\r\nВам теперь доступна команда /echo Текст\r\n");

            menu = "краткая справочная информация о том, как пользоваться программой"
                + $"\r\nДоступные команды:"
                + $"\r\n/start - начать работу с ботом"
                + $"\r\n/help - показать справку"
                + $"\r\n/info - информация о программе"
                + $"\r\n/echo [текст] - повторить введенный текст"
                + $"\r\n/addtask - добавить новую задачу"
                + $"\r\n/showtasks - показать активные задачи"
                + $"\r\n/showalltasks - показать все задачи (активные и завершенные)"
                + $"\r\n/removetask - удалить задачу"
                + $"\r\n/completetask [ID] - завершить задачу по ID"
                + $"\r\n/exit - выйти из программы";
            Console.WriteLine($"{currentUser.TelegramUserName}, {menu}");
        }

        public void CommandHelp(string menu, ToDoUser currentUser)
        {
            Console.WriteLine($"{currentUser.TelegramUserName}, {menu}");
        }

        public void CommandInfo(string menu, ToDoUser currentUser)
        {
            Console.WriteLine($"\r\nВерсия программы 1.0, создана 20.11.2025 года\r\n");
            Console.WriteLine($"{currentUser.TelegramUserName}, {menu}");
        }

        public void CommandEcho(string menu, string input, string[] commandText, ToDoUser currentUser)
        {
            string userName = currentUser.TelegramUserName;
            Console.WriteLine($"\r\n{userName}, Вы ввели: {input.Replace(commandText[0], "").Trim()}\r\n");
            Console.WriteLine($"{userName}, {menu}");
        }

        public void CommandAddtask(List<ToDoItem> tasks, ToDoUser currentUser)
        {
            int activeTasksCount = tasks.Count(t => t.State == ToDoItemState.Active && t.User.UserId == currentUser.UserId);
            if (activeTasksCount >= MaxCountTasks)
                throw new TaskCountLimitException(MaxCountTasks);

            Console.WriteLine("\r\nДобавьте описание новой задачи: ");
            string newTaskName = Console.ReadLine();

            (new ParseAndValidate()).ValidateString(newTaskName);
            if (newTaskName.Length > MaxLengthTasks)
                throw new TaskLengthLimitException(newTaskName.Length, MaxLengthTasks);

            bool hasDuplicate = tasks.Any(t =>
                t.Name == newTaskName &&
                t.State == ToDoItemState.Active &&
                t.User.UserId == currentUser.UserId);

            if (hasDuplicate)
                throw new DuplicateTaskException(newTaskName);

            ToDoItem newTask = new ToDoItem(currentUser, newTaskName);
            tasks.Add(newTask);

            Console.WriteLine($"Задача '{newTaskName}' добавлена с ID: {newTask.Id}");
        }

        public void CommandShowtasks(List<ToDoItem> tasks, ToDoUser currentUser)
        {
            var userActiveTasks = tasks
                .Where(t =>
                    t.User.UserId == currentUser.UserId &&
                    t.State == ToDoItemState.Active)
                .ToList();

            if (userActiveTasks.Count == 0)
                Console.WriteLine($"\r\n{currentUser.TelegramUserName}, Вы еще не добавляли задачи. Добавьте задачу по команде /addtask");
            else
            {
                Console.WriteLine("Список активных задач:");
                foreach (var task in userActiveTasks)
                {
                    Console.WriteLine($"{task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}");
                }
            }
        }

        public void CommandShowAllTasks(List<ToDoItem> tasks, ToDoUser currentUser)
        {
            var userTasks = tasks
                .Where(t => t.User.UserId == currentUser.UserId)
                .ToList();

            if (userTasks.Count == 0)
                Console.WriteLine($"\r\n{currentUser.TelegramUserName}, Вы еще не добавляли задачи. Добавьте задачу по команде /addtask");
            else
            {
                Console.WriteLine("Список всех задач:");
                foreach (var task in userTasks)
                {
                    string state = task.State == ToDoItemState.Active ? "(Active)" : "(Completed)";
                    Console.WriteLine($"{state} {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}");
                }
            }
        }

        public void CommandRemovetask(List<ToDoItem> tasks, ToDoUser currentUser)
        {
            var userTasks = tasks
                .Where(t => t.User.UserId == currentUser.UserId)
                .ToList();

            if (userTasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст");
                return;
            }

            Console.WriteLine("Список задач:");
            int i = 0;
            userTasks.ForEach(task => 
            {
                string state = task.State == ToDoItemState.Active ? "(Active)" : "(Completed)";
                Console.WriteLine($"{i}. {state} {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}");
                i += 1;
            });

            Console.WriteLine("\r\nВведите номер задачи для удаления: ");
            int deleteTaskPos;
            bool isDelete = int.TryParse(Console.ReadLine(), out deleteTaskPos);

            if (!isDelete)
            {
                Console.WriteLine($"Номер задачи {deleteTaskPos} задан не корректно");
                return;
            }

            if (deleteTaskPos < 0 || deleteTaskPos > userTasks.Count - 1)
            {
                Console.WriteLine($"Номер задачи {deleteTaskPos} не найден в списке задач");
                return;
            }

            var taskToRemove = userTasks[deleteTaskPos];
            tasks.Remove(taskToRemove);
            Console.WriteLine($"Задача '{taskToRemove.Name}' удалена");            
        }

        public void CommandCompleteTask(List<ToDoItem> tasks, string[] commandText, ToDoUser currentUser)
        {
            if (commandText.Length < 2)
            {
                Console.WriteLine("Не указан ID задачи. Использование: /completetask [ID]");
                return;
            }

            string taskIdString = commandText[1];
            if (!Guid.TryParse(taskIdString, out Guid taskId))
            {
                Console.WriteLine("Неверный формат ID задачи");
                return;
            }

            var task = tasks.FirstOrDefault(t =>
                t.Id == taskId &&
                t.User.UserId == currentUser.UserId);

            if (task == null)
            {
                Console.WriteLine($"Задача с ID {taskId} не найдена");
                return;
            }

            if (task.State == ToDoItemState.Completed)
            {
                Console.WriteLine($"Задача '{task.Name}' уже завершена");
                return;
            }

            task.MarkAsCompleted();
            Console.WriteLine($"Задача '{task.Name}' завершена");
        }
    }
}