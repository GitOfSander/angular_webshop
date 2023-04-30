using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Site.Migrations
{
    public partial class M130 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Company = table.Column<string>(maxLength: 200, nullable: false),
                    Vat = table.Column<string>(maxLength: 100, nullable: false),
                    Coc = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cultures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LanguageCode = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    CultureName = table.Column<string>(unicode: false, maxLength: 25, nullable: false),
                    DisplayName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    CultureCode = table.Column<string>(unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    Language = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Culture = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    TimeZoneId = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageTranslate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LanguageId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Translate = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageTranslate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LinkedToId = table.Column<int>(nullable: false),
                    LinkedToType = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Key = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyUsers_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Websites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    Domain = table.Column<string>(maxLength: 200, nullable: false),
                    Extension = table.Column<string>(maxLength: 15, nullable: false),
                    Folder = table.Column<string>(maxLength: 200, nullable: false),
                    Subdomain = table.Column<string>(maxLength: 200, nullable: true),
                    TypeClient = table.Column<string>(maxLength: 20, nullable: true),
                    RootPageAlternateGuid = table.Column<string>(maxLength: 450, nullable: true),
                    Subtitle = table.Column<string>(maxLength: 60, nullable: true),
                    Active = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Websites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Websites_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    Permissions = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    LastAccess = table.Column<DateTime>(type: "datetime", nullable: false),
                    WebsiteId = table.Column<int>(nullable: true),
                    ClientId = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    TruncatedKey = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 70, nullable: false),
                    Controller = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Action = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    DetailPage = table.Column<bool>(nullable: false),
                    PageAlternateGuid = table.Column<string>(maxLength: 450, nullable: false),
                    TitleHeading = table.Column<string>(maxLength: 200, nullable: false),
                    SubtitleHeading = table.Column<string>(maxLength: 200, nullable: false),
                    TextHeading = table.Column<string>(maxLength: 200, nullable: false),
                    HtmlEditorHeading = table.Column<string>(maxLength: 200, nullable: false),
                    PublishDateHeading = table.Column<string>(maxLength: 200, nullable: false),
                    FromDateHeading = table.Column<string>(maxLength: 200, nullable: false),
                    ToDateHeading = table.Column<string>(maxLength: 200, nullable: false),
                    Active = table.Column<bool>(nullable: true),
                    MenuType = table.Column<string>(unicode: false, maxLength: 40, nullable: true),
                    Icon = table.Column<string>(unicode: false, maxLength: 40, nullable: true),
                    CustomOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataTemplates_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Navigations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    MaxDepth = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Navigations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Navigations_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OAuthTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
                    AccessToken = table.Column<string>(nullable: false),
                    RefreshToken = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthTokens_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    BillingFirstName = table.Column<string>(nullable: false),
                    BillingLastName = table.Column<string>(nullable: false),
                    BillingCompany = table.Column<string>(nullable: false),
                    BillingZipCode = table.Column<string>(nullable: false),
                    BillingCity = table.Column<string>(nullable: false),
                    BillingCountry = table.Column<string>(nullable: false),
                    BillingState = table.Column<string>(nullable: false),
                    BillingAddressLine1 = table.Column<string>(nullable: false),
                    BillingAddressLine2 = table.Column<string>(nullable: false),
                    BillingEmail = table.Column<string>(nullable: true),
                    BillingPhoneNumber = table.Column<string>(nullable: true),
                    BillingVatNumber = table.Column<string>(nullable: false),
                    ShippingFirstName = table.Column<string>(nullable: false),
                    ShippingLastName = table.Column<string>(nullable: false),
                    ShippingCompany = table.Column<string>(nullable: false),
                    ShippingZipCode = table.Column<string>(nullable: false),
                    ShippingCity = table.Column<string>(nullable: false),
                    ShippingCountry = table.Column<string>(nullable: false),
                    ShippingState = table.Column<string>(nullable: false),
                    ShippingAddressLine1 = table.Column<string>(nullable: false),
                    ShippingAddressLine2 = table.Column<string>(nullable: false),
                    Note = table.Column<string>(nullable: false),
                    TransactionId = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<string>(maxLength: 40, nullable: false),
                    Currency = table.Column<string>(maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    OrderNumber = table.Column<string>(maxLength: 100, nullable: true),
                    InvoiceNumber = table.Column<string>(maxLength: 100, nullable: true),
                    ReserveGuid = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 35, nullable: false),
                    Controller = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Action = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTemplates_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 35, nullable: false),
                    Controller = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Action = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Attributes = table.Column<bool>(nullable: true),
                    Reviews = table.Column<bool>(nullable: true),
                    Virtual = table.Column<bool>(nullable: true),
                    Downloadable = table.Column<bool>(nullable: true),
                    Upsells = table.Column<bool>(nullable: true),
                    CrossSells = table.Column<bool>(nullable: true),
                    SimpleProduct = table.Column<bool>(nullable: true),
                    GroupedProduct = table.Column<bool>(nullable: true),
                    ExternalProduct = table.Column<bool>(nullable: true),
                    VariableProduct = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTemplates_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CheckBeforeOnline = table.Column<bool>(nullable: true),
                    LinkedToType = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewTemplates_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingClasses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Slug = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingClasses_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingZones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Slug = table.Column<string>(maxLength: 200, nullable: true),
                    WebsiteId = table.Column<int>(nullable: true),
                    Default = table.Column<bool>(nullable: true),
                    PriorityOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingZones_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxClasses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxClasses_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: false),
                    DeveloperOnly = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteFields_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    DefaultLanguage = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteLanguages_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteUploads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    MimeTypes = table.Column<string>(unicode: false, maxLength: 120, nullable: false),
                    FileExtensions = table.Column<string>(unicode: false, maxLength: 70, nullable: false),
                    MinFiles = table.Column<byte>(nullable: false),
                    MaxFiles = table.Column<byte>(nullable: false),
                    MaxSize = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    DeveloperOnly = table.Column<bool>(nullable: true),
                    CustomOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteUploads_Websites",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteLanguageId = table.Column<int>(nullable: false),
                    DataTemplateId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Subtitle = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    HtmlEditor = table.Column<string>(nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    PageUrl = table.Column<string>(maxLength: 200, nullable: false),
                    PageTitle = table.Column<string>(maxLength: 200, nullable: false),
                    PageKeywords = table.Column<string>(maxLength: 400, nullable: false),
                    PageDescription = table.Column<string>(maxLength: 400, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    AlternateGuid = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataItems_DataTemplates",
                        column: x => x.DataTemplateId,
                        principalTable: "DataTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataTemplateFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataTemplateId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true),
                    LinkedToDataTemplateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTemplateFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataTemplateFields_DataTemplates",
                        column: x => x.DataTemplateId,
                        principalTable: "DataTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataTemplateSections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataTemplateId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Section = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    LinkedToDataTemplateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTemplateSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataTemplateSections_DataTemplates",
                        column: x => x.DataTemplateId,
                        principalTable: "DataTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataTemplateUploads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataTemplateId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    MimeTypes = table.Column<string>(unicode: false, maxLength: 120, nullable: false),
                    FileExtensions = table.Column<string>(unicode: false, maxLength: 70, nullable: false),
                    MinFiles = table.Column<byte>(nullable: false),
                    MaxFiles = table.Column<byte>(nullable: false),
                    MaxSize = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    CustomOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTemplateUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataTemplateUploads_DataTemplates",
                        column: x => x.DataTemplateId,
                        principalTable: "DataTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NavigationItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteLanguageId = table.Column<int>(nullable: false),
                    NavigationId = table.Column<int>(nullable: false),
                    Parent = table.Column<int>(nullable: false),
                    LinkedToType = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    LinkedToSectionId = table.Column<int>(nullable: false),
                    LinkedToAlternateGuid = table.Column<string>(maxLength: 450, nullable: false),
                    FilterAlternateGuid = table.Column<string>(maxLength: 450, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Target = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    CustomUrl = table.Column<string>(maxLength: 500, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavigationItems_Navigations",
                        column: x => x.NavigationId,
                        principalTable: "Navigations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderCoupons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    CouponId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(20, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCoupons_Orders",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderFees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(20, 4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderFees_Orders",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderLines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    TaxShipping = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLines_Orders",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderRefunds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Refund = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Reason = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvoiceNumber = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRefunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderRefunds_Orders",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderShippingZoneMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShippingZoneMethodId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Taxable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderShippingZoneMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderShippingZoneMethods_Orders",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteLanguageId = table.Column<int>(nullable: false),
                    PageTemplateId = table.Column<int>(nullable: false),
                    Parent = table.Column<int>(nullable: false),
                    Url = table.Column<string>(maxLength: 200, nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Keywords = table.Column<string>(maxLength: 400, nullable: false),
                    Description = table.Column<string>(maxLength: 400, nullable: false),
                    AlternateGuid = table.Column<string>(maxLength: 450, nullable: false),
                    Active = table.Column<bool>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    CustomOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_PageTemplates",
                        column: x => x.PageTemplateId,
                        principalTable: "PageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageTemplateFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PageTemplateId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTemplateFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTemplateFields_PageTemplates",
                        column: x => x.PageTemplateId,
                        principalTable: "PageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageTemplateSections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PageTemplateId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Section = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    LinkedToDataTemplateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTemplateSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTemplateSections_PageTemplates",
                        column: x => x.PageTemplateId,
                        principalTable: "PageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageTemplateUploads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PageTemplateId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    MimeTypes = table.Column<string>(unicode: false, maxLength: 120, nullable: false),
                    FileExtensions = table.Column<string>(unicode: false, maxLength: 70, nullable: false),
                    MinFiles = table.Column<byte>(nullable: false),
                    MaxFiles = table.Column<byte>(nullable: false),
                    MaxSize = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    CustomOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTemplateUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTemplateUploads_PageTemplates",
                        column: x => x.PageTemplateId,
                        principalTable: "PageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductTemplateId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFields_ProductTemplates",
                        column: x => x.ProductTemplateId,
                        principalTable: "ProductTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductTemplateId = table.Column<int>(nullable: false),
                    TaxClassId = table.Column<int>(nullable: false),
                    ShippingClassId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    PromoPrice = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    PromoFromDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PromoToDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TaxStatus = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    SKU = table.Column<string>(maxLength: 200, nullable: false),
                    ManageStock = table.Column<bool>(nullable: false),
                    StockStatus = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    StockQuantity = table.Column<int>(nullable: false),
                    Backorders = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    MaxPerOrder = table.Column<int>(nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    DownloadLimit = table.Column<int>(nullable: false),
                    DownloadExpire = table.Column<int>(nullable: false),
                    Reviews = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: true),
                    PromoSchedule = table.Column<bool>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    CustomOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductTemplates",
                        column: x => x.ProductTemplateId,
                        principalTable: "ProductTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductUploads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductTemplateId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    MimeTypes = table.Column<string>(unicode: false, maxLength: 120, nullable: false),
                    FileExtensions = table.Column<string>(unicode: false, maxLength: 70, nullable: false),
                    MinFiles = table.Column<byte>(nullable: false),
                    MaxFiles = table.Column<byte>(nullable: false),
                    MaxSize = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    CustomOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductUploads_ProductTemplates",
                        column: x => x.ProductTemplateId,
                        principalTable: "ProductTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewTemplateFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReviewTemplateId = table.Column<int>(nullable: false),
                    CallName = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Heading = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTemplateFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewTemplateFields_ReviewTemplates",
                        column: x => x.ReviewTemplateId,
                        principalTable: "ReviewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingZoneLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShippingZoneId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingZoneLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingZoneLocations_ShippingZones",
                        column: x => x.ShippingZoneId,
                        principalTable: "ShippingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingZoneMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShippingZoneId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    Taxable = table.Column<bool>(nullable: false),
                    Cost = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    CalculationType = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    FreeShippingType = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CustomOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingZoneMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingZoneMethods_ShippingZones",
                        column: x => x.ShippingZoneId,
                        principalTable: "ShippingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaxClassId = table.Column<int>(nullable: false),
                    Country = table.Column<string>(unicode: false, maxLength: 2, nullable: false),
                    State = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Compound = table.Column<bool>(nullable: false),
                    Shipping = table.Column<bool>(nullable: false),
                    PriorityOrder = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxRates_TaxClasses",
                        column: x => x.TaxClassId,
                        principalTable: "TaxClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteLanguageId = table.Column<int>(nullable: false),
                    LinkedToId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Rating = table.Column<byte>(nullable: false),
                    Active = table.Column<bool>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ViewedByAdmin = table.Column<bool>(nullable: true),
                    ReviewTemplateId = table.Column<int>(nullable: true),
                    Anonymous = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_ReviewTemplates",
                        column: x => x.ReviewTemplateId,
                        principalTable: "ReviewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_WebsiteLanguages",
                        column: x => x.WebsiteLanguageId,
                        principalTable: "WebsiteLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteUploadId = table.Column<int>(nullable: false),
                    OriginalPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    CompressedPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    Alt = table.Column<string>(maxLength: 200, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    WebsiteLanguageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteFiles_WebsiteLanguages",
                        column: x => x.WebsiteLanguageId,
                        principalTable: "WebsiteLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteLanguageId = table.Column<int>(nullable: false),
                    WebsiteFieldId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteResources_WebsiteLanguages",
                        column: x => x.WebsiteLanguageId,
                        principalTable: "WebsiteLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataItemFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataItemId = table.Column<int>(nullable: false),
                    DataTemplateUploadId = table.Column<int>(nullable: false),
                    OriginalPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    CompressedPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    Alt = table.Column<string>(maxLength: 200, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataItemFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataItemFiles_DataItems",
                        column: x => x.DataItemId,
                        principalTable: "DataItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataItemResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataItemId = table.Column<int>(nullable: false),
                    DataTemplateFieldId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataItemResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataItemResources_DataItems",
                        column: x => x.DataItemId,
                        principalTable: "DataItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderRefundLines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderRefundId = table.Column<int>(nullable: false),
                    OrderLineId = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(20, 4)", nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRefundLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderRefundLines_OrderRefunds",
                        column: x => x.OrderRefundId,
                        principalTable: "OrderRefunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PageId = table.Column<int>(nullable: false),
                    PageTemplateUploadId = table.Column<int>(nullable: false),
                    OriginalPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    CompressedPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    Alt = table.Column<string>(maxLength: 200, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageFiles_Pages",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PageId = table.Column<int>(nullable: false),
                    PageTemplateFieldId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageResources_Pages",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    ProductUploadId = table.Column<int>(nullable: false),
                    OriginalPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    CompressedPath = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    Alt = table.Column<string>(maxLength: 200, nullable: false),
                    CustomOrder = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFiles_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteLanguageId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    PageAlternateGuid = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPages_ProductPages",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPageSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WebsiteLanguageId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Url = table.Column<string>(maxLength: 200, nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Keywords = table.Column<string>(maxLength: 400, nullable: false),
                    Description = table.Column<string>(maxLength: 400, nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPageSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPageSettings_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    ProductFieldId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    WebsiteLanguageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductResources_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingZoneMethodClasses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShippingZoneMethodId = table.Column<int>(nullable: false),
                    ShippingClassId = table.Column<int>(nullable: false),
                    Cost = table.Column<string>(unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingZoneMethodClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingZoneMethodClasses_ShippingZoneMethods",
                        column: x => x.ShippingZoneMethodId,
                        principalTable: "ShippingZoneMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxRateLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaxRateId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRateLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxRateLocations_TaxRates",
                        column: x => x.TaxRateId,
                        principalTable: "TaxRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<int>(nullable: false),
                    ReviewTemplateFieldId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewResources_Reviews",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_WebsiteId",
                table: "ApiKeys",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyId",
                table: "CompanyUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DataItemFiles_DataItemId",
                table: "DataItemFiles",
                column: "DataItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DataItemResources_DataItemId",
                table: "DataItemResources",
                column: "DataItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DataItems_DataTemplateId",
                table: "DataItems",
                column: "DataTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DataTemplateFields_DataTemplateId",
                table: "DataTemplateFields",
                column: "DataTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DataTemplates_WebsiteId",
                table: "DataTemplates",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_DataTemplateSections_DataTemplateId",
                table: "DataTemplateSections",
                column: "DataTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DataTemplateUploads_DataTemplateId",
                table: "DataTemplateUploads",
                column: "DataTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationItems_NavigationId",
                table: "NavigationItems",
                column: "NavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Navigations_WebsiteId",
                table: "Navigations",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthTokens_WebsiteId",
                table: "OAuthTokens",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCoupons_OrderId",
                table: "OrderCoupons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFees_OrderId",
                table: "OrderFees",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderId",
                table: "OrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRefundLines_OrderRefundId",
                table: "OrderRefundLines",
                column: "OrderRefundId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRefunds_OrderId",
                table: "OrderRefunds",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WebsiteId",
                table: "Orders",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderShippingZoneMethods_OrderId",
                table: "OrderShippingZoneMethods",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PageFiles_PageId",
                table: "PageFiles",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageResources_PageId",
                table: "PageResources",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageTemplateId",
                table: "Pages",
                column: "PageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTemplateFields_PageTemplateId",
                table: "PageTemplateFields",
                column: "PageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTemplates_WebsiteId",
                table: "PageTemplates",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTemplateSections_PageTemplateId",
                table: "PageTemplateSections",
                column: "PageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTemplateUploads_PageTemplateId",
                table: "PageTemplateUploads",
                column: "PageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFields_ProductTemplateId",
                table: "ProductFields",
                column: "ProductTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFiles_ProductId",
                table: "ProductFiles",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPages_ProductId",
                table: "ProductPages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPageSettings_ProductId",
                table: "ProductPageSettings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductResources_ProductId",
                table: "ProductResources",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTemplateId",
                table: "Products",
                column: "ProductTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplates_WebsiteId",
                table: "ProductTemplates",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUploads_ProductTemplateId",
                table: "ProductUploads",
                column: "ProductTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResources_ReviewId",
                table: "ReviewResources",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewTemplateId",
                table: "Reviews",
                column: "ReviewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_WebsiteLanguageId",
                table: "Reviews",
                column: "WebsiteLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTemplateFields_ReviewTemplateId",
                table: "ReviewTemplateFields",
                column: "ReviewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTemplates_WebsiteId",
                table: "ReviewTemplates",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingClasses_WebsiteId",
                table: "ShippingClasses",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZoneLocations_ShippingZoneId",
                table: "ShippingZoneLocations",
                column: "ShippingZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZoneMethodClasses_ShippingZoneMethodId",
                table: "ShippingZoneMethodClasses",
                column: "ShippingZoneMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZoneMethods_ShippingZoneId",
                table: "ShippingZoneMethods",
                column: "ShippingZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZones_WebsiteId",
                table: "ShippingZones",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxClasses_WebsiteId",
                table: "TaxClasses",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRateLocations_TaxRateId",
                table: "TaxRateLocations",
                column: "TaxRateId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_TaxClassId",
                table: "TaxRates",
                column: "TaxClassId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteFields_WebsiteId",
                table: "WebsiteFields",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteFiles_WebsiteLanguageId",
                table: "WebsiteFiles",
                column: "WebsiteLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteLanguages_WebsiteId",
                table: "WebsiteLanguages",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteResources_WebsiteLanguageId",
                table: "WebsiteResources",
                column: "WebsiteLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Websites_CompanyId",
                table: "Websites",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteUploads_WebsiteId",
                table: "WebsiteUploads",
                column: "WebsiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CompanyUsers");

            migrationBuilder.DropTable(
                name: "Cultures");

            migrationBuilder.DropTable(
                name: "DataItemFiles");

            migrationBuilder.DropTable(
                name: "DataItemResources");

            migrationBuilder.DropTable(
                name: "DataTemplateFields");

            migrationBuilder.DropTable(
                name: "DataTemplateSections");

            migrationBuilder.DropTable(
                name: "DataTemplateUploads");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "LanguageTranslate");

            migrationBuilder.DropTable(
                name: "NavigationItems");

            migrationBuilder.DropTable(
                name: "OAuthTokens");

            migrationBuilder.DropTable(
                name: "OrderCoupons");

            migrationBuilder.DropTable(
                name: "OrderFees");

            migrationBuilder.DropTable(
                name: "OrderLines");

            migrationBuilder.DropTable(
                name: "OrderRefundLines");

            migrationBuilder.DropTable(
                name: "OrderShippingZoneMethods");

            migrationBuilder.DropTable(
                name: "PageFiles");

            migrationBuilder.DropTable(
                name: "PageResources");

            migrationBuilder.DropTable(
                name: "PageTemplateFields");

            migrationBuilder.DropTable(
                name: "PageTemplateSections");

            migrationBuilder.DropTable(
                name: "PageTemplateUploads");

            migrationBuilder.DropTable(
                name: "ProductFields");

            migrationBuilder.DropTable(
                name: "ProductFiles");

            migrationBuilder.DropTable(
                name: "ProductPages");

            migrationBuilder.DropTable(
                name: "ProductPageSettings");

            migrationBuilder.DropTable(
                name: "ProductResources");

            migrationBuilder.DropTable(
                name: "ProductUploads");

            migrationBuilder.DropTable(
                name: "ReviewResources");

            migrationBuilder.DropTable(
                name: "ReviewTemplateFields");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "ShippingClasses");

            migrationBuilder.DropTable(
                name: "ShippingZoneLocations");

            migrationBuilder.DropTable(
                name: "ShippingZoneMethodClasses");

            migrationBuilder.DropTable(
                name: "TaxRateLocations");

            migrationBuilder.DropTable(
                name: "WebsiteFields");

            migrationBuilder.DropTable(
                name: "WebsiteFiles");

            migrationBuilder.DropTable(
                name: "WebsiteResources");

            migrationBuilder.DropTable(
                name: "WebsiteUploads");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DataItems");

            migrationBuilder.DropTable(
                name: "Navigations");

            migrationBuilder.DropTable(
                name: "OrderRefunds");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ShippingZoneMethods");

            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropTable(
                name: "DataTemplates");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PageTemplates");

            migrationBuilder.DropTable(
                name: "ProductTemplates");

            migrationBuilder.DropTable(
                name: "ReviewTemplates");

            migrationBuilder.DropTable(
                name: "WebsiteLanguages");

            migrationBuilder.DropTable(
                name: "ShippingZones");

            migrationBuilder.DropTable(
                name: "TaxClasses");

            migrationBuilder.DropTable(
                name: "Websites");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
