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
    [Route("/api/[controller]")]
    public class ProductsController : BaseController<ArchiLogDbContext, Product>
    {
        public ProductsController(ArchiLogDbContext context):base(context)
        {

        }
    }
}

