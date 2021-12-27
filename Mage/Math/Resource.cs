using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Mage
{
    public static class Resources
    {
        public static Effect DefaultEffect { get; private set; }

        public static string Root => Path.Combine(Path.GetDirectoryName(typeof(Resources).Assembly.Location), "Content");

        public static string GetPath(string path)
        {
            List<string> keywords = new List<string>();

            string currKeyword = "";
            foreach (char c in path)
            {
                if (c == '/')
                {
                    keywords.Add(currKeyword);
                    currKeyword = "";
                }
                else
                {
                    currKeyword += c;
                }
            }
            //Add last word
            keywords.Add(currKeyword);

            //Add 2 to get executing location and Content folder
            string[] finalFullPath = new string[keywords.Count + 2];
            finalFullPath[0] = Path.GetDirectoryName(typeof(Resources).Assembly.Location);
            finalFullPath[1] = "Content";

            for (int i = 0; i < keywords.Count; i++)
            {
                // Start from offset
                finalFullPath[i + 2] = keywords[i];
            }

            return Path.Combine(finalFullPath);
        }
    }
}