Console.WriteLine("Добро пожаловать! \r\nЗапущено базовое интерактивное меню будущего бота!");
string menu = $"Введите команду: /start, /help, /info, /exit";
menu += $"\r\nКоманды для работы с задачами: /addtask, /showtasks, /removetask";

Console.WriteLine($"\r\n{menu}");

string userName = "";
string input = "";
List<string> tasks = new List<string>();

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

        case "/addtask":
            Console.WriteLine("\r\nДобавьте описание новой задачи: ");
            string newTask = Console.ReadLine();
            tasks.Add(newTask);

            Console.WriteLine($"Задача {newTask} добавлена");
            break;

        case "/showtasks":
            if (tasks.Count() == 0)
                Console.WriteLine("\r\n{userName}, Вы еще не добавляли задачи. Добавьте задачу по клманду /addtask");
            else
            {
                Console.WriteLine("Список задач:");
                tasks.ForEach(x => Console.WriteLine(x));
            }
            break;

        case "/removetask":            
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
                            Console.WriteLine($"Номер задачи {deleteTaskPos} не найден в списке задач");

                        string deleteTaskVal = tasks[deleteTaskPos];
                        tasks.Remove(deleteTaskVal);
                        Console.WriteLine($"Задача {deleteTaskVal} удалена");                        
                    }
                    else
                        Console.WriteLine($"Номер задачи {deleteTaskPos} задан не корректно");
                }
            break;
        case "/exit":
            break;
    }
}
while (!input.Contains("/exit"));

Console.WriteLine($"\r\n{userName}, работа бота завершена. До свидания!");
