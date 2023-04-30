using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Site.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Models
{
    public class PDF
    {
        SiteContext _context;
        private readonly IHostingEnvironment _env;

        public PDF(SiteContext context, IHostingEnvironment env = null)
        {
            _context = context;
            _env = env;
        }

        readonly byte[] _ownerPassword = Encoding.UTF8.GetBytes("World");
        readonly byte[] _userPassword = Encoding.UTF8.GetBytes("Hello");

        public void Verify_EncryptionPdf_CanBeCreated()
        {
            var pdfFilePath = _env.WebRootPath + $@"\templates\pdf\test.pdf";
            var enc1 = createPdf();
            System.IO.File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".enc1.pdf"), enc1);

            var enc2 = decryptPdf(enc1);
            System.IO.File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".enc2.pdf"), enc2);

            var enc3 = encryptPdf(enc2);
            System.IO.File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".enc3.pdf"), enc3);
        }

        private byte[] createPdf()
        {
            using (var ms = new MemoryStream())
            {
                // step 1
                var document = new Document();

                var permissions = 0;
                permissions |= PdfWriter.AllowPrinting;
                permissions |= PdfWriter.AllowCopy;
                permissions |= PdfWriter.AllowScreenReaders;

                var writer = PdfWriter.GetInstance(document, ms);
                writer.SetEncryption(
                  _userPassword, _ownerPassword,
                  permissions,
                  PdfWriter.STANDARD_ENCRYPTION_128
                );
                document.AddAuthor("Unveil");
                writer.CreateXmpMetadata();
                // step 3
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello World"));

                document.Close();

                return ms.ToArray();
            }
        }

        private byte[] decryptPdf(byte[] src)
        {
            using (var ms = new MemoryStream())
            {
                var reader = new PdfReader(src, _ownerPassword);

                var stamper = new PdfStamper(reader, ms);
                stamper.Close();
                reader.Close();

                return ms.ToArray();
            }
        }

        private byte[] encryptPdf(byte[] src)
        {
            using (var ms = new MemoryStream())
            {
                var reader = new PdfReader(src);

                var stamper = new PdfStamper(reader, ms);

                stamper.SetEncryption(
                  _userPassword, _ownerPassword,
                  PdfWriter.ALLOW_PRINTING,
                  PdfWriter.ENCRYPTION_AES_128 | PdfWriter.DO_NOT_ENCRYPT_METADATA
                );

                stamper.Close();
                reader.Close();

                return ms.ToArray();
            }
        }
    }
}
