using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.UI.Models;
using System.Diagnostics;

namespace RagSystemFrontend.UI.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();  //renderizza la pagina Privacy.cshtml
        }

        //nel program.cs hai settato che errore non gestito va a /Home/Error cioe qui
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]  //0 xk la risposta non deve essere considerata valida per il riutilizzo, la risposta non deve essere memorizzata nella cache, e la risposta non deve essere memorizzata in alcun luogo
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });  //passo alla view Error.cshtml un oggetto ErrorViewModel con RequestId valorizzato con l'id della richiesta corrente o con l'identificatore di tracciamento della richiesta HTTP se Activity.Current è null
        }
    }

}
