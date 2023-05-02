using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Site.Models;

namespace Site.Data
{
    public partial class SiteContext : DbContext
    {
        public SiteContext()
        {
        }

        public SiteContext(DbContextOptions<SiteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiKeys> ApiKeys { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<CompanyUsers> CompanyUsers { get; set; }
        public virtual DbSet<Cultures> Cultures { get; set; }
        public virtual DbSet<DataItemFiles> DataItemFiles { get; set; }
        public virtual DbSet<DataItemResources> DataItemResources { get; set; }
        public virtual DbSet<DataItems> DataItems { get; set; }
        public virtual DbSet<DataTemplateFields> DataTemplateFields { get; set; }
        public virtual DbSet<DataTemplates> DataTemplates { get; set; }
        public virtual DbSet<DataTemplateSections> DataTemplateSections { get; set; }
        public virtual DbSet<DataTemplateUploads> DataTemplateUploads { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<LanguageTranslate> LanguageTranslate { get; set; }
        public virtual DbSet<NavigationItems> NavigationItems { get; set; }
        public virtual DbSet<Navigations> Navigations { get; set; }
        public virtual DbSet<OauthTokens> OauthTokens { get; set; }
        public virtual DbSet<OrderCoupons> OrderCoupons { get; set; }
        public virtual DbSet<OrderFees> OrderFees { get; set; }
        public virtual DbSet<OrderLines> OrderLines { get; set; }
        public virtual DbSet<OrderRefundLines> OrderRefundLines { get; set; }
        public virtual DbSet<OrderRefunds> OrderRefunds { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderShippingZoneMethods> OrderShippingZoneMethods { get; set; }
        public virtual DbSet<PageFiles> PageFiles { get; set; }
        public virtual DbSet<PageResources> PageResources { get; set; }
        public virtual DbSet<Pages> Pages { get; set; }
        public virtual DbSet<PageTemplateFields> PageTemplateFields { get; set; }
        public virtual DbSet<PageTemplates> PageTemplates { get; set; }
        public virtual DbSet<PageTemplateSections> PageTemplateSections { get; set; }
        public virtual DbSet<PageTemplateUploads> PageTemplateUploads { get; set; }
        public virtual DbSet<ProductFields> ProductFields { get; set; }
        public virtual DbSet<ProductFiles> ProductFiles { get; set; }
        public virtual DbSet<ProductPages> ProductPages { get; set; }
        public virtual DbSet<ProductPageSettings> ProductPageSettings { get; set; }
        public virtual DbSet<ProductResources> ProductResources { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<ProductTemplates> ProductTemplates { get; set; }
        public virtual DbSet<ProductUploads> ProductUploads { get; set; }
        public virtual DbSet<ReviewResources> ReviewResources { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<ReviewTemplateFields> ReviewTemplateFields { get; set; }
        public virtual DbSet<ReviewTemplates> ReviewTemplates { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<ShippingClasses> ShippingClasses { get; set; }
        public virtual DbSet<ShippingZoneLocations> ShippingZoneLocations { get; set; }
        public virtual DbSet<ShippingZoneMethodClasses> ShippingZoneMethodClasses { get; set; }
        public virtual DbSet<ShippingZoneMethods> ShippingZoneMethods { get; set; }
        public virtual DbSet<ShippingZones> ShippingZones { get; set; }
        public virtual DbSet<TaxClasses> TaxClasses { get; set; }
        public virtual DbSet<TaxRateLocations> TaxRateLocations { get; set; }
        public virtual DbSet<TaxRates> TaxRates { get; set; }
        public virtual DbSet<WebsiteFields> WebsiteFields { get; set; }
        public virtual DbSet<WebsiteFiles> WebsiteFiles { get; set; }
        public virtual DbSet<WebsiteLanguages> WebsiteLanguages { get; set; }
        public virtual DbSet<WebsiteResources> WebsiteResources { get; set; }
        public virtual DbSet<Websites> Websites { get; set; }
        public virtual DbSet<WebsiteUploads> WebsiteUploads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Startup.Configuration.GetConnectionString("DatabaseCon"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApiKeys>(entity =>
            {
                entity.Property(e => e.ClientId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ClientSecret)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LastAccess).HasColumnType("datetime");

                entity.Property(e => e.Permissions)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TruncatedKey)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.ApiKeys)
                    .HasForeignKey(d => d.WebsiteId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ApiKeys_Websites");
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

            modelBuilder.Entity<Companies>(entity =>
            {
                entity.Property(e => e.Coc)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Company)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Vat)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<CompanyUsers>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyUsers)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_CompanyUsers_Companies");
            });

            modelBuilder.Entity<Cultures>(entity =>
            {
                entity.Property(e => e.CultureCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CultureName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LanguageCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DataItemFiles>(entity =>
            {
                entity.Property(e => e.Alt)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CompressedPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.DataItem)
                    .WithMany(p => p.DataItemFiles)
                    .HasForeignKey(d => d.DataItemId)
                    .HasConstraintName("FK_DataItemFiles_DataItems");
            });

            modelBuilder.Entity<DataItemResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.DataItem)
                    .WithMany(p => p.DataItemResources)
                    .HasForeignKey(d => d.DataItemId)
                    .HasConstraintName("FK_DataItemResources_DataItems");
            });

            modelBuilder.Entity<DataItems>(entity =>
            {
                entity.Property(e => e.AlternateGuid).HasMaxLength(450);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.HtmlEditor).IsRequired();

                entity.Property(e => e.PageDescription)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.PageKeywords)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.PageTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PageUrl)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PublishDate).HasColumnType("datetime");

                entity.Property(e => e.Subtitle).IsRequired();

                entity.Property(e => e.Text).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.HasOne(d => d.DataTemplate)
                    .WithMany(p => p.DataItems)
                    .HasForeignKey(d => d.DataTemplateId)
                    .HasConstraintName("FK_DataItems_DataTemplates");
            });

