﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace imageOptimizer
{
    static class  Program
    {
        static void Main(string[] args)
        {
            var rootPath = "";

            if (args.Length > 0)
            {
                rootPath = args[0];
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("insert root of site");
                    rootPath = Console.ReadLine();
                    if (!System.IO.Directory.Exists(rootPath))
                    {
                        Console.WriteLine("folder does not exsist");
                    }
                    else
                    {
                        break;
                    }
                }
            }

            RecursiveFileProcessor.ProcessDirectory(rootPath);
            var dict = RecursiveFileProcessor.GetImagesPathsWithDirs();
            foreach (KeyValuePair<string, List<string>> entry in dict)
            {
                if (entry.Value.Count == 0)
                {
                    dict.Remove(entry.Key);
                    continue;
                }
                foreach (var imgPath in entry.Value)
                {
 
                  Console.WriteLine("Uploading img:" + imgPath + "...");
                  //upload file
                  myUtils.UploadImg(imgPath);

                  Console.WriteLine("uploaded finished");
                  //download file 
                  var imgBites = myUtils.DownloadImg(imgPath);
                  //write info
                  myUtils.ImgLog(imgPath, imgBites.Length);

                  //rewrite file
                  myUtils.OverrideImg(imgPath, imgBites);
                }
            }
            myUtils.SaveImgReport(rootPath + "\\");
        }
    }
}
