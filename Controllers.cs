using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

public class CORSControllers : Controller
{
    [HttpGet]
    [Route("/arbitraryorigin")]
    [EnableCors("ArbitraryOrigin")]
    public ActionResult ArbitraryOriginEndpoint()
    {
        return Content("Successfully exploited an arbitrary origin vulnerability!");
    }

    [HttpGet]
    [Route("/nullorigin")]
    [EnableCors("NullOrigin")]
    public ActionResult NullOriginEndpoint()
    {
        return Content("Successfully exploited a null origin vulnerability!");
    }
    // Check if cors policy was accepted.

    [HttpPost]
    [Route("/badregex")]
    [EnableCors("BadRegex")]
    public ActionResult BadRegex1()
    {
        // The .? check if the dot is there or not, but otherwise allows anything before it since .* allows all characters.
        if(Regex.Match(HttpContext.Request.Headers.Origin!, "^https?://.*.?localhost$").Success)
            return Content("Regex can be easily screwed up!");
        return Content("Nothing to see here.");
    }

    [HttpPost]
    [Route("/why")]
    [EnableCors("BadRegex")]
    public ActionResult BadCheck()
    {
        if(HttpContext.Request.Headers.Origin!.Contains("https://localhost"))
        {
            return Content("Why are you even using CORS?");
        }
        return Content("Nothing to see here.");
    }

    [HttpGet]
    [Route("/secure")]
    [EnableCors("ValidPolicy")]
    public ActionResult ValidPolicy()
    {
        // SET THE VARY HEADER!
        HttpContext.Response.Headers.Vary = "Origin";
        return Content("Super secret information.");
    }
}