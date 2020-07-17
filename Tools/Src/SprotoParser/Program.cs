using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace SprotoParser
{
    class Program
    {
        private static readonly string protosPath = @"./Protocols";
        private static StringBuilder stringBuilder = new StringBuilder();
        private static readonly string extName = ".sproto";
        private static string workDir = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            ScanSprotosName(protosPath);
            //Console.WriteLine(workDir);
            //Console.WriteLine(stringBuilder.ToString());
            string argscmdLine = " sprotodump.lua -spb" + stringBuilder.ToString() + " -o ./Outputs/sproto.bytes";
            Process.Start("lua", argscmdLine);
        }

        public static void ScanSprotosName(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileSystemInfo[] fileInfos = dirInfo.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fileInfos)
            {
                if (fsinfo is DirectoryInfo)
                {
                    ScanSprotosName(fsinfo.FullName);
                }
                else
                {
                    if (fsinfo.Extension == extName)
                    {
                        var fullName = fsinfo.FullName;
                        fullName = fullName.Replace(workDir, ".");
                        fullName = fullName.Replace(@"\", @"/");
                        //Console.WriteLine(fullName);
                        stringBuilder.Append(" ").Append(fullName);
                    }
                }
            }
        }
    }
}
