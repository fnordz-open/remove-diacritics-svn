using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace remove_diacritics_svn
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = "C:\\_SVN";
            
            Console.WriteLine("############ Starting remove-diacritics-svn ############");
            Console.WriteLine("Root path: " + basePath);
            Console.WriteLine("PRESS ANY KEY TO CONTINUE (OR Ctrl+C TO ABORT)");
            Console.ReadKey();

            RenameRecursively(basePath);

            Console.WriteLine("");
            Console.WriteLine("FINISHED!!! PRESS ANY KEY TO END");
            Console.ReadKey();
        }

        private static void RenameRecursively(string basePath)
        {
            //Do the work - rename the files using svn commandline tool to keep the change history (log)
            RenameFiles(basePath);

            //Get all directories in basePath
            IEnumerable<string> directories = Directory.EnumerateDirectories(basePath, "*.*", SearchOption.TopDirectoryOnly);

            //Call this method recursively for all of them
            foreach (var dirPath in directories)
            {
                var dir = Path.GetFileName(dirPath);

                //Ignore directories starting with '.' (like ".svn" or ".git")
                if (dir[0] != '.')
                {
                    RenameRecursively(dirPath);
                }

                //Rename the directory if needed
                SvnRenameIfNeeded(dirPath);
            }
        }

        private static void RenameFiles(string path)
        {
            //list all files
            IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                //rename the file if needed
                SvnRenameIfNeeded(file);
            }
        }

        private static void SvnRenameIfNeeded(string file)
        {
            var renamedFile = RemoveDiacriticsFromFile(file);

            if (renamedFile != file)
            {
                Console.WriteLine("");
                Console.WriteLine(string.Format("svn reanme \"{0}\" \"{1}\"", file, renamedFile));

                var currentDir = (new FileInfo(Assembly.GetEntryAssembly().Location)).Directory.FullName;
                //var command = string.Format("{0}\\svn.exe rename \"{1}\" \"{2}\"", currentDir, file, renamedFile);
                var command = string.Format("{0}\\svn.exe", currentDir);
                var args = string.Format("rename \"{0}\" \"{1}\"" , file, renamedFile);

                Process process = new Process();
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = args;
                process.Start(); 

                if (!process.WaitForExit(2000))
                    Console.WriteLine("   ERROR! Process frozen!");
            }
        }

        private static string RemoveDiacriticsFromFile(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            var file = RemoveDiacritics(Path.GetFileName(filePath));

            return dir + "\\" + file;
        }


        public static string RemoveDiacritics(string input)
        {
            string stFormD = input.Normalize(NormalizationForm.FormD);
            int len = stFormD.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[i]);
                }
            }
            
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

    }
}
