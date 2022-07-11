using Android.Gms.Common.Apis;
using Java.Lang;

namespace ManyManager
{
	public class SignOutResultCallback : Object, IResultCallback
	{
		public void OnResult(Object result)
		{
			Globals.Activity.UpdateUI(false);
		}
	}
}
