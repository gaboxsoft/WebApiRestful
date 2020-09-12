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

using Microsoft.AspNetCore.Mvc.ModelBinding;



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
            var autores = await context.Autores.Include(x=>x.Libros).ToListAsync();
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
        public async Task<ActionResult> Post([FromBody] AutorDTOS autorDTOS)
        {
            var autor = mapper.Map<Autor>(autorDTOS);
            autor.Indentificacion = "";
            context.Autores.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return new CreatedAtRouteResult("ObtenerAutor",new {id=autor.Id},autorDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AutorDTO autorDTO)
        {
            var autor = mapper.Map<Autor>(autorDTO);
            autorDTO.Id = id;
           
            context.Entry(autorDTO).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorDTOS> patchDocument)
        {            
            if (patchDocument==null)
            {
                return BadRequest();
            }

            var autorEnLaBD = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autorEnLaBD == null)
            {
                return NotFound();
            }

           
            var autorDTOS = mapper.Map<AutorDTOS>(autorEnLaBD);
            patchDocument.ApplyTo(autorDTOS, ModelState);
            mapper.Map(autorDTOS, autorEnLaBD);

            ////////////////
            var esValido = TryValidateModel(autorEnLaBD);
            if(!esValido)
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

            var autorId = await context.Autores.Select(x=>x.Id).FirstOrDefaultAsync(x => x == id);
            if (id == default(int))
            {
                return NotFound();
            }

            context.Remove(new Autor { Id = autorId });

            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
