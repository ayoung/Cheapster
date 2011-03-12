using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Cheapster.Support;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cheapster.ViewControllers;
using LibZipArchive;

namespace Cheapster.ViewControllers.Shared
{
	public class RestoreAlertViewDelegate : UIAlertViewDelegate
	{
		private HomeListNavigationController _navController;
		private NSUrl _urlToFile;
		
		public RestoreAlertViewDelegate(NSUrl url, HomeListNavigationController navController)
		{
			_urlToFile = url;
			_navController = navController;
		}
		
		public override void Dismissed(UIAlertView alertView, int buttonIndex)
		{
			Console.WriteLine(buttonIndex);
			
			// restore touched
			if(buttonIndex == 1)
			{
				_navController.PrepareForRestore(RestoreDb);
			}
		}
		
		private void RestoreDb(Action finishedRestoringCallback)
		{
			Console.WriteLine("Restoring backup from " + _urlToFile.AbsoluteString);
			var filePath = Path.Combine(Configuration.TEMP_FOLDER, Configuration.USER_DB_FILENAME);
			var unsuccessful = false;
			
			if(File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			
			var zip = new ZipArchive();
			unsuccessful = !zip.UnzipOpenFile(_urlToFile.Path) || unsuccessful;
			unsuccessful = !zip.UnzipFileTo(Configuration.TEMP_FOLDER, true) || unsuccessful;
			zip.CloseZipFile2();
			
			Console.WriteLine("File exists: " + File.Exists(filePath));
			if(!File.Exists(filePath) || unsuccessful)
			{
				new UIAlertView("Warning", "Could not restore data with the given file. Cheapster has left your data unchanged.", null, "Dismiss").Show();
				finishedRestoringCallback();
				return;
			}
			
			File.Copy(filePath, Configuration.USER_DB_INSTALLED_PATH, true);
			
			try
			{
				finishedRestoringCallback();
			}
			catch(Exception e)
			{
				Installation.ResetData();
				new UIAlertView("Error", "There were serious problems applying this backup. As a preventative measure, your data has been reset.", null, "Dismiss").Show();
				finishedRestoringCallback();
				return;
			}

			new UIAlertView("Backup Applied", "Cheapster has applied the backup file.", null, "Dismiss").Show();
		}
	}
}

