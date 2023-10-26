using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using BilleteraVirtualMVC.Models;
using Firebase.Storage;
using Newtonsoft.Json;
using Firebase.Auth;
using Firebase.Auth.Repository;

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

        public ActionResult CardDetail()
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
                    Id = item.Id,
                    CardId = data["CardId"].ToString(),
                    Name = data["Name"].ToString(),
                    Bank = data["Bank"].ToString(),
                    CVV = data["CVV"].ToString(),
                    Issuer = data["Issuer"].ToString(),
                    CodeDate = data["CodeDate"].ToString(),
                    Type = data["Type"].ToString()
                });
            }

            ViewBag.Cards = cardsList;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string cardId, string name, string bank, string issuer, string codedate, string cvv, string type)
        {
            try
            {
                // Primero, obtén la referencia al documento de la tarjeta que deseas editar en Firebase
                var cardDocRef = FirestoreDb.Create("wallet-6d70b")
                    .Collection("Cards")
                    .Document(cardId);

                // Crear un objeto anónimo con los campos a actualizar
                var updateData = new
                {
                    Name = name,
                    Bank = bank,
                    Issuer = issuer,
                    CodeDate = codedate,
                    CVV = cvv,
                    Type = type
                };

                // Realiza la actualización del documento
                await cardDocRef.SetAsync(updateData, SetOptions.MergeAll);

                // Después de editar la tarjeta, redirige a la vista principal (List) y actualiza la lista de tarjetas
                return RedirectToAction("List", "Wallet");
            }
            catch
            {
                // Manejo de errores
                return View();
            }
        }


        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string cardId, string name, string bank, string issuer, string codedate, string cvv, string type)
        {
            try
            {
                DocumentReference addedDocRef =
                    await FirestoreDb.Create("wallet-6d70b")
                        .Collection("Cards").AddAsync(new Dictionary<string, object>
                            {
                                { "CardId", cardId },
                                { "Name", name },
                                { "Bank", bank },
                                { "Issuer",  issuer },
                                { "CodeDate", codedate },
                                { "CVV", cvv },
                                { "Type", type },
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
