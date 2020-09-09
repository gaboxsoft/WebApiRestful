using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppBooks.Models
{
    public class LibroDTO
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public int AutorId { get; set; }
        public AutorDTO Autor { get; set; }
    }
}
