using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security8
{
    static class TextReader
    {
        public static string directory = "";
        public static string getText(string file)
        {
            string text = File.ReadAllText(file);
            return text;
        }

        public static void toFile(string file ,string text)
        {
            File.WriteAllText(file, text);
        }
    }
}
