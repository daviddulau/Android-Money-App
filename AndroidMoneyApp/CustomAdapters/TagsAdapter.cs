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
using static ManyManager.Resource;

namespace ManyManager
{
	public class TagsAdapter : BaseAdapter<Tag>
	{
		Activity CurrentAdapterActivity;
		List<Tag> listTags;
		/// <summary>
		/// Used to refresh Design when doing back changes
		/// </summary>
		public int indexOnce = -1;

		public TagsAdapter(Activity currentContext, List<Tag> lstTagInfo)
		{
			CurrentAdapterActivity = currentContext;
			listTags = lstTagInfo;
		}


		#region Events

		private void chkGridTagsSelectionCheck_Click(object sender, EventArgs e)
		{
			CheckBox t = sender as CheckBox;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridTagItemID);
				if (txtID.Text != "ID")
				{
					if (Globals.ParentForm != Globals.MainMenu)
					{
						listTags.ForEach(x => x.Checked = false);
						if (!t.Checked)
						{
							indexOnce = -1;
							(Globals.grdTags.Adapter as TagsAdapter).BackToDesignCheck();
							return;
						}
					}
					for (int i = listTags.Count - 1; i >= 0; i--)
					{
						if (listTags[i].ID == Convert.ToInt32(txtID.Text))
						{
							listTags[i].Checked = t.Checked;
							break;
						}
					}
					if (Globals.ParentForm != Globals.MainMenu)
					{
						indexOnce = -1;
						(Globals.grdTags.Adapter as TagsAdapter).BackToDesignCheck();
					}
				}
			}
		}

		public void LayoutClickableTag_LongClick(object sender, EventArgs e)
		{
			//enter edit tag state
			if (Globals.CurrentForm == Globals.TagsMenu)
			{
				EditState(true);
			}
			else
			{
				TagsGlobals.TagsEditState = true;
			}
			UpdateAdapter();
		}

		private void imgModifyTag_Click(object sender, EventArgs e)
		{
			//enter edit tag state
			//don't change editstate because we are maybe in CarMenu Form and we don't have just the grid
			if (Globals.CurrentForm != Globals.CarMenu)
			{
				EditState();
			}

			ImageView t = sender as ImageView;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridTagItemID);
				if (txtID.Text != "ID")
				{
					for (int i = listTags.Count - 1; i >= 0; i--)
					{
						if (listTags[i].ID == Convert.ToInt32(txtID.Text))
						{
							TagsGlobals.SelectedTag = listTags[i];
							Globals.TagsMenuParent = 0;

							int tempParent = Globals.CurrentForm;

							Globals.Activity.SetContentView(Globals.TagMenu);

							if (tempParent == Globals.CarMenu)
							{
								Globals.ParentForm = tempParent;
							}
							break;
						}
					}
				}
			}
		}

		private void imgRemoveTag_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtID = lay.FindViewById<TextView>(Id.txtGridTagItemID);
				if (txtID.Text != "ID")
				{
					for (int i = listTags.Count - 1; i >= 0; i--)
					{
						if (listTags[i].ID == Convert.ToInt32(txtID.Text))
						{
							Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
							alert.SetTitle("Remove Tag");
							alert.SetMessage("Are You Sure You Want To Remove This Tag ?");
							alert.SetPositiveButton("Yes", (senderAlert, args) =>
							{
								RemoveItem(listTags[i]);
							});
							alert.SetNegativeButton("No", (senderAlert, args) =>
							{
								EditState();
								CheckAll();
								(Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter(listTags);
								TagsGlobals.WhatTagToDisplay();
							});
							Dialog dialog = alert.Create();
							dialog.Show();
							break;
						}
					}
				}
			}
		}

		#endregion

		#region Utils

		public void BackToDesignCheck()
		{
			if (indexOnce < Globals.grdTags.ChildCount)
			{
				indexOnce = Globals.grdTags.ChildCount;
				for (int i = 0; i < Globals.grdTags.ChildCount; i++)
				{
					LinearLayout tag = Globals.grdTags.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableTag);
					CheckBox chk = tag.FindViewById<CheckBox>(Id.chkGridTagsSelectionCheck);
					chk.Checked = listTags[i].Checked;
				}
				TagsGlobals.DisplayTagsList = listTags;
				(Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter();
			}
		}

		public void DesignToBackCheck()
		{
			if (indexOnce < Globals.grdTags.ChildCount)
			{
				indexOnce = Globals.grdTags.ChildCount;
				for (int i = 0; i < Globals.grdTags.ChildCount; i++)
				{
					LinearLayout tag = Globals.grdTags.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableTag);
					CheckBox chk = tag.FindViewById<CheckBox>(Id.chkGridTagsSelectionCheck);
					listTags[i].Checked = chk.Checked;
				}
				TagsGlobals.DisplayTagsList = listTags;
				(Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter();
			}
		}

		public void CheckAll(bool checkState = false, string name = "")
		{
			TagsGlobals.CheckAll(checkState, listTags);
			foreach (Tag tag in listTags)
			{
				if (tag.Name == name)
				{
					tag.Checked = true;
				}
			}
			for (int i = 0; i < Globals.grdTags.ChildCount; i++)
			{
				LinearLayout Tag = Globals.grdTags.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableTag);
				if (!string.IsNullOrEmpty(name))
				{
					TextView txtName = Tag.FindViewById<TextView>(Id.txtGridTagItemName);
					if (txtName.Text == name)
					{
						CheckBox chk = Tag.FindViewById<CheckBox>(Id.chkGridTagsSelectionCheck);
						chk.Checked = true;
					}
				}
				else
				{
					CheckBox chk = Tag.FindViewById<CheckBox>(Id.chkGridTagsSelectionCheck);
					chk.Checked = checkState;
				}
			}

			TagsGlobals.DisplayTagsList = listTags;
			(Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter();
		}

		public void TagToAddInTransaction()
		{
			int count = listTags.FindAll(x => x.Checked).Count;
			if (count > 1)
			{
				Utils.ShowAlert("", "Can't Assign Multiple Tags To A Transaction");
			}
			else if (count == 0)
			{
				Globals.SelectedTagForTransaction = null;
			}
			else
			{
				Globals.SelectedTagForTransaction = listTags.Find(x=>x.Checked);
				MoneyGlobals.PendingTransaction.Tag = new Tag(Globals.SelectedTagForTransaction);
				MoneyGlobals.PendingTransaction.Tag.Cars.Clear();
				Globals.SelectedCarsForTransaction.Clear();
				MoneyGlobals.PendingTransaction.Tag.Notes.Clear();
				Globals.SelectedNotesForTransaction.Clear();
				//uncheck all before set right ones
				CarsGlobals.CheckAll(list: CarsGlobals.CarsList);
				foreach (Car car1 in Globals.SelectedTagForTransaction.Cars)
				{
					foreach (Car car2 in CarsGlobals.CarsList)
					{
						if (CarsGlobals.CarMatch(car1, car2))
						{
							car2.Checked = true;
							Globals.SelectedCarsForTransaction.Add(car2);
						}
					}
				}

				//uncheck all before set right ones
				NotesGlobals.CheckAll(list: NotesGlobals.NotesList);
				foreach (Note note1 in Globals.SelectedTagForTransaction.Notes)
				{
					foreach (Note note2 in NotesGlobals.NotesList)
					{
						if (NotesGlobals.NoteMatch(note1, note2))
						{
							note2.Checked = true;
							Globals.SelectedNotesForTransaction.Add(note2);
						}
					}
				}
			}
		}

		public void RemoveSelected()
		{
			for (int i = listTags.Count - 1; i >= 0; i--)
			{
				if (listTags[i].Checked)
				{
					RemoveItem(listTags[i]);
				}
			}
		}

		public void RevertChanges()
		{
			TagsGlobals.CheckAll(false, listTags);

			for (int i = 0; i < Globals.grdTags.ChildCount; i++)
			{
				LinearLayout tag = Globals.grdTags.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableTag);
				CheckBox chk = tag.FindViewById<CheckBox>(Id.chkGridTagsSelectionCheck);
				chk.Checked = false;
			}
			DesignToBackCheck();
			TagsGlobals.DisplayTagsList = listTags;
			(Globals.grdTags.Adapter as TagsAdapter).UpdateAdapter();
		}

		public void EditState(bool variable = false)
		{
			if (Globals.TagToAddInTransaction)
			{
				TagsGlobals.TagsEditState = true;
				Globals.layoutAddNewTag.Visibility = ViewStates.Invisible;
				Globals.imgDoneTags.Visibility = ViewStates.Visible;
				Globals.imgRemoveTag.Visibility = ViewStates.Invisible;
				Globals.chkTagsCheckAll.Visibility = ViewStates.Invisible;
			}
			else if (variable)
			{
				TagsGlobals.TagsEditState = true;
				Globals.layoutAddNewTag.Visibility = ViewStates.Invisible;
				Globals.imgDoneTags.Visibility = ViewStates.Visible;
				Globals.imgRemoveTag.Visibility = ViewStates.Visible;
				Globals.chkTagsCheckAll.Visibility = ViewStates.Visible;
			}
			else
			{
				TagsGlobals.TagsEditState = false;
				Globals.layoutAddNewTag.Visibility = ViewStates.Visible;
				Globals.imgDoneTags.Visibility = ViewStates.Invisible;
				Globals.imgRemoveTag.Visibility = ViewStates.Invisible;
				Globals.chkTagsCheckAll.Visibility = ViewStates.Invisible;
				Globals.chkTagsCheckAll.Checked = false;
			}
		}

		public void RemoveItem(Tag tag)
		{
			TagsGlobals.RemoveNotesAndCarsFromTag(tag);
		}

		public void UpdateAdapter(List<Tag> tagList = null)
		{
			if (tagList == null)
			{
				tagList = TagsGlobals.DisplayTagsList;
			}
			//refresh
			this.listTags = tagList;
			TagsGlobals.DisplayTagsList = tagList;
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
			Tag item = listTags[position];

			if (convertView == null)
				convertView = CurrentAdapterActivity.LayoutInflater.Inflate(Globals.GridViewTagItem, null);

			(Globals.grdTags.Adapter as TagsAdapter).BackToDesignCheck();

			CheckBox checkbox = convertView.FindViewById<CheckBox>(Id.chkGridTagsSelectionCheck);
			checkbox.Visibility = Globals.TagToAddInTransaction ? ViewStates.Visible : TagsGlobals.TagsEditState ? ViewStates.Visible : ViewStates.Invisible;
			checkbox.Checked = item.Checked;

			ImageView imageRemove = convertView.FindViewById<ImageView>(Id.imgRemoveTag);
			imageRemove.Visibility = Globals.TagToAddInTransaction ? ViewStates.Invisible : TagsGlobals.TagsEditState ? ViewStates.Visible : ViewStates.Invisible;

			ImageView imgModifyTag = convertView.FindViewById<ImageView>(Id.imgModifyTag);
			imgModifyTag.Visibility = Globals.TagToAddInTransaction ? ViewStates.Invisible : ViewStates.Visible;

			TextView txtItemName = convertView.FindViewById<TextView>(Id.txtGridTagItemName);
			TextView textItemID = convertView.FindViewById<TextView>(Id.txtGridTagItemID);

			textItemID.Text = Convert.ToString(item.ID);
			txtItemName.Text = item.Name;

			//verify to have only one click event attachedg
			convertView.FindViewById<LinearLayout>(Id.LayoutClickableTag).LongClick -= LayoutClickableTag_LongClick;
			convertView.FindViewById<LinearLayout>(Id.LayoutClickableTag).LongClick += LayoutClickableTag_LongClick;
			if (!imgModifyTag.HasOnClickListeners)
			{
				imgModifyTag.Click += imgModifyTag_Click;
			}
			if (!checkbox.HasOnClickListeners)
			{
				checkbox.Click += chkGridTagsSelectionCheck_Click;
			}
			if (!imageRemove.HasOnClickListeners)
			{
				imageRemove.Click += imgRemoveTag_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listTags == null ? -1 : listTags.Count;
			}
		}

		public override Tag this[int position]
		{
			get
			{
				return listTags == null ? null : listTags[position];
			}
		}

		#endregion

	}
}