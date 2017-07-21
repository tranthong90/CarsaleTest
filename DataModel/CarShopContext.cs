using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace DataModel
{
    public class CarShopContext : DbContext
    {
        public CarShopContext()
        {
            Database.SetInitializer(new CarShopDbInitializer());
        }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Dealer> Dealers { get; set; }

    }

    public class CarShopDbInitializer : CreateDatabaseIfNotExists<CarShopContext> //DropCreateDatabaseAlways -- CreateDatabaseIfNotExists<CarShopContext>
    {
        protected override void Seed(CarShopContext context)
        {
            string[] Makes = { "Honda", "Mazda", "Lexus", "BMW", "Toyota", "Kia", "Nissan", "Suzuki" };
            List<Dealer> dealers = new List<Dealer>();
            for (var c = 1; c <= 100; c++)
            {
                Dealer author = new Dealer()
                {
                    Name = "Dealer " + c,
                    Email = "Dealer" + c + "@testmail.com",
                    Username = "Dealer" + c,
                    Cars = new List<Car>()
                };
                int numberOfCars = new Random(c).Next(3, 20);
                for (var j = 0; j <= numberOfCars; j++)
                {
                    var car = new Car()
                    {
                        Year = new Random(j).Next(1999, 2017),
                        Make = Makes[new Random(j).Next(0, Makes.Length)],
                        Model = "Model " + c,
                        Badge = "Test Badge",
                        EngineSize = "Test EngineSize",
                        Transmission = "Test Transmission",
                        DateCreated = DateTime.Now.AddDays(new Random(j).Next(-30, 0))
                        // DateCreated = new DateTime(new Random(j).Next(2010, 2016), new Random(j).Next(1, 12), new Random(j).Next(1, 28))
                    };
                    author.Cars.Add(car);
                  
                }
                dealers.Add(author);
            }
            context.Dealers.AddRange(dealers);
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

            }
            base.Seed(context);
        }
    }
}
