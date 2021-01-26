using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHub.Models;

namespace SmartHub.Controllers
{
    public class DataControllerTemp : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Data/{value}")]
        public void SomeUrlSegment(int value)
        {
            int i = value;
            //do stuff
        }

        [HttpPost]
        [Route("Data/Message")]
        public string Message([FromBody] string content)
        {
            return content;
        }

        [HttpPost]
        [Route("api/BodyTypes/JsonStringBody")]
        public string JsonStringBody([FromBody] string content)
        {
            return content;
        }

        [HttpPost]
        [Route("api/PostDataEntity")]
        public DataEntity PostDataEntity([FromBody] DataEntity dataEntity)
        {
            var test = dataEntity;
            return dataEntity;
        }
    }
}
