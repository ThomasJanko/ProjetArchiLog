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

namespace ProjetArchiLog.Controllers
{
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class ProductsController : BaseController<ArchiLogDbContext, Product>
    {
        public ProductsController(ArchiLogDbContext context):base(context)
        {
            
        }

        [ApiVersion("2.0")]
        [HttpGet]
        public async Task<IEnumerable<Product>> GetAllFilter([FromQuery] String? category)
        {
            if(category == null)
            {
                return _context.Set<Product>().Where(x => x.Active).ToList();

            }

            return _context.Set<Product>().Where(x => x.Active && x.Category == category).ToList();


        }
    }
}

