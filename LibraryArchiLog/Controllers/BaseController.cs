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
using LibraryArchiLog.Wrappers;
using LibraryArchiLog.Extensions;
using System.Net;
using LibraryArchiLog.Services;
using LibraryArchiLog.Filter;
using LibraryArchiLog.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LibraryArchiLog.Controllers
{
    [ApiController]
    public abstract class BaseController<TContext, TModel, TUriService> : ControllerBase where TContext: BaseDbContext where TModel: BaseModel where TUriService : IUriService
    {
        protected readonly TContext _context;
        protected readonly TUriService _uriService;

        public BaseController(TContext context, TUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }

        [ApiVersion("1.0")]
        [HttpGet, Authorize]
        public async Task<IEnumerable<TModel>> GetAll()
        {

            //return  await _context.Brands.ToListAsync();
            return _context.Set<TModel>().Where(x => x.Active).ToList();

        }

        [ApiVersion("2.0")]
        [HttpGet("sort")]
        public async Task<IEnumerable<TModel>> GetAllSorted([FromQuery] SortParams param)
        {

            return await _context.Set<TModel>().Where(x => x.Active).Sort(param).ToListAsync();


        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ApiVersion("3.0")]
        [HttpGet("search")]
        public virtual async Task<ActionResult<IEnumerable<dynamic>>> SearchAsync([FromQuery] string name, string category)
        {
            var contents = _context.Set<TModel>().AsQueryable();

            if (!string.IsNullOrEmpty(name) /*|| !string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(rating) || !string.IsNullOrEmpty(date)*/)
            {
                contents = contents.SearchThis(name, category);
            }
            return Ok(await contents.ToListAsync());
        }

        [ApiVersion("1.0")]
        [HttpGet("{id}")]
        public async Task<ActionResult<TModel>> GetItem(int id)
        {
            var item = await _context.Set<TModel>().FindAsync(id);

            if (item == null || !item.Active)
            {
                return NotFound();
            }

            return item;
        }

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
        [HttpPost, Authorize]
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


        [HttpGet("Filters/v3")]
        [ApiVersion("3.0")]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAllFilters(string? range, string asc, string? desc, string? type, string? rating, string? date)
        {
            var contents = _context.Set<TModel>().AsQueryable();


            //filter 
            if (!string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(rating) || !string.IsNullOrEmpty(date))
            {
                contents = contents.FilterThis(type, rating, date);
            }

            var totalRecords = await contents.CountAsync();
            if (totalRecords > 0)
            {
                if (String.IsNullOrEmpty(range))
                {
                    range = 1 + "-" + totalRecords;
                }
                var route = Request.Path.Value;
            

            var tab = range.Split('-');
            var start = int.Parse(tab[0]);
            var end = int.Parse(tab[1]);
            var validRange = new RangeFilter(start, end, totalRecords);
            var pageSize = (1 + validRange.End - validRange.Start);
            var page = 1 + (validRange.Start / pageSize);
            var validFilter = new PaginationFilter(page, pageSize);

            var pagedData = await contents
                    .Skip((validFilter.Page - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize)
                    .ToListAsync();
            var pagedResponse = PaginationHelper.CreatePagedResponse<TModel>(pagedData, range, validFilter, totalRecords, _uriService, route, asc, desc, type, rating, date);

            return Ok(pagedResponse);
            }
            else
            {
                return new List<TModel>();
            }

        }
    }
}
