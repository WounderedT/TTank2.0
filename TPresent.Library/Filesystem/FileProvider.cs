using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Filesystem
{
    public static class FileProvider
    {
        private static String _animationPath;
        private static string _contentPath;
        private static string _modelsPath;
        private static string _texturePath;
        private static string _shadersPath;

        public static string AnimationsPath { get { return _animationPath; } }
        public static string ContentPath { get { return _contentPath; } }
        public static string ModelsPath { get { return _modelsPath; } }
        public static string TexturesPath { get { return _texturePath; } }
        public static string ShadersPath { get { return _shadersPath; } }

        public static void Init()
        {
            //contentPath = Path.Combine(RootDir, "Content"); content dir should be located in the same folder as exe file. Temporary changing this path.
            var tmp = RootDir.Split(new string[] { "\\" }, StringSplitOptions.None);
            StringBuilder path = new StringBuilder();
            for (int i = 0; i < tmp.Length - 4; i++)
            {
                path.Append(tmp[i]);
                path.Append("\\");
            }
            _contentPath = Path.Combine(path.ToString(), "Content");
            //contentPath = Path.Combine(RootDir, "Resources");
            _shadersPath = Path.Combine(RootDir, "Shaders");
            _animationPath = Path.Combine(_contentPath, "Animations");
            _modelsPath = Path.Combine(_contentPath, "Models");
            _texturePath = Path.Combine(_contentPath, "Textures");
        }

        public static string RootDir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
                //return @"H:\OneDrive\Study\C# Projects\TTank2.0\TTank 2.0\bin\Debug\";
            }
        }

        public static string FullFIlePath(string filename)
        {
            //TODO: add input path verification
            return Path.Combine(RootDir, filename);
        }

        public static void WriteFile(string filepath, string filename, StringBuilder builder)
        {
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);
            WriteFile(Path.Combine(filepath, filename), builder);
        }

        public static void WriteBytes(string filepath, string filename, byte[] array)
        {
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);
            using (FileStream fs = new FileStream(Path.Combine(filepath, filename), FileMode.Create, FileAccess.Write, FileShare.None))
                fs.Write(array, 0, array.Length);
        }

        public static void WriteFile(string filepath, StringBuilder builder)
        {
            //File.Create(filepath);
            using (StreamWriter writer = new StreamWriter(filepath))
                writer.Write(builder.ToString());
        }

        public static void WriteFile(string filepath, object content)
        {
            //File.Create(filepath);
            using (StreamWriter writer = new StreamWriter(filepath))
                writer.Write(content);
        }
    }
}
