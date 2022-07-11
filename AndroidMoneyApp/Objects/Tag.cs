using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi.Aware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SkiaSharp;

namespace ManyManager
{
	public class Tag
	{
		//todo delete
		public int ID;
		public bool Checked;
		/// <summary>
		/// Unique
		/// </summary>
		public string Name;
		public SKColor Color;
		public List<Car> Cars = new List<Car>();
		public List<Note> Notes = new List<Note>();

		public List<string> CarsFromXML = new List<string>();
		public List<string> NotesFromXML = new List<string>();

		public Tag()
		{
			ID = 1;
			foreach (Tag tag in TagsGlobals.TagsList)
			{
				if (tag.ID == ID)
				{
					ID++;
				}
			}
		}

		public Tag(Tag OldTag)
		{
			ID = 1;
			foreach (Tag tag in TagsGlobals.TagsList)
			{
				if (tag.ID == ID)
				{
					ID++;
				}
			}
			this.Checked = OldTag.Checked;
			this.Color = OldTag.Color;
			this.Name = OldTag.Name;
			this.Cars = new List<Car>();
			CarsGlobals.MoveElements(OldTag.Cars, this.Cars);
			NotesGlobals.MoveElements(OldTag.Notes, this.Notes);
		}

		public Tag(int ID, string Name, SKColor Color, List<string> CarsFromXML, List<string> NotesFromXML)
		{
			this.ID = ID;
			this.Checked = false;
			this.Color = Color;
			this.Name = Name.ToUpper();
			this.CarsFromXML = CarsFromXML;
			this.NotesFromXML = NotesFromXML;
		}

		public Tag(int ID, string Name, SKColor Color, List<Car> Cars, List<Note> Notes)
		{
			ID = 1;
			foreach (Tag tag in TagsGlobals.TagsList)
			{
				if (tag.ID == ID)
				{
					ID++;
				}
			}
			this.ID = ID;
			this.Checked = false;
			this.Color = Color;
			this.Name = Name.ToUpper();
			this.Cars = Cars;
			this.Notes = Notes;
		}

		public Tag(string Name, SKColor Color, List<Car> Cars, List<Note> Notes)
		{
			ID = 1;
			foreach (Tag tag in TagsGlobals.TagsList)
			{
				if (tag.ID == ID)
				{
					ID++;
				}
			}
			this.Checked = false;
			this.Color = Color;
			this.Name = Name.ToUpper();
			this.Cars = Cars;
			this.Notes = Notes;
		}
	}
}