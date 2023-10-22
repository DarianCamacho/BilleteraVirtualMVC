using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using BilleteraVirtualMVC.Models;
using Firebase.Storage;
using Newtonsoft.Json;

namespace BilleteraVirtualMVC.Controllers
{
    public class WalletController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            //Muestra el get en la visa
            return await GetCards();
        }

        public IActionResult GetUserName()
        {



            // Leemos de la sesión los datos del usuario
            Models.User? user = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            // Pasamos el nombre de usuario a la vista
            ViewBag.UserName = user?.Name;

            return View();
        }

        public async Task<IActionResult> List()
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("List", "Error");

            //Muestra el get en la visa
            return await GetCards();
        }

        private async Task<IActionResult> GetCards()
        {
            List<Card> visitsList = new List<Card>();
            Query query = FirestoreDb.Create("wallet-6d70b").Collection("Visits");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                visitsList.Add(new Card
                {
                    Id = data["Id"].ToString(),
                    Name = data["Name"].ToString(),
                    Bank = data["Bank"].ToString(),
                    Issuer = data["Issuer"].ToString(),
                    Owner = data["Owner"].ToString(),
                    CodeDate = data["CodeDate"].ToString(),   
                    CVV = data["CVV"].ToString(),
                    PhotoPath = data["PhotoPath"].ToString()
                });
            }

            ViewBag.Visits = visitsList;

            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, string name, string bank, string issuer, string owner, string codedate, string cvv, string photopath)
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
                                { "Owner", owner },
                                { "CodeDate", codedate },
                                { "CVV", cvv },
                            });

                return await GetCards();
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

        public ActionResult Wallet()
        {
            return View();
        }
    }
}