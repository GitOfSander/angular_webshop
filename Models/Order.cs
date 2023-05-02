using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using Site.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Site.Models.Product;
using static Site.Models.Shipping;

namespace Site.Models
{
    public class Order : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private readonly IDataProtectionProvider _provider;
        private readonly IEmailSender _emailSender;

        public Order(SiteContext context, IOptions<AppSettings> config = null, IDataProtectionProvider provider = null, IEmailSender emailSender = null)
        {
            _context = context;
            _config = config;
            _provider = provider;
            _emailSender = emailSender;
        }

        private Orders _order;
        private int _quantity;
        private int _shippingZoneMethodId = 0;
        private decimal _total;
        private decimal _productsTotal = 0.00m;
        private List<string> _shippingClassesList;
        private List<Dictionary<string, object>> _shippingMethods;
        private Dictionary<decimal, decimal> _prices;

        public Commerce Commerce;
        public string Currency;
        public int DigitsAfterDecimal;
        //public decimal Tax;
        public decimal ShippingTax;
        public bool LockPrices = false;
        public Dictionary<string, object> TaxClasses = new Dictionary<string, object>();

        public class OrderAndProduct
        {
            public OrderLines OrderLine { get; set; }
            public OrderShippingZoneMethods OrderShippingZoneMethod { get; set; }
            public IEnumerable<ProductFiles> ProductFiles { get; set; }
            public IEnumerable<ProductResources> ProductResources { get; set; }
            public Products Product { get; set; }
            public IEnumerable<ProductFields> ProductFields { get; set; }
            public IEnumerable<ProductUploads> ProductUploads { get; set; }
            public TaxRates TaxRate { get; set; }
        }

        public class OrderBundle
        {
            public Orders Order { get; set; }
            public IEnumerable<OrderCoupons> OrderCoupons { get; set; }
            public IEnumerable<OrderFees> OrderFees { get; set; }
            public IEnumerable<OrderLines> OrderLines { get; set; }
            public IEnumerable<OrderRefundLines> OrderRefundLines { get; set; }
            public IEnumerable<OrderRefunds> OrderRefunds { get; set; }
            public IEnumerable<OrderShippingZoneMethods> OrderShippingZoneMethods { get; set; }
        }

        public Orders GetOrderByReserveGuid(string reserveGuid)
        {
            //Get order by reserve guid and check if provided minutes are lower or equal as reserved minutes
            return _context.Orders.FirstOrDefault(Order => Order.ReserveGuid == reserveGuid && Order.Status.ToLower() == "reserved" && (int)DateTime.Now.Subtract(Order.CreatedDate).TotalMinutes <= Int32.Parse(new Setting(_context).GetSettingValueByKey("reserveMinuts", "website", _config.Value.WebsiteId)));
        }

        public List<OrderLines> GetOrderLinesByProductIdCheckedByReserveMinuts(int productId, int reserveMinuts)
        {
            //Get orders and check if provided minutes are lower or equal as reserved minutes and return order lines
            return _context.Orders.Where(Order => Order.Status.ToLower() == "reserved" && (int)DateTime.Now.Subtract(Order.CreatedDate).TotalMinutes <= reserveMinuts && Order.WebsiteId == _config.Value.WebsiteId)
                                  .Join(_context.OrderLines.Where(OrderLine => OrderLine.ProductId == productId), Order => Order.Id, OrderLine => OrderLine.OrderId, (Order, OrderLine) => new { Order, OrderLine })
                                  .Select(x => x.OrderLine)
                                  .ToList();
        }

        public OrderLines GetOrderLineByProductIdAndOrderId(int productId, int orderId)
        {
            return _context.OrderLines.FirstOrDefault(OrderLine => OrderLine.ProductId == productId && OrderLine.OrderId == orderId);
        }

        public OrderShippingZoneMethods GetOrderShippingZoneMethodByOrderId(int orderId)
        {
            return _context.OrderShippingZoneMethods.FirstOrDefault(OrderShippingZoneMethod => OrderShippingZoneMethod.OrderId == orderId);
        }

        public List<OrderAndProduct> GetOrderAndProducts(string reserveGuid, int websiteLanguageId)
        {
            //Get order line and product by reserve guid and check if provided minutes are lower or equal as reserved minutes
            return _context.OrderLines.Join(_context.Orders.Where(Order => Order.WebsiteId == _config.Value.WebsiteId && Order.ReserveGuid == reserveGuid && Order.Status.ToLower() == "reserved" && (int)DateTime.Now.Subtract(Order.CreatedDate).TotalMinutes <= Int32.Parse(new Setting(_context).GetSettingValueByKey("reserveMinuts", "website", _config.Value.WebsiteId))), OrderLine => OrderLine.OrderId, Order => Order.Id, (OrderLine, Order) => new { OrderLine, Order })
                                      .GroupJoin(_context.OrderShippingZoneMethods, x => x.Order.Id, OrderShippingZoneMethod => OrderShippingZoneMethod.OrderId, (x, OrderShippingZoneMethods) => new { x.OrderLine, OrderShippingZoneMethods })
                                      .Join(_context.Products, x => x.OrderLine.ProductId, Product => Product.Id, (x, Product) => new { x.OrderLine, x.OrderShippingZoneMethods, Product })
                                      .GroupJoin(_context.ProductFiles.Where(ProductFile => ProductFile.Active == true)
                                                                      .Join(_context.ProductUploads, ProductFile => ProductFile.ProductUploadId, ProductUpload => ProductUpload.Id, (ProductFile, ProductUpload) => new { ProductFile, ProductUpload })
                                                                      .OrderBy(x => x.ProductFile.CustomOrder),
                                      x => x.Product.Id, ProductFilesAndUpload => ProductFilesAndUpload.ProductFile.ProductId, (x, ProductFilesAndUpload) => new { x.OrderLine, x.OrderShippingZoneMethods, x.Product, ProductFilesAndUpload })
                                      .GroupJoin(_context.ProductResources.Where(ProductResource => ProductResource.WebsiteLanguageId == websiteLanguageId)
                                                                          .Join(_context.ProductFields, ProductResource => ProductResource.ProductFieldId, ProductField => ProductField.Id, (ProductResources, ProductField) => new { ProductResources, ProductField }),
                                      x => x.Product.Id, ProductResourcesAndField => ProductResourcesAndField.ProductResources.ProductId, (x, ProductResourcesAndField) => new { x.OrderLine, x.OrderShippingZoneMethods, x.Product, x.ProductFilesAndUpload, ProductResourcesAndField })
                                      .Join(_context.TaxRates.Where(TaxRate => TaxRate.Default == true), x => x.Product.TaxClassId, TaxRate => TaxRate.TaxClassId, (x, TaxRate) => new { x.OrderLine, x.OrderShippingZoneMethods, x.Product, x.ProductFilesAndUpload, x.ProductResourcesAndField, TaxRate })
                                      .Select(x => new OrderAndProduct()
                                      {
                                          OrderLine = x.OrderLine,
                                          OrderShippingZoneMethod = x.OrderShippingZoneMethods.FirstOrDefault(),
                                          ProductFiles = x.ProductFilesAndUpload.Select(y => y.ProductFile),
                                          ProductResources = x.ProductResourcesAndField.Select(y => y.ProductResources),
                                          Product = x.Product,
                                          ProductFields = x.ProductResourcesAndField.Select(y => y.ProductField),
                                          ProductUploads = x.ProductFilesAndUpload.Select(y => y.ProductUpload),
                                          TaxRate = x.TaxRate
                                      })
                                      .ToList();
        }

