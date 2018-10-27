using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace ExpenseMailService.Api.Tests.Controllers
{
    public class ControllerTestsBase
    {
        protected Mock<HttpContext> HttpContext;
        protected Mock<HttpRequest> Request;
        protected Mock<HttpResponse> Response;
        protected Mock<UrlHelper> UrlHelper;
        protected Mock<ClaimsPrincipal> User;

        public void MockRequest(Controller controller)
        {
            User = new Mock<ClaimsPrincipal>();
            var identity = new Mock<ClaimsIdentity>();
            User.Setup(t => t.Identity).Returns(identity.Object);

            Request = new Mock<HttpRequest>();
            Response = new Mock<HttpResponse>();
            Response.Setup(t => t.Body).Returns(new MemoryStream());

            HttpContext = new Mock<HttpContext>();
            HttpContext.Setup(t => t.User).Returns(User.Object);
            HttpContext.Setup(t => t.Request).Returns(Request.Object);
            HttpContext.Setup(t => t.Response).Returns(Response.Object);

            controller.ControllerContext = new ControllerContext(new ActionContext(HttpContext.Object, new RouteData(), new ControllerActionDescriptor()));
        }
    }
}
