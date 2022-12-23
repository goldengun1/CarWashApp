using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWash_App.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAnOwner = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    Cost = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "carWashes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CarWashName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningTime = table.Column<int>(type: "int", nullable: false),
                    ClosingTime = table.Column<int>(type: "int", nullable: false),
                    Profit = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carWashes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_carWashes_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarWashesServiceTypes",
                columns: table => new
                {
                    CarWashId = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarWashesServiceTypes", x => new { x.CarWashId, x.ServiceTypeId });
                    table.ForeignKey(
                        name: "FK_CarWashesServiceTypes_carWashes_CarWashId",
                        column: x => x.CarWashId,
                        principalTable: "carWashes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarWashesServiceTypes_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarWashId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EligibleForCancelation = table.Column<bool>(type: "bit", nullable: false),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false),
                    PaymentCollected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Services_carWashes_CarWashId",
                        column: x => x.CarWashId,
                        principalTable: "carWashes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Services_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsAnOwner", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "193E7C46-91A2-4F45-9E11-44964496FB0F", 0, "88c7427b-1565-4d06-a9f4-92bcf056ae44", "petar.petrovic@gmail.com", false, "Petar", false, "Petrovic", false, null, "PETAR.PETROVIC@GMAIL.COM", "PETAR.PETROVIC", "AQAAAAEAACcQAAAAEA26SAWxl5BIx1wdxzZWqwkGoW3cdGNVYTGclotyslVwvUy03lcrxDOi0i3U76CnMg==", null, false, "1f86c332-9675-428c-8cf7-15db66fe86ba", false, "petar.petrovic" },
                    { "264249DB-C9DC-4827-A5D0-D7494C1086FD", 0, "dc75ddc4-a3b6-4ef1-83cd-b343fd450afe", "jovan.jovic@gmail.com", false, "Jovan", false, "Jovic", false, null, "JOVAN.JOVIC@GMAIL.COM", "JOVAN.JOVIC", "AQAAAAEAACcQAAAAELdHHA8EN7CwerhMu7KMis4Ihn7nUNeefKRtRMEv7Niq6NSvHVETk7K14h2YVXl9KQ==", null, false, "7681982e-e7c7-4e97-bfad-ee20b92ad8cc", false, "jovan.jovic" },
                    { "4FE7601C-53C2-467B-8F0B-6AB8F048C680", 0, "a5967905-a00f-4545-8486-8e1ff4c2b46e", "pera.peric@gmail.com", false, "Pera", true, "Peric", false, null, "PERA.PERIC@GMAIL.COM", "PERA.PERIC", "AQAAAAEAACcQAAAAEEobRhuNLlTr1wEu+82T3TbqXlozIwcJRJW9HcWYU93eFaSGUpv9hQd5/jSbagAZSQ==", null, false, "820294cd-813e-4c3a-aa0a-36c6961d67b8", false, "pera.peric" },
                    { "722D96C6-60D1-4233-B41E-D788E2451D4F", 0, "75531d3d-0e6d-4995-bb98-1c0f3cafcb85", "ana.anic@gmail.com", false, "Ana", false, "Anic", false, null, "ANA.ANIC@GMAIL.COM", "ANA.ANIC", "AQAAAAEAACcQAAAAEBnGTQMGKpcOsszIlaJkFpqZu5ZhCyXRG20hUQv8zVImeJpa1PqxX3qN9900Hhb9qg==", null, false, "0ac2d38f-8716-4497-8345-de48ccb5315e", false, "ana.anic" },
                    { "7949B386-70FB-403D-9692-8BE9DCF2BC1E", 0, "d6011d8c-53c4-4b18-bdef-a92b7b4ed2d9", "milovan.milovanovic@gmail.com", false, "Milovan", false, "Milovanovic", false, null, "MILOVAN.MILOVANOVIC@GMAIL.COM", "MILOVAN.MILOVANOVIC", "AQAAAAEAACcQAAAAEHxC5dCtFzpScHummTSjnk8uGkA6UZ2Xh5+b99MBZ/zjGVZvZKwCHS9tc1PVR86s9Q==", null, false, "292a801d-6cf8-4fa5-abbf-67b6eeb1cfeb", false, "milovan.milovanovic" },
                    { "838C5B52-9F4F-435E-809D-BD7D5864BB5E", 0, "526b1964-bded-4515-ba0d-0a53d7e70b91", "admin@admin.com", false, "Administrator", false, "Arministrator", false, null, "ADMIN@ADMIN.COM", "ADMIN.ADMIN", "AQAAAAEAACcQAAAAEKQ4lXhCAmW25Erfa3VavmPxPfaFBM5cE57LyF5X/RypYP7LFRPE+YfSVAjNRya0FQ==", null, false, "2a6e1ed7-e7fd-4d81-8f39-fe274058867b", false, "admin.admin" },
                    { "8F0A1391-5547-43F0-AEC9-59908CB381D9", 0, "a7984954-f0f4-4570-a8ce-1ab1138bea53", "stefanija.markovic@gmail.com", false, "Stefanija", true, "Markovic", false, null, "STEFANIJA.MARKOVIC@GMAIL.COM", "STEFANIJA.MARKOVIC", "AQAAAAEAACcQAAAAEOQdrSfZHka5DwhDmP//bW5NcMhZBKz5tK0wXb1afaizmQe9nLxHuZGt9EyNXS/d+g==", null, false, "dd6cdcd9-dfff-4ec2-8def-e834f8e00489", false, "stefanija.markovic" },
                    { "BB4077EC-96BD-4FF9-8365-F522AF43ED30", 0, "06a9e767-ef9e-485a-afc7-7f681081aaf7", "laza.lazic@gmail.com", false, "Laza", true, "Lazic", false, null, "LAZA.LAZIC@GMAIL.COM", "LAZA.LAZIC", "AQAAAAEAACcQAAAAEP3yuaKr/SYI6DgAqWN4hoAZLdMtJKYTgFozq2j9D3uXnkEZAHxO6DFaxqDBn7ufZg==", null, false, "cbed408c-dc34-4bc6-bbb5-3f53722ca2f6", false, "laza.lazic" },
                    { "C3F0EF4D-A5A1-46CE-91CF-4443B95734C6", 0, "fdf69d30-7d69-4fd0-ba97-a3cab2f7e157", "milan.milanovic@gmail.com", false, "Milan", false, "Milanovic", false, null, "MILAN.MILANOVIC@GMAIL.COM", "MILAN.MILANOVIC", "AQAAAAEAACcQAAAAEJ0z+1meueGwsJwHYtoSaVgpp8i0fOYM76eCsrInQFJJtlOipTatwy3AMF264E1+gA==", null, false, "5142f691-58b9-4ad7-a4b1-d102844d910c", false, "milan.milanovic" },
                    { "E8560300-CD67-45FB-B07F-7713F45D131A", 0, "db19703f-21c7-48df-a9ac-9fc4e15c9c9c", "mika.mikic@gmail.com", false, "Mika", true, "Mikic", false, null, "MIKA.MIKIC@GMAIL.COM", "MIKA.MIKIC", "AQAAAAEAACcQAAAAEGpNaK8G1mUTfLIVozEKWFxp+DyoXo0QlHCqvTvklsRmGQL4LYL1/gcSWT6blOTmfQ==", null, false, "5570e2c7-4a31-47d5-8782-0f34bc056d77", false, "mika.mikic" },
                    { "FBB5FC51-4270-48C1-BADD-A441FF5759F3", 0, "81a13e26-4376-4bf1-aa9d-4b6a5144ae10", "mihailo.simic@gmail.com", false, "Mihailo", true, "Simic", false, null, "MIHAILO.SIMIC@GMAIL.COM", "MIHAILO.SIMIC", "AQAAAAEAACcQAAAAEDZevxryJyRwL11AAIzpaXZyy++IPZ67hHB/41P+a5homYVcMrZ42JIlV41Td6ZROw==", null, false, "470269f7-35a2-4bfd-9fa9-de63ac1d0e1b", false, "mihailo.simic" }
                });

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "Cost", "Duration", "ServiceName" },
                values: new object[,]
                {
                    { 1, 2.5f, new TimeSpan(0, 1, 0, 0, 0), "Regular" },
                    { 2, 4.5f, new TimeSpan(0, 2, 0, 0, 0), "Extended" },
                    { 3, 8.75f, new TimeSpan(0, 3, 0, 0, 0), "Premium" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Owner", "8F0A1391-5547-43F0-AEC9-59908CB381D9" },
                    { 2, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Owner", "FBB5FC51-4270-48C1-BADD-A441FF5759F3" },
                    { 3, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Owner", "4FE7601C-53C2-467B-8F0B-6AB8F048C680" },
                    { 4, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Owner", "BB4077EC-96BD-4FF9-8365-F522AF43ED30" },
                    { 5, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Owner", "E8560300-CD67-45FB-B07F-7713F45D131A" },
                    { 6, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Customer", "722D96C6-60D1-4233-B41E-D788E2451D4F" },
                    { 7, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Customer", "264249DB-C9DC-4827-A5D0-D7494C1086FD" },
                    { 8, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Customer", "7949B386-70FB-403D-9692-8BE9DCF2BC1E" },
                    { 9, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Customer", "193E7C46-91A2-4F45-9E11-44964496FB0F" },
                    { 10, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Customer", "C3F0EF4D-A5A1-46CE-91CF-4443B95734C6" },
                    { 11, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "838C5B52-9F4F-435E-809D-BD7D5864BB5E" }
                });

            migrationBuilder.InsertData(
                table: "carWashes",
                columns: new[] { "Id", "CarWashName", "ClosingTime", "OpeningTime", "OwnerId", "Profit" },
                values: new object[,]
                {
                    { 1, "CarWashExtra", 17, 9, "FBB5FC51-4270-48C1-BADD-A441FF5759F3", 0f },
                    { 2, "MegaWash", 22, 12, "FBB5FC51-4270-48C1-BADD-A441FF5759F3", 0f },
                    { 3, "StecoPoint", 15, 6, "8F0A1391-5547-43F0-AEC9-59908CB381D9", 0f },
                    { 4, "4U2Wash", 24, 0, "4FE7601C-53C2-467B-8F0B-6AB8F048C680", 0f }
                });

            migrationBuilder.InsertData(
                table: "CarWashesServiceTypes",
                columns: new[] { "CarWashId", "ServiceTypeId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 3, 3 },
                    { 4, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

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
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_carWashes_OwnerId",
                table: "carWashes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashesServiceTypes_ServiceTypeId",
                table: "CarWashesServiceTypes",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CarWashId",
                table: "Services",
                column: "CarWashId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CustomerId",
                table: "Services",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceTypeId",
                table: "Services",
                column: "ServiceTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "CarWashesServiceTypes");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "carWashes");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
