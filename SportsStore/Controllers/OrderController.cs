using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        public OrderController(IOrderRepository repoService, Cart cartService)
        {
            repository = repoService;
            cart = cartService;
        }
        [Authorize]
        public ViewResult List() => View(repository.Orders.Where(o=> !o.Shipped));
        [HttpPost]
        [Authorize]
        public IActionResult MarkShipped(int orderID)
        {
            Order order = repository.Orders.FirstOrDefault(o=> o.OrderID==orderID);
            if (order != null)
            {
                order.Shipped = true;
                repository.SaveOrder(order);
            }
            return RedirectToAction(nameof(List));
        }
        public IActionResult Checkout() => View(new Order ());
        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (cart.CartLines.Count() == 0)
            {
                ModelState.AddModelError("","Sorry ,you cart is  empty!");
            }
            if (ModelState.IsValid)
            {
                order.Lines = cart.CartLines.ToArray();
                repository.SaveOrder(order);
                return RedirectToAction(nameof(Completed));
            }
            else
            {
                return View(order);
            }
        }
        public IActionResult Completed()
        {
            cart.Clear();
            return View();
        }
    }
}