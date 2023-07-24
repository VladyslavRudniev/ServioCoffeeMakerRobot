using ServioCoffeMakerRobot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebAPI.OWIN.Controllers
{
    public class CoffeStateController : ApiController
    {
        RobotService _robotService;
        public CoffeStateController()
        {
            _robotService = RobotService.getInstance();
        }

        [HttpGet] //localhost:8008/CoffeState
        public int Index()
        {
            return 0;
        }
    }
}
