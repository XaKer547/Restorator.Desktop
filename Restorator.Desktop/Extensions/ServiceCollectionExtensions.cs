using Microsoft.Extensions.DependencyInjection;
using Restorator.Application.Client.Services;
using Restorator.Desktop.Controls;
using Restorator.Desktop.ExceptionHandlers;
using Restorator.Desktop.ExceptionHandlers.Abstract;
using Restorator.Desktop.Infrastructure;
using Restorator.Desktop.Services;
using Restorator.Desktop.Session;
using Restorator.Desktop.ViewModels;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Desktop.Views.Pages;
using Restorator.Desktop.Views.Windows;
using Restorator.Domain.Services;
using System.Net.Http;
using System.Reflection;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.DependencyInjection;

namespace Restorator.Desktop.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Configure(this IServiceCollection services)
        {
            return services.ConfigureServices()
                           .ConfigureHandlers()
                           .ConfigureApiClients()
                           .ConfigureViews();
        }

        public static IServiceCollection ConfigureHandlers(this IServiceCollection services)
        {
            services.AddKeyedScoped<ExceptionHandlerBase, UnauthorizedExceptionHandler>(typeof(HttpRequestException));

            return services;
        }
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();
            services.AddSingleton<INavigationViewPageProvider, DependencyInjectionNavigationViewPageProvider>();
            services.AddSingleton<Services.INavigationService, Services.NavigationService>();
            services.AddSingleton<Wpf.Ui.INavigationService, Wpf.Ui.NavigationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<ISessionManager, SessionManager>();
            services.AddSingleton<IUserManager, UserManager>();

            return services;
        }
        public static IServiceCollection ConfigureApiClients(this IServiceCollection services)
        {
            Action<IServiceProvider, HttpClient> configureClient = (provider, client) =>
            {
                client.BaseAddress = new Uri($"https://localhost:8862");

                var manager = provider.GetRequiredService<ISessionManager>();

                if (manager.TryGetToken(out var token))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            };

            Func<HttpClientHandler> configureHandler = () =>
            {
                var handler = new HttpClientHandler();
#if DEBUG
                //because of invalid SSL from dev
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
#endif
                return handler;
            };

            services.ConfigureHttpClientDefaults(config =>
            {
                config.ConfigurePrimaryHttpMessageHandler(configureHandler);

                config.ConfigureHttpClient(configureClient);
            });

            services.AddHttpClient<IAccountService, AccountService>();
            services.AddHttpClient<IRestaurantService, RestaurantService>();
            services.AddHttpClient<IReservationService, ReservationService>();
            services.AddHttpClient<ITemplateService, TemplateService>();
            services.AddHttpClient<IReportService, ReportService>();

            return services;
        }
        public static IServiceCollection ConfigureViews(this IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<RestaurantSearchViewModel>();

            var manager = new DataTemplateManager().RegisterDataTemplate<AuthenticationViewModel, AuthenticationPage>()
                                                   .RegisterDataTemplate<SignInViewModel, SignInControl>()
                                                   .RegisterDataTemplate<SignUpViewModel, SignUpControl>()
                                                   .RegisterDataTemplate<AccountRestoreViewModel, AccountRestoreControl>()
                                                   .RegisterDataTemplate<MenuViewModel, MenuPage>()
                                                   .RegisterDataTemplate<RestaurantInfoViewModel, RestaurantInfoPage>()
                                                   .RegisterDataTemplate<RestaurantReservationViewModel, RestraurantReservationPage>()
                                                   .RegisterDataTemplate<UserReservationsViewModel, UserReservationsPage>()
                                                   .RegisterDataTemplate<RestaurantSearchViewModel, RestaurantSearchPage>()
                                                   .RegisterDataTemplate<EditRestaurantViewModel, RestaurantEditorPage>()
                                                   .RegisterDataTemplate<CreateRestaurantViewModel, RestaurantMakerPage>()
                                                   .RegisterDataTemplate<RestaurantReservationsManagementViewModel, ReservsationsManagementPage>()
                                                   .RegisterDataTemplate<RestaurantsVerificationViewModel, RestaurantsVerificationPage>()
                                                   .RegisterDataTemplate<RestaurantVerificationViewModel, RestaurantVerificationPage>()
                                                   .RegisterDataTemplate<RestaurantTemplateGeneratorViewModel, RestaurantTemplateGeneratorPage>()
                                                   .RegisterDataTemplate<RestaurantManagementViewModel, RestaurantManagementPage>();

            manager.SetControlsCulture();

            var assembly = Assembly.GetExecutingAssembly();

            services.AddTransientFromNamespace("Restorator.Desktop.ViewModels", assembly);
            services.AddTransientFromNamespace("Restorator.Desktop.Views", assembly);

            var application = System.Windows.Application.Current;

            application.Startup += (assemb, args) => manager.InitilizeTemplates(application.Resources);

            services.AddSingleton(manager);

            return services;
        }
        public static IServiceCollection AddTransientFromNamespace(this IServiceCollection services, string namespaceName, params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> types = assembly.GetTypes()
                                                  .Where(x => x.IsClass && (x.Namespace?.StartsWith(namespaceName, StringComparison.InvariantCultureIgnoreCase) ?? false));

                foreach (var type in types.Where(type => services.All(x => x.ServiceType != type)))
                {
                    if (type == typeof(ViewModelBase))
                        continue;

                    _ = services.AddTransient(type);
                }
            }

            return services;
        }
    }
}
