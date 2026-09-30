using Biblioteca.Negocio;
using Biblioteca.Entidades;
using System;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
using System.Data;

namespace Biblioteca.WPF.Views
{
    public partial class ReportesView : UserControl
    {
        private readonly PrestamoNegocio _prestamoNegocio;

        // Modificamos el constructor para recibir dependencias (no te preocupes, MainWindow no necesita cambio
        // si lo instanciamos pasando null, pero en MainWindow.xaml.cs debemos arreglarlo. 
        // Ya que ReportesView no lo inyectaba, vamos a agregarlo).
        public ReportesView(PrestamoNegocio prestamoNegocio)
        {
            InitializeComponent();
            _prestamoNegocio = prestamoNegocio;
            
            // Asignar fechas por defecto a inicio del mes y fin del mes (o hoy) si no usamos xaml
            dpInicio.SelectedDate = DateTime.Now.AddDays(-30);
            dpFin.SelectedDate = DateTime.Now;
        }

        private async void BtnCargar_Click(object sender, RoutedEventArgs e)
        {
            if (dpInicio.SelectedDate == null || dpFin.SelectedDate == null) return;
            
            try
            {
                var resultados = await _prestamoNegocio.ObtenerReportePrestamosAsync(dpInicio.SelectedDate.Value, dpFin.SelectedDate.Value);
                dgReporte.ItemsSource = resultados;
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message, "Error");
            }
        }
    }
}
