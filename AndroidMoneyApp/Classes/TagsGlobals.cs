using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Camera2;
using Android.Net.Wifi.Aware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;

namespace ManyManager
{
	public static class TagsGlobals
	{
		#region Internal Properties

		internal static List<Tag> tempTagsList = new List<Tag>();

		#endregion

		#region Public Properties


		/// <summary>
		/// Used To Store Tags Name
		/// </summary>
		public static List<string> TagsNameList = new List<string>();

		public static List<Tag> DisplayTagsList = new List<Tag>();

		/// <summary>
		/// Store all Tags
		/// </summary>
		public static List<Tag> TagsList
		{
			get
			{
				return tempTagsList;
			}
			set
			{
				tempTagsList = value;

				TagsNameList = new List<string>();
				foreach (Tag tag in value)
				{
					TagsNameList.Add(tag.Name);
				}
			}
		}

		/// <summary>
		/// Tags to work with
		/// </summary>
		public static Tag SelectedTag;
		public static Tag PendingTag;

		public static bool TagsEditState = false;
		public static bool AddNewTag = false;

		#endregion

		#region Public Methods

		public static void WhatTagToDisplay()
		{
			if (Globals.CurrentForm == Globals.CarMenu)
			{
				CarsGlobals.WhatCarStuffToDisplay();
			}
			else if (Globals.CurrentForm == Globals.TagsMenu)
			{
				Globals.txtNoTags.Visibility = DisplayTagsList.Count > 0 ? ViewStates.Invisible : ViewStates.Visible;
				Globals.grdTags.Visibility = DisplayTagsList.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
			}
		}

		/// <summary>
		/// Used to Add tag to Global List and update TagsNameList
		/// </summary>
		/// <param name="tag"></param>
		public static void TagsListAdd(Tag tag)
		{
			TagsList.Add(tag);
			TagsList = TagsList;
		}

		/// <summary>
		/// Used to Add tag to Global List and update TagsNameList
		/// </summary>
		/// <param name="tag"></param>
		public static void TagsListRemove(Tag tag)
		{
			TagsList.Remove(tag);
			TagsList = TagsList;
		}

		public static void RemoveNotesAndCarsFromTag(Tag tag)
		{
			Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
			alert.SetTitle("Delete Tag");
			alert.SetMessage("By Removing " + tag.Name + " It Will Be Removed From All Cars, Notes And Transactions Assigned. Proceed ?");
			alert.SetPositiveButton("Yes", (senderAlert, args) =>
			{
				//TagsList.Remove(tag);
				//DisplayTagsList.Remove(tag);

				foreach (Note note in NotesGlobals.NotesList)
				{
					if (note.Tag != null && TagMatch(note.Tag, tag))
					{
						note.Tag = null;
					}
				}
				foreach (Car car in CarsGlobals.CarsList)
				{
					if (car.Tag != null && TagMatch(car.Tag, tag))
					{
						car.Tag = null;
					}
				}

				//remove tag from global list
				foreach (Tag item in TagsList.Reverse<Tag>())
				{
					if (TagMatch(item, tag))
					{
						TagsList.Remove(item);
						break;
					}
				}
				//remove tag from displaying
				foreach (Tag item in DisplayTagsList.Reverse<Tag>())
				{
					if (TagMatch(item, tag))
					{
						DisplayTagsList.Remove(item);
						break;
					}
				}
				//find & remove from xml
				XML.RefreshTags();

				//refresh all transactions from XML
				foreach (Month month in Globals.MonthsList)
				{
					foreach (Transaction transaction in month.Transactions)
					{
						if (transaction.Tag != null)
						{
							transaction.Tag = SearchOrAddNewTag(transaction.Tag.Name, false, false);
						}
					}
				}
				//save new transactions in XML
				for (int i = 0; i < Globals.MonthsList.Count; i++)
				{
					XML.SaveMonth(Globals.MonthsList[i]);
				}
				//and then reload transactions to be changed in global list of transactions
				XML.LoadYears(true);

				Globals.ReLoadNotesAndCars();

				(Globals.grdTags.Adapter as TagsAdapter).EditState();
				(Globals.grdTags.Adapter as TagsAdapter).CheckAll();
				(Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter(DisplayTagsList);
				WhatTagToDisplay();
			});
			alert.SetNegativeButton("No", (senderAlert, args) =>
			{
				(Globals.grdTags.Adapter as TagsAdapter).EditState();
				(Globals.grdTags.Adapter as TagsAdapter).CheckAll();
				(Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter(DisplayTagsList);
				WhatTagToDisplay();
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		/// <summary>
		/// XML set to -1 and "" when doesn't have one
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static Tag SearchTag(int ID = -1, string Name = "")
		{
			if (ID == -1 && string.IsNullOrWhiteSpace(Name))
			{
				return null;
			}
			if (TagsList != null && TagsList.Count > 0)
			{
				foreach (Tag item in TagsList)
				{
					if ((ID == -1 || item.ID == ID) && item.Name == Name)
					{
						return item;
					}
				}
			}
			return null;
		}

		public static Tag SearchOrAddNewTag(string Name, bool prompt = true, bool AddTag = true)
		{
			if (Name == "")
			{
				return null;
			}
			Name = Name.ToUpper();
			if (TagsList != null)
			{
				foreach (Tag item in TagsList)
				{
					if (item.Name == Name)
					{
						return item;
					}
				}
				//if (prompt)
				//{
				//	Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
				//	alert.SetTitle("");
				//	alert.SetMessage("Inserted Tag Doesn't Exist. Do You Want To Create New Tag?");
				//	alert.SetPositiveButton("Yes", (senderAlert, args) =>
				//	{
				//		TagsList.Add(new Tag(Name, SKColor.Empty, new List<Car>(), new List<Note>()));
				//	});
				//	alert.SetNegativeButton("No", (senderAlert, args) =>
				//	{
				//	});
				//	Dialog dialog = alert.Create();
				//	dialog.Show();
				//}
				//else
				//{
				//	//comes from XML and we need to create and to add it
				if (AddTag)
				{
					Tag newtag = new Tag(Name, SKColor.Empty, new List<Car>(), new List<Note>());
					TagsListAdd(newtag);
					if (prompt)
					{
						Utils.ShowToast("New Tag: " + Name + " Was Added");
					}
					return newtag;
				}
				//}
			}
			return null;
		}

		public static bool TagInList(List<Tag> list, Tag obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (TagMatch(list[i], obj))
				{
					return true;
				}
			}
			return false;
		}

		public static bool TagMatch(Tag t1, Tag t2)
		{
			if (t1 == null && t2 == null)
			{
				return true;
			}
			else if (t1 == null || t2 == null)
			{
				return false;
			}
			if (t1.ID != t2.ID)
			{
				return false;
			}
			else if (t1.Name != t2.Name)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static void CheckAll(bool check = false, List<Tag> list = null)
		{
			if (list == null)
			{
				list = TagsList;
			}
			foreach (Tag tag in list)
			{
				tag.Checked = check;
			}
		}

		#endregion
	}
}