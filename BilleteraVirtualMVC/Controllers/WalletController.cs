using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using BilleteraVirtualMVC.Models;
using Firebase.Storage;
using Newtonsoft.Json;

namespace BilleteraVirtualMVC.Controllers
{
    public class WalletController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            //Muestra el get en la visa
            return GetCards();

        }

        private IActionResult GetCards()
        {
            CardsHandler cardsHandler = new CardsHandler();

            ViewBag.Cards = cardsHandler.GetCardsCollection().Result;

            return View("List");
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCard(string id, string cardNumber, string name, string bank, string issuer, string codedate, string cvv, string type)
        {
            try
            {
                CardsHandler cardsHandler = new CardsHandler();

                bool result = cardsHandler.Edit(id, cardNumber, name, bank, issuer, codedate, cvv, type).Result;

                return GetCards();
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

        //Edit
        public IActionResult Edit(string id, string cardNumber, string name, string bank, string cvv, string issuer, string codedate, string type)
        {
            Card edited = new Card
            {
                Id = id,
                CardNumber = cardNumber,
                Name = name,
                Bank = bank,
                CVV = cvv,
                Issuer = issuer,
                CodeDate = codedate,
                Type = type,
            };

            ViewBag.Edited = edited;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string CardId)
        {
            try
            {
                // Primero, obtén la referencia al documento de la tarjeta que deseas eliminar en Firebase
                var cardDocRef = FirestoreDb.Create("wallet-6d70b")
                    .Collection("Cards")
                    .Document(CardId);

                // Borra el documento de la tarjeta
                await cardDocRef.DeleteAsync();

                // Redirige a la vista principal (Index) después de eliminar la tarjeta
                return RedirectToAction("List", "Wallet");
            }
            catch (Exception ex)
            {
                // Manejar errores
                Console.WriteLine("Error al eliminar tarjeta: " + ex.Message);
                return View();
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string cardNumber, string name, string bank, string issuer, string codedate, string cvv, string type)
        {
            try
            {
                CardsHandler cardsHandler = new CardsHandler();

                bool result = cardsHandler.Create(cardNumber, name, bank, issuer, codedate, cvv, type).Result;

                return GetCards();
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
