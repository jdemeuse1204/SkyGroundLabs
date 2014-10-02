using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.FTP
{
    public class Class1
    {
		// download files
		private void Button_Click(object sender)
		{
			string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			string pathDownload = System.IO.Path.Combine(pathUser, "Downloads");
			string finalDownloadPath = pathDownload + @"\Published"; //  get local downloads folder
			// -----------------------------
			// read the files from the ftp
			// -----------------------------
			string fileString = string.Empty;
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://lin.arvixe.com/Published/");

			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Credentials = new NetworkCredential("jdemeuse", "aiwa1122");

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();

			Stream responseStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(responseStream);

			fileString += reader.ReadToEnd();

			reader.Close();

			IList<string> ftpFiles = new List<string>();
			string line = string.Empty;
			for (int i = 0; i < fileString.Length; i++)
			{
				string c = fileString.Substring(i, 1);
				if (c != "\n")
				{
					line += c;
					continue;
				}
				int pos = GetFirstSpaceReverse(line);
				line = line.Substring(pos, line.Length - pos).Replace("\r", "");
				ftpFiles.Add(@"Main\" + line);
				line = string.Empty;
			}

			fileString = string.Empty;
			request = (FtpWebRequest)WebRequest.Create("ftp://lin.arvixe.com/Published/Application Files/Data");

			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Credentials = new NetworkCredential("jdemeuse", "aiwa1122");

			response = (FtpWebResponse)request.GetResponse();

			responseStream = response.GetResponseStream();
			reader = new StreamReader(responseStream);

			fileString += reader.ReadToEnd();

			reader.Close();

			line = string.Empty;
			for (int i = 0; i < fileString.Length; i++)
			{
				string c = fileString.Substring(i, 1);
				if (c != "\n")
				{
					line += c;
					continue;
				}
				int pos = GetFirstSpaceReverse(line);
				line = line.Substring(pos, line.Length - pos).Replace("\r", "");
				ftpFiles.Add(@"Data\" + line);
				line = string.Empty;
			}

			// -----------------------------
			// remove old contents
			// -----------------------------
			if (Directory.Exists(finalDownloadPath))
			{
				string[] files = System.IO.Directory.GetFiles(finalDownloadPath, "*", SearchOption.AllDirectories);
				foreach (string s in files)
					File.Delete(s);
				if (Directory.Exists(finalDownloadPath))
					Directory.Delete(finalDownloadPath, true);
			}

			Directory.CreateDirectory(finalDownloadPath);

			// -----------------------------
			// download the files
			// -----------------------------
			string dataFolderName = string.Empty;
			foreach (string fileToDownload in ftpFiles)
			{
				if (fileToDownload == "Main\\Files")
					continue;
				WebClient webRequest = new WebClient();
				webRequest.Credentials = new NetworkCredential("jdemeuse", "aiwa1122");

				string ftp = "ftp://lin.arvixe.com/Published/";
				if (fileToDownload.Contains("Data"))
					ftp += "Application Files/Data/";

				byte[] fileData = webRequest.DownloadData(ftp + fileToDownload.Replace(@"Main\", "").Replace(@"Data\", ""));

				if (fileToDownload.Contains("Data"))
				{
					string lineRead;
					string[] files = System.IO.Directory.GetFiles(finalDownloadPath, "*_txt*", SearchOption.AllDirectories);
					// Read the file and display it line by line.
					System.IO.StreamReader fileSetup = new System.IO.StreamReader(files[0]);
					while ((lineRead = fileSetup.ReadLine()) != null)
					{
						if (!lineRead.Contains("Published"))
						{
							dataFolderName = lineRead.Substring(0, lineRead.IndexOf(@"\"));
							break;
						}
					}
					fileSetup.Close();

					if (!Directory.Exists(finalDownloadPath + @"\Application Files\" + dataFolderName + @"\"))
						Directory.CreateDirectory(finalDownloadPath + @"\Application Files\" + dataFolderName + @"\");

					FileStream file = File.Create(finalDownloadPath + @"\Application Files\" + dataFolderName + @"\" + fileToDownload.Replace(@"Main\", "").Replace(@"Data\", ""));
					file.Write(fileData, 0, fileData.Length);
					file.Close();
				}
				else
				{
					FileStream file = File.Create(finalDownloadPath + @"\" + fileToDownload.Replace(@"Main\", "").Replace(@"Data\", ""));
					file.Write(fileData, 0, fileData.Length);
					file.Close();
				}
			}

			AddFileExtensions(finalDownloadPath);
			//MessageBox.Show("Files Downloaded to the below location:\n" + finalDownloadPath);
		}

		// upload files
		private void Button_Click_1(object sender)
		{
			string path = "";// this.txtPublishPath.Text;
			RemoveFileExtensions(path);
			CreateSetupTextFile(path);
			RemoveFileExtensions(path);

			string fileString = string.Empty;
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://lin.arvixe.com/Published/");

			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Credentials = new NetworkCredential("jdemeuse", "aiwa1122");

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();

			Stream responseStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(responseStream);

			fileString += reader.ReadToEnd();

			reader.Close();

			IList<string> ftpFiles = new List<string>();
			string line = string.Empty;
			for (int i = 0; i < fileString.Length; i++)
			{
				string c = fileString.Substring(i, 1);
				if (c != "\n")
				{
					line += c;
					continue;
				}
				int pos = GetFirstSpaceReverse(line);
				line = line.Substring(pos, line.Length - pos).Replace("\r", "");
				ftpFiles.Add("ftp://lin.arvixe.com/Published/" + line);
				line = string.Empty;
			}

			fileString = string.Empty;
			request = (FtpWebRequest)WebRequest.Create("ftp://lin.arvixe.com/Published/Application Files/Data");

			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Credentials = new NetworkCredential("jdemeuse", "aiwa1122");

			response = (FtpWebResponse)request.GetResponse();

			responseStream = response.GetResponseStream();
			reader = new StreamReader(responseStream);

			fileString += reader.ReadToEnd();

			reader.Close();

			line = string.Empty;
			for (int i = 0; i < fileString.Length; i++)
			{
				string c = fileString.Substring(i, 1);
				if (c != "\n")
				{
					line += c;
					continue;
				}
				int pos = GetFirstSpaceReverse(line);
				line = line.Substring(pos, line.Length - pos).Replace("\r", "");
				ftpFiles.Add("ftp://lin.arvixe.com/Published/Application Files/Data/" + line);
				line = string.Empty;
			}

			// Delete existing files
			foreach (string fileToDelete in ftpFiles)
			{
				if (fileToDelete == "ftp://lin.arvixe.com/Published/Files")
					continue;
				FtpWebRequest requestFileDelete = (FtpWebRequest)WebRequest.Create(fileToDelete);
				requestFileDelete.Credentials = new NetworkCredential("jdemeuse", "aiwa1122");
				requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;

				FtpWebResponse responseFileDelete = (FtpWebResponse)requestFileDelete.GetResponse();
			}

			string[] files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			Stream ftpStream = null;
			foreach (string s in files)
			{
				FileInfo toUpload = new FileInfo(s);

				string ftpPath = "ftp://lin.arvixe.com/Published/";
				if (s.Contains("Application Files") && s.Contains("IonV3"))
					ftpPath += "Application Files/Data/";


				request = (FtpWebRequest)WebRequest.Create(ftpPath + GetFileName(s));

				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.Credentials = new NetworkCredential("jdemeuse", "aiwa1122");
				ftpStream = request.GetRequestStream();
				FileStream file = File.OpenRead(s);
				int length = 1024;
				byte[] buffer = new byte[length];
				int bytesRead = 0;

				do
				{
					bytesRead = file.Read(buffer, 0, length);
					ftpStream.Write(buffer, 0, bytesRead);
				} while (bytesRead != 0);
				file.Close();
				ftpStream.Close();
			}

			//MessageBox.Show("Upload complete");
		}

		private void RemoveFileExtensions(string path)
		{
			string[] files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			int count = 0;
			foreach (string s in files)
			{
				if (s.Contains("."))
				{
					File.Move(s, GetFilePath(s) + GetFileName(s).Replace(".", "_"));
					File.Delete(s);
				}
				count++;
			}
		}

		private void CreateSetupTextFile(string path)
		{
			string[] files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			string SetupFile = string.Empty;
			foreach (string s in files)
			{
				SetupFile += GetFileName(s, 2) + "\n";
			}

			System.IO.File.WriteAllText(path + "\\FilePaths.txt", SetupFile);
		}

		private void AddFileExtensions(string path)
		{
			string[] files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

			foreach (string s in files)
			{
				if (GetFileName(s).Contains("_"))
				{
					File.Move(s, GetFilePath(s) + GetFileName(s).Replace("_", "."));
					File.Delete(s);
				}
			}
		}

		private string GetFileName(string filePath)
		{
			string fileName = string.Empty;
			for (int i = filePath.Length; i > 0; i--)
			{
				string c = filePath.Substring(i - 1, 1);
				if (c == @"\")
					break;
				fileName = filePath.Substring(i - 1, 1) + fileName;
			}
			return fileName;
		}

		private int GetFirstSpaceReverse(string lineItem)
		{
			for (int i = lineItem.Length; i > 0; i--)
			{
				string c = lineItem.Substring(i - 1, 1);
				if (c == " ")
					return i;
			}
			return -1;
		}

		private string GetFileName(string filePath, int charCount)
		{
			string fileName = string.Empty;
			int charCt = 0;
			for (int i = filePath.Length; i > 0; i--)
			{
				string c = filePath.Substring(i - 1, 1);
				if (c == @"\")
				{
					charCt++;
					if (charCt == charCount)
						break;
				}
				fileName = filePath.Substring(i - 1, 1) + fileName;
			}
			return fileName;
		}

		private string GetFilePath(string filePath)
		{
			int count = filePath.Count<char>(w => w.ToString() == @"\");
			int pos = 0;
			for (int i = filePath.Length; i > 0; i--)
			{
				string c = filePath.Substring(i - 1, 1);
				if (c == @"\")
					break;
				pos = i - 1;
			}

			return filePath.Substring(0, filePath.Length - (filePath.Length - pos));
		}

		private string GetFilePath(string filePath, int charCount)
		{
			int count = filePath.Count<char>(w => w.ToString() == @"\");
			int charCt = 0;
			int pos = 0;
			for (int i = filePath.Length; i > 0; i--)
			{
				string c = filePath.Substring(i - 1, 1);
				if (c == @"\")
				{
					charCt++;

					if (charCt >= charCount)
						break;
				}
				pos = i - 1;
			}

			return filePath.Substring(0, filePath.Length - (filePath.Length - pos));
		}
    }
}
