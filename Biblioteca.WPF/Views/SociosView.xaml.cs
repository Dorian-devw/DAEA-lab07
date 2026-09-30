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
    public partial class SociosView : UserControl
    {
        private readonly SocioNegocio _socioNegocio;

        public SociosView(SocioNegocio socioNegocio)
        {
            InitializeComponent();
            _socioNegocio = socioNegocio;
            Loaded += SociosView_Loaded;
        }

        private async void SociosView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarSociosAsync();
        }

        private async System.Threading.Tasks.Task CargarSociosAsync()
        {
            try
            {
                dgSocios.ItemsSource = await _socioNegocio.ObtenerTodosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message, "Error");
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var socio = new Socio
                {
                    DNI = txtDni.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Email = "", // Opcional
                    Activo = true
                };

                await _socioNegocio.InsertarAsync(socio);
                MessageBox.Success("Socio registrado con éxito.", "Éxito");
                txtDni.Clear(); txtNombre.Clear();
                await CargarSociosAsync();
            }
            catch (ReglaNegocioException rne)
            {
                MessageBox.Info(rne.Message, "Regla de Negocio");
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message, "Error crítico");
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgSocios.SelectedItem is Socio socioSel)
            {
                try
                {
                    await _socioNegocio.EliminarAsync(socioSel.SocioId);
                    MessageBox.Success("Socio dado de baja lógicamente.", "Éxito");
                    await CargarSociosAsync();
                }
                catch (ReglaNegocioException rne)
                {
                    MessageBox.Warning(rne.Message, "Regla de Negocio");
                }
                catch (Exception ex)
                {
                    MessageBox.Error(ex.Message, "Error");
                }
            }
            else
            {
                MessageBox.Warning("Seleccione un socio para eliminar.", "Aviso");
            }
        }
    }
}
