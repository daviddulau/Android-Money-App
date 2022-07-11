using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ManyManager
{
	public class MiniNotesAdapter : BaseAdapter<MiniNote>
	{
		Activity CurrentAdapterContext;
		List<MiniNote> listMiniNotes;

		public MiniNotesAdapter(Activity currentContext, List<MiniNote> lstinsertInfo)
		{
			CurrentAdapterContext = currentContext;
			listMiniNotes = lstinsertInfo;
		}

		#region Events

		private void Delete_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Resource.Id.txtGridItemID);
				if (txtID.Text != "ID")
				{
					for (int i = listMiniNotes.Count - 1; i >= 0; i--)
					{
						if (listMiniNotes[i].ID == Convert.ToInt32(txtID.Text))
						{
							Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Globals.Context);
							alert.SetMessage("Are You Sure You Want To Remove This MiniNote?");
							alert.SetPositiveButton("Yes", (senderAlert, args) =>
							{
								Remove(i);
								CalculateSum();
								UpdateAdapter(NotesGlobals.SelectedNote.MiniNotesList);
							});
							alert.SetNegativeButton("No", (senderAlert, args) =>
							{
							});
							Dialog dialog = alert.Create();
							dialog.Show();
							break;
						}
					}
				}
			}
		}

		private void Modify_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Resource.Id.txtGridItemID);
				if (txtID.Text != "ID")
				{
					for (int i = listMiniNotes.Count - 1; i >= 0; i--)
					{
						if (listMiniNotes[i].ID == Convert.ToInt32(txtID.Text))
						{
							NotesGlobals.SelectedMiniNote = listMiniNotes[i];
							Globals.Activity.SetContentView(Globals.MiniNoteMenu);
							break;
						}
					}
				}
			}
		}

		private void CheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			CheckBox t = sender as CheckBox;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Resource.Id.txtGridItemID);
				if (txtID.Text != "ID")
				{
					for (int i = listMiniNotes.Count - 1; i >= 0; i--)
					{
						if (listMiniNotes[i].ID == Convert.ToInt32(txtID.Text))
						{
							if (listMiniNotes[i].Checked != t.Checked)
							{
								listMiniNotes[i].Checked = t.Checked;
								(Globals.grdMiniNotes.Adapter as MiniNotesAdapter).UpdateAdapter(NotesGlobals.SelectedNote.MiniNotesList);
								break;
							}
						}
					}
					CalculateSum();
					CheckCheckAllCheckBox();
				}
			}
		}

		#endregion

		#region Utils

		public void CheckCheckAllCheckBox()
		{
			if (Globals.chkAllMiniNotes != null)
			{
				Globals.doNothing = true;
				int checkboxes = listMiniNotes.FindAll(x=>x.Checked).Count;
				Globals.chkAllMiniNotes.Checked = listMiniNotes.Count == 0 ? false : listMiniNotes.Count == checkboxes;
				Globals.doNothing = false;
			}
		}

		public void CheckAll(bool checkState)
		{
			for (int i = listMiniNotes.Count - 1; i >= 0; i--)
			{
				listMiniNotes[i].Checked = checkState;
			}
			for (int i = 0; i < Globals.grdMiniNotes.ChildCount; i++)
			{
				LinearLayout mininote = Globals.grdMiniNotes.GetChildAt(i).FindViewById<LinearLayout>(Resource.Id.linearLayoutHorizontal);
				CheckBox chk = mininote.FindViewById<CheckBox>(Resource.Id.chkGridItem);
				chk.Checked = checkState;
			}

			CalculateSum();

			NotesGlobals.SelectedNote.MiniNotesList = listMiniNotes;
			(Globals.grdMiniNotes.Adapter as MiniNotesAdapter).UpdateAdapter(NotesGlobals.SelectedNote.MiniNotesList);
		}

		public void CalculateSum()
		{
			double sum = 0;
			foreach (MiniNote miniNote in listMiniNotes.FindAll(x=>x.Checked))
			{
				sum += miniNote.Sum;
			}
			Globals.txtNoteTotalSum.Text = sum.ToString("#,###,##0.00");

			//recalculate goal with the remove
			if (NotesGlobals.SelectedNote.NoteType == NotesGlobals.NoteType.Goal)
			{
				Globals.LayoutGoalProgress.Visibility = ViewStates.Visible;
				double amount = 0;
				foreach (MiniNote item in NotesGlobals.SelectedNote.MiniNotesList)
				{
					amount += item.Sum;
				}
				if (amount == 0 || NotesGlobals.SelectedNote.TotalGoal == 0)
				{
					Globals.pbGoalProgress.Progress = 0;
				}
				else
				{
					Globals.pbGoalProgress.Progress = Convert.ToInt32(amount / NotesGlobals.SelectedNote.TotalGoal * 100);
				}
				Globals.textviewGoalTotal.Text = "Total: " + NotesGlobals.SelectedNote.TotalGoal.ToString("#,###,##0.00");
				Globals.textviewGoalLeft.Text = "Left: " + (NotesGlobals.SelectedNote.TotalGoal - amount).ToString("#,###,##0.00");
			}
		}

		public void RemoveSelected()
		{
			for (int i = listMiniNotes.Count - 1; i >= 0; i--)
			{
				if (listMiniNotes[i].Checked)
				{
					Remove(i);
				}
			}
			UpdateAdapter(NotesGlobals.SelectedNote.MiniNotesList);
			CalculateSum();
		}

		public void Remove(int index)
		{
			listMiniNotes.RemoveAt(index);
		}

		public void UpdateAdapter(List<MiniNote> listMiniNotes)
		{
			//refresh
			this.listMiniNotes = listMiniNotes;
			NotesGlobals.SelectedNote.MiniNotesList = listMiniNotes;
			XML.RefreshNotes();
			NotifyDataSetChanged();
		}

		#endregion

		#region implemented abstract members of BaseAdapter

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			MiniNote item = listMiniNotes[position];

			if (convertView == null)
				convertView = CurrentAdapterContext.LayoutInflater.Inflate(Globals.GridViewMiniNoteItem, null);

			bool shopList = item.ParentNote.NoteType == NotesGlobals.NoteType.ShopList;

			TextView txtID = convertView.FindViewById<TextView>(Resource.Id.txtGridItemID);
			TextView sum = convertView.FindViewById<TextView>(Resource.Id.txtGridItemSum);
			TextView description = convertView.FindViewById<TextView>(Resource.Id.txtGridItemDescription);
			CheckBox checkBox = convertView.FindViewById<CheckBox>(Resource.Id.chkGridItem);
			ImageView modify = convertView.FindViewById<ImageView>(Resource.Id.imgModify);
			ImageView delete = convertView.FindViewById<ImageView>(Resource.Id.imgDelete);

			txtID.Text = item.ID.ToString();

			sum.Visibility = NotesGlobals.SelectedNote.HasTotal ? ViewStates.Visible : ViewStates.Invisible;
			sum.Text = item.Sum.ToString("#,###,##0.00");

			description.Text = item.Description.Substring(0, item.Description.Length > 50 ? 50 : item.Description.Length) + (item.Description.Length > 50 ? "..." : "");

			if (shopList)
			{
				checkBox.Checked = item.Checked;
				if (item.Checked)
				{
					sum.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
					description.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
				}
				else
				{
					sum.PaintFlags = Android.Graphics.PaintFlags.LinearText;
					description.PaintFlags = Android.Graphics.PaintFlags.LinearText;
				}
			}

			if (!checkBox.HasOnClickListeners)
			{
				checkBox.CheckedChange += CheckBox_CheckedChange;
			}

			if (!modify.HasOnClickListeners)
			{
				modify.Click += Modify_Click;
			}


			if (!delete.HasOnClickListeners)
			{
				delete.Click += Delete_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listMiniNotes == null ? -1 : listMiniNotes.Count;
			}
		}

		public override MiniNote this[int position]
		{
			get
			{
				return listMiniNotes == null ? null : listMiniNotes[position];
			}
		}

		#endregion

	}
}