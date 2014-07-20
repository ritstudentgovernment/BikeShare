using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Repositories;
using Moq;
using System.Web.Mvc;
using SendASmile.Controllers;

namespace SendASmile.Tests
{
    [TestClass]
    public class DashboardIndexTests
    {
        Mock<IDashboardRepository> dashRepo = new Mock<IDashboardRepository>();
        Mock<IAuthorizationRepository> authRepo = new Mock<IAuthorizationRepository>();
        private readonly string user = "sgfresh";
        private readonly int orgid = 1;
        private readonly string orgName = "Student Government";

        [TestMethod]
        public void IndexOutputHappyPath()
        {
            //Arrange
            dashRepo.Setup(m => m.orgName()).Returns(orgName);
            dashRepo.Setup(i => i.initialize(orgid));
            authRepo.Setup(i => i.userCanManageOrg(It.IsAny<string>(), It.IsAny<int>())).Returns((string n, int i) => true);
            DashboardController x = new DashboardController(dashRepo.Object, authRepo.Object);
            
            //Act
            ViewResult result = (ViewResult)x.Index(user,1);
            //Assert
            Assert.AreEqual(result.ViewBag.organizationId, orgid.ToString());
            Assert.AreEqual(result.ViewBag.organizationName, "Student Government");
        }

        [TestMethod]
        public void IndexOutputBadUser()
        {
            //Arrange
            //Arrange
            dashRepo.Setup(m => m.orgName()).Returns(orgName);
            dashRepo.Setup(i => i.initialize(orgid));
            authRepo.Setup(i => i.userCanManageOrg(It.IsAny<string>(), It.IsAny<int>())).Returns((string n, int i) => false);
            DashboardController x = new DashboardController(dashRepo.Object, authRepo.Object);
            //Act
            RedirectToRouteResult result = (RedirectToRouteResult)x.Index(user, orgid);
            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }


    }
}