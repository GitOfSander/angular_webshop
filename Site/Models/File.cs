using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Site.Models.Product;
using static Site.Models.Shipping;

namespace Site.Models
{
    public class File
    {
        public File()
        {
        }

        public string RemoveInvalidCharacters(string fileName)
        {
            Regex illegalInFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);

            return illegalInFileName.Replace(fileName, "");
        }
    }
}