        public OrderBundle GetOrderBundle(int id)
        {
            return _context.Orders.GroupJoin(_context.OrderCoupons.OrderBy(OrderCoupon => OrderCoupon.Name), Order => Order.Id, OrderCoupon => OrderCoupon.OrderId, (Order, OrderCoupon) => new { Order, OrderCoupon })
                                  .GroupJoin(_context.OrderFees.OrderBy(OrderFee => OrderFee.Name), x => x.Order.Id, OrderFee => OrderFee.OrderId, (x, OrderFee) => new { x.Order, x.OrderCoupon, OrderFee })
                                  .GroupJoin(_context.OrderLines.OrderBy(OrderLine => OrderLine.Name), x => x.Order.Id, OrderLine => OrderLine.OrderId, (x, OrderLine) => new { x.Order, x.OrderCoupon, x.OrderFee, OrderLine })
                                  .GroupJoin(_context.OrderRefundLines.Join(_context.OrderLines, OrderRefundLine => OrderRefundLine.OrderLineId, OrderLine => OrderLine.Id, (OrderRefundLine, OrderLine) => new { OrderRefundLine, OrderLine }).Select(x => x.OrderRefundLine), x => x.Order.Id, OrderRefundLine => OrderRefundLine.OrderLineId, (x, OrderRefundLine) => new { x.Order, x.OrderCoupon, x.OrderFee, x.OrderLine, OrderRefundLine })
                                  .GroupJoin(_context.OrderRefunds, x => x.Order.Id, OrderRefund => OrderRefund.OrderId, (x, OrderRefund) => new { x.Order, x.OrderCoupon, x.OrderFee, x.OrderLine, x.OrderRefundLine, OrderRefund })
                                  .GroupJoin(_context.OrderShippingZoneMethods.OrderBy(OrderShippingZoneMethod => OrderShippingZoneMethod.Name), x => x.Order.Id, OrderShippingZoneMethod => OrderShippingZoneMethod.OrderId, (x, OrderShippingZoneMethod) => new { x.Order, x.OrderCoupon, x.OrderFee, x.OrderLine, x.OrderRefundLine, x.OrderRefund, OrderShippingZoneMethod })
                                  .Select(x => new OrderBundle()
                                  {
                                      Order = x.Order,
                                      OrderCoupons = x.OrderCoupon,
                                      OrderFees = x.OrderFee,
                                      OrderLines = x.OrderLine,
                                      OrderRefundLines = x.OrderRefundLine,
                                      OrderRefunds = x.OrderRefund,
                                      OrderShippingZoneMethods = x.OrderShippingZoneMethod,
                                  })
                                  .FirstOrDefault(OrderBundle => OrderBundle.Order.WebsiteId == _config.Value.WebsiteId && OrderBundle.Order.Id == id);
        }

        public OrderBundle GetOrderBundleByTransactionId(string transactionId)
        {
            return _context.Orders.GroupJoin(_context.OrderCoupons.OrderBy(OrderCoupon => OrderCoupon.Name), Order => Order.Id, OrderCoupon => OrderCoupon.OrderId, (Order, OrderCoupon) => new { Order, OrderCoupon })
                                  .GroupJoin(_context.OrderFees.OrderBy(OrderFee => OrderFee.Name), x => x.Order.Id, OrderFee => OrderFee.OrderId, (x, OrderFee) => new { x.Order, x.OrderCoupon, OrderFee })
                                  .GroupJoin(_context.OrderLines.OrderBy(OrderLine => OrderLine.Name), x => x.Order.Id, OrderLine => OrderLine.OrderId, (x, OrderLine) => new { x.Order, x.OrderCoupon, x.OrderFee, OrderLine })
                                  .GroupJoin(_context.OrderRefundLines.Join(_context.OrderLines, OrderRefundLine => OrderRefundLine.OrderLineId, OrderLine => OrderLine.Id, (OrderRefundLine, OrderLine) => new { OrderRefundLine, OrderLine }).Select(x => x.OrderRefundLine), x => x.Order.Id, OrderRefundLine => OrderRefundLine.OrderLineId, (x, OrderRefundLine) => new { x.Order, x.OrderCoupon, x.OrderFee, x.OrderLine, OrderRefundLine })
                                  .GroupJoin(_context.OrderRefunds, x => x.Order.Id, OrderRefund => OrderRefund.OrderId, (x, OrderRefund) => new { x.Order, x.OrderCoupon, x.OrderFee, x.OrderLine, x.OrderRefundLine, OrderRefund })
                                  .GroupJoin(_context.OrderShippingZoneMethods.OrderBy(OrderShippingZoneMethod => OrderShippingZoneMethod.Name), x => x.Order.Id, OrderShippingZoneMethod => OrderShippingZoneMethod.OrderId, (x, OrderShippingZoneMethod) => new { x.Order, x.OrderCoupon, x.OrderFee, x.OrderLine, x.OrderRefundLine, x.OrderRefund, OrderShippingZoneMethod })
                                  .Select(x => new OrderBundle()
                                  {
                                      Order = x.Order,
                                      OrderCoupons = x.OrderCoupon,
                                      OrderFees = x.OrderFee,
                                      OrderLines = x.OrderLine,
                                      OrderRefundLines = x.OrderRefundLine,
                                      OrderRefunds = x.OrderRefund,
                                      OrderShippingZoneMethods = x.OrderShippingZoneMethod,
                                  })
                                  .FirstOrDefault(OrderBundle => OrderBundle.Order.WebsiteId == _config.Value.WebsiteId && OrderBundle.Order.TransactionId == transactionId);
        }

