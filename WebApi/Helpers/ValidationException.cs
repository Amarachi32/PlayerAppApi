namespace WebApi.Helpers
{
    public class ValidationException : Exception
    {
        public string Field { get; }
        public string InvalidValue { get; }

        public ValidationException(string field, string invalidValue)
        {
            Field = field;
            InvalidValue = invalidValue;
        }
    }

}
