using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace LogWizard {
    public class zip_util {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string zip_friendly_path_name(string name) {
            string prefix = "x:\\";
            var fi = new FileInfo(prefix + name);
            return fi.Name + ( fi.DirectoryName != prefix ? " (" + fi.DirectoryName.Substring(prefix.Length) + ")" : "");
        }

        public static List<string> enum_file_names_in_zip(string file_name) {
            List<string> names = new List<string>();
            try {
                using (var fs = new FileStream(file_name, FileMode.Open, FileAccess.Read)) {
                    using (var zf = new ZipFile(fs)) {
                        foreach (ZipEntry ze in zf) {
                            if (ze.IsDirectory)
                                continue;

                            names.Add(ze.Name);
                        }
                    }
                }
            } catch(Exception e) {
                logger.Fatal("can't enum zip file " + file_name + " : " + e.Message);
            }
            return names;
        }

        public static bool try_extract_file_names_in_zip(string file_name, string extract_dir, Dictionary<string,string> extract_files) {
            try {
                using (var fs = new FileStream(file_name, FileMode.Open, FileAccess.Read)) {
                    using (var zf = new ZipFile(fs)) {
                        foreach (ZipEntry ze in zf) {
                            if (ze.IsDirectory)
                                continue;

                            if (!extract_files.ContainsKey(ze.Name))
                                continue;

                            string name = extract_dir + "\\" + extract_files[ze.Name];
                            using (Stream s = zf.GetInputStream(ze)) {
                                byte[] buf = new byte[4096];
                                using (FileStream file = File.Create(name)) 
                                  StreamUtils.Copy(s, file, buf);                                
                            }
                        }
                    }
                }
            } catch(Exception e) {
                logger.Fatal("can't extract file " + file_name + ": " + e.Message);
                return false;
            }

            return true;
        }


        public static void dump_file_names_in_zip(string file_name) {
            var names = enum_file_names_in_zip(file_name);
            foreach ( var name in names)
                Console.WriteLine( zip_friendly_path_name( name));
        }

        // https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples 
        // Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        private static void CreateZip(string outPathname, string folderName) {

            FileStream fsOut = File.Create(outPathname);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            CompressFolder(folderName, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        // https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples 
        // Recurses down the folder structure
        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset) {

            string[] files = Directory.GetFiles(path);

            foreach (string filename in files) {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
        // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[ ] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename)) {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[ ] folders = Directory.GetDirectories(path);
            foreach (string folder in folders) {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

        public static bool create_zip(string file_name, Dictionary<string, string> files) {
            // at this time, we don't allow putting files into directories WITHIN the zip file
            Debug.Assert(files.Values.Count( x => x.Contains("\\") || x.Contains("/") ) == 0);

            try {
                string temp = Environment.CurrentDirectory + "\\" + DateTime.Now.Ticks;
                Directory.CreateDirectory(temp);

                foreach ( var file in files) 
                    File.Copy(file.Key, temp + "\\" + file.Value, true);

                Directory.CreateDirectory(new FileInfo(file_name).DirectoryName);
                CreateZip(file_name, temp);

                Directory.Delete(temp, true);
            } catch (Exception e) {
                logger.Fatal("can't create zip " + file_name + " : " + e.Message);
                return false;
            }


            return true;
        }
    }
}
