using Dessins.Events;
using Microsoft.AspNetCore.Mvc;

namespace Dessins.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DessinsController : ControllerBase
    {
        [HttpGet]
        // Rien à modifier ici, juste un exemple de dessin très simple
        public ActionResult GetDrawing1()
        {
            var drawSquare = new DrawSquare(2, 2);
            
            return Ok(drawSquare);
        }

        // (cercle -> 3s -> 2 carrés rouges -> 1s -> étoile jaune).
        [HttpGet]
        public ActionResult GetDrawing2()
        {
            var drawSquare1 = new DrawSquare(0, 2);
            var drawSquare2 = new DrawSquare(2, 2);
            var drawCircle = new DrawCircle(1, 1);
            var drawStar = new DrawStar(1, 3, 20);
            var wait1 = new Wait(3);
            var wait2 = new Wait(1);
            var changeRed = new ChangeColor("red");
            var changeYellow = new ChangeColor("yellow");

            drawCircle.DrawingEvents = new List<DrawingEvent>
            {
                wait1,
                changeRed,
                drawSquare1,
                drawSquare2,
                wait2,
                changeYellow,
                drawStar
            };

            return Ok(drawCircle);
        }
    }
}
