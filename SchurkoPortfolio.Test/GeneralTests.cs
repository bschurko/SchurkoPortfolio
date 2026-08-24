using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchurkoPortfolio.Core.Extensions;
using System;

namespace SchurkoPortfolio.Test
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string password = "Password123!";
            string hashedPassword = password.ToSha256();
            Console.WriteLine($"Original Password: {password}");
            Console.WriteLine($"Hashed Password: {hashedPassword}");
            Assert.IsNotNull(hashedPassword);
        }
    }
}