            modelBuilder.Entity<DataTemplateFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.DataTemplate)
                    .WithMany(p => p.DataTemplateFields)
                    .HasForeignKey(d => d.DataTemplateId)
                    .HasConstraintName("FK_DataTemplateFields_DataTemplates");
            });

            modelBuilder.Entity<DataTemplates>(entity =>
            {
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.FromDateHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.HtmlEditorHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Icon)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.MenuType)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.PageAlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.PublishDateHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.SubtitleHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TextHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TitleHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ToDateHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.DataTemplates)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_DataTemplates_Websites");
            });

            modelBuilder.Entity<DataTemplateSections>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.DataTemplate)
                    .WithMany(p => p.DataTemplateSections)
                    .HasForeignKey(d => d.DataTemplateId)
                    .HasConstraintName("FK_DataTemplateSections_DataTemplates");
            });

            modelBuilder.Entity<DataTemplateUploads>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FileExtensions)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MimeTypes)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.HasOne(d => d.DataTemplate)
                    .WithMany(p => p.DataTemplateUploads)
                    .HasForeignKey(d => d.DataTemplateId)
                    .HasConstraintName("FK_DataTemplateUploads_DataTemplates");
            });

            modelBuilder.Entity<Languages>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Culture)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Language)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.TimeZoneId).HasMaxLength(150);
            });

            modelBuilder.Entity<LanguageTranslate>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Translate)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<NavigationItems>(entity =>
            {
                entity.Property(e => e.CustomUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FilterAlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.LinkedToAlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.LinkedToType)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Target)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.Navigation)
                    .WithMany(p => p.NavigationItems)
                    .HasForeignKey(d => d.NavigationId)
                    .HasConstraintName("FK_NavigationItems_Navigations");
            });

            modelBuilder.Entity<Navigations>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.Navigations)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_Navigations_Websites");
            });

            modelBuilder.Entity<OauthTokens>(entity =>
            {
                entity.ToTable("OAuthTokens");

                entity.Property(e => e.AccessToken).IsRequired();

                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshToken).IsRequired();

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.OauthTokens)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_OAuthTokens_Websites");
            });

            modelBuilder.Entity<OrderCoupons>(entity =>
            {
                entity.Property(e => e.Discount).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderCoupons)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderCoupons_Orders");
            });

            modelBuilder.Entity<OrderFees>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.TaxRate).HasColumnType("decimal(20, 4)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderFees)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderFees_Orders");
            });

            modelBuilder.Entity<OrderLines>(entity =>
            {
                entity.Property(e => e.Discount).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.TaxRate).HasColumnType("decimal(20, 4)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderLines)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderLines_Orders");
            });

            modelBuilder.Entity<OrderRefundLines>(entity =>
            {
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(20, 4)");

                entity.HasOne(d => d.OrderRefund)
                    .WithMany(p => p.OrderRefundLines)
                    .HasForeignKey(d => d.OrderRefundId)
                    .HasConstraintName("FK_OrderRefundLines_OrderRefunds");
            });

            modelBuilder.Entity<OrderRefunds>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceNumber).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Refund).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderRefunds)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderRefunds_Orders");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.Property(e => e.BillingAddressLine1).IsRequired();

                entity.Property(e => e.BillingAddressLine2).IsRequired();

                entity.Property(e => e.BillingCity).IsRequired();

                entity.Property(e => e.BillingCompany).IsRequired();

                entity.Property(e => e.BillingCountry).IsRequired();

                entity.Property(e => e.BillingFirstName).IsRequired();

                entity.Property(e => e.BillingLastName).IsRequired();

                entity.Property(e => e.BillingState).IsRequired();

                entity.Property(e => e.BillingVatNumber).IsRequired();

                entity.Property(e => e.BillingZipCode).IsRequired();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.InvoiceNumber).HasMaxLength(100);

                entity.Property(e => e.RefInvoiceNumber).HasMaxLength(100);

                entity.Property(e => e.Note).IsRequired();

                entity.Property(e => e.OrderNumber).HasMaxLength(100);

                entity.Property(e => e.ReserveGuid).HasMaxLength(450);

                entity.Property(e => e.ShippingAddressLine1).IsRequired();

                entity.Property(e => e.ShippingAddressLine2).IsRequired();

                entity.Property(e => e.ShippingCity).IsRequired();

                entity.Property(e => e.ShippingCompany).IsRequired();

                entity.Property(e => e.ShippingCountry).IsRequired();

                entity.Property(e => e.ShippingFirstName).IsRequired();

                entity.Property(e => e.ShippingLastName).IsRequired();

                entity.Property(e => e.ShippingState).IsRequired();

                entity.Property(e => e.ShippingZipCode).IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.TransactionId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_Orders_Websites");
            });

            modelBuilder.Entity<OrderShippingZoneMethods>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(20, 4)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderShippingZoneMethods)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderShippingZoneMethods_Orders");
            });

            modelBuilder.Entity<PageFiles>(entity =>
            {
                entity.Property(e => e.Alt)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CompressedPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.PageFiles)
                    .HasForeignKey(d => d.PageId)
                    .HasConstraintName("FK_PageFiles_Pages");
            });

            modelBuilder.Entity<PageResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.PageResources)
                    .HasForeignKey(d => d.PageId)
                    .HasConstraintName("FK_PageResources_Pages");
            });

            modelBuilder.Entity<Pages>(entity =>
            {
                entity.Property(e => e.AlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Keywords)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.PageTemplate)
                    .WithMany(p => p.Pages)
                    .HasForeignKey(d => d.PageTemplateId)
                    .HasConstraintName("FK_Pages_PageTemplates");

                entity.HasOne(d => d.WebsiteLanguage)
                    .WithMany(p => p.Pages)
                    .HasForeignKey(d => d.WebsiteLanguageId)
                    .HasConstraintName("FK_Pages_WebsiteLanguages");
            });

            modelBuilder.Entity<PageTemplateFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.PageTemplate)
                    .WithMany(p => p.PageTemplateFields)
                    .HasForeignKey(d => d.PageTemplateId)
                    .HasConstraintName("FK_PageTemplateFields_PageTemplates");
            });

            modelBuilder.Entity<PageTemplates>(entity =>
            {
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(35);

                entity.Property(e => e.Type)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.PageTemplates)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_PageTemplates_Websites");
            });

            modelBuilder.Entity<PageTemplateSections>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.PageTemplate)
                    .WithMany(p => p.PageTemplateSections)
                    .HasForeignKey(d => d.PageTemplateId)
                    .HasConstraintName("FK_PageTemplateSections_PageTemplates");
            });

            modelBuilder.Entity<PageTemplateUploads>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FileExtensions)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MimeTypes)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.HasOne(d => d.PageTemplate)
                    .WithMany(p => p.PageTemplateUploads)
                    .HasForeignKey(d => d.PageTemplateId)
                    .HasConstraintName("FK_PageTemplateUploads_PageTemplates");
            });

            modelBuilder.Entity<ProductFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DefaultValue).IsRequired();

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProductTemplate)
                    .WithMany(p => p.ProductFields)
                    .HasForeignKey(d => d.ProductTemplateId)
                    .HasConstraintName("FK_ProductFields_ProductTemplates");
            });

            modelBuilder.Entity<ProductFiles>(entity =>
            {
                entity.Property(e => e.Alt)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CompressedPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductFiles)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductFiles_Products");
            });

            modelBuilder.Entity<ProductPages>(entity =>
            {
                entity.Property(e => e.PageAlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductPages_ProductPages");
            });

            modelBuilder.Entity<ProductPageSettings>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Keywords)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPageSettings)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductPageSettings_Products");
            });

            modelBuilder.Entity<ProductResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductResources)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductResources_Products");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e.Backorders)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Height).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Length).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.PromoFromDate).HasColumnType("datetime");

                entity.Property(e => e.PromoPrice).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.PromoToDate).HasColumnType("datetime");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("SKU")
                    .HasMaxLength(200);

                entity.Property(e => e.StockStatus)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.TaxStatus)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Weight).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Width).HasColumnType("decimal(20, 4)");

                entity.HasOne(d => d.ProductTemplate)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductTemplateId)
                    .HasConstraintName("FK_Products_ProductTemplates");
            });

            modelBuilder.Entity<ProductTemplates>(entity =>
            {
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(35);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.ProductTemplates)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_ProductTemplates_Websites");
            });

            modelBuilder.Entity<ProductUploads>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FileExtensions)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MimeTypes)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProductTemplate)
                    .WithMany(p => p.ProductUploads)
                    .HasForeignKey(d => d.ProductTemplateId)
                    .HasConstraintName("FK_ProductUploads_ProductTemplates");
            });

            modelBuilder.Entity<ReviewResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewResources)
                    .HasForeignKey(d => d.ReviewId)
                    .HasConstraintName("FK_ReviewResources_Reviews");
            });

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Text).IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.ReviewTemplate)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ReviewTemplateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Reviews_ReviewTemplates");

                entity.HasOne(d => d.WebsiteLanguage)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.WebsiteLanguageId)
                    .HasConstraintName("FK_Reviews_WebsiteLanguages");
            });

            modelBuilder.Entity<ReviewTemplateFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.ReviewTemplate)
                    .WithMany(p => p.ReviewTemplateFields)
                    .HasForeignKey(d => d.ReviewTemplateId)
                    .HasConstraintName("FK_ReviewTemplateFields_ReviewTemplates");
            });

            modelBuilder.Entity<ReviewTemplates>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.LinkedToType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.ReviewTemplates)
                    .HasForeignKey(d => d.WebsiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReviewTemplates_Websites");
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.LinkedToType)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShippingClasses>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.ShippingClasses)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_ShippingClasses_Websites");
            });

            modelBuilder.Entity<ShippingZoneLocations>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.ShippingZone)
                    .WithMany(p => p.ShippingZoneLocations)
                    .HasForeignKey(d => d.ShippingZoneId)
                    .HasConstraintName("FK_ShippingZoneLocations_ShippingZones");
            });

            modelBuilder.Entity<ShippingZoneMethodClasses>(entity =>
            {
                entity.Property(e => e.Cost)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.ShippingZoneMethod)
                    .WithMany(p => p.ShippingZoneMethodClasses)
                    .HasForeignKey(d => d.ShippingZoneMethodId)
                    .HasConstraintName("FK_ShippingZoneMethodClasses_ShippingZoneMethods");
            });

            modelBuilder.Entity<ShippingZoneMethods>(entity =>
            {
                entity.Property(e => e.CalculationType)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Cost)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FreeShippingType)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.MinimumAmount).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.ShippingZone)
                    .WithMany(p => p.ShippingZoneMethods)
                    .HasForeignKey(d => d.ShippingZoneId)
                    .HasConstraintName("FK_ShippingZoneMethods_ShippingZones");
            });

            modelBuilder.Entity<ShippingZones>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Slug).HasMaxLength(200);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.ShippingZones)
                    .HasForeignKey(d => d.WebsiteId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ShippingZones_Websites");
            });

            modelBuilder.Entity<TaxClasses>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.TaxClasses)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_TaxClasses_Websites");
            });

            modelBuilder.Entity<TaxRateLocations>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.TaxRate)
                    .WithMany(p => p.TaxRateLocations)
                    .HasForeignKey(d => d.TaxRateId)
                    .HasConstraintName("FK_TaxRateLocations_TaxRates");
            });

            modelBuilder.Entity<TaxRates>(entity =>
            {
                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Rate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.TaxClass)
                    .WithMany(p => p.TaxRates)
                    .HasForeignKey(d => d.TaxClassId)
                    .HasConstraintName("FK_TaxRates_TaxClasses");
            });

            modelBuilder.Entity<WebsiteFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DefaultValue).IsRequired();

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.WebsiteFields)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_WebsiteFields_Websites");
            });

            modelBuilder.Entity<WebsiteFiles>(entity =>
            {
                entity.Property(e => e.Alt)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CompressedPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.WebsiteLanguage)
                    .WithMany(p => p.WebsiteFiles)
                    .HasForeignKey(d => d.WebsiteLanguageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WebsiteFiles_WebsiteLanguages");
            });

            modelBuilder.Entity<WebsiteLanguages>(entity =>
            {
                entity.HasOne(d => d.Website)
                    .WithMany(p => p.WebsiteLanguages)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_WebsiteLanguages_Websites");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.WebsiteLanguages)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_WebsiteLanguages_Language");
            });

            modelBuilder.Entity<WebsiteResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.WebsiteLanguage)
                    .WithMany(p => p.WebsiteResources)
                    .HasForeignKey(d => d.WebsiteLanguageId)
                    .HasConstraintName("FK_WebsiteResources_WebsiteLanguages");
            });

            modelBuilder.Entity<Websites>(entity =>
            {
                entity.Property(e => e.Domain)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Extension)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Folder)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.RootPageAlternateGuid).HasMaxLength(450);

                entity.Property(e => e.Subdomain).HasMaxLength(200);

                entity.Property(e => e.Subtitle).HasMaxLength(60);

                entity.Property(e => e.TypeClient).HasMaxLength(20);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Websites)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Websites_Companies");
            });

            modelBuilder.Entity<WebsiteUploads>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FileExtensions)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MimeTypes)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.WebsiteUploads)
                    .HasForeignKey(d => d.WebsiteId)
                    .HasConstraintName("FK_WebsiteUploads_Websites");
            });
        }
    }
}
