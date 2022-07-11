using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Java.Lang;

namespace ManyManager
{
	public class SignInResultCallback : Object, IResultCallback
	{
		public void OnResult(Object result)
		{
			var googleSignInResult = result as GoogleSignInResult;
			Globals.HideProgressDialog();
			Globals.Activity.HandleSignInResult(googleSignInResult);
		}
	}
}
