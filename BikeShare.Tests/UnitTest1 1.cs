using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Repositories;
using Moq;
using System.Web.Mvc;
using SendASmile.Controllers;

namespace SendASmile.Tests
{
    [TestClass]
    public class DashboardTests
    {
        Mock<IDashboardRepository> dashRepo = new Mock<IDashboardRepository>();
        Mock<IAuthorizationRepository> authRepo = new Mock<IAuthorizationRepository>();
        [TestMethod]
        public void TestIndex()
        {
            //Arrange
            dashRepo.Setup(m => m.orgName()).Returns<String>(t => "Student Government");
            dashRepo.Setup(i => i.initialize(1));
            DashboardController x = new DashboardController(dashRepo.Object, authRepo.Object);
            
            //Act
            ViewResult result = (ViewResult)x.Index("sgfresh",1);
            //Assert
            Assert.AreEqual(result.ViewBag.organizationId, 1);
            Assert.AreEqual(result.ViewBag.organizationName, "Student Government");
        }
    }
}
