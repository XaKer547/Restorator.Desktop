namespace Restorator.Desktop.ExceptionHandlers.Abstract
{
    public interface IExceptionHandler<in TException> where TException : Exception
    {
        public Task HandleAsync(TException exception);
    }
}
