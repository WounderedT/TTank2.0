using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TPresenterStructure
{
    class Structure
    {
        private static string solutionPath;

        private static string prefix = "|---";
        private static string subPrefix = "|   ";

        public static List<string> ignoreDirs = new List<string>(new string[] { "External", ".git", ".vs" });
        public static List<string> ignoreSubDirs = new List<string>(new string[] { "bin", "obj", "Properties" });
        public static string SolutionPath
        {
            get
            {
                if (String.IsNullOrEmpty(solutionPath))
                {
                    string[] path = AppDomain.CurrentDomain.BaseDirectory.Split('\\');
                    StringBuilder strB = new StringBuilder();
                    for (int ind = 0; ind < (path.Length - 4); ind++)
                    {
                        strB.Append(path[ind]);
                        strB.Append("\\");
                    }
                    solutionPath = strB.ToString();
                }
                return solutionPath;
            }
        }

        static void Main(string[] args)
        {
            var directrories = Directory.GetDirectories(SolutionPath);
            StringBuilder builder = new StringBuilder();
            foreach (string dir in directrories)
            {
                string dirName = dir.Split('\\').Last();
                if (ignoreDirs.Contains(dirName))
                    continue;

                builder.Append(dirName);
                builder.AppendLine();
                var subDirs = Directory.GetDirectories(dir);
                AppendSubDir(subDirs, ref builder);
                var files = Directory.GetFiles(dir);
                foreach (string file in files)
                    AppendFile(file, ref builder);
                builder.AppendLine();
            }
            File.WriteAllText(Path.Combine(SolutionPath, "Files.txt"), builder.ToString());
        }

        public static void AppendSubDir(string[] subDirs, ref StringBuilder builder)
        {
            foreach (string dir in subDirs)
            {
                string dirName = dir.Split('\\').Last();
                if (ignoreSubDirs.Contains(dirName))
                    continue;

                builder.Append(prefix);
                builder.Append(dirName);
                builder.AppendLine();
                var files = Directory.GetFiles(dir);
                foreach(string file in files)
                    AppendFile(file, ref builder, subPrefix);
            }
        }

        public static void AppendFile(string file, ref StringBuilder builder, string subDirPrefix = "")
        {
            if (file.EndsWith(".cs"))
            {
                builder.Append(subDirPrefix);
                builder.Append(prefix);
                builder.Append(file.Split('\\').Last());
                builder.AppendLine();
            }
        }
    }
}
