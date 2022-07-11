using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ManyManager
{
	public class NotesAdapter : BaseAdapter<Note>
	{
		Activity CurrentNotesContext;
		List<Note> listNotes;
		public int indexOnce = -1;

		public NotesAdapter(Activity currentContext, List<Note> lstnotesInfo)
		{
			CurrentNotesContext = currentContext;
			listNotes = lstnotesInfo;
		}

		#region Events

		public void LayoutClickableNote_LongClick(object sender, EventArgs e)
		{
			//enter edit notes state
			EditState(true);
			UpdateAdapter(NotesGlobals.NotesList);
		}

		public void LayoutClickableNote_Click(object sender, EventArgs e)
		{
			if (NotesGlobals.NotesEditState)
			{
				// don't enter in mininote if edit state is on
				return;
			}
			//enter edit notes state
			EditState();

			//Enter in Note
			LinearLayout layout = sender as LinearLayout;
			TextView txtID = layout.FindViewById<TextView>(Resource.Id.txtGridNotesID);
			if (txtID != null)
			{
				NotesGlobals.SelectedNote = NotesGlobals.NotesList.Find(x=>x.ID == Convert.ToInt32(txtID.Text));
				if (NotesGlobals.SelectedNote != null)
				{
					if (NotesGlobals.SelectedNote.HasMiniNotes)
					{
						Globals.Activity.SetContentView(Globals.NoteMenu);
					}
					else
					{
						Globals.Activity.SetContentView(Globals.NoteSettings);
					}
				}
			}
		}

		public void imgGridNotesRemove_Click(object sender, EventArgs e)
		{
			Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
			alert.SetTitle("Remove?");
			alert.SetMessage("Are you sure you want to remove the note ?");
			alert.SetPositiveButton("Yes", (senderAlert, args) =>
			{
				ImageView t = sender as ImageView;
				TextView txtID = (t.Parent.Parent.Parent as RelativeLayout).FindViewById<TextView>(Resource.Id.txtGridNotesID);
				if (txtID.Text != "ID")
				{
					for (int i = listNotes.Count - 1; i >= 0; i--)
					{
						if (listNotes[i].ID == Convert.ToInt32(txtID.Text))
						{
							listNotes.RemoveAt(i);
							(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter(listNotes);
							XML.RefreshNotes();
							break;
						}
					}
				}
				EditState();
				CheckAll();
				(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
				NotesGlobals.WhatNoteToDisplay();
			});
			alert.SetNeutralButton("No", (senderAlert, args) =>
			{
				EditState();
				CheckAll();
				(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
				NotesGlobals.WhatNoteToDisplay();
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		private void chkGridNotesSelectionCheck_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			CheckBox t = sender as CheckBox;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Resource.Id.txtGridNotesID);
				if (txtID.Text != "ID")
				{
					for (int i = listNotes.Count - 1; i >= 0; i--)
					{
						if (listNotes[i].ID == Convert.ToInt32(txtID.Text))
						{
							listNotes[i].Checked = t.Checked;
							break;
						}
					}
					CheckCheckAllCheckBox();
				}
			}
		}

		#endregion

		#region Utils

		public void CheckCheckAllCheckBox()
		{
			if (Globals.chkNoteCheckAll != null)
			{
				Globals.doNothing = true;
				int checkboxes = listNotes.FindAll(x => x.Checked).Count;
				Globals.chkNoteCheckAll.Checked = listNotes.Count == 0 ? false : listNotes.Count == checkboxes;
				Globals.doNothing = false;
			}
		}

		public void BackToDesignCheck()
		{
			if (indexOnce < Globals.grdNotes.ChildCount)
			{
				indexOnce = Globals.grdNotes.ChildCount;
				for (int i = 0; i < Globals.grdNotes.ChildCount; i++)
				{
					RelativeLayout note = Globals.grdNotes.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfNote);
					CheckBox chk = note.FindViewById<CheckBox>(Resource.Id.chkGridNotesSelectionCheck);
					chk.Checked = listNotes[i].Checked;
				}
				NotesGlobals.NotesList = listNotes;
				(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
			}
		}

		public void DesignToBackCheck()
		{
			if (indexOnce < Globals.grdNotes.ChildCount)
			{
				indexOnce = Globals.grdNotes.ChildCount;
				for (int i = 0; i < Globals.grdNotes.ChildCount; i++)
				{
					RelativeLayout note = Globals.grdNotes.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfNote);
					CheckBox chk = note.FindViewById<CheckBox>(Resource.Id.chkGridNotesSelectionCheck);
					listNotes[i].Checked = chk.Checked;
				}
				NotesGlobals.NotesList = listNotes;
				(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
			}
		}

		public void NoteToAddInTag()
		{
			Globals.SelectedNotesForTag.Clear();
			foreach (Note note in listNotes)
			{
				if (note.Checked && note.Visible)
				{
					Globals.SelectedNotesForTag.Add(note);
				}
			}
			#region OldCode - Check to don't assign multiple tags to one note
				//string notes = string.Empty;
				//for (int i = listNotes.Count - 1; i >= 0; i--)
				//{
				//	bool found = false;
				//	foreach (Tag tag in TagsGlobals.TagsList)
				//	{
				//		if (listNotes[i].Checked && !TagsGlobals.TagMatch(TagsGlobals.SelectedTag, tag) && tag.Notes.Contains(listNotes[i]))
				//		{
				//			notes += listNotes[i].Title + " ";
				//			found = true;
				//		}
				//	}
				//	if (!found)
				//	{
				//		Globals.SelectedNotesForTag.Add(listNotes[i]);
				//	}
				//}
				//if (!string.IsNullOrEmpty(notes))
				//{
				//	Utils.ShowAlert("Can't Assign Note To Multiple Tags", "The Notes: " + notes + "Are Already Assigned To Different Tag!");
				//}
				#endregion
		}

		public void NoteToAddInTransaction()
		{
			Globals.SelectedNotesForTransaction.Clear();
			for (int i = listNotes.Count - 1; i >= 0; i--)
			{
				Globals.SelectedNotesForTransaction.Add(listNotes[i]);
			}
		}

		public void RevertChanges()
		{
			NotesGlobals.CheckAll(false, listNotes);

			for (int i = 0; i < Globals.grdNotes.ChildCount; i++)
			{
				RelativeLayout note = Globals.grdNotes.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfNote);
				CheckBox chk = note.FindViewById<CheckBox>(Resource.Id.chkGridNotesSelectionCheck);
				chk.Checked = false;
			}
			DesignToBackCheck();
			NotesGlobals.NotesList = listNotes;
			(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
		}

		public void CheckAll(bool checkState = false)
		{
			NotesGlobals.CheckAll(checkState, listNotes);

			for (int i = 0; i < Globals.grdNotes.ChildCount; i++)
			{
				RelativeLayout note = Globals.grdNotes.GetChildAt(i).FindViewById<RelativeLayout>(Resource.Id.linearLayoutHorizontalOfNote);
				CheckBox chk = note.FindViewById<CheckBox>(Resource.Id.chkGridNotesSelectionCheck);
				chk.Checked = checkState;
			}
			NotesGlobals.NotesList = listNotes;
			(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter();
		}

		public void RemoveSelected()
		{
			for (int i = listNotes.Count - 1; i >= 0; i--)
			{
				if (listNotes[i].Checked)
				{
					listNotes.RemoveAt(i);
				}
			}
			EditState();
			CheckAll();
			(Globals.grdNotes.Adapter as NotesAdapter).UpdateAdapter(listNotes);
			XML.RefreshNotes();
			NotesGlobals.WhatNoteToDisplay();
		}

		public void UpdateAdapter(List<Note> listNotes = null)
		{
			if (listNotes == null)
			{
				listNotes = NotesGlobals.NotesList;
			}
			//refresh
			this.listNotes = listNotes;
			NotesGlobals.NotesList = listNotes;
			NotifyDataSetChanged();
		}

		public void EditState(bool variable = false)
		{
			if (Globals.NoteToAddInTransaction || Globals.NoteToAddInTag)
			{
				NotesGlobals.NotesEditState = true;
				Globals.layoutAddNewNote.Visibility = ViewStates.Invisible;
				Globals.imgDoneNotes.Visibility = ViewStates.Visible;
				Globals.imgRemoveNote.Visibility = ViewStates.Invisible;
				Globals.chkNoteCheckAll.Visibility = ViewStates.Visible;
			}
			else if (variable)
			{
				NotesGlobals.NotesEditState = true;
				Globals.layoutAddNewNote.Visibility = ViewStates.Visible;
				Globals.imgDoneNotes.Visibility = ViewStates.Visible;
				Globals.imgRemoveNote.Visibility = ViewStates.Visible;
				Globals.chkNoteCheckAll.Visibility = ViewStates.Visible;
			}
			else
			{
				NotesGlobals.NotesEditState = false;
				Globals.layoutAddNewNote.Visibility = ViewStates.Visible;
				Globals.imgDoneNotes.Visibility = ViewStates.Invisible;
				Globals.imgRemoveNote.Visibility = ViewStates.Invisible;
				Globals.chkNoteCheckAll.Visibility = ViewStates.Invisible;
				Globals.chkNoteCheckAll.Checked = false;
			}
		}

		#endregion

		#region implemented abstract members of BaseAdapter

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			Note item = listNotes[position];

			if (item == null)
			{
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).Dispose();
			}
			if (convertView == null)
				convertView = CurrentNotesContext.LayoutInflater.Inflate(Globals.GridViewNoteItem, null);
			
			convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).Visibility = !item.Visible ? ViewStates.Gone : ViewStates.Visible;

			if (convertView.FindViewById<TextView>(Resource.Id.txtGridNotesID).Text == "ID")
			{
				convertView.FindViewById<TextView>(Resource.Id.txtGridNotesID).Text = item.ID.ToString();
			}
			(Globals.grdNotes.Adapter as NotesAdapter).BackToDesignCheck();

			CheckBox checkbox = convertView.FindViewById<CheckBox>(Resource.Id.chkGridNotesSelectionCheck);

			checkbox.Visibility = NotesGlobals.NotesEditState ? ViewStates.Visible : ViewStates.Invisible;
			checkbox.Checked = item.Checked;
			convertView.FindViewById<TextView>(Resource.Id.txtGridNotesTitle).Text = item.Title;
			convertView.FindViewById<TextView>(Resource.Id.txtGridNotesLastModified).Text = item.LastModifiedDate.ToShortDateString();
			convertView.FindViewById<ImageView>(Resource.Id.imgGridNotesRemove).Visibility = NotesGlobals.NotesEditState ? ViewStates.Visible : ViewStates.Invisible;

			Globals.layoutAddNewNote.Visibility = NotesGlobals.NotesEditState ? ViewStates.Invisible : ViewStates.Visible;
			Globals.imgDoneNotes.Visibility = NotesGlobals.NotesEditState ? ViewStates.Visible : ViewStates.Invisible;

			//verify to have only one click event attached
			if (NotesGlobals.NotesEditState)
			{
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).LongClick -= LayoutClickableNote_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).LongClick += LayoutClickableNote_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).Click -= LayoutClickableNote_Click;
			}
			else if (!NotesGlobals.NotesEditState)
			{
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).LongClick -= LayoutClickableNote_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).LongClick += LayoutClickableNote_LongClick;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).Click -= LayoutClickableNote_Click;
				convertView.FindViewById<LinearLayout>(Resource.Id.LayoutClickableNote).Click += LayoutClickableNote_Click;
			}
			if (!checkbox.HasOnClickListeners)
			{
				checkbox.CheckedChange += chkGridNotesSelectionCheck_CheckedChange;
			}
			if (!convertView.FindViewById<ImageView>(Resource.Id.imgGridNotesRemove).HasOnClickListeners)
			{
				convertView.FindViewById<ImageView>(Resource.Id.imgGridNotesRemove).Click += imgGridNotesRemove_Click;
			}
			return convertView;
		}

		public override int Count
		{
			get
			{
				return listNotes == null ? -1 : listNotes.Count;
			}
		}

		public override Note this[int position]
		{
			get
			{
				return listNotes == null ? null : listNotes[position];
			}
		}

		#endregion

	}
}