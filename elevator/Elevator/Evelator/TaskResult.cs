namespace Elevator
{
    public class TaskResult
    {
        public readonly bool HasError;
        public readonly string ErrorMessage;
        protected TaskResult(bool hasError, string errorMessage)
        {
            HasError = hasError;
            ErrorMessage = errorMessage;
        }

        public static TaskResult Error(string errorMesasge)
        {
            return new TaskResult(true, errorMesasge);
        }

        public static TaskResult Success()
        {
            return new TaskResult(false, null);
        }
    }
}