        public Orders GetOrderByTransactionId(string transactionId)
        {
            return _context.Orders.FirstOrDefault(Order => Order.WebsiteId == _config.Value.WebsiteId && Order.TransactionId == transactionId);
        }

        public Orders InsertOrder(Orders order)
        {
            Encryptor encryptor = new Encryptor(_provider);
            order.BillingAddressLine1 = encryptor.Encrypt(order.BillingAddressLine1);
            order.BillingAddressLine2 = encryptor.Encrypt(order.BillingAddressLine2);
            order.BillingCity = encryptor.Encrypt(order.BillingCity);
            order.BillingCompany = encryptor.Encrypt(order.BillingCompany);
            order.BillingCountry = encryptor.Encrypt(order.BillingCountry);
            order.BillingEmail = encryptor.Encrypt(order.BillingEmail);
            order.BillingFirstName = encryptor.Encrypt(order.BillingFirstName);
            order.BillingLastName = encryptor.Encrypt(order.BillingLastName);
            order.BillingPhoneNumber = encryptor.Encrypt(order.BillingPhoneNumber);
            order.BillingState = encryptor.Encrypt(order.BillingState);
            order.BillingVatNumber = encryptor.Encrypt(order.BillingVatNumber);
            order.BillingZipCode = encryptor.Encrypt(order.BillingZipCode);
            order.ShippingAddressLine1 = encryptor.Encrypt(order.ShippingAddressLine1);
            order.ShippingAddressLine2 = encryptor.Encrypt(order.ShippingAddressLine2);
            order.ShippingCity = encryptor.Encrypt(order.ShippingCity);
            order.ShippingCompany = encryptor.Encrypt(order.ShippingCompany);
            order.ShippingCountry = encryptor.Encrypt(order.ShippingCountry);
            order.ShippingFirstName = encryptor.Encrypt(order.ShippingFirstName);
            order.ShippingLastName = encryptor.Encrypt(order.ShippingLastName);
            order.ShippingState = encryptor.Encrypt(order.ShippingState);
            order.ShippingZipCode = encryptor.Encrypt(order.ShippingZipCode);

            _context.Orders.Add(order);
            _context.SaveChanges();

            return order;
        }

        public void InsertOrderLine(OrderLines orderLine)
        {
            _context.OrderLines.Add(orderLine);
            _context.SaveChanges();
        }

        public void InsertOrderShippingZoneMethod(OrderShippingZoneMethods orderShippingZoneMethods)
        {
            _context.OrderShippingZoneMethods.Add(orderShippingZoneMethods);
            _context.SaveChanges();
        }

        public void UpdateOrderCreatedDate(Orders order, DateTime createdDate)
        {
            _context.Entry(order).CurrentValues.SetValues(order.CreatedDate = createdDate);
            _context.SaveChanges();
        }

        public void UpdateOrderShippingZoneMethodShippingZoneMethodId(OrderShippingZoneMethods orderShippingZoneMethods, int shippingZoneMethodId)
        {
            _context.Entry(orderShippingZoneMethods).CurrentValues.SetValues(orderShippingZoneMethods.ShippingZoneMethodId = shippingZoneMethodId);
            _context.SaveChanges();
        }

        public void UpdateOrderShippingZoneMethod(OrderShippingZoneMethods orderShippingZoneMethod)
        {
            _context.OrderShippingZoneMethods.Update(orderShippingZoneMethod);
            _context.SaveChanges();
        }

