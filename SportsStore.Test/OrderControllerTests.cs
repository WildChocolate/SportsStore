﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SportsStore.Test
{
    public class OrderControllerTests
    {
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange - create a mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            //Arrange - create a empty cart
            Cart cart = new Cart();
            //Arrange - create a order
            Order order = new Order();
            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);
            //Act
            ViewResult result = target.Checkout(order) as ViewResult;
            //Assert - check that the order hasn't been stored
            mock.Verify(m=> m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            //Assert - check that I am processing  an invalid model to a view
            Assert.False(result.ViewData.ModelState.IsValid);
        }
        [Fact]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //Arrange - create a mock order repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            //Arrange - create a cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);
            //Arrange - add an error to the model
            target.ModelState.AddModelError("error","error");

            //Act - try to checkout
            ViewResult result = target.Checkout(new Order()) as ViewResult;

            //Assert - check that the order hasn't been pass stored
            mock.Verify(m=> m.SaveOrder(It.IsAny<Order>()), Times.Never);
            //Assert - check that the method is return a default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            //Assert - check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }
        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            //Arrange - create a mock order repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            //Arrange - create a cart with on item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            //Arrange - create  an instance of the controlelr
            OrderController target = new OrderController(mock.Object, cart);

            //Act -try to checkout
            RedirectToActionResult result = target.Checkout(new Order()) as RedirectToActionResult;

            //Assert - check that the order has been stored
            mock.Verify(m=> m.SaveOrder(It.IsAny<Order>()), Times.Once);
            //Assert - check the method is redirect to the completed action
            Assert.Equal("Completed",result.ActionName);
        }
    }
}