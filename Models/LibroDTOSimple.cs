using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppBooks.Models
{
    public class LibroDTOSimple
    {
        [Required]
        public string Titulo { get; set; }
        [Required]
        public int AutorId { get; set; }
    }
}
