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
	public class Note
	{
		//todo delete
		public int ID;
		public DateTime CreatedDate;
		public DateTime LastModifiedDate;
		public bool Checked;
		/// <summary>
		/// Unique
		/// </summary>
		public string Title;
		public string ShortDescription;
		public Tag Tag;
		public bool HasTotal;
		public bool HasMiniNotes;
		public NotesGlobals.NoteType NoteType;
		public double TotalGoal;
		public List<MiniNote> MiniNotesList;
		public bool Visible;

		public Note(int ID, DateTime CreatedDate, DateTime LastModifiedDate, string Title, string ShortDescription, Tag Tag, bool HasTotal, NotesGlobals.NoteType NoteType, double TotalGoal, bool HasMiniNotes, List<MiniNote> NotesList)
		{
			this.ID = ID;
			this.CreatedDate = CreatedDate;
			this.LastModifiedDate = LastModifiedDate;
			this.Checked = false;
			this.Visible = true;
			this.Title = Title;
			this.ShortDescription = ShortDescription;
			this.Tag = Tag;
			this.HasTotal = HasTotal;
			this.HasMiniNotes = HasMiniNotes;
			this.NoteType = NoteType;
			this.TotalGoal = TotalGoal;
			this.MiniNotesList = NotesList;
		}

		public Note(DateTime CreatedDate, DateTime LastModifiedDate, string Title, string ShortDescription, Tag Tag, bool HasTotal, NotesGlobals.NoteType NoteType, double TotalGoal, bool HasMiniNotes, List<MiniNote> NotesList)
		{
			ID = 1;
			foreach (Note note in NotesGlobals.NotesList)
			{
				if (note.ID == ID)
				{
					ID++;
				}
			}
			this.CreatedDate = CreatedDate;
			this.LastModifiedDate = LastModifiedDate;
			this.Title = Title;
			this.ShortDescription = ShortDescription;
			this.Tag = Tag;
			this.HasTotal = HasTotal;
			this.HasMiniNotes = HasMiniNotes;
			this.NoteType = NoteType;
			this.TotalGoal = TotalGoal;
			this.MiniNotesList = NotesList;
			this.Visible = true;
			this.Checked = false;
		}
	}
}