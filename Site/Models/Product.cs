using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Site.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Options;
using Site.Models.App;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Site.Models
{
    public class Product : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Product(SiteContext context, IOptions<AppSettings> config = null)
        {
            _context = context;
            _config = config;
        }

        private Page _page;
        private Commerce _commerce;
        public int _reserveMinuts;
        private int _digitsAfterDecimal;
        private string _currency;
        public bool AddBreadcrumbsJson = false;
        //private int digitsAfterDecimal;
        //private string currency;

        public class ProductBundle
        {
            public ProductFiles ProductFile { get; set; }
            public IEnumerable<ProductFiles> ProductFiles { get; set; }
            public ProductResources ProductResource { get; set; }
            public IEnumerable<ProductResources> ProductResources { get; set; }
            public Products Product { get; set; }
            public ProductFields ProductField { get; set; }
            public IEnumerable<ProductFields> ProductFields { get; set; }
            public ProductTemplates ProductTemplate { get; set; }
            public ProductUploads ProductUpload { get; set; }
            public IEnumerable<ProductUploads> ProductUploads { get; set; }
            public ProductPages ProductPage { get; set; }
            public ProductPageSettings ProductPageSetting { get; set; }
        }

        public Products GetProductById(int id)
        {
            return _context.Products.Join(_context.ProductTemplates, Product => Product.ProductTemplateId, ProductTemplate => ProductTemplate.Id, (Product, ProductTemplate) => new { Product, ProductTemplate })
                                    .Where(x => x.ProductTemplate.WebsiteId == _config.Value.WebsiteId)
                                    .Select(x => x.Product)
                                    .FirstOrDefault(Product => Product.Id == id && Product.Active == true);
        }

        public ProductBundle GetProductBundle(int websiteLanguageId, string alternateGuid, string url, bool active)
        {
            return GetProductBundles(websiteLanguageId, alternateGuid, active).FirstOrDefault(x => x.ProductPageSetting.Url == url);
        }

        public ProductBundle GetProductBundleByProductId(int websiteLanguageId, string alternateGuid, int productId, bool active)
        {
            return GetProductBundles(websiteLanguageId, alternateGuid, active).FirstOrDefault(x => x.Product.Id == productId);
        }

        public List<ProductBundle> GetProductBundles(int websiteLanguageId, string alternateGuid, bool active)
        {
            return _context.ProductTemplates.Join(_context.Products.OrderBy(Product => Product.CustomOrder), ProductTemplate => ProductTemplate.Id, Product => Product.ProductTemplateId, (ProductTemplate, Product) => new { ProductTemplate, Product })
                                            .GroupJoin(_context.ProductFiles.Where(ProductFile => ProductFile.Active == true)
                                                                            .Join(_context.ProductUploads, ProductFile => ProductFile.ProductUploadId, ProductUpload => ProductUpload.Id, (ProductFile, ProductUpload) => new { ProductFile, ProductUpload })
                                                                            .OrderBy(x => x.ProductFile.CustomOrder),
                                            x => x.Product.Id, ProductFilesAndUpload => ProductFilesAndUpload.ProductFile.ProductId, (x, ProductFilesAndUpload) => new { x.Product, x.ProductTemplate, ProductFilesAndUpload })
                                            .GroupJoin(_context.ProductResources.Where(ProductResource => ProductResource.WebsiteLanguageId == websiteLanguageId)
                                                                                .Join(_context.ProductFields, ProductResource => ProductResource.ProductFieldId, ProductField => ProductField.Id, (ProductResources, ProductField) => new { ProductResources, ProductField }),
                                            x => x.Product.Id, ProductResourcesAndField => ProductResourcesAndField.ProductResources.ProductId, (x, ProductResourcesAndField) => new { x.Product, x.ProductTemplate, x.ProductFilesAndUpload, ProductResourcesAndField })
                                            .GroupJoin(_context.ProductPageSettings.Where(ProductPageSetting => ProductPageSetting.WebsiteLanguageId == websiteLanguageId), x => x.Product.Id, ProductPageSetting => ProductPageSetting.ProductId, (x, ProductPageSettings) => new { x.Product, x.ProductTemplate, x.ProductFilesAndUpload, x.ProductResourcesAndField, ProductPageSettings })
                                            .Join(_context.ProductPages.Where(ProductPage => ProductPage.PageAlternateGuid == alternateGuid && ProductPage.WebsiteLanguageId == websiteLanguageId), x => x.Product.Id, ProductPage => ProductPage.ProductId, (x, ProductPage) => new { x.Product, x.ProductTemplate, x.ProductFilesAndUpload, x.ProductResourcesAndField, x.ProductPageSettings, ProductPage })
                                            .Where(x => x.Product.Active == active)
                                            .Where(x => x.ProductTemplate.WebsiteId == _config.Value.WebsiteId)
                                            .Select(x => new ProductBundle()
                                            {
                                                ProductFiles = x.ProductFilesAndUpload.Select(y => y.ProductFile),
                                                ProductResources = x.ProductResourcesAndField.Select(y => y.ProductResources),
                                                Product = x.Product,
                                                ProductFields = x.ProductResourcesAndField.Select(y => y.ProductField),
                                                ProductTemplate = x.ProductTemplate,
                                                ProductUploads = x.ProductFilesAndUpload.Select(y => y.ProductUpload),
                                                ProductPage = x.ProductPage,
                                                ProductPageSetting = x.ProductPageSettings.FirstOrDefault()
                                            })
                                            .ToList();
        }

        public List<Products> GetProductsByPageAlternateGuidAndWebsiteLanguageId(string pageAlternateGuid, int websiteLanguageId)
        {
            return _context.Products.Where(Product => Product.Active == true)
                                    .Join(_context.ProductPages.Where(ProductPage => ProductPage.PageAlternateGuid == pageAlternateGuid && ProductPage.WebsiteLanguageId == websiteLanguageId), Product => Product.Id, ProductPage => ProductPage.ProductId, (Product, ProductPage) => new { Product, ProductPage })
                                    .Select(x => x.Product)
                                    .ToList();
        }

        public Dictionary<string, object> ConvertProductUploadsToJson(ProductBundle bundle, string url)
        {
            Dictionary<string, object> uploads = new Dictionary<string, object>();

            if (bundle.ProductUploads != null)
            {
                foreach (ProductUploads productUpload in bundle.ProductUploads.Distinct())
                {
                    List<Dictionary<string, object>> files = new List<Dictionary<string, object>>();
                    foreach (ProductFiles productFile in bundle.ProductFiles.Where(ProductFile => ProductFile.ProductUploadId == productUpload.Id && ProductFile.Active == true))
                    {
                        files.Add(new Dictionary<string, object>()
                        {
                            { "originalPath", url + productFile.OriginalPath.Replace("~/", "/")},
                            { "compressedPath", url + productFile.CompressedPath.Replace("~/", "/")},
                            { "alt", productFile.Alt}
                        });
                    }

                    uploads.Add(productUpload.CallName, files);
                }
            }

            return uploads;
        }

        public Dictionary<string, object> ConvertProductFieldsToJson(ProductBundle productBundle)
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();

            if (productBundle.ProductFields != null)
            {
                foreach (ProductFields productField in productBundle.ProductFields.Distinct())
                {
                    if (productField.Type.ToLower() != "selectlinkedto")
                    {
                        string text = productBundle.ProductResources.FirstOrDefault(ProductResource => ProductResource.ProductFieldId == productField.Id).Text;
                        if (productField.Type.ToLower() == "textarea")
                        {
                            text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                        }

                        fields.Add(productField.CallName, text);
                    }
                    else
                    {
                        //In progress
                    }
                }
            }

            return fields;
        }

        public Dictionary<string, object> ConvertProductToJson(ProductBundle productBundle, Dictionary<string, object> uploads, Dictionary<string, object> fields)
        {
            return new Dictionary<string, object>() {
                { "id", productBundle.Product.Id },
                { "price", _commerce.GetPriceFormat(decimal.Round(productBundle.Product.Price, _digitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), _currency) },
                { "priceClean", decimal.Round(productBundle.Product.Price, _digitsAfterDecimal, MidpointRounding.AwayFromZero).ToString() },
                { "promoPrice", _commerce.GetPriceFormat(decimal.Round(productBundle.Product.PromoPrice, _digitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), _currency) },
                { "promoPriceClean", decimal.Round(productBundle.Product.PromoPrice, _digitsAfterDecimal, MidpointRounding.AwayFromZero).ToString() },
                { "currency", _currency },
                { "promo", new Commerce(_context).IsPromoEnabled(productBundle.Product.PromoSchedule ?? false, productBundle.Product.PromoFromDate, productBundle.Product.PromoToDate, productBundle.Product.PromoPrice, productBundle.Product.Price) },
                { "promoTill", (productBundle.Product.PromoSchedule ?? false) ? productBundle.Product.PromoToDate.ToString("yyyy-MM-dd") : "" },
                { "name", productBundle.Product.Name },
                { "height", productBundle.Product.Height },
                { "length", productBundle.Product.Length },
                { "width", productBundle.Product.Width },
                { "weight", productBundle.Product.Weight },
                { "stockQuantity", CheckProductQuantity(productBundle.Product.Id, productBundle.Product.StockQuantity) },
                { "stockStatus", productBundle.Product.StockStatus },
                { "manageStock", productBundle.Product.ManageStock },
                { "maxPerOrder", productBundle.Product.MaxPerOrder },
                { "backorders", productBundle.Product.Backorders },
                { "active", productBundle.Product.Active },
                { "pageUrl", _page.GetPageUrlByAlternateGuid(productBundle.ProductPage.PageAlternateGuid) + "/" + productBundle.ProductPageSetting?.Url },
                { "pageTitle", productBundle.ProductPageSetting?.Title },
                { "pageKeywords", productBundle.ProductPageSetting?.Keywords },
                { "pageDescription", productBundle.ProductPageSetting?.Description },
                { "htmlIdentifier", Regex.Replace(productBundle.Product.Id + "_" + productBundle.Product.Name, @"[^A-Za-z0-9_\.~]+", "-") },
                { "files", uploads },
                { "resources", fields }
            };
        }

        public int CheckProductQuantity(int productId, int stockQuantity)
        {
            if (_reserveMinuts == 0)
            {
                _reserveMinuts = Int32.Parse(new Setting(_context).GetSettingValueByKey("reserveMinuts", "website", _config.Value.WebsiteId));
            }

            List<OrderLines> _orderLines = new Order(_context, _config).GetOrderLinesByProductIdCheckedByReserveMinuts(productId, _reserveMinuts);
            foreach(OrderLines orderLine in _orderLines)
            {
                stockQuantity = stockQuantity - orderLine.Quantity;
            }

            return (stockQuantity < 0) ? 0 : stockQuantity;
        }

        public List<Dictionary<string, object>> ConvertProductBundlesToJson(List<ProductBundle> productBundles, int websiteLanguageId)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            _page = new Page(_context, _config);
            _page.SetPageBundleAndWebsiteBundle(websiteLanguageId);

            _commerce = new Commerce(_context, _config);
            _commerce.SetPriceFormatVariables();

            _digitsAfterDecimal = Int32.Parse(new Setting(_context).GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
            _currency = new Setting(_context).GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (ProductBundle productBundle in productBundles)
            {
                Dictionary<string, object> _productBundle = ConvertProductBundleToJson(productBundle, websiteLanguageId, url);
                if (productBundle != null)
                {
                    list.Add(_productBundle);
                }
            }

            return list;
        }

        public Dictionary<string, object> ConvertProductBundleToJson(ProductBundle productBundle, int websiteLanguageId, string url = "")
        {
            if (url == "") { url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId); }

            if (_page == null && websiteLanguageId != 0)
            {
                _page = new Page(_context, _config);
                _page.SetPageBundleAndWebsiteBundle(websiteLanguageId);

                _commerce = new Commerce(_context, _config);
                _commerce.SetPriceFormatVariables();

                _digitsAfterDecimal = Int32.Parse(new Setting(_context).GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
                _currency = new Setting(_context).GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);
            }

            if (productBundle != null)
            {
                Dictionary<string, object> uploads = ConvertProductUploadsToJson(productBundle, url);
                Dictionary<string, object> fields = ConvertProductFieldsToJson(productBundle);
                Dictionary<string, object> result = ConvertProductToJson(productBundle, uploads, fields);

                if (AddBreadcrumbsJson && websiteLanguageId != 0) { result = _page.AddBreadcrumbsToJson(result, null, websiteLanguageId, null, productBundle); }

                return result;
            }

            return null;
        }

        public string GetProductResourceTextByCallName(ProductBundle productBundle, string callName)
        {
            ProductResources _productResource = productBundle.ProductFields.Where(ProductField => ProductField.CallName == callName).Join(productBundle.ProductResources, ProductField => ProductField.Id, ProductResource => ProductResource.ProductFieldId, (ProductField, ProductResource) => new { ProductField, ProductResource }).Select(x => x.ProductResource).FirstOrDefault();

            return _productResource != null ? _productResource.Text : "";
        }

        public ObjectResult GetProductBundleJson(int websiteLanguageId, string alternateGuid, string url, bool active)
        {
            ProductBundle _productBundle =  GetProductBundle(websiteLanguageId, alternateGuid, url, active);
            if (_productBundle == null) return NotFound("");

            return Ok(ConvertProductBundleToJson(_productBundle, websiteLanguageId));
        }

        public ObjectResult GetProductBundlesJson(int websiteLanguageId, string alternateGuid, bool active)
        {
            return Ok(ConvertProductBundlesToJson(GetProductBundles(websiteLanguageId, alternateGuid, active), websiteLanguageId));
        }
    }
}
