using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ManyManager
{
	public static class NotesGlobals
	{

		#region Enums

		public enum NoteType
		{
			Note = 0,
			Goal = 1,
			ShopList = 2
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Used to show Notes
		/// </summary>
		public static List<Note> DisplayNotesList = new List<Note>();

		/// <summary>
		/// Store all Notes & theirs mininotes
		/// </summary>
		public static List<Note> NotesList = new List<Note>();

		/// <summary>
		/// Note to work with MiniNotes
		/// </summary>
		public static Note SelectedNote;
		public static MiniNote SelectedMiniNote;

		public static bool NotesEditState = false;

		#endregion

		#region Public Methods

		public static void WhatNoteToDisplay()
		{
			Globals.txtNoNotes.Visibility = NotesList.Count > 0 ? ViewStates.Invisible : ViewStates.Visible;
			Globals.grdNotes.Visibility = NotesList.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
		}

		public static void RemoveDuplicated(List<Note> list)
		{
			foreach (Note note in list.Reverse<Note>())
			{
				int count = NoteCountInList(list, note);
				if (count > 1)
				{
					list.Remove(note);
				}
			}
		}

		public static List<Note> MoveElements(List<Note> oldList, List<Note> newList)
		{
			foreach (Note element in oldList)
			{
				newList.Add(element);
			}
			return newList;
		}

		public static string AlocateNumber(string name)
		{
			int index = 1;
			foreach (Note note in NotesList)
			{
				if (note.Title.Contains(name + " " + index))
				{
					index++;
				}
			}
			return name + " " + index;
		}

		/// <summary>
		/// If list has the note will be added from that list. If doesn't exist will be searched in Global List
		/// </summary>
		/// <param name="list"></param>
		/// <param name="note"></param>
		public static void AddNoteToList(List<Note> list, Note note)
		{
			int i = NoteIndexInList(list, note);
			if (i != -1)
			{
				list.Remove(note);
				list.Add(note);
			}
			else
			{
				i = NoteIndexInList(NotesList, note);
				list.Add(NotesList[i]);
			}
		}

		public static int NoteCountInList(List<Note> list, Note obj)
		{
			int j = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (NoteMatch(list[i], obj))
				{
					j++;
				}
			}
			return j;
		}

		public static int NoteIndexInList(List<Note> list, Note obj)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (NoteMatch(list[i], obj))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool NoteInList(List<Note> list, Note note)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (NoteMatch(list[i], note))
				{
					return true;
				}
			}
			return false;
		}

		public static bool NoteMatch(Note n1, Note n2, bool checkMiniNotes = true)
		{
			if (n1.Title != n2.Title)
			{
				return false;
			}
			else if (n1.CreatedDate != n2.CreatedDate)
			{
				return false;
			}
			else if (n1.NoteType != n2.NoteType)
			{
				return false;
			}
			else if (n1.HasMiniNotes != n2.HasMiniNotes)
			{
				return false;
			}
			else if (n1.HasTotal != n2.HasTotal)
			{
				return false;
			}
			else if (n1.LastModifiedDate != n2.LastModifiedDate)
			{
				return false;
			}
			else if (n1.ShortDescription != n2.ShortDescription)
			{
				return false;
			}
			else if (checkMiniNotes)
			{
				bool found = true;
				foreach (MiniNote miniNote in n1.MiniNotesList)
				{
					if (!MiniNoteInList(n2.MiniNotesList, miniNote))
					{
						found = false;
						break;
					}
				}
				return found;
			}
			else
			{
				return true;
			}
		}

		public static bool MiniNoteInList(List<MiniNote> list, MiniNote mini)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (MiniNoteMatch(list[i], mini))
				{
					return true;
				}
			}
			return false;
		}

		public static bool MiniNoteMatch(MiniNote m1, MiniNote m2)
		{
			if (m1.Description != m2.Description)
			{
				return false;
			}
			else if (m1.Sum != m2.Sum)
			{
				return false;
			}
			else if (!NoteMatch(m1.ParentNote, m2.ParentNote, false))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="transaction"></param>
		public static void AssignTransactionToNote(Transaction transaction)
		{
			if (Globals.SelectedNotesForTransaction.Count > 0)
			{
				foreach (Note selectedNote in Globals.SelectedNotesForTransaction.Reverse<Note>())
				{
					if (Globals.SelectedNotesForTransaction.Count > 0)
					{
						//remove from list and then process it
						Globals.SelectedNotesForTransaction.Remove(selectedNote);

						bool found = false;
						if (selectedNote.Checked)
						{
							foreach (MiniNote mini in selectedNote.MiniNotesList)
							{
								if (MoneyGlobals.TransactionsMatch(transaction, mini.Transaction))
								{
									//there's already a mininote attached to this Note with the same transaction
									found = true;
									break;
								}
							}
							if (!found)
							{
								MiniNote miniNote = new MiniNote(selectedNote, transaction);
								selectedNote.MiniNotesList.Add(miniNote);
								transaction.MiniNotes.Add(miniNote);
								break;
							}
						}
						else if (!selectedNote.Checked)
						{
							foreach (MiniNote miniNote in selectedNote.MiniNotesList.Reverse<MiniNote>())
							{
								if (miniNote.Transaction != null && MoneyGlobals.TransactionsMatch(transaction, miniNote.Transaction) && !selectedNote.Checked)
								{
									miniNote.Transaction.MiniNotes.Remove(miniNote);
									selectedNote.HasMiniNotes = true;
									selectedNote.MiniNotesList.Remove(miniNote);
									break;
								}
							}
						}
					}
				}
			}
			else
			{
				//if no modification and Globals.SelectedNotesForTransaction.Count is 0 check if there's already attached and don't remove by default
				bool found = false;
				foreach (Note note in NotesList)
				{
					foreach (MiniNote miniNote in note.MiniNotesList.Reverse<MiniNote>())
					{
						if (miniNote.Transaction != null && MoneyGlobals.TransactionsMatch(transaction, miniNote.Transaction) && !note.Checked)
						{
							miniNote.Transaction.MiniNotes.Remove(miniNote);
							note.HasMiniNotes = true;
							note.MiniNotesList.Remove(miniNote);
							if (miniNote.Transaction.MiniNotes.Count == 0)
							{
								found = true;
							}
						}
					}
					if (found)
					{
						break;
					}
				}
			}
			XML.RefreshNotes();
			Globals.SelectedNotesForTransaction.Clear();
			Globals.NoteToAddInTransaction = false;
			MoneyGlobals.PendingTransaction = null;
		}

		public static void VisibleAll(bool visible = true, List<Note> list = null)
		{
			if (list == null)
			{
				list = NotesList;
			}
			foreach (Note note in list)
			{
				note.Visible = visible;
			}
		}

		public static void CheckAll(bool check = false, List <Note> list = null)
		{
			if (list == null)
			{
				list = NotesList;
			}
			foreach (Note note in list)
			{
				note.Checked = check;
			}
		}

		#endregion

	}
}