        public void UpdateOrder(Orders order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void UpdateOrderLineQuantity(OrderLines orderLine, int quantity)
        {
            _context.Entry(orderLine).CurrentValues.SetValues(orderLine.Quantity = quantity);
            _context.SaveChanges();
        }

        public void IncrementOrderLineQuantity(OrderLines orderLine, int quantity)
        {
            _context.Entry(orderLine).CurrentValues.SetValues(orderLine.Quantity = orderLine.Quantity + quantity);
            _context.SaveChanges();
        }

        public ObjectResult insertOrUpdateOrderShippingZoneMethod(int shippingZoneMethodId, string reseverGuid)
        {
            Orders _order = GetOrderByReserveGuid(reseverGuid);
            if (_order != null)
            {
                OrderShippingZoneMethods _orderShippingZoneMethod = GetOrderShippingZoneMethodByOrderId(_order.Id);
                if (_orderShippingZoneMethod != null)
                {
                    UpdateOrderShippingZoneMethodShippingZoneMethodId(_orderShippingZoneMethod, shippingZoneMethodId);
                }
                else
                {
                    InsertOrderShippingZoneMethod(new OrderShippingZoneMethods()
                    {
                        OrderId = _order.Id,
                        ShippingZoneMethodId = shippingZoneMethodId
                    });
                }
            }

            return Ok("");
        }

        public void insertOrUpdateOrderShippingZoneMethod(int shippingZoneMethodId, string name, decimal price, bool taxable, string reseverGuid)
        {
            if (_order != null)
            {
                OrderShippingZoneMethods _orderShippingZoneMethod = GetOrderShippingZoneMethodByOrderId(_order.Id);
                if (_orderShippingZoneMethod != null)
                {
                    _orderShippingZoneMethod.ShippingZoneMethodId = shippingZoneMethodId;
                    _orderShippingZoneMethod.Name = name;
                    _orderShippingZoneMethod.Price = price;
                    _orderShippingZoneMethod.Taxable = taxable;
                    UpdateOrderShippingZoneMethod(_orderShippingZoneMethod);
                }
                else
                {
                    InsertOrderShippingZoneMethod(new OrderShippingZoneMethods()
                    {
                        OrderId = _order.Id,
                        ShippingZoneMethodId = shippingZoneMethodId,
                        Name = name,
                        Price = price,
                        Taxable = taxable
                    });
                }
            }
        }

        public void UpdateProductsStockQuantity(IEnumerable<OrderLines> orderLines)
        {
            foreach(OrderLines orderLine in orderLines)
            {
                DecreaseProductQuantity(orderLine.Quantity, orderLine.ProductId);
            }
        }

        public void DecreaseProductQuantity(int quantity, int productId)
        {
            _context.Database.ExecuteSqlCommand("Decrease_Product_Quantity @Quantity, @ProductId", new SqlParameter("Quantity", quantity),
                                                                                                   new SqlParameter("ProductId", productId));
        }

        public void DeleteOrderLineByOrderIdAndProductId(int orderId, int productId)
        {
            OrderLines _orderLine = _context.OrderLines.FirstOrDefault(OrderLine => OrderLine.OrderId == orderId && OrderLine.ProductId == productId);
            _context.OrderLines.Remove(_orderLine);
            _context.SaveChanges();
        }

        public async Task<ObjectResult> DeleteOrderLineAndGetOrderJsonAsync(int productId, string reserveGuid, int websiteLanguageId)
        {
            //Create order if reserveGuid is empty
            Orders _order = GetOrderByReserveGuid(reserveGuid);

            //Delete order line
            DeleteOrderLineByOrderIdAndProductId(_order.Id, productId);

            //Update CreatedDate column in Order table after adding/updating a product
            UpdateOrderCreatedDate(_order, DateTime.Now);

            return await GetOrderJsonAsync(_order.ReserveGuid, websiteLanguageId);
        }

        public async Task<ObjectResult> UpdateOrderLineAndGetOrderJsonAsync(int productId, string reserveGuid, int quantity, bool increment, int websiteLanguageId)
        {
            //Create order if reserveGuid is empty
            Orders _order = (reserveGuid != "") ? GetOrderByReserveGuid(reserveGuid) : CreateReserveOrder();

            //Create order if there is no order found with the guid or the provided minutes are higher then the reserved minuts
            _order = (_order != null) ? _order : CreateReserveOrder();

            //Update CreatedDate column in Order table
            UpdateOrderCreatedDate(_order, DateTime.Now);

            //Get order line
            OrderLines _orderLine = GetOrderLineByProductIdAndOrderId(productId, _order.Id);

            //CHeck is product exist
            Product product = new Product(_context, _config);
            Products _product = product.GetProductById(productId);
            if (_product != null)
            {
                //Is product in stock
                int stockQuantity = product.CheckProductQuantity(_product.Id, _product.StockQuantity);
                if (_product.ManageStock == false && _product.StockStatus.ToLower() != "out" || 
                    _product.ManageStock && stockQuantity == 0 && _product.Backorders.ToLower() != "no" || 
                    _product.ManageStock && stockQuantity != 0)
                {
                    //MaxPerOrder is enabled if higher then 0
                    //Do not continue if MaxPerOrder is enabled and quantity is equal as MaxPerOrder
                    int orderQty = (_orderLine != null ? _orderLine.Quantity : 0);
                    if (_product.MaxPerOrder == 0 ||
                        increment && _product.MaxPerOrder > 0 && orderQty < _product.MaxPerOrder ||
                        !increment && _product.MaxPerOrder > 0)
                    {
                        //Update order line if it exist otherwise create new one
                        if (_orderLine != null)
                        {
                            if (increment)
                            {
                                IncrementOrderLineQuantity(_orderLine, quantity);
                            }
                            else
                            {
                                //Decrease quantity if it is higher then MaxPerOrder
                                if (_product.MaxPerOrder > 0 && quantity > _product.MaxPerOrder)
                                {
                                    quantity = (quantity - (quantity - _product.MaxPerOrder));
                                }
                                UpdateOrderLineQuantity(_orderLine, quantity);
                            }
                        }
                        else
                        {
                            InsertOrderLine(new OrderLines()
                            {
                                OrderId = _order.Id,
                                ProductId = productId,
                                Quantity = quantity
                            });
                        }
                    }
                }

                var result = await GetOrderJsonAsync(_order.ReserveGuid, websiteLanguageId);
                return Ok(new Dictionary<string, object>() {
                    { "stockQuantity", stockQuantity },
                    { "backorders", _product.Backorders.ToLower() },
                    { "maxPerOrder", _product.MaxPerOrder },
                    { "manageStock", _product.ManageStock },
                    { "order", result.Value }
                });
            }

            //Product does not exist any more so delete the product
            DeleteOrderLineByOrderIdAndProductId(_order.Id, productId);

            return Ok(new Dictionary<string, object>() {
                { "productRemoved", true },
                { "order", GetOrderJsonAsync(_order.ReserveGuid, websiteLanguageId) }
            });
        }

        public Orders CreateReserveOrder()
        {
            return InsertOrder(new Orders()
            {
                CreatedDate = DateTime.Now,
                ReserveGuid = GetUniqueGuid(),
                WebsiteId = _config.Value.WebsiteId,
                Status = "reserved"
            });
        }

        public string GetUniqueGuid()
        {
            string guid = Guid.NewGuid().ToString();

            return (GetOrderByReserveGuid(guid) != null) ? GetUniqueGuid() : guid;
        }

        public List<OrderAndProduct> LockOrderPrices(List<OrderAndProduct> orderAndProducts)
        {
            foreach(OrderAndProduct orderAndProduct in orderAndProducts)
            {
                bool promo = Commerce.IsPromoEnabled(orderAndProduct.Product.PromoSchedule ?? false, orderAndProduct.Product.PromoFromDate, orderAndProduct.Product.PromoToDate, orderAndProduct.Product.PromoPrice, orderAndProduct.Product.Price);
                string name = new Product(_context, _config).GetProductResourceTextByCallName(new ProductBundle()
                {
                    ProductFields = orderAndProduct.ProductFields,
                    ProductResources = orderAndProduct.ProductResources
                }, "title");

                decimal price = (promo ? orderAndProduct.Product.PromoPrice : orderAndProduct.Product.Price);
                decimal discount = (promo ? (orderAndProduct.Product.Price - orderAndProduct.Product.PromoPrice) : 0.0000M);

                ////Remove taxes
                //price = (price - (((price) / (100 + orderAndProduct.TaxRate.Rate)) * orderAndProduct.TaxRate.Rate));
                //discount = (discount - (((discount) / (100 + orderAndProduct.TaxRate.Rate)) * orderAndProduct.TaxRate.Rate));

                _context.Entry(orderAndProduct.OrderLine).CurrentValues.SetValues(orderAndProduct.OrderLine.Price = decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero));
                _context.Entry(orderAndProduct.OrderLine).CurrentValues.SetValues(orderAndProduct.OrderLine.Discount = decimal.Round(discount, DigitsAfterDecimal, MidpointRounding.AwayFromZero));
                _context.Entry(orderAndProduct.OrderLine).CurrentValues.SetValues(orderAndProduct.OrderLine.Name = name);
                _context.Entry(orderAndProduct.OrderLine).CurrentValues.SetValues(orderAndProduct.OrderLine.TaxRate = orderAndProduct.TaxRate.Rate);
                _context.Entry(orderAndProduct.OrderLine).CurrentValues.SetValues(orderAndProduct.OrderLine.TaxShipping = orderAndProduct.TaxRate.Shipping);
                _context.SaveChanges();
            }

            return orderAndProducts;
        }

