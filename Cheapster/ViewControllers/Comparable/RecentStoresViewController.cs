using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Cheapster.Data;
using Cheapster.Data.Models;

namespace Cheapster.ViewControllers.Comparable
{
	public class RecentStoresViewController : UIViewController
	{
		private UITableView _tableView;
		private TableSource _source;
		private Action<string> _storeSelectedCallback;
			
		public RecentStoresViewController(Action<string> storeSelectedCallback)
		{
			_storeSelectedCallback = storeSelectedCallback;
		}
		
		public override void LoadView()
		{
			base.LoadView();
			Title = "Stores";
			_tableView = new UITableView(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height), UITableViewStyle.Grouped);
			_source = new TableSource(_storeSelectedCallback);
			_tableView.Source = _source;
			View.AddSubview(_tableView);
			_storeSelectedCallback = null;
		}
		
		private class TableSource : UITableViewSource
		{
			private List<RecentStore> _stores;
			private const string _reuseIdentifier = "recentstorecell";
			private Action<string> _callback;
			
			public TableSource(Action<string> callback)
			{
				_callback = callback;
				_stores = DataService.GetRecentStoreNames();
			}
			
			#region implemented abstract members of MonoTouch.UIKit.UITableViewSource
			public override int RowsInSection(UITableView tableview, int section)
			{
				return _stores.Count;
			}
			
			public override string TitleForHeader(UITableView tableView, int section)
			{
				return "Recently used stores";
			}
			
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(_reuseIdentifier);
				if(cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, _reuseIdentifier);
				}
				cell.TextLabel.Text = _stores[indexPath.Row].Name;
				return cell;
			}
			
			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.CellAt(indexPath);
				cell.Accessory = UITableViewCellAccessory.Checkmark;
				tableView.DeselectRow(indexPath, true);
				if(_callback != null)
				{
					_callback(_stores[indexPath.Row].Name);
				}
			}
			
			#endregion
			
		}
	}
}

