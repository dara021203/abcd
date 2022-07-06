using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class EmpleadoViewModel
    {
        public int IdEmpleado { get; set; }
        public string Apellidos { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
        public System.DateTime FechaContratacion { get; set; }
        public string Ciudad { get; set; }
        public int Telefono { get; set; }
    }
}
