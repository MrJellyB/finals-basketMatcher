using Microsoft.AspNetCore.Mvc;
using Basket.Common.Data;
using System.Collections.Generic;

namespace finals_basketMatch
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        [HttpGet]
        public IActionResult PostInvokeSuggetionsMatch()
        {
            // TODO: Send a request/function call (from lib) that generates 1000 baskets

            BasketDTO[] list = new BasketDTO[4];

            return Ok(list);
        }
    }
}
