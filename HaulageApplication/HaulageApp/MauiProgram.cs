using Microsoft.Extensions.Logging;
using System.Reflection;
using HaulageApp.Data;
using HaulageApp.ViewModels;
using HaulageApp.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HaulageApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "HaulageApp.appsettings.json"; // Ensure this matches the namespace and file location

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new InvalidOperationException($"Failed to load configuration file '{resourceName}'.");
            }

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);

            var connectionString = builder.Configuration.GetConnectionString("LocalConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'LocalConnection' is missing or empty.");
            }

            Console.WriteLine($"Loaded connection string: {connectionString}");

            builder.Services.AddDbContext<HaulageDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }));

            builder.Services.AddSingleton<AllNotesViewModel>();
            builder.Services.AddTransient<NoteViewModel>();

            builder.Services.AddSingleton<AllNotesPage>();
            builder.Services.AddTransient<NotePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
