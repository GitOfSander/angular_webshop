using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ProductUploads
    {
        public int Id { get; set; }
        public int ProductTemplateId { get; set; }
        public string CallName { get; set; }
        public string Heading { get; set; }
        public string MimeTypes { get; set; }
        public string FileExtensions { get; set; }
        public byte MinFiles { get; set; }
        public byte MaxFiles { get; set; }
        public int MaxSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int CustomOrder { get; set; }

        public ProductTemplates ProductTemplate { get; set; }
    }
}
