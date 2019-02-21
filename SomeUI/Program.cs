using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeUI
{
    internal class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        private static void Main(string[] args)
        {
            _context.GetService<ILoggerFactory>().AddProvider(new MyLoggerProvider());
            //InsertSamurai();
            //InsertMultipleSamurais();
            //SimpleSamuraiQuery();
            //MoreQueries();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleOperations();
            //DeleteWhileTracked();
            //DeleteMany();
            //RawSqlQuery();
            //QueryWithNonSql();
            //RawSqlCommand();
            //RawSqlQueryOther();
            RawSqlCommandWithOutput();
        }

        private static void InsertSamurai()
        {
            var name = "Oscar";
            var samurai = new Samurai { Name = name };
            using (var context = new SamuraiContext())
            {
                context.GetService<ILoggerFactory>().AddProvider(new MyLoggerProvider());
                context.Samurais.Add(samurai);
                context.SaveChanges();
            }
        }

        private static void InsertMultipleSamurais()
        {
            var samurai = new Samurai { Name = "Zuniga" };
            var samurai1 = new Samurai { Name = "Garay" };
            using (var context = new SamuraiContext())
            {
                context.GetService<ILoggerFactory>().AddProvider(new MyLoggerProvider());
                //you can use separate by comma
                context.Samurais.AddRange(samurai, samurai1);
                //or use a List
                context.Samurais.AddRange(new List<Samurai> { samurai, samurai1 });
                context.SaveChanges();
            }
        }

        private static void SimpleSamuraiQuery()
        {
            using (var context = new SamuraiContext())
            {
                //this option retrieve the entire list
                var samurais = context.Samurais.ToList();
                var query = context.Samurais;
                //var samuraisAgain = query.ToList();

                //also can use foreach looping using the query var above
                //foreach (var samurai in query)

                //or using the method to retrieve a result
                foreach (var samurai in context.Samurais)
                {
                    Console.WriteLine(samurai.Name);
                }
            }
        }

        private static void MoreQueries()
        {
            // use a variable to retrieve the matches
            //var name = "Zuniga";
            //var samurais = _context.Samurais.Where(s => s.Name == name).ToList();

            //FirstOrDefault() method search for the first match and returns the value
            //if no match then return null
            var name = "Oscar";
            //var samurais = _context.Samurais.Where(s => s.Name == name).FirstOrDefault();
            //simplest way
            var samurais = _context.Samurais.FirstOrDefault(s => s.Name == name);

            //ahother way is using Find
            //var samurais = _context.Samurais.Find(2);
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "+Zuniga";
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.ToList();
            samurais.ForEach(s => s.Name += " Garay");
            _context.SaveChanges();
        }

        private static void MultipleOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "new name";
            _context.Samurais.Add(new Samurai { Name = "Victoria" });
            _context.SaveChanges();
        }

        private static void DeleteWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Oscar Garay");
            _context.Samurais.Remove(samurai);

            /*
             * other options to delete
             * 
             */
            //_context.Remove(samurai);
            //_context.Entry(samurai).State = EntityState.Deleted;
            //_context.Samurais.Remove(_context.Samurais.Find(1));

            _context.SaveChanges();
        }

        private static void DeleteMany()
        {
            var samurais = _context.Samurais.Where(s => s.Name.Contains("s"));
            _context.Samurais.RemoveRange(samurais);

            //other option
            //_context.RemoveRange(samurais);

            _context.SaveChanges();
        }

        private static void RawSqlQuery()
        {
            /*first option is to perform a simple SELECT to db*/
            //var samurais = _context.Samurais.FromSql("Select * from Samurais")
            //    .OrderByDescending(s => s.Name)
            //    .Where(s => s.Name.Contains("s")).ToList();

            /*other option is using a Store Procedure*/
            var namePart = "s";
            var samurais = _context.Samurais
                .FromSql("EXEC FilterSamuraiByNamePart {0}", namePart)
                .OrderByDescending(s => s.Name).ToList();

            samurais.ForEach(s => Console.WriteLine(s.Name));
            Console.WriteLine();
        }

        private static void QueryWithNonSql()
        {
            //example
            //Console.WriteLine(ReverseString("apple"));

            var samurais = _context.Samurais
                .Select(s => new { newName = ReverseString(s.Name) })
                .ToList();
            samurais.ForEach(s => Console.WriteLine(s.newName));
            Console.WriteLine();

        }

        private static string ReverseString(string value)
        {
            var stringChar = value.AsEnumerable();
            return string.Concat(stringChar.Reverse());
        }

        private static void RawSqlQueryOther()
        {
            var samurais = _context.Samurais.FromSql("Select * From Samurais")
                .OrderByDescending(s => s.Name).ToList();
            samurais.ForEach(s => Console.WriteLine(s.Name));
            Console.WriteLine();
        }

        private static void RawSqlCommand()
        {
            var affected = _context.Database.ExecuteSqlCommand(
                "Update Samurais Set Name = REPLACE(Name, 'Garay', 'Gray')");
            Console.WriteLine($"Affected rows {affected}");
        }

        private static void RawSqlCommandWithOutput()
        {
            var procResult = new SqlParameter
            {
                PrameterName = "@procResult",
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Output,
                Size = 50
            };
            _context.Database.ExecuteSqlCommand(
                "EXEC FindLongestName @procResult OUT", procResult);
            Console.WriteLine($"Longest name: {procResult.Value}");
        }

    }
}
