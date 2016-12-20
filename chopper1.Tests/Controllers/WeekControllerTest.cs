using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using chopper1;
using chopper1.Controllers;

namespace chopper1.Tests.Controllers
{
    [TestClass]
    public class WeekControllerTest
    {
        [TestMethod]
        public void getWeek()
        {
            // Arrange
            WeeksController controller = new WeeksController();
            // Act
            ViewResult result = controller.getWeek() as ViewResult;
            // Assert
            Assert.IsNotNull(result);            
        }
    }
}
