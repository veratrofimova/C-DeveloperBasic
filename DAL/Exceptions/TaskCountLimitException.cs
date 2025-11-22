namespace DZ_Lesson_5.DAL.Exceptions
{
    public class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit)
            : base($"Превышено максимальное количество задач равное {taskCountLimit}")
        {
        }
    }
}
