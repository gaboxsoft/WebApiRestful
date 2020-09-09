using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppBooks.Entities
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Indentificacion { get; set; }
        public DateTime FechaNacimiento { get; set; }

    }
}
