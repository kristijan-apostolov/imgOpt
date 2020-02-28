using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Imagekit;

namespace imageOptimizer
{
    public static class myUtils
    {
        //public static ImagekitResponse resp;
        public static string publicKey= "public_G25kpjVvDJiKAbVe7/6wPVLYBf4=";
        public static string privateKey = "private_8lIVPcXArv7dScQ3IuI9bq7iIFs=";
        public static string urlEndPoint = "https://ik.imagekit.io/6ubty6n6g/";
        public static Imagekit.Imagekit imagekit = new Imagekit.Imagekit(publicKey, privateKey , urlEndPoint , "path");
        public static StringBuilder imgReport =  new StringBuilder();

        public static byte[] DownloadImg(string imgPath)
        {
            //var urlToImage = "https://ik.imagekit.io/6ubty6n6g/" +

            var imgName = System.IO.Path.GetFileName(imgPath);
           
            imgName = imgName.Replace(" ","_").Replace("[" , "_").Replace("]","_").Replace("(","_").Replace(")","_");

            var absoluteImagePath = urlEndPoint + imgName;
            
            byte[] downloadedImg;
            using (var client = new System.Net.WebClient())
            {
                downloadedImg = client.DownloadData(absoluteImagePath);
            }
            return downloadedImg;
        }

        public static ImagekitResponse UploadImg(string imgPath)
        {
            var imgName = System.IO.Path.GetFileName(imgPath);
            byte[] file = System.IO.File.ReadAllBytes(imgPath);
            var resp = imagekit.FileName(imgName);
            resp.UseUniqueFileName("false");
            ImagekitResponse response = resp.Upload(file);
            return response;
        }
        public static void OverrideImg(string imgPath , byte[] imgBites)
        {
            File.WriteAllBytes(imgPath, imgBites);
        }
        public static string BytesToKb(long bytes)
        {
            if(bytes > 1024)
            {
                return bytes / 1024 + "kb";
            }
            else
            {
                return bytes + "b";
            }
        }
        public static void ImgLog(string pathImgOld, long lengthNewImg)
        {
            long lengthOld = new System.IO.FileInfo(pathImgOld).Length;
            long lengthNew = lengthNewImg;
            imgReport.Append("image name:" + System.IO.Path.GetFileName(pathImgOld) + " path:" + pathImgOld + " size:" + BytesToKb(lengthOld) + " was optimized, now size is:" + BytesToKb(lengthNew) + Environment.NewLine);
        }

        public static void SaveImgReport (string filePath)
        {
            File.AppendAllText(filePath + "log.txt", imgReport.ToString());
            imgReport.Clear();
        }
        }
    }

