using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WMPLib;
using Shell32;

namespace Video_Downloader
{
    class LatestMedia
    {
        private string name { get; set;}
        private double size { get; set; }
        private string path { get; set; }
        private string extension { get; set; }

        public string GetExtension()
        {
            return extension;
        }
        public string GetName()
        {
            if(name.Length>45)
            {
                return name=name.Substring(0, 45)+"...";
            }
           else  return name;
        }
  
        public double GetSize()
        {
            return Math.Round(size / 1024/1024,2); 
        }
        public string GetPath()
        {
            if (path.Length > 45)
                return path = path.Substring(0, 45) + "...";
           else  return path;
        }
        public LatestMedia( string _path)
        {
            
            path = _path;
            name= Path.GetFileName(path);
            extension = Path.GetExtension(path);
            if (File.Exists(path)) 
            size = new FileInfo(path).Length;

        }


    }
}
