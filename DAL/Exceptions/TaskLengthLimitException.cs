namespace DZ_Lessons.DAL.Exceptions
{
    public class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit)
            : base($"Длина задачи {taskLength} превышает максимально допустимое значение {taskLengthLimit}")
        {
        }
    }
}