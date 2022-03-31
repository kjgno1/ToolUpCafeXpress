using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace ToolUpCafePress
{
    [Table("TBL_IMAGE_INFO")]
    public class ImageInfo
    {
        string id, name, url, descriptions, tags;
        string status;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Url { get => url; set => url = value; }
       
        public string Descriptions { get => descriptions; set => descriptions = value; }
        public string Tags { get => tags; set => tags = value; }

        public string Status { get => status; set => status = value; }
    }
}