namespace DZ_Lesson_5.DAL.Exceptions
{
    public class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task)
            : base($"Задача {task} уже существует")
        {
        }
    }
}
