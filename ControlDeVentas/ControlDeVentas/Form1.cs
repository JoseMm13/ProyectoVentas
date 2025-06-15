using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlDeVentas
{
    public partial class frmVentas : Form
    {
        //static string[] productos = {"Teclado","Impresora","Monitor", "Auriculares"};
        //ArrayList arProducto = new ArrayList(productos);

        Ventas objV = new Ventas();
        double total;

        public frmVentas()
        {
            InitializeComponent();
        }


        private void frmVentas_Load(object sender, EventArgs e)
        {
            //CrearArchivosProductosSiNoExiste();
            MostrarFecha();
            MostrarHora();
            //CargarProductosDesdeArchivo();
            LlenarProducto();
            lblTotalNeto.Text = "0.00";
            LimpiarCampos();
        }
        private void btRegistrar_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MessageBox.Show("Error: Debes seleccionar un cliente antes de registrar la venta.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //Estos objetoss los envio a la clase ventas
            objV.Producto = cobProducto.Text;
            objV.Cantidad = int.Parse(txtCantidad.Text);
            objV.Cliente = txtCliente.Text;

            double totalAnterior = 0;
            string ruta = $"Ventas_{objV.Cliente}.txt";

            if (File.Exists(ruta))
            {
                string ultimaLinea = File.ReadLines(ruta).LastOrDefault();
                if (ultimaLinea != null)
                {
                    string[] datos = ultimaLinea.Split('|');
                    if (datos.Length >= 8)
                    {
                        string totalStr = datos[7].Replace("Total Acumulado:", "").Replace("€", "").Trim();
                        if (double.TryParse(totalStr, NumberStyles.Any, CultureInfo.GetCultureInfo("es-ES"), out double totalAcumulado))
                        {
                            totalAnterior = totalAcumulado;
                        }
                    }
                }
            }

            double nuevoTotal = totalAnterior + objV.CalcularNeto();


            //Ahora saco por pantalla los datos que he enviado a la clase ventas

            ListViewItem fila = new ListViewItem(objV.Cliente);
            fila.SubItems.Add(objV.Producto);
            fila.SubItems.Add(objV.Cantidad.ToString());
            fila.SubItems.Add(objV.AsignarPrecio().ToString("C"));
            fila.SubItems.Add(objV.CalcularSubtotal().ToString("C"));
            fila.SubItems.Add(objV.CalcularDescuento().ToString("C"));
            fila.SubItems.Add(objV.CalcularNeto().ToString("C"));
            fila.SubItems.Add(nuevoTotal.ToString("C"));

            // Añado los objetos a la tabla 
            lvRegistro.Items.Add(fila);

            // Guardardo  los datos en el archivo del cliente
            objV.GuardarRegistro();

            lblTotalNeto.Text = $"Total de {objV.Cliente}: {nuevoTotal:C}";
            lblTotalNeto.Refresh();


            //Limpiar  los Campos
            LimpiarCampos();
        }
        private void cobProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            objV.Producto = cobProducto.Text;
            lblPrecio.Text = objV.AsignarPrecio().ToString("C");
            
        }
        
        private void MostrarFecha()
        {
            lblFecha.Text = DateTime.Now.ToShortDateString();
        }
        private void MostrarHora()
        {
            lblHora.Text = DateTime.Now.ToLongDateString();
        }
        private void LimpiarCampos()
        {
            txtCliente.Clear();
            cobProducto.Text = "Seleccione un producto.";
            txtCantidad.Clear();
            lblPrecio.Text = "0.00";
            txtCliente.Focus();
        }
        private void LlenarProducto()
        {

            cobProducto.Items.Clear();

            Dictionary<string, double>.KeyCollection listaProductos = Ventas.ProductosConPrecios.Keys;

            foreach (string producto in listaProductos)
            {
                cobProducto.Items.Add(producto);
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Seguro que desea salir", "Ventas", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                LimpiarCampos(); 
            }
        }


        private void lvRegistro_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
