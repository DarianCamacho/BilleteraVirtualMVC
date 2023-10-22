using Google.Type;
using System.Xml.Linq;

namespace BilleteraVirtualMVC.Models
{
	public class Card
	{
        public string? PhotoPath { get; set; }
        public string? Id { get; set; }
		public string? Name { get; set; }
		public string? Bank { get; set; }
		public string? Issuer { get; set; }
        public string? Owner { get; set; }
        public string? CVV { get; set; }
        public string? CodeDate { get; set; }
    }
}

//Visa and Mastercard