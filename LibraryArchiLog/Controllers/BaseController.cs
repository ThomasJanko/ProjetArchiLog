using LibraryArchiLog.data;
using LibraryArchiLog.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using LibraryArchiLog.Wrappers;

namespace LibraryArchiLog.Controllers
{
    [ApiController]
    public class BaseController<TContext, TModel> : ControllerBase where TContext: BaseDbContext where TModel: BaseModel
    {
        protected readonly TContext _context;
        

        public BaseController(TContext context)
        {
            _context = context;
        }

        //[ApiVersion("1.0")]
        //[HttpGet]
        //public async Task<IEnumerable<TModel>> GetAll()
        //{
            
        //    //return  await _context.Brands.ToListAsync();
        //    return   _context.Set<TModel>().Where(x => x.Active).ToList();

        //}

        //[ApiVersion("2.0")]
        //[HttpGet]
        //public async Task<IEnumerable<TModel>> GetAllFilter([FromQuery] String category)
        //{

        //        return _context.Set<TModel>().Where(x => x.Active).ToList();


        //}

      

        [ApiVersion("1.0")] 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, TModel item)
        {
            if (id != item.ID)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound("No Car with id" + id);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [ApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<TModel>> PostItem(TModel item)
        {
            _context.Set<TModel>().Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.ID }, item);
        }

        [ApiVersion("1.0")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Set<TModel>().FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Set<TModel>().Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool ItemExists(int id)
        {
            return _context.Set<TModel>().Any(e => e.ID == id);
        }
    }
}
