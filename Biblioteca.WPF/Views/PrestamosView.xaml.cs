using Biblioteca.Entidades;
using Biblioteca.Negocio;
using Biblioteca.Negocio.Excepciones;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Biblioteca.WPF.Views
{
    public partial class PrestamosView : UserControl
    {
        private readonly PrestamoNegocio _prestamoNegocio;
        private ObservableCollection<int> _librosLista = new ObservableCollection<int>();

        public PrestamosView(PrestamoNegocio prestamoNegocio)
        {
            InitializeComponent();
            _prestamoNegocio = prestamoNegocio;
            dgLibrosAPrestar.ItemsSource = _librosLista;
        }

        private void BtnAgregarLibro_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtLibroId.Text, out int libroId))
            {
                if (!_librosLista.Contains(libroId))
                {
                    _librosLista.Add(libroId);
                }
                txtLibroId.Clear();
            }
            else
            {
                MessageBox.Warning("Ingrese un ID de libro válido.", "Aviso");
            }
        }

        private async void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (_librosLista.Count == 0)
            {
                MessageBox.Warning("Agregue al menos un libro.", "Aviso");
                return;
            }

            if (!int.TryParse(txtSocioId.Text, out int socioId))
            {
                MessageBox.Warning("ID de socio inválido.", "Aviso");
                return;
            }

            try
            {
                var prestamo = new Prestamo
                {
                    SocioId = socioId,
                    FechaPrestamo = DateTime.Now,
                    FechaLimite = DateTime.Now.AddDays(7), // Prestamo de 7 dias
                    Estado = "Pendiente"
                };

                var lista = new List<int>(_librosLista);
                await _prestamoNegocio.RegistrarPrestamoAsync(prestamo, lista);
                
                MessageBox.Success("Préstamo registrado correctamente. (Transacción exitosa)", "Éxito");
                _librosLista.Clear();
                txtSocioId.Clear();
            }
            catch (ReglaNegocioException rne)
            {
                MessageBox.Info(rne.Message, "Regla de Negocio (Límite 3 / Sin Stock)");
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message, "Error al registrar");
            }
        }
    }
}
