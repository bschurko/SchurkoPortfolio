using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SchurkoPortfolio.Core.Data;
using SchurkoPortfolio.Core.Extensions;

namespace Schurko.Tests
{
    [TestClass]
    public sealed class GeneralTests
    {
        [TestMethod]
        public void ConnectToDatabaseTest()
        {
            try
            {
                var connStr = "Server=tcp:bschurkoserver.database.windows.net,1433;Initial Catalog=bschurko;Persist Security Info=False;User ID=brettschurko;Password=Password123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

                var options = new DbContextOptionsBuilder<EmailDbContext>()
                    .UseSqlServer(connStr)
                    .Options;
                using var context = new SchurkoPortfolio.Core.Data.EmailDbContext(options);

                bool canConnect = context.Database.CanConnect();
                Console.WriteLine($"Database connection successful: {canConnect}");
                Assert.IsTrue(canConnect);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                Assert.Fail("Database connection failed.");
            }
        }


        [TestMethod]
        public void PasswordToShaHashTest()
        {
            const string password = "Password123!";
            string hashedPassword = password.ToSha256();
            Console.WriteLine($"Original Password: {password}");
            Console.WriteLine($"Hashed Password: {hashedPassword}");
            Assert.IsNotNull(hashedPassword);
        }

        [TestMethod]
        public void ConnectToMongoDbTest()
        {
            try
            {
                string connectionString = "mongodb+srv://dbuser3kpuw7:Password123!@docdb-cluster-1945.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";
                string databaseName = "docdb-cluster-1945";
                var mongoClient = new MongoClient(connectionString);
                var database = mongoClient.GetDatabase(databaseName); // Replace with your database name
                // Try to list collections to verify the connection
                var collections = database.ListCollections().ToList();
                Console.WriteLine($"Connected to MongoDB. Collections count: {collections.Count}");
                Assert.IsTrue(collections.Count >= 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MongoDB connection failed: {ex.Message}");
                Assert.Fail("MongoDB connection failed.");
            }
        }
    }
}
