namespace DZ_Lessons.Core.Exceptions
{
    public class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task)
            : base($"Задача {task} уже существует")
        {
        }
    }
}
