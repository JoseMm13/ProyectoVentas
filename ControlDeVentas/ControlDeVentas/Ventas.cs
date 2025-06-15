using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlDeVentas
{
    class Ventas
    {
		private string  _Producto;
		private int _Cantidad;
        private string _Cliente;

        public static Dictionary<string, double> ProductosConPrecios = new Dictionary<string, double>();

        public Ventas()
        {
            CargarProductosDesdeArchivo("productos.txt");
        }
        public string Producto
        {
            get { return _Producto; }
            set { _Producto = value; }
        }

        public int Cantidad
		{
			get { return _Cantidad; }
			set { _Cantidad = value; }
		}
        public string Cliente
        {
            get { return _Cliente; }
            set { _Cliente = value; }
        }
        public static void CargarProductosDesdeArchivo(string rutaArchivo)
        {
            if (File.Exists(rutaArchivo))
            {
                try
                {
                    // Se leen todas las líneas del archivo
                    string[] lineas = File.ReadAllLines(rutaArchivo);
                    foreach (string linea in lineas)
                    {
                        if (!string.IsNullOrWhiteSpace(linea))
                        {
                            // Se espera el formato: Producto|Precio
                            string[] partes = linea.Split('|');
                            if (partes.Length == 2)
                            {
                                string producto = partes[0].Trim();
                                double precio;
                                if (double.TryParse(partes[1].Trim(), NumberStyles.Any, CultureInfo.GetCultureInfo("es-ES"), out precio))
                                {
                                    // Se agrega o actualiza el producto en el diccionario
                                    if (ProductosConPrecios.ContainsKey(producto))
                                    {
                                        ProductosConPrecios[producto] = precio;
                                    }
                                    else
                                    {
                                        ProductosConPrecios.Add(producto, precio);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al leer el archivo de productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("El archivo '" + rutaArchivo + "' no existe. Por favor, créalo con los productos en el formato 'Producto-Precio'.",
                    "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public double AsignarPrecio()
        {
            double precio;
            if (ProductosConPrecios.TryGetValue(Producto, out precio))
            {
                return precio;
            }
            return 0;
        }
        public double CalcularSubtotal()
        {
            return AsignarPrecio() * Cantidad;
        }
        public double CalcularDescuento()
        {
            double subTotal = CalcularSubtotal();

            if (subTotal <= 300) return 5.0 / 100 * subTotal;
            else if (subTotal > 300 && subTotal <= 500) return 10.0 / 100 * subTotal;
            else return 12.5 / 100 * subTotal;
        }
        public double CalcularNeto()
        {
            return CalcularSubtotal() - CalcularDescuento();
        }
        public void GuardarRegistro()
        {
            string ruta = $"Ventas_{Cliente}.txt";
            double totalAnterior = 0;

            // Leer el total acumulado si el archivo ya existe
            if (File.Exists(ruta))
            {
                string ultimaLinea = File.ReadLines(ruta).LastOrDefault();
                if (ultimaLinea != null)
                {
                    string[] datos = ultimaLinea.Split('|');
                    if (datos.Length >= 8)
                    {
                        string totalStr = datos[7].Replace("€", "").Trim();
                        if (double.TryParse(totalStr, NumberStyles.Any, CultureInfo.GetCultureInfo("es-ES"), out double totalAcumulado))
                        {
                            totalAnterior = totalAcumulado;
                        }
                    }
                }
            }

            double neto = CalcularNeto();
            double nuevoTotal = totalAnterior + neto;

            // Guardar la nueva compra en una sola línea con el total acumulado
            using (StreamWriter sw = new StreamWriter(ruta, true))
            {
                sw.WriteLine($"{Cliente}|{Producto}|{Cantidad}|{AsignarPrecio():C}|{CalcularSubtotal():C}|{CalcularDescuento():C}|{neto:C}|{nuevoTotal:C}");
            }
        }


    }
}
