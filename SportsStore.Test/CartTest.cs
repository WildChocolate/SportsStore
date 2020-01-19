using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SportsStore.Test
{
    public class CartTest
    {
        [Fact]
        public void Can_Add_New_Lines()
        {
            //Arrange - create some test product
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            //Arrange - create a new cart
            Cart target = new Cart();
            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] result = target.CartLines.ToArray();
            //Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(p1, result[0].Product);
            Assert.Equal(p2, result[1].Product);
        }
        [Fact]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            //Arrange
            Cart target = new Cart();
            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.CartLines.OrderBy(c=> c.Product.ProductID).ToArray();
            //assert
            Assert.Equal(2, results.Length);
            Assert.Equal(11, results[0].Quantity);
            Assert.Equal(1, results[1].Quantity);
        }
        [Fact]
        public void Can_Remove_Line()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            //Arrange
            Cart target = new Cart();
            //Act
            target.AddItem(p1, 2);
            target.AddItem(p2, 1);
            target.RemoveLine(p1);
            var result = target.CartLines.ToArray();
            //Assert
            Assert.Equal(1, result[0].Quantity);
            Assert.NotEqual(p1.ProductID, result[0].Product.ProductID);
        }
        [Fact]
        public void Calculate_Cart_Total()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" , Price=10};
            Product p2 = new Product { ProductID = 2, Name = "P2" , Price=5m};
            Product p3 = new Product { ProductID = 3, Name = "P3", Price = 15m };
            //Arrange
            Cart target = new Cart();
            //act
            target.AddItem(p1, 2);
            target.AddItem(p2, 3);
            target.AddItem(p3, 8);
            decimal total = 2 * 10 + 5 * 3 + 15 * 8;
            //assert
            Assert.Equal(13, target.CartLines.Sum(i=> i.Quantity));
            Assert.Equal(total, target.ComputeTotalValue());
        }
        [Fact]
        public void Can_Clear_Contents()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 10 };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 5m };
            Product p3 = new Product { ProductID = 3, Name = "P3", Price = 15m };
            //Arrange
            Cart target = new Cart();
            //act
            target.AddItem(p1, 2);
            target.AddItem(p2, 3);
            target.AddItem(p3, 8);
            target.Clear();
            //Assert
            Assert.Equal(0, target.CartLines.Count());
            Assert.Null(target.CartLines.FirstOrDefault());

        }
    }
}
