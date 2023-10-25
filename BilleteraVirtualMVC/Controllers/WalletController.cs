using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using BilleteraVirtualMVC.Models;
using Firebase.Storage;
using Newtonsoft.Json;
using Firebase.Auth;

namespace BilleteraVirtualMVC.Controllers
{
    public class WalletController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            //Muestra el get en la visa
            return await GetCards();

        }

        public ActionResult Edit()
        {
            return View();
        }

        public IActionResult GetUserName()
        {

            // Leemos de la sesión los datos del usuario
            Models.User? user = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            // Pasamos el nombre de usuario a la vista
            ViewBag.UserName = user?.Name;

            return View();
        }

        private async Task<IActionResult> GetCards()
        {
            List<Card> cardsList = new List<Card>();
            Query query = FirestoreDb.Create("wallet-6d70b").Collection("Cards");
            QuerySnapshot querySnaphot = await query.GetSnapshotAsync();

            foreach (var item in querySnaphot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                cardsList.Add(new Card
                {
                    Id = data["Id"].ToString(),
                    Name = data["Name"].ToString(),
                    Bank = data["Bank"].ToString(),
                    CVV = data["CVV"].ToString(),
                    Issuer = data["Issuer"].ToString(),
                    CodeDate = data["CodeDate"].ToString()
                });
            }

            ViewBag.Cards = cardsList;

            return View();
        }

        public async Task<IActionResult> DeleteCard(string id)
        {
            // Verifica si el usuario está autenticado (puedes agregar más validaciones si es necesario).
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
            {
                return RedirectToAction("Index", "Error");
            }

            // Crear una referencia al documento que deseas eliminar
            DocumentReference cardRef = FirestoreDb.Create("wallet-6d70b")
                .Collection("Cards")
                .Document(id);

            // Elimina el documento
            await cardRef.DeleteAsync();

            // Redirecciona a la vista de tarjetas actualizada
            return RedirectToAction("List");
        }


        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string id, string name, string bank, string issuer, string owner, string codedate, string cvv)
        {
            try
            {
                DocumentReference addedDocRef =
                    await FirestoreDb.Create("wallet-6d70b")
                        .Collection("Cards").AddAsync(new Dictionary<string, object>
                            {
                                { "Id", id },
                                { "Name", name },
                                { "Bank", bank },
                                { "Issuer",  issuer },
                                { "CodeDate", codedate },
                                { "CVV", cvv },
                            });

                return View("Index");
            }

            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Wallet",
                    Path = "/Wallet"
                };

                return View("ErrorHandler");
            }
        }
    }
}
