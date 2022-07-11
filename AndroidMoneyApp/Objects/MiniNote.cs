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
	public class MiniNote
	{
		/// <summary>
		/// Unique
		/// </summary>
		public int ID;
		public string Description;
		public double Sum;
		public bool Checked;
		public bool HasTransaction;
		public Transaction Transaction;
		public Note ParentNote;

		public MiniNote(Note parentNote, Transaction transaction)
		{
			ID = 1;
			foreach (MiniNote note in parentNote.MiniNotesList)
			{
				if (note.ID == ID)
				{
					ID++;
				}
			}
			this.HasTransaction = true;
			this.Transaction = transaction;
			this.Sum = transaction.Sum;
			this.Description = transaction.Name + " " + transaction.Tag?.Name;
			this.ParentNote = parentNote;
		}

		public MiniNote(Note parentNote, int ID, Transaction transaction)
		{
			this.ID = ID;
			this.HasTransaction = true;
			this.Transaction = transaction;
			this.Sum = transaction.Sum;
			this.Description = transaction.Name + " " + transaction.Tag?.Name;
			this.ParentNote = parentNote;
		}

		public MiniNote(Note parentNote, int ID, string Description, double Sum, bool check)
		{
			this.ID = ID;
			this.Description = Description;
			this.Sum = Sum;
			this.Checked = check;
			this.HasTransaction = false;
			this.ParentNote = parentNote;
		}

		public MiniNote(Note parentNote, string Description, double Sum)
		{
			ID = 1;
			foreach (MiniNote note in parentNote.MiniNotesList)
			{
				if (note.ID == ID)
				{
					ID++;
				}
			}
			this.Description = Description;
			this.Sum = Sum;
			this.Checked = false;
			this.HasTransaction = false;
			this.ParentNote = parentNote;
		}
	}
}