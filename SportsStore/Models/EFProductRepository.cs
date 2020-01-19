using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class EFProductRepository : IProductRepository
    {
        private ApplicationDbContext context;
        public EFProductRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Product> Products => context.Products;

        public async Task<Product> DeleteProduct(int productId)
        {
            if (productId > 0)
            {
                var product = await context.Products.FirstOrDefaultAsync(p=> p.ProductID==productId);
                if (product != null)
                    context.Products.Remove(product);
                context.SaveChanges();
                return product;
            }
            else
            {
                return null;
            }
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductID==0)
            {
                context.Products.Add(product);
            }
            else
            {
                //Product dbEntry = context.Products.FirstOrDefault(p => p.ProductID == product.ProductID);
                //if (dbEntry != null)
                //{
                //    dbEntry.Name = product.Name;
                //    dbEntry.Description = product.Description;
                //    dbEntry.Price = product.Price;
                //    dbEntry.Category = product.Category;
                //}
                var entry = context.Entry(product);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
