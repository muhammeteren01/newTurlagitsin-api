using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    TourId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    CoverImage = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SpecialNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.TourId);
                    table.ForeignKey(
                        name: "FK_Tours_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TourCoupons",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    DiscountType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourCoupons", x => x.CouponId);
                    table.ForeignKey(
                        name: "FK_TourCoupons_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourExcludes",
                columns: table => new
                {
                    ExcludeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourExcludes", x => x.ExcludeId);
                    table.ForeignKey(
                        name: "FK_TourExcludes_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourExtraFees",
                columns: table => new
                {
                    ExtraFeeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourExtraFees", x => x.ExtraFeeId);
                    table.ForeignKey(
                        name: "FK_TourExtraFees_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourHotels",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CheckIn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOut = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourHotels", x => x.HotelId);
                    table.ForeignKey(
                        name: "FK_TourHotels_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsCover = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_TourImages_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourIncludes",
                columns: table => new
                {
                    IncludeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourIncludes", x => x.IncludeId);
                    table.ForeignKey(
                        name: "FK_TourIncludes_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPrices",
                columns: table => new
                {
                    PriceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    PriceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPrices", x => x.PriceId);
                    table.ForeignKey(
                        name: "FK_TourPrices_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourRoutes",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    OrderNo = table.Column<int>(type: "integer", nullable: false),
                    LocationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ArrivalTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourRoutes", x => x.RouteId);
                    table.ForeignKey(
                        name: "FK_TourRoutes_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourTags",
                columns: table => new
                {
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourTags", x => new { x.TourId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TourTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourTags_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourCouponUsages",
                columns: table => new
                {
                    UsageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CouponId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourCouponUsages", x => x.UsageId);
                    table.ForeignKey(
                        name: "FK_TourCouponUsages_TourCoupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "TourCoupons",
                        principalColumn: "CouponId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[,]
                {
                    { 1, "Kültür Turu" },
                    { 2, "Doğa Turu" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "TagId", "Name" },
                values: new object[,]
                {
                    { 1, "Karadeniz" },
                    { 2, "Doğa" },
                    { 3, "Yemek" },
                    { 4, "Tarih" },
                    { 5, "Deniz" }
                });

            migrationBuilder.InsertData(
                table: "Tours",
                columns: new[] { "TourId", "CategoryId", "CoverImage", "Description", "Name", "SpecialNotes" },
                values: new object[,]
                {
                    { 1L, 1, "karadeniz_cover.jpg", "Trabzon’dan başlayan ve yaylaları kapsayan doğa ve kültür turu.", "Karadeniz Yayla Turu", "Yanınıza mutlaka yağmurluk alın" },
                    { 2L, 1, "ege_cover.jpg", "İzmir’den başlayıp Efes ve Bergama antik kentlerini kapsayan kültürel tur.", "Ege Antik Kentler Turu", "Güneş kremi ve şapka önerilir" }
                });

            migrationBuilder.InsertData(
                table: "TourCoupons",
                columns: new[] { "CouponId", "Code", "DiscountType", "EndDate", "StartDate", "TourId", "Value" },
                values: new object[,]
                {
                    { 1, "KARADENIZ20", "percent", new DateTime(2025, 9, 30, 23, 59, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, 20.00m },
                    { 2, "EGE300", "amount", new DateTime(2025, 9, 20, 23, 59, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc), 2L, 300.00m },
                    { 3, "YAYLA150", "amount", new DateTime(2025, 9, 25, 23, 59, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1L, 150.00m }
                });

            migrationBuilder.InsertData(
                table: "TourExcludes",
                columns: new[] { "ExcludeId", "Description", "TourId" },
                values: new object[,]
                {
                    { 1, "Kişisel harcamalar", 1L },
                    { 2, "Müze giriş ücretleri", 1L },
                    { 3, "Öğle ve akşam yemekleri", 2L },
                    { 4, "Müze giriş ücretleri", 2L }
                });

            migrationBuilder.InsertData(
                table: "TourExtraFees",
                columns: new[] { "ExtraFeeId", "Amount", "Name", "TourId" },
                values: new object[,]
                {
                    { 1, 50.00m, "Müze giriş ücreti", 1L },
                    { 2, 100.00m, "Teleferik ücreti", 1L },
                    { 3, 150.00m, "Efes giriş bileti", 2L },
                    { 4, 120.00m, "Bergama Akropol ücreti", 2L }
                });

            migrationBuilder.InsertData(
                table: "TourHotels",
                columns: new[] { "HotelId", "CheckIn", "CheckOut", "Description", "Name", "TourId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 1, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 2, 11, 0, 0, 0, DateTimeKind.Utc), "Trabzon merkezde konaklama", "Trabzon Otel", 1L },
                    { 2, new DateTime(2025, 9, 2, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 3, 11, 0, 0, 0, DateTimeKind.Utc), "Yayla manzaralı konaklama", "Ayder Dağ Evi", 1L },
                    { 3, new DateTime(2025, 9, 10, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 11, 11, 0, 0, 0, DateTimeKind.Utc), "İzmir merkezde konaklama", "İzmir City Hotel", 2L },
                    { 4, new DateTime(2025, 9, 11, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 12, 11, 0, 0, 0, DateTimeKind.Utc), "Deniz kenarında resort otel", "Kuşadası Resort", 2L }
                });

            migrationBuilder.InsertData(
                table: "TourImages",
                columns: new[] { "ImageId", "IsCover", "TourId", "Url" },
                values: new object[,]
                {
                    { 1, true, 1L, "karadeniz_cover.jpg" },
                    { 2, false, 1L, "karadeniz1.jpg" },
                    { 3, false, 1L, "karadeniz2.jpg" },
                    { 4, true, 2L, "ege_cover.jpg" },
                    { 5, false, 2L, "ege1.jpg" },
                    { 6, false, 2L, "ege2.jpg" }
                });

            migrationBuilder.InsertData(
                table: "TourIncludes",
                columns: new[] { "IncludeId", "Description", "TourId" },
                values: new object[,]
                {
                    { 1, "Ulaşım", 1L },
                    { 2, "Rehberlik hizmeti", 1L },
                    { 3, "2 gece otel konaklaması", 1L },
                    { 4, "Ulaşım", 2L },
                    { 5, "Profesyonel rehber", 2L },
                    { 6, "1 gece otel konaklaması", 2L }
                });

            migrationBuilder.InsertData(
                table: "TourPrices",
                columns: new[] { "PriceId", "Amount", "PriceType", "TourId" },
                values: new object[,]
                {
                    { 1, 2500.00m, "Yetişkin", 1L },
                    { 2, 2000.00m, "Öğrenci", 1L },
                    { 3, 1500.00m, "Çocuk", 1L },
                    { 4, 3000.00m, "Yetişkin", 2L },
                    { 5, 2500.00m, "Öğrenci", 2L },
                    { 6, 1800.00m, "Çocuk", 2L }
                });

            migrationBuilder.InsertData(
                table: "TourRoutes",
                columns: new[] { "RouteId", "ArrivalTime", "LocationName", "OrderNo", "TourId" },
                values: new object[,]
                {
                    { 1, new TimeOnly(9, 0, 0), "Trabzon", 1, 1L },
                    { 2, new TimeOnly(10, 30, 0), "Sürmene", 2, 1L },
                    { 3, new TimeOnly(12, 0, 0), "Uzungöl", 3, 1L },
                    { 4, new TimeOnly(15, 0, 0), "Ayder Yaylası", 4, 1L },
                    { 5, new TimeOnly(8, 0, 0), "İzmir", 1, 2L },
                    { 6, new TimeOnly(10, 0, 0), "Efes Antik Kenti", 2, 2L },
                    { 7, new TimeOnly(13, 0, 0), "Şirince", 3, 2L },
                    { 8, new TimeOnly(16, 0, 0), "Bergama", 4, 2L }
                });

            migrationBuilder.InsertData(
                table: "TourTags",
                columns: new[] { "TagId", "TourId" },
                values: new object[,]
                {
                    { 1, 1L },
                    { 2, 1L },
                    { 3, 1L },
                    { 4, 2L },
                    { 5, 2L }
                });

            migrationBuilder.InsertData(
                table: "TourCouponUsages",
                columns: new[] { "UsageId", "CouponId", "UsedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), 101L },
                    { 2, 1, new DateTime(2025, 9, 7, 15, 30, 0, 0, DateTimeKind.Utc), 102L },
                    { 3, 1, new DateTime(2025, 9, 8, 9, 45, 0, 0, DateTimeKind.Utc), 103L },
                    { 4, 2, new DateTime(2025, 9, 12, 18, 0, 0, 0, DateTimeKind.Utc), 104L },
                    { 5, 2, new DateTime(2025, 9, 13, 11, 20, 0, 0, DateTimeKind.Utc), 105L },
                    { 6, 3, new DateTime(2025, 9, 10, 10, 30, 0, 0, DateTimeKind.Utc), 106L },
                    { 7, 3, new DateTime(2025, 9, 11, 14, 45, 0, 0, DateTimeKind.Utc), 107L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourCoupons_TourId_Code",
                table: "TourCoupons",
                columns: new[] { "TourId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourCouponUsages_CouponId",
                table: "TourCouponUsages",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_TourExcludes_TourId",
                table: "TourExcludes",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourExtraFees_TourId",
                table: "TourExtraFees",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourHotels_TourId",
                table: "TourHotels",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourImages_TourId",
                table: "TourImages",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourIncludes_TourId",
                table: "TourIncludes",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPrices_TourId",
                table: "TourPrices",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourRoutes_TourId",
                table: "TourRoutes",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_CategoryId",
                table: "Tours",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TourTags_TagId",
                table: "TourTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourCouponUsages");

            migrationBuilder.DropTable(
                name: "TourExcludes");

            migrationBuilder.DropTable(
                name: "TourExtraFees");

            migrationBuilder.DropTable(
                name: "TourHotels");

            migrationBuilder.DropTable(
                name: "TourImages");

            migrationBuilder.DropTable(
                name: "TourIncludes");

            migrationBuilder.DropTable(
                name: "TourPrices");

            migrationBuilder.DropTable(
                name: "TourRoutes");

            migrationBuilder.DropTable(
                name: "TourTags");

            migrationBuilder.DropTable(
                name: "TourCoupons");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
