namespace DZ_Lesson_5
{
    public class ParseAndValidate
    {
        public int ParseAndValidateInt(string? str, int min, int max)
        {
            try
            {
                ValidateString(str);

                int num = 0;
                bool success = int.TryParse(str, out num);

                if (!success)
                    throw new ArgumentException("Введенное значение не является числом");
                
                if (num < min || num > max)                
                    throw new ArgumentException($"Введенное число должно быть больше {min}");

                return num;
            }
            catch (ArgumentException ex)
            {
                throw;
            }
        }

        public void ValidateString(string? str)
        {
            if (string.IsNullOrEmpty(str.Trim()))
                throw new ArgumentException("Строка не может быть пустой или состоять только из пробелов");
        }
    }
}
