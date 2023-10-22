using Firebase.Auth.Providers;
using Firebase.Auth;
using Microsoft.AspNetCore.DataProtection;

namespace BilleteraVirtualMVC.FirebaseAuth
{
	public static class FirebaseAuthHelper
	{
		public const string firebaseAppId = "wallet-6d70b";
		public const string firebaseApiKey = "AIzaSyAOU0p6BbJBoOt8SjC-Qx3i8ONnAT1XViw";

		public static FirebaseAuthClient setFirebaseAuthClient()
		{
			var response = new FirebaseAuthClient(new FirebaseAuthConfig
			{
				ApiKey = firebaseApiKey,
				AuthDomain = $"{firebaseAppId}.firebaseapp.com",
				Providers = new FirebaseAuthProvider[]
					{
						new EmailProvider()
					}
			});

			return response;
		}
	}
}
