using Biblioteca.Entidades;
using Biblioteca.Negocio;
using Biblioteca.Negocio.Excepciones;
using System;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Biblioteca.WPF.Views
{
    public partial class LibrosView : UserControl
    {
        private readonly LibroNegocio _libroNegocio;

        public LibrosView(LibroNegocio libroNegocio)
        {
            InitializeComponent();
            _libroNegocio = libroNegocio;
            Loaded += LibrosView_Loaded;
        }

        private async void LibrosView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarLibrosAsync();
        }

        private async System.Threading.Tasks.Task CargarLibrosAsync()
        {
            try
            {
                var libros = await _libroNegocio.ObtenerTodosAsync();
                dgLibros.ItemsSource = libros;
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message, "Error");
            }
        }

        private async void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            await CargarLibrosAsync();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validación rápida de UI
                if (string.IsNullOrWhiteSpace(txtTitulo.Text))
                {
                    MessageBox.Warning("Ingrese título", "Validación");
                    return;
                }

                var libro = new Libro
                {
                    Titulo = txtTitulo.Text.Trim(),
                    ISBN = txtISBN.Text.Trim(),
                    AutorId = 1, // Hardcodeado por simplicidad, se debe elegir de un ComboBox
                    Ejemplares = 1,
                    Activo = true
                };

                await _libroNegocio.InsertarAsync(libro);
                MessageBox.Success("Libro guardado exitosamente.", "Éxito");
                
                txtTitulo.Clear();
                txtISBN.Clear();
                await CargarLibrosAsync();
            }
            catch (ReglaNegocioException rne)
            {
                // Atrapa reglas de negocio sin caerse
                MessageBox.Info(rne.Message, "Regla de Negocio");
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message, "Error crítico");
            }
        }
    }
}
