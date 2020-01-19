using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SportsStore.Test
{
    public class AdminControllerTests
    {
        [Fact]
        public void Index_Contains_All_Products()
        {
            //Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
            }).AsQueryable());
            //Arrange - create the controlelr
            AdminController target = new AdminController(mock.Object);
            //Action
            var result = (target.Index().ViewData.Model as IEnumerable<Product>).ToArray();
            //Assert
            Assert.Equal(3, result.Length);
            Assert.Equal("P1", result[0].Name);
            Assert.Equal("P2", result[1].Name);
            Assert.Equal("P3", result[2].Name);
        }
        [Fact]
        public void Can_Edit_Product()
        {
            //Arrange - create a mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
            }).AsQueryable());
            //Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            //Act
            Product p1 = GetViewModel<Product>(target.Edit(1));
            Product p2 = GetViewModel<Product>(target.Edit(2));
            Product p3 = GetViewModel<Product>(target.Edit(3));
            //Assert
            Assert.Equal(1, p1.ProductID);
            Assert.Equal(2, p2.ProductID);
            Assert.Equal(3, p3.ProductID);
        }

        private T GetViewModel<T>(ViewResult result )  where T:class
        {
            return result.ViewData.Model as T;
        }
        [Fact]
        public void Can_Save_Valid_Changes()
        {
            //Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            //Arrange - create mock tempdata
            Mock<ITempDataDictionary> tempdata = new Mock<ITempDataDictionary>();
            //Arrange - create the controller
            AdminController target = new AdminController(mock.Object)
            {
                TempData = tempdata.Object
            };
            //Arrange - create an product
            Product product = new Product { Name="test" };
            //Act - try to save the product
            var result = target.Edit(product);
            //Assert - check that repository was called
            mock.Verify(m => m.SaveProduct(product));
            //Assert - check the result type is redirection
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
        }
        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            //Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            //Arrange -create a controlelr
            AdminController target = new AdminController(mock.Object);
            //Arrange - create a product
            Product product = new Product() { Name="Test"};
            //Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            //act - try to edit the product
            IActionResult result = target.Edit(product);

            //Assert - check that repository was not call
            mock.Verify(m=> m.SaveProduct(It.IsAny<Product>()), Times.Never());
            //Assert - check the method result type
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void Can_Delete_Valid_Products()
        {
            //Arrange - create a product
            Product prod = new Product { ProductID = 2, Name = "Test" };

            //Arrange - create  the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1" }, prod,
                new Product { ProductID = 3, Name = "P3" }, }
            .AsQueryable<Product>());

            //Arrange - create the controlelr
            AdminController target = new AdminController(mock.Object);

            //Act - delete the test object
            target.Delete(prod.ProductID);
            //Assert - check if sequence length and quantity is correct
            mock.Verify(m=> m.DeleteProduct(It.IsAny<int>()), Times.Once);
        }
    }
}
