using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppBooks.Contexts;
using WebAppBooks.Entities;
using WebAppBooks.Models;

namespace WebAppBooks.Controllers
{
    [Route("[Controller]")]
    [ApiController]

    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{Id}", Name="ObtenerAutor")]
        public async Task<ActionResult<AutorDTO>> Get(int Id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == Id);
            if(autor==null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return autorDTO;
        }

        [HttpPost()]
        public async Task<ActionResult<AutorDTO>> Post(AutorDTOS autorDTOS)
        {
            var autor = mapper.Map<Autor>(autorDTOS);
            autor.Indentificacion = "";
            context.Autores.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return autorDTO;
        }

    }
}
