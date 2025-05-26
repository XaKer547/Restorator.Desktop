using System.Windows.Threading;

namespace Restorator.Desktop.ExceptionHandlers.Abstract
{
    public abstract class ExceptionHandlerBase<TException> : ExceptionHandlerBase, IExceptionHandler<TException> where TException : Exception
    {
        public abstract Task HandleAsync(TException exception);
        protected virtual bool CanBeHandled(TException exception) => true;
        public override async Task<DispatcherUnhandledExceptionEventArgs> HandleAsync(DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is TException exception && CanBeHandled(exception))
            {
                await HandleAsync(exception);

                e.Handled = true;
            }

            return e;
        }
    }
    public abstract class ExceptionHandlerBase
    {
        public abstract Task<DispatcherUnhandledExceptionEventArgs> HandleAsync(DispatcherUnhandledExceptionEventArgs e);
    }
}