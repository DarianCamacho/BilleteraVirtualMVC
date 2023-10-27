using BilleteraVirtualMVC.FirebaseAuth;
using Firebase.Storage;
using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BilleteraVirtualMVC.Models
{
    public class Card
    {
        public string? Id { get; set; }
        public string? CardNumber { get; set; }
        public string? CodeDate { get; set; }
        public string? Name { get; set; }
        public string? Bank { get; set; }
        public string? Issuer { get; set; }
        public string? CVV { get; set; }
        public string? Type { get; set; }
    }

    public class CardsHandler
    {
        public async Task<List<Card>> GetCardsCollection()
        {
            List<Card> cardsList = new List<Card>();
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Cards");
            QuerySnapshot querySnaphot = await query.GetSnapshotAsync();

            foreach (var item in querySnaphot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                cardsList.Add(new Card
                {
                    Id = item.Id,
                    CardNumber = data["CardNumber"].ToString(),
                    Name = data["Name"].ToString(),
                    Bank = data["Bank"].ToString(),
                    CVV = data["CVV"].ToString(),
                    Issuer = data["Issuer"].ToString(),
                    CodeDate = data["CodeDate"].ToString(),
                    Type = data["Type"].ToString()
                });
            }

            return cardsList;
        }

        public async Task<bool> Create(string cardNumber, string name, string bank, string issuer, string codedate, string cvv, string type)
        {
            try
            {
                DocumentReference addedDocRef =
                    await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                        .Collection("Cards").AddAsync(new Dictionary<string, object>
                            {
                                { "CardNumber", cardNumber },
                                { "Name", name },
                                { "Bank", bank },
                                { "Issuer",  issuer },
                                { "CodeDate", codedate },
                                { "CVV", cvv },
                                { "Type", type },
                            });

                return true;
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Edit(string id, string cardNumber, string name, string bank, string issuer, string codedate, string cvv, string type)
        {
            try
            {
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Cards").Document(id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>

                    {
                        { "CardNumber", cardNumber },
                        { "Name", name },
                        { "Bank", bank },
                        { "Issuer",  issuer },
                        { "CodeDate", codedate },
                        { "CVV", cvv },
                        { "Type", type }
                    };

                WriteResult result = await docRef.UpdateAsync(dataToUpdate);

                return true;
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }

    }
}
