using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Mage
{
    public static class SaveData
    {
        public static string ErrorLogPath => Path.Combine(SavePath, "errorlog.txt");
        private static string _name;

        public static void Initialize(string name)
        {
            _name = name;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // try to follow XDG standards first on Linux
                // https://specifications.freedesktop.org/basedir-spec/basedir-spec-latest.html
                string homeDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                if (!string.IsNullOrEmpty(homeDir))
                    Directory.CreateDirectory(Path.Combine(homeDir, _name));
                else Directory.CreateDirectory(Path.Combine(Environment.GetEnvironmentVariable("HOME"),
                ".local", "share", _name));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library",
                "Application Support", _name));
            }
            else
                Directory.CreateDirectory(Path.Combine(typeof(SaveData).Assembly.Location, "Saves"));
        }

        public static string SavePath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string homeDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                    if (!string.IsNullOrEmpty(homeDir))
                        return Path.Combine(homeDir, _name);
                    else
                        return Path.Combine(Environment.GetEnvironmentVariable("HOME"),
                        ".local", "share", _name);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library",
                    "Application Support", _name);
                else
                    return Path.Combine(typeof(SaveData).Assembly.Location, "Saves");
            }
        }

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

            string[] finalFullPath = new string[keywords.Count + 1];
            finalFullPath[0] = SavePath;

            for (int i = 0; i < keywords.Count; i++)
            {
                // Start from offset
                finalFullPath[i + 1] = keywords[i];
            }

            return Path.Combine(finalFullPath);
        }

    }
}