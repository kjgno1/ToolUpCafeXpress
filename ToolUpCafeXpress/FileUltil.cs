using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ToolUpCafeXpress
{
    public class FileUltil
    {
        public  static void Download(string url, string fileName)
        {
            var save = "img/" + fileName;
            if (!Directory.Exists("img"))
            {
                Directory.CreateDirectory("img");
            }
            if (!File.Exists(save))
            {
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(url, save);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
               
            }
            Console.WriteLine("Start processing file");
        }
        
        public static void ScaleImage(Image image, int height, String fileName)
        {
            double ratio = (double)height/ image.Height;
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            image.Dispose();
            
            if (!Directory.Exists("img2"))
            {
                Directory.CreateDirectory("img2");
            }
            
            newImage.Save( "img2/" + fileName,ImageFormat.Png);
        }
        
      
        
    }
    
   
}