using DZ_Lesson_5.Exceptions;

namespace DZ_Lesson_5
{
    public class Commands
    {
        public int MaxCountTasks { get; set; }
        public int MaxLengthTasks { get; set; }
        public string UserName { get; set; } = "";

        public void CommandStart(string menu)
        {
            Console.WriteLine("\r\nКак к Вам обращаться? Введите Ваше имя:");
            UserName = Console.ReadLine();

            Console.WriteLine($"\r\nВам теперь доступна команда /echo Текст\r\n");

            menu += " /echo Текст";
            Console.WriteLine($"{UserName}, {menu}");
        }

        public void CommandHelp(string menu)
        {
            Console.WriteLine($"\r\nКраткая справочная информация о том, как пользоваться программой\r\n");
            Console.WriteLine($"{UserName}, {menu}");
        }

        public void CommandInfo(string menu)
        {
            Console.WriteLine($"\r\nВерсия программы 1.0, создана 20.09.2024 года\r\n");
            Console.WriteLine($"{UserName}, {menu}");
        }

        public void CommandEcho(string menu, string input, string[] commandText)
        {
            Console.WriteLine($"\r\n{UserName}, Вы ввели: {input.Replace(commandText[0], "").Trim()}\r\n");
            Console.WriteLine($"{UserName}, {menu}");
        }

        public void CommandAddtask(List<string> tasks)
        {
            if (tasks.Count >= MaxCountTasks)
                throw new TaskCountLimitException(MaxCountTasks);

            Console.WriteLine("\r\nДобавьте описание новой задачи: ");
            string newTask = Console.ReadLine();

            (new ParseAndValidate()).ValidateString(newTask);
            if (newTask.Length > MaxLengthTasks)
                throw new TaskLengthLimitException(newTask.Length, MaxLengthTasks);
            if (tasks.Contains(newTask))
                throw new DuplicateTaskException(newTask);

            tasks.Add(newTask);

            Console.WriteLine($"Задача {newTask} добавлена");
        }

        public void CommandShowtasks(List<string> tasks)
        {
            if (tasks.Count() == 0)
                Console.WriteLine("\r\n{UserName}, Вы еще не добавляли задачи. Добавьте задачу по клманду /addtask");
            else
            {
                Console.WriteLine("Список задач:");
                tasks.ForEach(x => Console.WriteLine(x));
            }
        }

        public void CommandRemovetask(List<string> tasks)
        {
            if (tasks.Count() == 0)
                Console.WriteLine("Cписок задач пуст");
            else
            {
                Console.WriteLine("Список задач:");
                int i = 0;
                tasks.ForEach(x => { Console.WriteLine($"{i}. {x}"); i += 1; });

                Console.WriteLine("\r\nВведите номер задачи для удаления: ");
                int deleteTaskPos;
                bool isDelete = int.TryParse(Console.ReadLine(), out deleteTaskPos);

                if (isDelete)
                {
                    if (deleteTaskPos < 0 || deleteTaskPos > tasks.Count() - 1)
                    {
                        Console.WriteLine($"Номер задачи {deleteTaskPos} не найден в списке задач");
                        return;
                    }

                    string deleteTaskVal = tasks[deleteTaskPos];
                    tasks.Remove(deleteTaskVal);
                    Console.WriteLine($"Задача {deleteTaskVal} удалена");
                }
                else
                    Console.WriteLine($"Номер задачи {deleteTaskPos} задан не корректно");
            }
        }
    }
}
