package crc64751805a58b33156b;


public class ExpandableDataAdapter_MyOnGroupExpandListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.widget.ExpandableListView.OnGroupExpandListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onGroupExpand:(I)V:GetOnGroupExpand_IHandler:Android.Widget.ExpandableListView/IOnGroupExpandListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("ManyManager.ExpandableDataAdapter+MyOnGroupExpandListener, ManyManager", ExpandableDataAdapter_MyOnGroupExpandListener.class, __md_methods);
	}


	public ExpandableDataAdapter_MyOnGroupExpandListener ()
	{
		super ();
		if (getClass () == ExpandableDataAdapter_MyOnGroupExpandListener.class)
			mono.android.TypeManager.Activate ("ManyManager.ExpandableDataAdapter+MyOnGroupExpandListener, ManyManager", "", this, new java.lang.Object[] {  });
	}

	public ExpandableDataAdapter_MyOnGroupExpandListener (android.content.Context p0, int p1, android.widget.ExpandableListView p2)
	{
		super ();
		if (getClass () == ExpandableDataAdapter_MyOnGroupExpandListener.class)
			mono.android.TypeManager.Activate ("ManyManager.ExpandableDataAdapter+MyOnGroupExpandListener, ManyManager", "Android.Content.Context, Mono.Android:System.Int32, mscorlib:Android.Widget.ExpandableListView, Mono.Android", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void onGroupExpand (int p0)
	{
		n_onGroupExpand (p0);
	}

	private native void n_onGroupExpand (int p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
