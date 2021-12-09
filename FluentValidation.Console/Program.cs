using FluentValidation.Console;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace FluentValidation
{
    public static class Program
    {
        private const string enUSCulture = "en-US";
        private const string trTRCulture = "tr-TR";

        private static string DefaultCulture => trTRCulture;
        internal static IServiceProvider ServiceProvider;

        public static void Main(string[] args)
        {
            configureCulture();
            ConfigureDependency();
            var student = new Student
            {
                Id = 0,
            };
            var validator = ServiceProvider.GetRequiredService<IValidator<Student>>();
            var validationResult = validator.Validate(student);
            PrintError(validationResult);
            System.Console.ReadLine();
        }
        private static void configureCulture()
        {
            var culture = new CultureInfo(DefaultCulture);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
        private static void ConfigureDependency()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<RequestLocalizationOptions>(options =>
            {
                options.ApplyCurrentCultureToResponseHeaders = true;

                var supportedCultures = new[] {
                    new CultureInfo(enUSCulture),
                    new CultureInfo(trTRCulture),

                };
                options.DefaultRequestCulture = new RequestCulture(culture: DefaultCulture, uiCulture: DefaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                {
                    return new ProviderCultureResult(DefaultCulture.Substring(0, 2));
                }));
            });
            serviceCollection.AddLogging();
            serviceCollection.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            serviceCollection.AddTransient<IValidator<Student>, StudentValidator>();
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
        private static void PrintError(ValidationResult result)
        {
            if(!result.Errors?.Any() ?? false)
                return;

            System.Console.WriteLine("### Hata Mesajları ####");
            foreach(var error in result.Errors.Select((t, i) => new { index = i, item = t }))
            {
                System.Console.WriteLine($"{error.index} - Message:{error.item.ErrorMessage}, PropertyName:{error.item.PropertyName}");
            }
        }
    }
}
