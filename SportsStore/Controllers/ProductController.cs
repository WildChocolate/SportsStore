using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository repository;
        public int PageSize = 4;
        public ProductController(IProductRepository repo)
        {
            repository = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ViewResult List(string category, int productPage=1) =>
            View(new ProductsListViewModel {
                Products = repository.Products
                .Where(p=> p.Category==null || p.Category==category)
                .OrderBy(p => p.ProductID)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage=productPage,
                    ItemPerpage=PageSize,
                    TotalItems= category==null? repository.Products.Count():repository.Products.Where(e=> e.Category==category).Count()
                },
                CurrentCategory=category
                });
        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            var result = await repository.DeleteProduct(productId);
            if (result != null)
                TempData["message"] = $"{result.Name} was deleted";
            return RedirectToAction(nameof(Index)) ;
        }
    }
}