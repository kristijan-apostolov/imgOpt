using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace imageOptimizer
{
    public  class RecursiveFileProcessor
    {
        //public static String[] filter = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };

        static string[]  patterns = new[] { "*.jpg", "*.jpeg", "*.jpe", "*.jif", "*.jfif", "*.jfi", "*.webp", "*.gif", "*.png", "*.apng", "*.bmp", "*.dib", "*.tiff", "*.tif", "*.svg", "*.svgz", "*.ico", "*.xbm" };
        /// <summary>
        /// dir with value
        /// </summary>
        public static Dictionary<string, List<string>> imagesPathsWithDirs = new Dictionary<string, List<string>>();
        public static List<string> tmpImagesPaths;
        public static List<string> imagesPathsOver1mb = new List<string>();
        public static String[] filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.


        public static void ProcessDirectory(string targetDirectory)
        {
            tmpImagesPaths = new List<string>();
            // Process the list of files found in the directory.
            string[] fileEntries = CustomDirectoryTools.GetFiles(targetDirectory, patterns);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            
            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            imagesPathsWithDirs.Add(targetDirectory, tmpImagesPaths);

            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            long length = new System.IO.FileInfo(path).Length;
            if (length > 1024000 || length  < 10000)
            {
                imagesPathsOver1mb.Add(path);
            }
            else
            {
                tmpImagesPaths.Add(path);
            }

        }

        public static class CustomDirectoryTools
        {
            public static string[] GetFiles(string path, string[] patterns = null, SearchOption options = SearchOption.TopDirectoryOnly)
            {
                if (patterns == null || patterns.Length == 0)
                    return Directory.GetFiles(path, "*", options);
                if (patterns.Length == 1)
                    return Directory.GetFiles(path, patterns[0], options);
                return patterns.SelectMany(pattern =>Directory.GetFiles(path, pattern, options)).Distinct().ToArray();
            }
        }

        public static List<string> GetImagesPaths()
        {
            return tmpImagesPaths;
        }
        public static List<string> GetImagesPathsOver1mb()
        {
            return imagesPathsOver1mb;
        }
        public static Dictionary<string,List<string>> GetImagesPathsWithDirs()
        {
            return imagesPathsWithDirs;
        }

    }
}
