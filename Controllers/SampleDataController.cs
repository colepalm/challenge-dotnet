using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace challenge_dotnet.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        
        [HttpPost]
        public FeeData[] PostFeeDatas([FromBody]FeeData[] orderData)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
            
            
            
            
            return orderData;
        }

        public class FeeData
        {
            public string order_date { get; set; }
            public object[] order_items { get; set; }
            public string order_number { get; set; }
        }
    }
}
