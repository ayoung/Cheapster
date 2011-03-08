using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;
using Cheapster.Data;
using Cheapster.Data.Models;

namespace Cheapster.ViewControllers.Shared
{
	public class UnitPicker : UIPickerView
	{
		private List<UnitModel> _units;
		private UnitPickerDelegate _delegate;
		public event EventHandler OnSelectionChanged;
		
		public UnitPicker(int unitTypeId, RectangleF frame) : base(frame)
		{
			UnitTypeId = unitTypeId;
			_units = DataService.GetUnits(UnitTypeId);
			_delegate = new UnitPickerDelegate(this);
			Delegate = _delegate;
			DataSource = new UnitPickerDataSource(this);
			ShowSelectionIndicator = true;
		}
		
		public int UnitTypeId { get; private set; }

		public List<UnitModel> Units {
			get 
			{
				return _units;
			}
		}
		
		public void SetUnitType(int unitTypeId)
		{
			UnitTypeId = unitTypeId;
			_units = DataService.GetUnits(UnitTypeId);
			_delegate.SelectedRow = 0;
			ReloadAllComponents();
		}
		
		/// <summary>
		/// this method should not be used outside of this picker and it's datasource and delegate
		/// </summary>
		public void FireOnSelectionChanged()
		{
			OnSelectionChanged.Fire(this, EventArgs.Empty);
		}
		
		public void SetSelectedUnit(int unitId)
		{
			var unit = (from u in _units
				where u.Id == unitId
				select u).First();
			_delegate.SelectedRow = _units.IndexOf(unit);
			Select(_delegate.SelectedRow, 0, true);
		}
		
		public UnitModel SelectedUnit
		{
			get 
			{
				return _units[_delegate.SelectedRow];
			}
		}

		private class UnitPickerDataSource : UIPickerViewDataSource
		{
			private UnitPicker _picker;

			public UnitPickerDataSource(UnitPicker picker)
			{
				_picker = picker;
			}

			public override int GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override int GetRowsInComponent(UIPickerView pickerView, int component)
			{
				return _picker.Units.Count;
			}
		}

		private class UnitPickerDelegate : UIPickerViewDelegate
		{
			private UnitPicker _picker;

			public UnitPickerDelegate(UnitPicker picker)
			{
				_picker = picker;
				SelectedRow = 0;
			}

			public override string GetTitle(UIPickerView pickerView, int row, int component)
			{
				return _picker.Units[row].FullName;
			}

			public override void Selected(UIPickerView pickerView, int row, int component)
			{
				SelectedRow = row;
				_picker.FireOnSelectionChanged();
			}

			public int SelectedRow { get; set; }
		}
	}
}

