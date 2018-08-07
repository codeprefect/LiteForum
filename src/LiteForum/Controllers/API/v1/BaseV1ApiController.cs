using Microsoft.AspNetCore.Mvc;

namespace LiteForum.Controllers.API.v1
{
    [ApiVersion("1")]
    public class BaseV1ApiController : BaseApiController
    {
        protected string userMismatchMessage => "you are not permitted to alter this resource";
        protected string resourceMismatchMessage = "requested resource does not belong to specified parent resource.";
    }
}
