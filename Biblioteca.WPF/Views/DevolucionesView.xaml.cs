using Biblioteca.Negocio;
using System;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Biblioteca.WPF.Views
{
    public partial class DevolucionesView : UserControl
    {
        private readonly PrestamoNegocio _prestamoNegocio;

        public DevolucionesView(PrestamoNegocio prestamoNegocio)
        {
            InitializeComponent();
            _prestamoNegocio = prestamoNegocio;
        }

        private async void BtnDevolver_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtPrestamoId.Text, out int pId) && int.TryParse(txtLibroId.Text, out int lId))
            {
                try
                {
                    // Nota: Para simular fechas de limite vencidas, en un entorno real la fecha limite la buscaria
                    // de la base de datos antes de mandar a calcular la multa. 
                    // Por simplicidad en este lab, usaremos un valor aproximado o requeriremos otro metodo para obtener el prestamo.
                    DateTime fechaLimiteFicticia = DateTime.Now.AddDays(-2); // Simular 2 dias de retraso

                    decimal multa = await _prestamoNegocio.RegistrarDevolucionAsync(pId, lId, fechaLimiteFicticia);
                    
                    if (multa > 0)
                    {
                        lblMensajeMulta.Text = $"Multa generada: S/ {multa:0.00} por retraso.";
                        MessageBox.Warning($"El libro fue devuelto, pero tiene una multa de S/ {multa:0.00}.", "Devolución con Retraso");
                    }
                    else
                    {
                        lblMensajeMulta.Text = "Devolución a tiempo.";
                        MessageBox.Success("Libro devuelto exitosamente, sin multas.", "Éxito");
                    }
                    
                    txtPrestamoId.Clear();
                    txtLibroId.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Error(ex.Message, "Error");
                }
            }
            else
            {
                MessageBox.Warning("Valores inválidos.", "Aviso");
            }
        }
    }
}
