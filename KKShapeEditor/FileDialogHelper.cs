using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000041 RID: 65
	public static class FileDialogHelper
	{
		// Token: 0x06000304 RID: 772
		[DllImport("comdlg32.dll", CharSet = CharSet.Unicode)]
		private static extern bool GetSaveFileName([In] [Out] FileDialogHelper.OpenFileName ofn);

		// Token: 0x06000305 RID: 773
		[DllImport("comdlg32.dll", CharSet = CharSet.Unicode)]
		private static extern bool GetOpenFileName([In] [Out] FileDialogHelper.OpenFileName ofn);

		// Token: 0x06000306 RID: 774 RVA: 0x00019F74 File Offset: 0x00018174
		public static string ShowSaveDialog(string title, string defaultName, string filter, string defaultExt)
		{
			FileDialogHelper.OpenFileName ofn = FileDialogHelper.CreateOfn(title, filter);
			ofn.defExt = defaultExt;
			ofn.flags = 2621450;
			char[] array = new char[2048];
			if (!string.IsNullOrEmpty(defaultName))
			{
				defaultName.CopyTo(0, array, 0, defaultName.Length);
			}
			string text = new string(array);
			ofn.file = Marshal.StringToBSTR(text);
			ofn.maxFile = text.Length;
			string text2;
			try
			{
				text2 = FileDialogHelper.RunDialog(() => FileDialogHelper.GetSaveFileName(ofn), ofn);
			}
			finally
			{
				Marshal.FreeBSTR(ofn.file);
			}
			return text2;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0001A038 File Offset: 0x00018238
		public static string ShowOpenDialog(string title, string filter)
		{
			FileDialogHelper.OpenFileName ofn = FileDialogHelper.CreateOfn(title, filter);
			ofn.flags = 2625544;
			string text = new string(new char[2048]);
			ofn.file = Marshal.StringToBSTR(text);
			ofn.maxFile = text.Length;
			string text2;
			try
			{
				text2 = FileDialogHelper.RunDialog(() => FileDialogHelper.GetOpenFileName(ofn), ofn);
			}
			finally
			{
				Marshal.FreeBSTR(ofn.file);
			}
			return text2;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0001A0D8 File Offset: 0x000182D8
		private static FileDialogHelper.OpenFileName CreateOfn(string title, string filter)
		{
			FileDialogHelper.OpenFileName openFileName = new FileDialogHelper.OpenFileName();
			openFileName.structSize = Marshal.SizeOf<FileDialogHelper.OpenFileName>(openFileName);
			openFileName.title = title;
			openFileName.filter = filter.Replace("|", "\0") + "\0";
			openFileName.fileTitle = new string(new char[2048]);
			openFileName.maxFileTitle = openFileName.fileTitle.Length;
			return openFileName;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001A144 File Offset: 0x00018344
		private static string RunDialog(Func<bool> showDialog, FileDialogHelper.OpenFileName ofn)
		{
			bool runInBackground = Application.runInBackground;
			Application.runInBackground = false;

			string currentDir = Environment.CurrentDirectory;
			bool keepResetting = true;

			ThreadStart resetCurrentDirectory = delegate()
			{
				while (keepResetting)
				{
					Environment.CurrentDirectory = currentDir;
					Thread.Sleep(1);
				}
			};

			Thread resetThread = new Thread(resetCurrentDirectory);
			resetThread.IsBackground = true;
			resetThread.Start();

			string result;

			try
			{
				if (showDialog())
				{
					string selectedPath = Marshal.PtrToStringUni(ofn.file);

					if (selectedPath != null)
					{
						int nullIndex = selectedPath.IndexOf('\0');

						if (nullIndex >= 0)
						{
							selectedPath = selectedPath.Substring(0, nullIndex);
						}
					}

					result = string.IsNullOrEmpty(selectedPath) ? null : selectedPath;
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				keepResetting = false;
				Environment.CurrentDirectory = currentDir;
				Application.runInBackground = runInBackground;
			}

			return result;
		}

		// Token: 0x040001C2 RID: 450
		private const int OFN_OVERWRITEPROMPT = 2;

		// Token: 0x040001C3 RID: 451
		private const int OFN_NOCHANGEDIR = 8;

		// Token: 0x040001C4 RID: 452
		private const int OFN_FILEMUSTEXIST = 4096;

		// Token: 0x040001C5 RID: 453
		private const int OFN_LONGNAMES = 2097152;

		// Token: 0x040001C6 RID: 454
		private const int OFN_EXPLORER = 524288;

		// Token: 0x040001C7 RID: 455
		private const int MAX_FILE_LENGTH = 2048;

		// Token: 0x0200006C RID: 108
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private class OpenFileName
		{
			// Token: 0x040004CD RID: 1229
			public int structSize;

			// Token: 0x040004CE RID: 1230
			public IntPtr dlgOwner = IntPtr.Zero;

			// Token: 0x040004CF RID: 1231
			public IntPtr instance = IntPtr.Zero;

			// Token: 0x040004D0 RID: 1232
			public string filter;

			// Token: 0x040004D1 RID: 1233
			public string customFilter;

			// Token: 0x040004D2 RID: 1234
			public int maxCustFilter;

			// Token: 0x040004D3 RID: 1235
			public int filterIndex;

			// Token: 0x040004D4 RID: 1236
			public IntPtr file;

			// Token: 0x040004D5 RID: 1237
			public int maxFile;

			// Token: 0x040004D6 RID: 1238
			public string fileTitle;

			// Token: 0x040004D7 RID: 1239
			public int maxFileTitle;

			// Token: 0x040004D8 RID: 1240
			public string initialDir;

			// Token: 0x040004D9 RID: 1241
			public string title;

			// Token: 0x040004DA RID: 1242
			public int flags;

			// Token: 0x040004DB RID: 1243
			public short fileOffset;

			// Token: 0x040004DC RID: 1244
			public short fileExtension;

			// Token: 0x040004DD RID: 1245
			public string defExt;

			// Token: 0x040004DE RID: 1246
			public IntPtr custData = IntPtr.Zero;

			// Token: 0x040004DF RID: 1247
			public IntPtr hook = IntPtr.Zero;

			// Token: 0x040004E0 RID: 1248
			public string templateName;

			// Token: 0x040004E1 RID: 1249
			public IntPtr reservedPtr = IntPtr.Zero;

			// Token: 0x040004E2 RID: 1250
			public int reservedInt;

			// Token: 0x040004E3 RID: 1251
			public int flagsEx;
		}
	}
}
