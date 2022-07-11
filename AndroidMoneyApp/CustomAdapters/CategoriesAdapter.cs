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
	public class CategoriesAdapter : BaseAdapter<Category>
	{
		Activity CurrentAdapterActivity;
		List<Category> listCategories;
		/// <summary>
		/// Used to refresh Design when doing back changes
		/// </summary>
		public int indexOnce = -1;

		public CategoriesAdapter(Activity currentContext, List<Category> lstCategoryInfo)
		{
			CurrentAdapterActivity = currentContext;
			listCategories = lstCategoryInfo;
		}


		#region Events

		private void chkGridCategoriesSelectionCheck_Click(object sender, EventArgs e)
		{
			CheckBox t = sender as CheckBox;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtName = lay.FindViewById<TextView>(Id.txtGridCategoryItemName);
				if (!string.IsNullOrWhiteSpace(txtName.Text))
				{
					if (Globals.ParentForm != Globals.MainMenu)
					{
						listCategories.ForEach(x => x.Checked = false);
						if (!t.Checked)
						{
							indexOnce = -1;
							(Globals.grdCategories.Adapter as CategoriesAdapter).BackToDesignCheck();
							return;
						}
					}
					for (int i = listCategories.Count - 1; i >= 0; i--)
					{
						if (listCategories[i].Name == txtName.Text)
						{
							listCategories[i].Checked = t.Checked;
							break;
						}
					}
					if (Globals.ParentForm != Globals.MainMenu)
					{
						indexOnce = -1;
						(Globals.grdCategories.Adapter as CategoriesAdapter).BackToDesignCheck();
					}
				}
			}
		}

		public void LayoutClickableCategory_LongClick(object sender, EventArgs e)
		{
			//enter edit tag state
			if (Globals.CurrentForm == Globals.CategoriesMenu)
			{
				EditState(true);
			}
			else
			{
				CategoriesGlobals.CategoryEditState = true;
			}
			UpdateAdapter();
		}

		private void imgModifyCategory_Click(object sender, EventArgs e)
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
				TextView txtName = lay.FindViewById<TextView>(Id.txtGridCategoryItemName);
				if (!string.IsNullOrWhiteSpace(txtName.Text))
				{
					for (int i = listCategories.Count - 1; i >= 0; i--)
					{
						if (listCategories[i].Name == txtName.Text)
						{
							CategoriesGlobals.SelectedCategory = listCategories[i];
							Globals.Activity.SetContentView(Globals.CategoryMenu);
							break;
						}
					}
				}
			}
		}

		private void imgRemoveCategory_Click(object sender, EventArgs e)
		{
			ImageView t = sender as ImageView;
			if (t.Parent is LinearLayout lay)
			{
				TextView txtName = lay.FindViewById<TextView>(Id.txtGridCategoryItemName);
				if (!string.IsNullOrWhiteSpace(txtName.Text))
				{
					for (int i = listCategories.Count - 1; i >= 0; i--)
					{
						if (listCategories[i].Name == txtName.Text)
						{
							Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(Globals.Context);
							alert.SetTitle("Remove Category");
							alert.SetMessage("Are You Sure You Want To Remove This Category ?");
							alert.SetPositiveButton("Yes", (senderAlert, args) =>
							{
								CategoriesGlobals.RemoveItem(listCategories[i]);
							});
							alert.SetNegativeButton("No", (senderAlert, args) =>
							{
								EditState();
								CheckAll();
								(Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter(listCategories);
								CategoriesGlobals.WhatCategoryToDisplay();
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
			if (indexOnce < Globals.grdCategories.ChildCount)
			{
				indexOnce = Globals.grdCategories.ChildCount;
				for (int i = 0; i < Globals.grdCategories.ChildCount; i++)
				{
					LinearLayout tag = Globals.grdCategories.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableCategory);
					CheckBox chk = tag.FindViewById<CheckBox>(Id.chkGridCategorySelectionCheck);
					chk.Checked = listCategories[i].Checked;
				}
				CategoriesGlobals.DisplayCategoriesList = listCategories;
				(Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter();
			}
		}

		public void DesignToBackCheck()
		{
			if (indexOnce < Globals.grdCategories.ChildCount)
			{
				indexOnce = Globals.grdCategories.ChildCount;
				for (int i = 0; i < Globals.grdCategories.ChildCount; i++)
				{
					LinearLayout tag = Globals.grdCategories.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableCategory);
					CheckBox chk = tag.FindViewById<CheckBox>(Id.chkGridCategorySelectionCheck);
					listCategories[i].Checked = chk.Checked;
				}
				CategoriesGlobals.DisplayCategoriesList = listCategories;
				(Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter();
			}
		}

		public void CheckAll(bool checkState = false, string name = "")
		{
			CategoriesGlobals.CheckAll(checkState, listCategories);
			foreach (Category category in listCategories)
			{
				if (category.Name == name)
				{
					category.Checked = true;
				}
			}
			for (int i = 0; i < Globals.grdCategories.ChildCount; i++)
			{
				LinearLayout Category = Globals.grdCategories.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableCategory);
				if (!string.IsNullOrEmpty(name))
				{
					TextView txtName = Category.FindViewById<TextView>(Id.txtGridCategoryItemName);
					if (txtName.Text == name)
					{
						CheckBox chk = Category.FindViewById<CheckBox>(Id.chkGridCategorySelectionCheck);
						chk.Checked = true;
					}
				}
				else
				{
					CheckBox chk = Category.FindViewById<CheckBox>(Id.chkGridCategorySelectionCheck);
					chk.Checked = checkState;
				}
			}

			CategoriesGlobals.DisplayCategoriesList = listCategories;
			(Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter();
		}

		public void RemoveSelected()
		{
			for (int i = listCategories.Count - 1; i >= 0; i--)
			{
				if (listCategories[i].Checked)
				{
					CategoriesGlobals.RemoveItem(listCategories[i]);
				}
			}
		}

		public void RevertChanges()
		{
			CategoriesGlobals.CheckAll(false, listCategories);

			for (int i = 0; i < Globals.grdCategories.ChildCount; i++)
			{
				LinearLayout tag = Globals.grdCategories.GetChildAt(i).FindViewById<LinearLayout>(Id.LayoutClickableCategory);
				CheckBox chk = tag.FindViewById<CheckBox>(Id.chkGridCategorySelectionCheck);
				chk.Checked = false;
			}
			DesignToBackCheck();
			CategoriesGlobals.DisplayCategoriesList = listCategories;
			(Globals.grdCategories.Adapter as CategoriesAdapter).UpdateAdapter();
		}

		public void EditState(bool variable = false)
		{
			if (variable)
			{
				CategoriesGlobals.CategoryEditState = true;
				Globals.layoutAddNewCategory.Visibility = ViewStates.Invisible;
				Globals.imgDoneCategories.Visibility = ViewStates.Visible;
				Globals.imgRemoveCategory.Visibility = ViewStates.Visible;
				Globals.chkCategoriesCheckAll.Visibility = ViewStates.Visible;
			}
			else
			{
				CategoriesGlobals.CategoryEditState = false;
				Globals.layoutAddNewCategory.Visibility = ViewStates.Visible;
				Globals.imgDoneCategories.Visibility = ViewStates.Invisible;
				Globals.imgRemoveCategory.Visibility = ViewStates.Invisible;
				Globals.chkCategoriesCheckAll.Visibility = ViewStates.Invisible;
				Globals.chkCategoriesCheckAll.Checked = false;
			}
		}

		public void UpdateAdapter(List<Category> categoriesList = null)
		{
			if (categoriesList == null)
			{
				categoriesList = CategoriesGlobals.DisplayCategoriesList;
			}
			//refresh
			this.listCategories = categoriesList;
			CategoriesGlobals.DisplayCategoriesList = categoriesList;
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
			Category item = listCategories[position];

			if (convertView == null)
				convertView = CurrentAdapterActivity.LayoutInflater.Inflate(Globals.GridViewCategoryItem, null);

			(Globals.grdCategories.Adapter as CategoriesAdapter).BackToDesignCheck();

			CheckBox checkbox = convertView.FindViewById<CheckBox>(Id.chkGridCategorySelectionCheck);
			checkbox.Visibility = CategoriesGlobals.CategoryEditState ? ViewStates.Visible : ViewStates.Invisible;
			checkbox.Checked = item.Checked;

			ImageView imageRemove = convertView.FindViewById<ImageView>(Id.imgRemoveCategory);
			imageRemove.Visibility = CategoriesGlobals.CategoryEditState ? ViewStates.Visible : ViewStates.Invisible;

			ImageView imgModifyCategory = convertView.FindViewById<ImageView>(Id.imgModifyCategory);
			imgModifyCategory.Visibility = ViewStates.Visible;

			TextView txtItemName = convertView.FindViewById<TextView>(Id.txtGridCategoryItemName);

			txtItemName.Text = item.Name;

			//verify to have only one click event attachedg
			convertView.FindViewById<LinearLayout>(Id.LayoutClickableCategory).LongClick -= LayoutClickableCategory_LongClick;
			convertView.FindViewById<LinearLayout>(Id.LayoutClickableCategory).LongClick += LayoutClickableCategory_LongClick;
			if (!imgModifyCategory.HasOnClickListeners)
			{
				imgModifyCategory.Click += imgModifyCategory_Click;
			}
			if (!checkbox.HasOnClickListeners)
			{
				checkbox.Click += chkGridCategoriesSelectionCheck_Click;
			}
			if (!imageRemove.HasOnClickListeners)
			{
				imageRemove.Click += imgRemoveCategory_Click;
			}

			return convertView;
		}

		public override int Count
		{
			get
			{
				return listCategories == null ? -1 : listCategories.Count;
			}
		}

		public override Category this[int position]
		{
			get
			{
				return listCategories == null ? null : listCategories[position];
			}
		}

		#endregion

	}
}