using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Controllers;
using ProductApi.Models;
using Xunit;

namespace XUnitTestProject
{
    public class ProductsControllerTests : ControllerBase
    {
        [Fact]
        public async System.Threading.Tasks.Task GetProductByValidIdAsync()
        {
            var options = new DbContextOptionsBuilder<AdventureWorks2019Context>()
              .UseInMemoryDatabase(databaseName: "ProductDataBase")
              .Options;

            using (var context = new AdventureWorks2019Context(options))
            {
                context.Product.Add(new Product
                {
                    ProductId = 12345,
                    Name = "Short for boys",
                    ProductNumber = "2555",
                });

                context.Product.Add(new Product
                {
                    ProductId = 12346,
                    Name = "Short for girls",
                    ProductNumber = "NY-235",
                });
                context.SaveChanges();
            }

            using (var context = new AdventureWorks2019Context(options))
            {
                ProductsController controller = new ProductsController(context, null, null);
                var result = await controller.GetProduct(12345);
                var actualResult = result.Value;

                Assert.Equal(12345, ((Product)actualResult).ProductId);
                Assert.Equal("Short for boys", ((Product)actualResult).Name);
                Assert.Equal("2555", ((Product)actualResult).ProductNumber);
            }
        }


        [Fact]
        public async System.Threading.Tasks.Task GetProductByInValidIdAsync()
        {
            var options = new DbContextOptionsBuilder<AdventureWorks2019Context>()
              .UseInMemoryDatabase(databaseName: "ProductDataBase")
              .Options;


            using var context = new AdventureWorks2019Context(options);
            ProductsController controller = new ProductsController(context, null, null);
            var result = await controller.GetProduct(123456);
            var actualResult = result.Result;
            var actualValue = result.Value;


            Assert.IsAssignableFrom<NotFoundResult>(actualResult);
            Assert.Null(actualValue);

        }
    }
}
