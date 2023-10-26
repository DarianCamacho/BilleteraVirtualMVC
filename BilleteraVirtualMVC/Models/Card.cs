﻿using Google.Type;
using System.Xml.Linq;

namespace BilleteraVirtualMVC.Models
{
	public class Card
	{
        public string? Id { get; set; }
        public string? CardId { get; set; }
        public string? CodeDate { get; set; }
        public string? Name { get; set; }
		public string? Bank { get; set; }
		public string? Issuer { get; set; }
		public string? CVV { get; set; }
        public string? Type { get; set; }
    }
}

//Visa and Mastercard