using Restorator.Desktop.ExceptionHandlers.Abstract;
using Restorator.Desktop.ViewModels;
using Restorator.Domain.Services;
using System.Net.Http;
using Wpf.Ui;
using Wpf.Ui.Extensions;
using INavigationService = Restorator.Desktop.Services.INavigationService;

namespace Restorator.Desktop.ExceptionHandlers
{
    public class UnauthorizedExceptionHandler : ExceptionHandlerBase<HttpRequestException>
    {
        private readonly INavigationService _navigationService;
        private readonly ISessionManager _sessionManager;
        private readonly ISnackbarService _snackbarService;
        public UnauthorizedExceptionHandler(INavigationService navigationService,
                                            ISessionManager sessionManager,
                                            ISnackbarService snackbarService)
        {
            _navigationService = navigationService;
            _sessionManager = sessionManager;
            _snackbarService = snackbarService;
        }

        public override async Task HandleAsync(HttpRequestException exception)
        {
            _sessionManager.RemoveSession();

            await _navigationService.NavigateAsync<AuthenticationViewModel>();

            _snackbarService.Show("Кажется ваша сессия кончилась", "Пора напомнить этому миру кто вы", Wpf.Ui.Controls.ControlAppearance.Danger);
        }
        protected override bool CanBeHandled(HttpRequestException exception)
        {
            return exception.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }
    }
}
