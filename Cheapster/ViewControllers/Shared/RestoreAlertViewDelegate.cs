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
			var unsuccessful = false;
			
			if(File.Exists(Configuration.USER_DB_TEMP_DB_PATH))
			{
				File.Delete(Configuration.USER_DB_TEMP_DB_PATH);
			}
			
			try
			{
				// unzip file to temp folder
				var zip = new ZipArchive();
				unsuccessful = !zip.UnzipOpenFile(_urlToFile.Path) || unsuccessful;
				unsuccessful = !zip.UnzipFileTo(Configuration.TEMP_FOLDER, true) || unsuccessful;
				zip.CloseZipFile2();
				
				// check if expected dbd file exists
				Console.WriteLine("File exists: " + File.Exists(Configuration.USER_DB_TEMP_DB_PATH));
				if(!File.Exists(Configuration.USER_DB_TEMP_DB_PATH) || unsuccessful)
				{
					throw new Exception("Unzipping file was unsuccessful.");
				}
				
				// verify db version is compatible
				var version = Data.DataService.GetDbVersion(Configuration.USER_DB_TEMP_DB_PATH);
				if(version != 1)
				{
					throw new Exception("Import db was not in a compatible version");
				}
				
				// copy db to user db installed path
				File.Copy(Configuration.USER_DB_TEMP_DB_PATH, Configuration.USER_DB_INSTALLED_PATH, true);
			}
			catch (Exception e)
			{
				new UIAlertView("Warning", "Could not restore data with the given file. Cheapster has left your data unchanged.", null, "Dismiss").Show();
				finishedRestoringCallback();
				return;
			}
			
			try
			{
				//Configuration.USER_DB_TEMP_DB_PATH
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

