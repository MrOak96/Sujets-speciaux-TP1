using System;
using System.Collections.Generic;
using System.Text;

namespace FileScanner.Models
{
    public class Item
    {
        public string Path { get; set; }
        public string Image { get; set; }

        public Item(string path, string image)
        {
            Path = path;
            Image = image;
        }

    }
}
