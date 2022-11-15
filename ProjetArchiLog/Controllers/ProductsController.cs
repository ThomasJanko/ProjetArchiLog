using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetArchiLog.Models;
using ProjetArchiLog.data;
using LibraryArchiLog.Controllers;
using LibraryArchiLog.Wrappers;
using LibraryArchiLog.Filter;
using LibraryArchiLog.Services;

namespace ProjetArchiLog.Controllers
{
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class ProductsController : BaseController<ArchiLogDbContext, Product, IUriService>
    {
        public ProductsController(ArchiLogDbContext context, IUriService service) : base(context, service)
        {
            
        }

        //[ApiVersion("2.0")]
        //[HttpGet("all")]
        //public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        //{
        //    var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
        //    var pagedData = await _context.Products
        //        .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        //        .Take(validFilter.PageSize)
        //        .ToListAsync();
        //    var totalRecords = await _context.Products.CountAsync();
        //    return Ok(new PagedResponse<List<Product>>(pagedData, validFilter.PageNumber, validFilter.PageSize));
        //}

        [ApiVersion("1.0")]
        [HttpGet("filterV1")]
        public async Task<IEnumerable<Product>> GetAllFilter([FromQuery] string? category)
        {
            if (category == null)
            {
                return _context.Set<Product>().Where(x => x.Active).ToList();

            }

            return _context.Set<Product>().Where(x => x.Active && x.Category == category).ToList();


        }

        [ApiVersion("2.0")]
        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var content = await _context.Products.Where(a => a.ID == id).FirstOrDefaultAsync();
            return Ok(new Response<Product>(content));
        }
    }
}

