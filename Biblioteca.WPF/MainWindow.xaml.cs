using System.Windows;
using Biblioteca.Negocio;

namespace Biblioteca.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(LibroNegocio libroNegocio, SocioNegocio socioNegocio, PrestamoNegocio prestamoNegocio)
        {
            InitializeComponent();
            
            // Inyectamos las vistas dentro de los tabs
            TabLibros.Content = new Views.LibrosView(libroNegocio);
            TabSocios.Content = new Views.SociosView(socioNegocio);
            TabPrestamos.Content = new Views.PrestamosView(prestamoNegocio);
            TabDevoluciones.Content = new Views.DevolucionesView(prestamoNegocio);
            TabReportes.Content = new Views.ReportesView(prestamoNegocio);
        }
    }
}