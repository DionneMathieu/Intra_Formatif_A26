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

        // TODO: Il faut ajouter une nouvelle action pour dessiner la séquence mentionnée dans l'énoncé

        [HttpGet]
        public ActionResult GetDrawing2()
        {
            
            
            var drawSquare1 = new DrawSquare(0, 2);
            var drawSquare2 = new DrawSquare(2, 2);
            var drawCircle = new DrawCircle(1, 1);
            var drawStar = new DrawStar(1, 3, 20);
            var wait1 = new Wait(3);
            var wait2 = new Wait(1);

            var listevents = new List<DrawingEvent>
            {
                drawCircle,
                wait1,
                drawSquare1,
                drawSquare2,
                wait2,
                drawStar
            };

            drawCircle.DrawingEvents = new List<DrawingEvent>
            {
                
                wait1,
                drawSquare1,
                drawSquare2,
                wait2,
                drawStar
            };


            return Ok(drawCircle);

        }


    }
}
