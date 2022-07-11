package crc64751805a58b33156b;


public class ExpandableDataAdapter_MyOnGroupCollapseListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.widget.ExpandableListView.OnGroupCollapseListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onGroupCollapse:(I)V:GetOnGroupCollapse_IHandler:Android.Widget.ExpandableListView/IOnGroupCollapseListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("ManyManager.ExpandableDataAdapter+MyOnGroupCollapseListener, ManyManager", ExpandableDataAdapter_MyOnGroupCollapseListener.class, __md_methods);
	}


	public ExpandableDataAdapter_MyOnGroupCollapseListener ()
	{
		super ();
		if (getClass () == ExpandableDataAdapter_MyOnGroupCollapseListener.class)
			mono.android.TypeManager.Activate ("ManyManager.ExpandableDataAdapter+MyOnGroupCollapseListener, ManyManager", "", this, new java.lang.Object[] {  });
	}

	public ExpandableDataAdapter_MyOnGroupCollapseListener (android.content.Context p0, int p1, android.widget.ExpandableListView p2)
	{
		super ();
		if (getClass () == ExpandableDataAdapter_MyOnGroupCollapseListener.class)
			mono.android.TypeManager.Activate ("ManyManager.ExpandableDataAdapter+MyOnGroupCollapseListener, ManyManager", "Android.Content.Context, Mono.Android:System.Int32, mscorlib:Android.Widget.ExpandableListView, Mono.Android", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void onGroupCollapse (int p0)
	{
		n_onGroupCollapse (p0);
	}

	private native void n_onGroupCollapse (int p0);

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
