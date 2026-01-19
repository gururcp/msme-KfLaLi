using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HotelwebLisMVC.Models
{
    public partial class HotelWebLisDBContext : DbContext
    {
        public HotelWebLisDBContext()
        {
        }

        public HotelWebLisDBContext(DbContextOptions<HotelWebLisDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Branch> Branches { get; set; } = null!;
        public virtual DbSet<ComparisonUnit> ComparisonUnits { get; set; } = null!;
        public virtual DbSet<Gst> Gsts { get; set; } = null!;
        public virtual DbSet<Item> Items { get; set; } = null!;
        public virtual DbSet<ItemCategory> ItemCategories { get; set; } = null!;
        public virtual DbSet<Ledger> Ledgers { get; set; } = null!;
        public virtual DbSet<LedgerType> LedgerTypes { get; set; } = null!;
        public virtual DbSet<Login> Logins { get; set; } = null!;
        public virtual DbSet<ManageStock> ManageStocks { get; set; } = null!;
        public virtual DbSet<Production> Productions { get; set; } = null!;
        public virtual DbSet<ProductionDetail> ProductionDetails { get; set; } = null!;
        public virtual DbSet<Purchase> Purchases { get; set; } = null!;
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; } = null!;
        public virtual DbSet<RawMaterial> RawMaterials { get; set; } = null!;
        public virtual DbSet<RawMaterialCategory> RawMaterialCategories { get; set; } = null!;
        public virtual DbSet<Recipe> Recipes { get; set; } = null!;
        public virtual DbSet<RecipeDetail> RecipeDetails { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Sale> Sales { get; set; } = null!;
        public virtual DbSet<SaleDetail> SaleDetails { get; set; } = null!;
        public virtual DbSet<TransactionDatum> TransactionData { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Wastage> Wastages { get; set; } = null!;
        public virtual DbSet<WastageDetail> WastageDetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.Address).HasColumnType("text");

                entity.Property(e => e.BranchName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ComparisonUnit>(entity =>
            {
                entity.HasKey(e => e.EquivalentId)
                    .HasName("PK__Comparis__29FA9246DE92CA66");

                entity.ToTable("ComparisonUnit");

                entity.Property(e => e.EquivalentValue).HasMaxLength(20);

                entity.Property(e => e.FromUnit).HasMaxLength(20);

                entity.Property(e => e.ToUnit).HasMaxLength(20);
            });

            modelBuilder.Entity<Gst>(entity =>
            {
                entity.ToTable("GST");

                entity.Property(e => e.GstId).HasColumnName("GST_ID");

                entity.Property(e => e.Cgst)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("CGST");

                entity.Property(e => e.Gstpercentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("GSTPercentage");

                entity.Property(e => e.Igst)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("IGST");

                entity.Property(e => e.Sgst)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("SGST");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ContainerCharges).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Dietary)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.GstId).HasColumnName("GST_ID");

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ItemName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ItemNameOnline)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.MinStockToMaintain).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Mrpprice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("MRPPrice");

                entity.Property(e => e.OpeningStock).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.OpeningStockDate).HasColumnType("datetime");

                entity.Property(e => e.OpeningStockPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.SalePrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Unit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Item_Category");

                entity.HasOne(d => d.Gst)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.GstId)
                    .HasConstraintName("FK__Item__GST_ID__3D2915A8");
            });

            modelBuilder.Entity<ItemCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__ItemCate__19093A2B0D4AB253");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ledger>(entity =>
            {
                entity.ToTable("Ledger");

                entity.Property(e => e.LedgerId).HasColumnName("LedgerID");

                entity.Property(e => e.BillingAddress)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreditLimit).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.EmailId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EmailID");

                entity.Property(e => e.Gstnumber)
                    .HasMaxLength(50)
                    .HasColumnName("GSTNumber");

                entity.Property(e => e.Gsttype)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("GSTType");

                entity.Property(e => e.LedgerName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.LedgerTypeId).HasColumnName("LedgerTypeID");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.OpeningBalance).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.OpeningBalanceDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ShipingAddress)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LedgerType>(entity =>
            {
                entity.ToTable("LedgerType");

                entity.Property(e => e.LedgerTypeId).HasColumnName("LedgerTypeID");

                entity.Property(e => e.LedgerTypeName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.ToTable("Login");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<ManageStock>(entity =>
            {
                entity.HasKey(e => e.ClosingStockId)
                    .HasName("PK__ManageSt__3F9AD63E3CBFBBDE");

                entity.ToTable("ManageStock");

                entity.Property(e => e.ClosingQuantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Comments)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.RawMaterialId).HasColumnName("RawMaterialID");

                entity.Property(e => e.SubQuantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Unit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.RawMaterial)
                    .WithMany(p => p.ManageStocks)
                    .HasForeignKey(d => d.RawMaterialId)
                    .HasConstraintName("FK__ManageSto__RawMa__4589517F");
            });

            modelBuilder.Entity<Production>(entity =>
            {
                entity.ToTable("Production");

                entity.Property(e => e.ProductionId).HasColumnName("ProductionID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.ProductionName)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.RawMaterialId).HasColumnName("RawMaterialID");

                entity.Property(e => e.Unit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.RawMaterial)
                    .WithMany(p => p.Productions)
                    .HasForeignKey(d => d.RawMaterialId)
                    .HasConstraintName("FK__Productio__RawMa__6E8B6712");
            });

            modelBuilder.Entity<ProductionDetail>(entity =>
            {
                entity.HasKey(e => e.ProductionDetailsId)
                    .HasName("PK__Producti__7B6BCF5D240644C1");

                entity.Property(e => e.ProductionDetailsId).HasColumnName("ProductionDetailsID");

                entity.Property(e => e.ProductionId).HasColumnName("ProductionID");

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.RawMaterialId).HasColumnName("RawMaterialID");

                entity.Property(e => e.Unit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Production)
                    .WithMany(p => p.ProductionDetails)
                    .HasForeignKey(d => d.ProductionId)
                    .HasConstraintName("FK__Productio__Produ__7167D3BD");

                entity.HasOne(d => d.RawMaterial)
                    .WithMany(p => p.ProductionDetails)
                    .HasForeignKey(d => d.RawMaterialId)
                    .HasConstraintName("FK__Productio__RawMa__725BF7F6");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("Purchase");

                entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");

                entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LedgerId).HasColumnName("LedgerID");

                entity.Property(e => e.PaymentMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Received).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalDiscount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalGst)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TotalGST");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("FK__Purchase__Branch__56E8E7AB");

                entity.HasOne(d => d.Ledger)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.LedgerId)
                    .HasConstraintName("FK__Purchase__Ledger__55F4C372");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Purchase__UserID__57DD0BE4");
            });

            modelBuilder.Entity<PurchaseDetail>(entity =>
            {
                entity.Property(e => e.PurchaseDetailId).HasColumnName("PurchaseDetailID");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Gstamount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GSTAmount");

                entity.Property(e => e.Gstpercentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("GSTPercentage");

                entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");

                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RawMaterialId).HasColumnName("RawMaterialID");

                entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Unit).HasMaxLength(20);

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("FK__PurchaseD__Purch__5AB9788F");

                entity.HasOne(d => d.RawMaterial)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.RawMaterialId)
                    .HasConstraintName("FK__PurchaseD__RawMa__5BAD9CC8");
            });

            modelBuilder.Entity<RawMaterial>(entity =>
            {
                entity.ToTable("RawMaterial");

                entity.HasIndex(e => e.Name, "UQ__RawMater__737584F657E5404B")
                    .IsUnique();

                entity.Property(e => e.RawMaterialId).HasColumnName("RawMaterialID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ConsumptionUnit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EquivalentUnit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MinStockToMaintain).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.OpeningStock).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.OpeningStockDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OpeningStockPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PurchaseUnit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ReconciliationPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TransferPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.YeildPercenatge).HasMaxLength(50);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.RawMaterials)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__RawMateri__Categ__3493CFA7");
            });

            modelBuilder.Entity<RawMaterialCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__RawMater__19093A2B98380CCC");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__Recipe__ItemID__40058253");
            });

            modelBuilder.Entity<RecipeDetail>(entity =>
            {
                entity.HasKey(e => e.RecipeDetailsId)
                    .HasName("PK__RecipeDe__C6B230BE3DF846FD");

                entity.Property(e => e.RecipeDetailsId).HasColumnName("RecipeDetailsID");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.RawMaterialId).HasColumnName("RawMaterialID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.Unit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.RawMaterial)
                    .WithMany(p => p.RecipeDetails)
                    .HasForeignKey(d => d.RawMaterialId)
                    .HasConstraintName("FK__RecipeDet__RawMa__43D61337");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeDetails)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__RecipeDet__Recip__42E1EEFE");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61606CF3A4CF")
                    .IsUnique();

                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160B3B2DD75")
                    .IsUnique();

                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160FC8BED2D")
                    .IsUnique();

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasIndex(e => e.InvoiceNo, "UQ__Sales__D796B22750A0E17F")
                    .IsUnique();

                entity.Property(e => e.SaleId).HasColumnName("SaleID");

                entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LedgerId).HasColumnName("LedgerID");

                entity.Property(e => e.PaymentMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Received).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalDiscount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalGst)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("TotalGST");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("FK__Sales__BranchID__15DA3E5D");

                entity.HasOne(d => d.Ledger)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.LedgerId)
                    .HasConstraintName("FK__Sales__LedgerID__14E61A24");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Sales__UserID__16CE6296");
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.Property(e => e.SaleDetailId).HasColumnName("SaleDetailID");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Gstamount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("GSTAmount");

                entity.Property(e => e.Gstpercentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("GSTPercentage");

                entity.Property(e => e.Hsncode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("HSNCode");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.Rate).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.SaleId).HasColumnName("SaleID");

                entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.SaleDetails)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__SaleDetai__ItemI__1B9317B3");

                entity.HasOne(d => d.Sale)
                    .WithMany(p => p.SaleDetails)
                    .HasForeignKey(d => d.SaleId)
                    .HasConstraintName("FK__SaleDetai__SaleI__1A9EF37A");
            });

            modelBuilder.Entity<TransactionDatum>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Transact__55433A4B01022920");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LedgerId).HasColumnName("LedgerID");

                entity.Property(e => e.Narration)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PayMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Received).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TransactionMode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.VoucherNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Ledger)
                    .WithMany(p => p.TransactionData)
                    .HasForeignKey(d => d.LedgerId)
                    .HasConstraintName("FK__Transacti__Ledge__531856C7");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");

                entity.Property(e => e.UnitName).HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ__Users__536C85E423E3F5F6")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "UQ__Users__536C85E4C9BD3171")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "UQ__Users__536C85E4E34BD7AC")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("FK__Users__BranchID__08B54D69");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleID__0F624AF8");
            });

            modelBuilder.Entity<Wastage>(entity =>
            {
                entity.ToTable("Wastage");

                entity.Property(e => e.WastageId).HasColumnName("WastageID");

                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<WastageDetail>(entity =>
            {
                entity.HasKey(e => e.WastageDetailsId)
                    .HasName("PK__WastageD__FF22631BA5E19794");

                entity.Property(e => e.WastageDetailsId).HasColumnName("WastageDetailsID");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.AvgPurchasePrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.RawMaterialId).HasColumnName("RawMaterialID");

                entity.Property(e => e.Unit)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WastageId).HasColumnName("WastageID");

                entity.HasOne(d => d.RawMaterial)
                    .WithMany(p => p.WastageDetails)
                    .HasForeignKey(d => d.RawMaterialId)
                    .HasConstraintName("FK__WastageDe__RawMa__345EC57D");

                entity.HasOne(d => d.Wastage)
                    .WithMany(p => p.WastageDetails)
                    .HasForeignKey(d => d.WastageId)
                    .HasConstraintName("FK__WastageDe__Wasta__336AA144");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
