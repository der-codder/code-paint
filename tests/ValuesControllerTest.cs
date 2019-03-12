using System;
using CodePaint.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CodePaint.WebApi.Tests {

    public class ValuesControllerTest {
        [Fact]
        public void ValuesController_Get_Should_Return_Result() {
            //Arrange
            var controller = new ValuesController();

            //Act
            var result = controller.Get(0);

            //Assert
            Assert.IsType<ActionResult<string>>(result);
        }
    }
}
