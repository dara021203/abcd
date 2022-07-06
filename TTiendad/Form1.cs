using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewModel;

namespace TTiendad
{
    public partial class Form1 : Form
    {

        private string url = "https://localhost:44359/api/Empleado";
        private int id = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private bool validando()
        {
            if (string.IsNullOrWhiteSpace(txtApellidos.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCargo.Text) ||
                string.IsNullOrWhiteSpace(txtCiudad.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                return false;
            }
            return true;
        }
        private async void GetAll()
        {
            using (var cliente = new HttpClient())
            {
                using (var response = await cliente.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        dgvEmpleados.DataSource = JsonConvert.DeserializeObject<List<EmpleadoViewModel>>(jsonString);
                    }
                }
            }
        }
        private async void Update(int id)
        {
            if (!validando())
            {
                MessageBox.Show("Hay campos vacios");
                return;
            }
            if (!int.TryParse(txtTelefono.Text, out int telefono))
            {
                MessageBox.Show("No puede letras");
                return;
            }
            if (telefono.ToString().Length != 8)
            {
                MessageBox.Show("El telefono solo puede tener 8 digitos");
                return;
            }
            var empleado = new EmpleadoViewModel()
            {
                IdEmpleado = id,
                Nombre = txtNombre.Text,
                Apellidos = txtApellidos.Text,
                Cargo = txtCargo.Text,
                Ciudad = txtCiudad.Text,
                Telefono = telefono,
                FechaContratacion = dtpFecha.Value
            };

            using (var client = new HttpClient())
            {
                var serializedEmpleado = JsonConvert.SerializeObject(empleado);
                var content = new StringContent(serializedEmpleado, Encoding.UTF8, "application/json");
                var result = await client.PutAsync(url + "/" + id.ToString(), content);
                if (result.IsSuccessStatusCode)
                    MessageBox.Show(" actualizado");
                else
                    MessageBox.Show($"Error al actualizar: {result.StatusCode}");
            }
            Limpiar();
            GetAll();
        }

        private async void Add()
        {
            try
            {
                if (!validando())
                {
                    MessageBox.Show("Hay campos vacios");
                    return;
                }
                if (!int.TryParse(txtTelefono.Text, out int telefono))
                {
                    MessageBox.Show("No puede letras");
                    return;
                }
                var empleado = new EmpleadoViewModel()
                {
                    Nombre = txtNombre.Text,
                    Apellidos = txtApellidos.Text,
                    Cargo = txtCargo.Text,
                    Ciudad = txtCiudad.Text,
                    Telefono = telefono,
                    FechaContratacion = dtpFecha.Value
                };

                using (var client = new HttpClient())
                {
                    var serializedEmpleado = JsonConvert.SerializeObject(empleado);
                    var content = new StringContent(serializedEmpleado, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(url, content);
                }
                Limpiar();
                GetAll();
            }
            catch (Exception)
            {
                MessageBox.Show($"Ha ocurrido un error");
                return;
            }
        }
        private async void Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                HttpResponseMessage result = await client.DeleteAsync(string.Format("{0}/{1}", url, id));
                if (result.IsSuccessStatusCode)
                    MessageBox.Show("Empleado eliminado con exito");
                else
                    MessageBox.Show($"No se pudo eliminar el empleado: {result.StatusCode}");

                Limpiar();
                GetAll();
            }
        }

        private async void GetById(int id)
        {
            using (var client = new HttpClient())
            {
                string uri = url + "/" + id.ToString();
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    EmpleadoViewModel oEmpleado = JsonConvert.DeserializeObject<EmpleadoViewModel>(jsonString);
                    txtNombre.Text = oEmpleado.Nombre;
                    txtTelefono.Text = oEmpleado.Telefono.ToString();
                    txtCiudad.Text = oEmpleado.Ciudad;
                    txtCargo.Text = oEmpleado.Cargo;
                    txtApellidos.Text = oEmpleado.Apellidos;
                    dtpFecha.Value = oEmpleado.FechaContratacion;
                }
                else
                {
                    MessageBox.Show($"No se puede leer el empleado: {response.StatusCode}");
                }
            }
        }
        private void Limpiar()
        {
            txtApellidos.Clear();
            txtCargo.Clear();
            txtCiudad.Clear();
            txtNombre.Clear();
            txtTelefono.Clear();
            dtpFecha.Value = DateTime.Now;
            id = 0;
        }


        private void btnBorrar_Click(object sender, EventArgs e)
        {

            if (id != 0)
                Delete(id);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (id != 0)
                Update(id);

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

   

        private void Form1_Load(object sender, EventArgs e)
        {
            GetAll();
        }

        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            id = (int)dgvEmpleados.CurrentRow.Cells[0].Value;
            GetById(id);
        }
    }
}
