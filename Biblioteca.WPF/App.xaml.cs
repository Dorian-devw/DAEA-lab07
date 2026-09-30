using Biblioteca.Datos.Repositorios;
using Biblioteca.Negocio;
using Biblioteca.Negocio.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Biblioteca.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Configurar el idioma de HandyControl a Español
            HandyControl.Tools.ConfigHelper.Instance.SetLang("es");

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Repositorios
            services.AddTransient<ILibroRepositorio, LibroRepositorio>();
            services.AddTransient<ISocioRepositorio, SocioRepositorio>();
            services.AddTransient<IPrestamoRepositorio, PrestamoRepositorio>();
            
            // Negocio
            services.AddTransient<LibroNegocio>();
            services.AddTransient<SocioNegocio>();
            services.AddTransient<PrestamoNegocio>();

            // Vistas (Windows)
            services.AddTransient<MainWindow>();
        }
    }
}
