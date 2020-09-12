using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibroDTO>>> Get()
        {
            var libros = await context.Libros.Include(x => x.Autor).ToListAsync();
            return mapper.Map<List<LibroDTO>>(libros);
        }

        [HttpGet("{Id}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTO>> Get(int Id)
        {
            var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == Id);
            if (libro == null)
            {
                return NotFound();
            }
            var LibroDTO = mapper.Map<LibroDTO>(libro);
            return LibroDTO;
        }

        [HttpPost()]
        public async Task<ActionResult> Post([FromBody] LibroDTOS LibroDTOS)
        {
            var libro = mapper.Map<Libro>(LibroDTOS);
            
            context.Libros.Add(libro);
            await context.SaveChangesAsync();
            var LibroDTO = mapper.Map<LibroDTO>(libro);
            return new CreatedAtRouteResult("ObtenerLibro", new { id = libro.Id }, LibroDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] LibroDTO LibroDTO)
        {
            var libro = mapper.Map<Autor>(LibroDTO);
            LibroDTO.Id = id;

            context.Entry(LibroDTO).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<LibroDTOS> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroEnLaBD = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);
            if (libroEnLaBD == null)
            {
                return NotFound();
            }


            var LibroDTOS = mapper.Map<LibroDTOS>(libroEnLaBD);
            patchDocument.ApplyTo(LibroDTOS, ModelState);
            mapper.Map(LibroDTOS, libroEnLaBD);

            ////////////////
            var esValido = TryValidateModel(libroEnLaBD);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == default(int))
            {
                return BadRequest();
            }

            var libroId = await context.Libros.Select(x => x.Id).FirstOrDefaultAsync(x => x == id);
            if (id == default(int))
            {
                return NotFound();
            }

            context.Remove(new Autor { Id = libroId });

            await context.SaveChangesAsync();

            return NoContent();
        }


    }
}
