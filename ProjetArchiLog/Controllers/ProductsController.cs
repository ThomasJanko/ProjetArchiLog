using LibraryArchiLog.Controllers;
using ProjetArchiLog.data;
using ProjetArchiLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ProjetArchiLog.Controllers
{
    [Route("/api/[controller]")]

    public class ProductsController: BaseController<ArchiLogDbContext, Product>
    {
        public ProductsController(ArchiLogDbContext context) : base(context)
        {

        }
    }
}