        public async Task<ObjectResult> GetOrderJsonAsync(string reserveGuid, int websiteLanguageId)
        {
            Setting setting = new Setting(_context);
            DigitsAfterDecimal = Int32.Parse(setting.GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
            Currency = setting.GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);

            Commerce = new Commerce(_context, _config);
            Commerce.SetPriceFormatVariables();

            //if (websiteLanguageId == 0)
            //{
            //    new Error().ReportError(_config.Value.SystemWebsiteId, "NAVIGATIONCONTROLLER#02", "Kan navigaties niet ophalen.", "", e.Message);
            //    string[] emails = { "sander@unveil.nl" };
            //    string[] bccEmails = { };
            //    string[] ccEmails = { };
            //    await _emailSender.SendEmailAsync(emails, "FOUTMELDING: DOMEIN.nl", "Bij het verkrijgen van een order werd er geen websiteLanguageId (" + websiteLanguageId + ") meegestuurd", "Unveil", "info@unveil.nl", bccEmails, ccEmails, null);
            //}

            List<OrderAndProduct> _orderAndProducts = GetOrderAndProducts(reserveGuid, websiteLanguageId);
            DistributePricesToTaxes(_orderAndProducts);
            decimal shippingCosts = GetTotalShippingCosts(_orderAndProducts, reserveGuid);
            if (LockPrices) {
                //Lock product Prices
                _orderAndProducts = LockOrderPrices(_orderAndProducts);

                //Update CreatedDate column in Order table after adding/updating a product
                _order = GetOrderByReserveGuid(reserveGuid);
                if (_order != null)
                {
                    _order.CreatedDate = DateTime.Now;
                    _order.Currency = Currency;
                    UpdateOrder(_order);
                }

                if (_shippingMethods != null) { 
                    Dictionary<string, object> _shippingMethod = _shippingMethods.FirstOrDefault(x => new Dictionary<string, object>() { { "selected", true } }.All(x.Contains));
                    if (_shippingMethod == null)
                    {
                        _shippingMethod = _shippingMethods.FirstOrDefault();
                    }
                    insertOrUpdateOrderShippingZoneMethod(Int32.Parse(_shippingMethod["id"].ToString()), _shippingMethod["name"].ToString(), decimal.Parse(_shippingMethod["priceWithoutTax"].ToString()), bool.Parse(_shippingMethod["taxable"].ToString()), reserveGuid);
                }
            }
            
            List<Dictionary<string, object>> products = ConvertOrderAndProductsToJson(_orderAndProducts);
            _productsTotal = decimal.Round(_productsTotal, DigitsAfterDecimal, MidpointRounding.AwayFromZero);

            return Ok(new Dictionary<string, object>()
            {
                { "products", products },
                //{ "currency", new Setting(_context).GetSettingValueByKey("currency", "website", _config.Value.WebsiteId) },
                { "guid", reserveGuid },
                { "subtotal", Commerce.GetPriceFormat(_productsTotal.ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) },
                { "shippingCosts", Commerce.GetPriceFormat(shippingCosts.ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) },
                { "shippingMethods", _shippingMethods },
                { "tax",  Commerce.GetPriceFormat(decimal.Round(GetTotalTax(), DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) },
                { "total", Commerce.GetPriceFormat(decimal.Round(_productsTotal + shippingCosts, DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) },
                { "quantity", _quantity }
            });
        }

        public decimal GetTotalTax()
        {
            decimal tax = 0.00m;
            foreach(KeyValuePair<string, object> taxClass in TaxClasses)
            {
                tax = tax + decimal.Round(decimal.Parse(taxClass.Value.ToString()), DigitsAfterDecimal, MidpointRounding.AwayFromZero);
            }

            return tax;
        }

        public List<Dictionary<string, object>> ConvertOrderAndProductsToJson(List<OrderAndProduct> orderAndProducts)
        {
            IEnumerable<TaxClasses> _taxClasses = new Tax(_context).GetTaxClassesByWebsiteId(_config.Value.WebsiteId);

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (OrderAndProduct orderAndProduct in orderAndProducts)
            {
                OrderLines _orderLine = orderAndProduct.OrderLine;
                Products _product = orderAndProduct.Product;
                _quantity = _quantity + orderAndProduct.OrderLine.Quantity;
                bool promo = Commerce.IsPromoEnabled(_product.PromoSchedule ?? false, _product.PromoFromDate, _product.PromoToDate, _product.PromoPrice, _product.Price);
                decimal price = decimal.Round((promo ? _product.PromoPrice : _product.Price), DigitsAfterDecimal, MidpointRounding.AwayFromZero);
                decimal total = price * _orderLine.Quantity;

                var productTax = ((price / (100 + orderAndProduct.TaxRate.Rate)) * orderAndProduct.TaxRate.Rate);
                //Tax = Tax + (productTax * _orderLine.Quantity);

                if (TaxClasses.ContainsKey(orderAndProduct.TaxRate.Rate.ToString()))
                {
                    TaxClasses[orderAndProduct.TaxRate.Rate.ToString()] = decimal.Parse(TaxClasses[orderAndProduct.TaxRate.Rate.ToString()].ToString()) + (productTax * _orderLine.Quantity);
                }
                else
                {
                    TaxClasses[orderAndProduct.TaxRate.Rate.ToString()] = (productTax * _orderLine.Quantity);
                }


                _productsTotal = _productsTotal + total;
                Product product = new Product(_context, _config);

                string name = new Product(_context, _config).GetProductResourceTextByCallName(new ProductBundle()
                {
                    ProductFields = orderAndProduct.ProductFields,
                    ProductResources = orderAndProduct.ProductResources
                }, "title");

                Dictionary<string, object> resources = product.ConvertProductFieldsToJson(new ProductBundle()
                {
                    ProductResources = orderAndProduct.ProductResources,
                    ProductFields = orderAndProduct.ProductFields
                });

                Dictionary<string, object> files = product.ConvertProductUploadsToJson(new ProductBundle()
                {
                    ProductFiles = orderAndProduct.ProductFiles,
                    ProductUploads = orderAndProduct.ProductUploads
                }, new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId));

                list.Add(new Dictionary<string, object>()
                {
                    { "id", _product.Id },
                    { "name", name },
                    { "files", files },
                    { "resources", resources },
                    { "price", Commerce.GetPriceFormat(decimal.Round(promo ? _product.PromoPrice : _product.Price, DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) },
                    { "quantity", _orderLine.Quantity },
                    { "manageStock", _product.ManageStock },
                    { "maxPerOrder", _product.MaxPerOrder },
                    { "stockStatus", _product.StockStatus.ToLower() },
                    { "backorders", _product.Backorders.ToLower() },
                    { "stockQuantity", product.CheckProductQuantity(_product.Id, _product.StockQuantity) },
                    { "total", Commerce.GetPriceFormat(decimal.Round(total, DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) }
                });
            }

            return list;
        }

        public decimal GetProductsTotal(IEnumerable<OrderLines> orderLines)
        {
            decimal productsTotal = 0;

            foreach (OrderLines orderLine in orderLines)
            {
                decimal productTax = ((orderLine.Price / (100 + orderLine.TaxRate)) * orderLine.TaxRate);
                decimal totalProductTax = (productTax * orderLine.Quantity);
                //Tax = Tax + totalProductTax;
                if (TaxClasses.ContainsKey(orderLine.TaxRate.ToString()))
                {
                    TaxClasses[orderLine.TaxRate.ToString()] = decimal.Parse(TaxClasses[orderLine.TaxRate.ToString()].ToString()) + totalProductTax;
                }
                else
                {
                    TaxClasses[orderLine.TaxRate.ToString()] = totalProductTax;
                }

                decimal total = (decimal.Round(orderLine.Price, DigitsAfterDecimal, MidpointRounding.AwayFromZero) * orderLine.Quantity);

                productsTotal = (productsTotal + decimal.Round(total, DigitsAfterDecimal, MidpointRounding.AwayFromZero));
            }

            return productsTotal;
        }

        public decimal GetTotalShippingCosts(List<OrderAndProduct> orderAndProducts, string reserveGuid)
        {
            if (orderAndProducts.Count > 0)
            {
                Dictionary<decimal, decimal> percentages = Commerce.CalculatePercentageOfTaxWithPrice(_prices, _total);
                decimal totalShippingPrice = 0.00M;

                //Get shipping zonde methods and linked classes
                List<ShippingZoneMethodAndClasses> _shippingZoneMethodAndClasses = new Shipping(_context, _config).GetShippingZoneMethodAndClassesByDefaultAndWebsiteId();

                _shippingMethods = new List<Dictionary<string, object>>();

                //Check if shipping is not free
                if (!IsShippingFree(_shippingZoneMethodAndClasses, _total))
                {
                    //Check is there is a previous selected shipping method
                    OrderShippingZoneMethods _orderShippingZondeMethod = orderAndProducts.FirstOrDefault().OrderShippingZoneMethod;

                    _shippingZoneMethodId = (_orderShippingZondeMethod != null) ? _orderShippingZondeMethod.ShippingZoneMethodId : 0;

                    ShippingZoneMethodAndClasses _shippingZoneMethodAndClass = null;
                    if (_shippingZoneMethodId != 0)
                    {
                        _shippingZoneMethodAndClass = _shippingZoneMethodAndClasses.OrderBy(ShippingZoneMethodAndClass => ShippingZoneMethodAndClass.ShippingZoneMethod.CustomOrder)
                                                                                   .FirstOrDefault(ShippingZoneMethodAndClass => ShippingZoneMethodAndClass.ShippingZoneMethod.Id == _shippingZoneMethodId);
                    }

                    //Get first fiat rate shipping method if shippingZoneMethodId is 0 or there is no if none is found
                    if (_shippingZoneMethodAndClass == null)
                    {
                        _shippingZoneMethodAndClass = _shippingZoneMethodAndClasses.OrderBy(ShippingZoneMethodAndClass => ShippingZoneMethodAndClass.ShippingZoneMethod.CustomOrder)
                                                                                   .FirstOrDefault(ShippingZoneMethodAndClass => ShippingZoneMethodAndClass.ShippingZoneMethod.Type.ToLower() == "fiatrate");
                    }

                    //Continue if there is a fiat rate available
                    if (_shippingZoneMethodAndClass != null)
                    {
                        _shippingZoneMethodId = _shippingZoneMethodAndClass.ShippingZoneMethod.Id;

                        decimal cost = 0;
                        switch (_shippingZoneMethodAndClass.ShippingZoneMethod.CalculationType.ToLower())
                        {
                            case "perorder":
                                cost = CalculateShippingPricePerOrder(_shippingZoneMethodAndClass);
                                break;
                            default: //"perclass"
                                cost = CalculateShippingPricePerClass(_shippingZoneMethodAndClass, orderAndProducts);
                                break;
                        }

                        //Is shipping method taxable or not?
                        totalShippingPrice = (_shippingZoneMethodAndClass.ShippingZoneMethod.Taxable) ? CalculateShippingWithTax(percentages, cost) : cost;
                        ///Tax = Tax + ShippingTax;
                    }

                    CalculateShippingPrices(percentages, _shippingZoneMethodAndClasses, orderAndProducts);
                } 
                else
                {
                    ShippingZoneMethodAndClasses _shippingZoneMethodAndClass = _shippingZoneMethodAndClasses.FirstOrDefault(x => x.ShippingZoneMethod.Type.ToLower() == "freeshipping");
                    _shippingMethods.Add(new Dictionary<string, object>() {
                        { "id", _shippingZoneMethodAndClass.ShippingZoneMethod.Id },
                        { "name", _shippingZoneMethodAndClass.ShippingZoneMethod.Name },
                        { "taxable", (_shippingZoneMethodAndClass.ShippingZoneMethod.Type.ToLower() == "freeshipping") ? false : _shippingZoneMethodAndClass.ShippingZoneMethod.Taxable },
                        { "cost", Commerce.GetPriceFormat(decimal.Round(0, DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) },
                        { "price", decimal.Round(0.00M, DigitsAfterDecimal, MidpointRounding.AwayFromZero) },
                        { "priceWithoutTax", decimal.Round(0.00M, DigitsAfterDecimal, MidpointRounding.AwayFromZero) },
                        { "selected", true }
                    });
                }

                return decimal.Round(totalShippingPrice, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
            }

            return decimal.Round(0.00M, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
        }

        public decimal GetTotalShippingCosts(IEnumerable<OrderLines> orderLines, IEnumerable<OrderShippingZoneMethods> orderShippingZoneMethods)
        {
            if (orderLines.Count() > 0)
            {
                Dictionary<decimal, decimal> percentages = Commerce.CalculatePercentageOfTaxWithPrice(_prices, _total);

                decimal total = 0;
                decimal notTaxable = 0;
                foreach(OrderShippingZoneMethods orderShippingZoneMethod in orderShippingZoneMethods)
                {
                    if (orderShippingZoneMethod.Taxable ?? true) { total = total + orderShippingZoneMethod.Price; } else { notTaxable = orderShippingZoneMethod.Price; };
                }

                decimal totalShippingPrice = 0;
                foreach(KeyValuePair<decimal, decimal> percentage in percentages)
                {
                    decimal price = (total / 100) * percentage.Value;
                    //Tax = Tax + ((price / (100 + percentage.Key)) * percentage.Key);
                    if (TaxClasses.ContainsKey(percentage.Value.ToString()))
                    {
                        TaxClasses[percentage.Value.ToString()] = decimal.Parse(TaxClasses[percentage.Value.ToString()].ToString()) + ((price / (100 + percentage.Key)) * percentage.Key);
                    }
                    else
                    {
                        TaxClasses[percentage.Value.ToString()] = ((price / (100 + percentage.Key)) * percentage.Key);
                    }
                    totalShippingPrice = totalShippingPrice + price;
                }

                return decimal.Round(totalShippingPrice + notTaxable, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
            }

            return decimal.Round(0.00M, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
        }


        public void DistributePricesToTaxes(List<OrderAndProduct> orderAndProducts)
        {
            _prices = new Dictionary<decimal, decimal>();
            _shippingClassesList = new List<string>();
            foreach (OrderAndProduct orderAndProduct in orderAndProducts)
            {
                _shippingClassesList.Add(orderAndProduct.Product.ShippingClassId.ToString());

                if (orderAndProduct.TaxRate.Shipping)
                {
                    bool promo = Commerce.IsPromoEnabled(orderAndProduct.Product.PromoSchedule ?? false, orderAndProduct.Product.PromoFromDate, orderAndProduct.Product.PromoToDate, orderAndProduct.Product.PromoPrice, orderAndProduct.Product.Price);

                    decimal price = (promo ? orderAndProduct.Product.PromoPrice : orderAndProduct.Product.Price);
                    price = decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero) * orderAndProduct.OrderLine.Quantity;
                    _total = _total + decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero);

                    if (_prices.ContainsKey(orderAndProduct.TaxRate.Rate))
                    {
                        _prices[orderAndProduct.TaxRate.Rate] = _prices.FirstOrDefault(x => x.Key == orderAndProduct.TaxRate.Rate).Value + decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        _prices.Add(orderAndProduct.TaxRate.Rate, decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero));
                    }
                }
            }
        }

        public void DistributePricesToTaxes(IEnumerable<OrderLines> orderLines)
        {
            _prices = new Dictionary<decimal, decimal>();
            foreach (OrderLines orderLine in orderLines)
            {
                if (orderLine.TaxShipping ?? true)
                {
                    decimal price = decimal.Round(orderLine.Price, DigitsAfterDecimal, MidpointRounding.AwayFromZero) * orderLine.Quantity;
                    _total = _total + decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
                    if (_prices.ContainsKey(orderLine.TaxRate))
                    {
                        _prices[orderLine.TaxRate] = _prices.FirstOrDefault(x => x.Key == orderLine.TaxRate).Value + decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        _prices.Add(orderLine.TaxRate, decimal.Round(price, DigitsAfterDecimal, MidpointRounding.AwayFromZero));
                    }
                }
            }
        }

        public void CalculateShippingPrices(Dictionary<decimal, decimal> percentages, List<ShippingZoneMethodAndClasses> shippingZoneMethodAndClasses, List<OrderAndProduct> orderAndProducts)
        {
            foreach(ShippingZoneMethodAndClasses shippingZoneMethodAndClass in shippingZoneMethodAndClasses.Where(x => x.ShippingZoneMethod.Type.ToLower() == "fiatrate" || x.ShippingZoneMethod.Type.ToLower() == "localpickup"))
            {
                decimal cost = 0;
                switch (shippingZoneMethodAndClass.ShippingZoneMethod.CalculationType.ToLower())
                {
                    case "perorder":
                        cost = CalculateShippingPricePerOrder(shippingZoneMethodAndClass);
                        break;
                    default: //"perclass"
                        cost = CalculateShippingPricePerClass(shippingZoneMethodAndClass, orderAndProducts);
                        break;
                }

                decimal priceWithTax = (shippingZoneMethodAndClass.ShippingZoneMethod.Taxable) ? CalculateShippingWithTax(percentages, cost) : cost;

                _shippingMethods.Add(new Dictionary<string, object>() {
                    { "id", shippingZoneMethodAndClass.ShippingZoneMethod.Id },
                    { "name", shippingZoneMethodAndClass.ShippingZoneMethod.Name },
                    { "taxable", (shippingZoneMethodAndClass.ShippingZoneMethod.Type.ToLower() == "freeshipping") ? false : shippingZoneMethodAndClass.ShippingZoneMethod.Taxable },
                    { "cost", Commerce.GetPriceFormat(decimal.Round(priceWithTax, DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), Currency) },
                    { "price", decimal.Round(priceWithTax, DigitsAfterDecimal, MidpointRounding.AwayFromZero) },
                    { "priceWithoutTax", decimal.Round(cost, DigitsAfterDecimal, MidpointRounding.AwayFromZero) },
                    { "selected", (shippingZoneMethodAndClass.ShippingZoneMethod.Id == _shippingZoneMethodId) ? true : false }
                });
            }
        }

        public decimal CalculateShippingWithTax(Dictionary<decimal, decimal> percentages, decimal taxable)
        {
            //decimal totalShippingPrice = 0;
            foreach (decimal key in percentages.Keys)
            {
                decimal price = (taxable / 100) * percentages[key];
                //ShippingTax = ShippingTax + ((price / (100 + key)) * key);

                if (TaxClasses.ContainsKey(percentages[key].ToString()))
                {
                    TaxClasses[percentages[key].ToString()] = decimal.Parse(TaxClasses[percentages[key].ToString()].ToString()) + ((price / (100 + key)) * key);
                }
                else
                {
                    TaxClasses[percentages[key].ToString()] = ((price / (100 + key)) * key);
                }

                //totalShippingPrice = totalShippingPrice + (((price / 100) * key) + price);
            }

            return taxable; //totalShippingPrice
        }

        public decimal CalculateShippingPricePerOrder(ShippingZoneMethodAndClasses shippingZoneMethodAndClass)
        {
            //Pick the most expensive shipping class
            ShippingZoneMethodClasses _shippingZoneMethodClass = shippingZoneMethodAndClass.ShippingZoneMethodClasses.OrderByDescending(ShippingZoneMethodClass => decimal.Parse(ShippingZoneMethodClass.Cost, CultureInfo.InvariantCulture))
                                                                                                                     .FirstOrDefault(ShippingZoneMethodClass => _shippingClassesList.Contains(ShippingZoneMethodClass.ShippingClassId.ToString()));
            decimal number;
            if (_shippingZoneMethodClass != null && _shippingZoneMethodClass.Cost != "")
            {
                Decimal.TryParse(_shippingZoneMethodClass.Cost, out number);
                return decimal.Round(number, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
            }
            else
            {
                //Use the default cost value if there is no shipping class available or if cost is empty
                Decimal.TryParse(shippingZoneMethodAndClass.ShippingZoneMethod.Cost, out number);
                return decimal.Round(number, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
            }
        }

        public decimal CalculateShippingPricePerClass(ShippingZoneMethodAndClasses shippingZoneMethodAndClass, List<OrderAndProduct> orderAndProducts)
        {
            decimal cost = 0;
            foreach (ShippingZoneMethodClasses shippingZoneMethodClass in shippingZoneMethodAndClass.ShippingZoneMethodClasses)
            {
                //Is there a product in this order that has this shipping class
                if (orderAndProducts.FirstOrDefault(orderLineAndProduct => orderLineAndProduct.Product.ShippingClassId == shippingZoneMethodClass.ShippingClassId) != null)
                {
                    decimal number;
                    if (shippingZoneMethodClass.Cost != "")
                    {
                        Decimal.TryParse(shippingZoneMethodClass.Cost, out number);
                        cost = cost + decimal.Round(number, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        Decimal.TryParse(shippingZoneMethodAndClass.ShippingZoneMethod.Cost, out number);
                        cost = cost + decimal.Round(number, DigitsAfterDecimal, MidpointRounding.AwayFromZero);
                    }
                }
            }

            return cost;
        }

        public bool IsShippingFree(List<ShippingZoneMethodAndClasses> shippingZoneMethodAndClasses, decimal total)
        {
            ShippingZoneMethodAndClasses _shippingZoneMethodAndClass = shippingZoneMethodAndClasses.FirstOrDefault(x => x.ShippingZoneMethod.Type.ToLower() == "freeshipping");
            if (_shippingZoneMethodAndClass != null)
            {
                switch (_shippingZoneMethodAndClass.ShippingZoneMethod.FreeShippingType.ToLower()) {
                    case "minimumamount":
                        //If total price is higher then the minimum amount it will return true
                        return total >= _shippingZoneMethodAndClass.ShippingZoneMethod.MinimumAmount ? true : false;
                    default: // n/a ( Not applicable)
                        //If value contains N/A then the shipping is always free
                        return true;
                }
            }

            return false;
        }
    }
}