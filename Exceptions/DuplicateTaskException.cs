namespace DZ_Lesson_5.Exceptions
{
    public class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task)
            : base($"Задача {task} уже существует")
        {
        }    
    }
}
