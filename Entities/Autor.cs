using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppBooks.Entities
{
    public class Autor
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Indentificacion { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }
        public IEnumerable<Libro> Libros { get; set; }

    }
}
