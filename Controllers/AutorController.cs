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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebAppBooks.Controllers
{
    

    [Route("[Controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="admin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// Constructor Autores
        /// </summary>
        /// <param name="context">Inyecta el contexto de la base de datos.</param>
        /// <param name="mapper">Inyecta el mapeo de objetos.</param>

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los Autores --
        /// </summary>
        /// <returns> Una lista IEnumerable de tipo AutorDTO, puede ser null si no hay elementos. </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> Get()
        {
            var autores = await context.Autores.Include(x=>x.Libros).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        /// <summary>
        /// Obtiene un Autor dado su Id en un objeto Autor DTO
        /// </summary>
        /// <param name="Id"> Id del autor que se requiere.</param>
        /// <returns>Un objeto AutorDTO. Si no existe regresa NotFound.</returns>
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

        /// <summary>
        /// Crea un Autor con los datos enviados desde Body en un objeto de tipo AutorDTOS.
        /// </summary>
        /// <param name="autorDTOS"></param>
        /// <returns> Regresa una ruta al Autor creado:  /ObtenerAutor/Id </returns>
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

        /// <summary>
        /// Modifica un Autor segun datos enviado por el Body en un objeto AutorDTO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autorDTO"></param>
        /// <returns>NoContent()</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AutorDTO autorDTO)
        {
            var autor = mapper.Map<Autor>(autorDTO);
            autorDTO.Id = id;
           
            context.Entry(autorDTO).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Modifica los datos de un Autor dado su Id, con datos enviados por el Body en JsonPatchDocument de tipo AutorDTOS. 
        /// Solo actualiza los datos que son diferentes.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns> 
        /// BadRequest si el patchDocument es null.
        /// NotFound() si no existe el Id de Autor
        /// BadRequest() si hay errores en la validación del Model.
        /// NoContent() si fue exitoso.
        /// </returns>
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

        /// <summary>
        /// Borrar Autor --
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Si Id == 0 regresa BadRequest()
        /// Si No existe Id de Autor regresa NotFound()
        /// Si fue exitoso regresa NoContent()
        /// </returns>
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
