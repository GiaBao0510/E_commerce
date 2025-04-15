using System;
using System.Collections.Generic;
using E_commerce.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<ChatbotInteraction> ChatbotInteractions { get; set; }

    public virtual DbSet<Collaborate> Collaborates { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerRoleDetail> CustomerRoleDetails { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentDetail> DepartmentDetails { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Good> Goods { get; set; }

    public virtual DbSet<GoodsReceipt> GoodsReceipts { get; set; }

    public virtual DbSet<GoodsReceiptDetail> GoodsReceiptDetails { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<LoginHistory> LoginHistories { get; set; }

    public virtual DbSet<MessageBox> MessageBoxes { get; set; }

    public virtual DbSet<OauthProvider> OauthProviders { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentSlip> PaymentSlips { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PositionStaff> PositionStaffs { get; set; }

    public virtual DbSet<ProductPhoto> ProductPhotos { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<ProductTypeDetail> ProductTypeDetails { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Rank> Ranks { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SalesPrice> SalesPrices { get; set; }

    public virtual DbSet<ShippingBill> ShippingBills { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<StaffRoleDetail> StaffRoleDetails { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierLogo> SupplierLogos { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserOauth> UserOauths { get; set; }

    public virtual DbSet<UserPhotoDetail> UserPhotoDetails { get; set; }

    public virtual DbSet<WebSite> WebSites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PRIMARY");

            entity.ToTable("Bill");

            entity.HasIndex(e => e.BillId, "idx_bill_id");

            entity.HasIndex(e => e.InvoiceDate, "idx_invoice_date");

            entity.HasIndex(e => e.PmtId, "idxbill_pmt_id");

            entity.HasIndex(e => e.StatusId, "idxbill_status_id");

            entity.HasIndex(e => e.UserEmp, "idxbill_user_emp");

            entity.HasIndex(e => e.UserClient, "user_client");

            entity.Property(e => e.BillId)
                .HasMaxLength(15)
                .HasColumnName("bill_id");
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("invoice_date");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.PmtId).HasColumnName("pmt_id");
            entity.Property(e => e.ShippingFee)
                .HasPrecision(10)
                .HasDefaultValueSql("'0'")
                .HasColumnName("shipping_fee");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.Pmt).WithMany(p => p.Bills)
                .HasForeignKey(d => d.PmtId)
                .HasConstraintName("Bill_ibfk_3");

            entity.HasOne(d => d.Status).WithMany(p => p.Bills)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("Bill_ibfk_2");

            entity.HasOne(d => d.UserClientNavigation).WithMany(p => p.Bills)
                .HasForeignKey(d => d.UserClient)
                .HasConstraintName("Bill_ibfk_4");

            entity.HasOne(d => d.UserEmpNavigation).WithMany(p => p.Bills)
                .HasForeignKey(d => d.UserEmp)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Bill_ibfk_1");
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => new { e.BillId, e.ProductId }, "idxBillDetails_bill_id_product_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.BillId)
                .HasMaxLength(15)
                .HasColumnName("bill_id");
            entity.Property(e => e.ProductId)
                .HasMaxLength(10)
                .HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Bill).WithMany()
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("BillDetails_ibfk_1");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("BillDetails_ibfk_2");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PRIMARY");

            entity.ToTable("Branch");

            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BranchName)
                .HasMaxLength(50)
                .HasColumnName("branch_name");
            entity.Property(e => e.Describe)
                .HasMaxLength(255)
                .HasColumnName("describe");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PRIMARY");

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.UserClient, e.ProductId }, "idx_user_client_product_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId)
                .HasMaxLength(18)
                .HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Time)
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10)
                .HasColumnName("unit_price");
            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Cart_ibfk_1");

            entity.HasOne(d => d.UserClientNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserClient)
                .HasConstraintName("Cart_ibfk_2");
        });

        modelBuilder.Entity<ChatbotInteraction>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PRIMARY");

            entity.ToTable("ChatbotInteraction");

            entity.HasIndex(e => new { e.UserClient, e.Time }, "idxChatbotInteraction_user_client_time");

            entity.HasIndex(e => e.UserEmp, "user_emp");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.Response)
                .HasColumnType("text")
                .HasColumnName("response");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.UserClientNavigation).WithMany(p => p.ChatbotInteractions)
                .HasForeignKey(d => d.UserClient)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ChatbotInteraction_ibfk_1");

            entity.HasOne(d => d.UserEmpNavigation).WithMany(p => p.ChatbotInteractions)
                .HasForeignKey(d => d.UserEmp)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ChatbotInteraction_ibfk_2");
        });

        modelBuilder.Entity<Collaborate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Collaborate");

            entity.HasIndex(e => e.BranchId, "FK_Branch_staff");

            entity.HasIndex(e => e.DepId, "dep_id");

            entity.HasIndex(e => e.PositionId, "position_id");

            entity.HasIndex(e => e.UserEmp, "user_emp");

            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.DepId).HasColumnName("dep_id");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");
            entity.Property(e => e.WorkingTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("working_time");

            entity.HasOne(d => d.Branch).WithMany()
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Branch_staff");

            entity.HasOne(d => d.Dep).WithMany()
                .HasForeignKey(d => d.DepId)
                .HasConstraintName("Collaborate_ibfk_2");

            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("Collaborate_ibfk_3");

            entity.HasOne(d => d.UserEmpNavigation).WithMany()
                .HasForeignKey(d => d.UserEmp)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Collaborate_ibfk_1");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.TopicId, "topic_id");

            entity.Property(e => e.Comments)
                .HasMaxLength(255)
                .HasColumnName("comments");
            entity.Property(e => e.Gmail)
                .HasMaxLength(100)
                .HasColumnName("gmail");
            entity.Property(e => e.Sdt)
                .HasMaxLength(100)
                .HasColumnName("sdt");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.TopicId).HasColumnName("topic_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Topic).WithMany()
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Comments_ibfk_1");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.UserClient).HasName("PRIMARY");

            entity.ToTable("Customer");

            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");

            entity.HasOne(d => d.UserClientNavigation).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.UserClient)
                .HasConstraintName("Customer_ibfk_1");
        });

        modelBuilder.Entity<CustomerRoleDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.RankId, "rank_id");

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.HasIndex(e => e.UserClient, "user_client");

            entity.Property(e => e.RankId).HasColumnName("rank_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");

            entity.HasOne(d => d.Rank).WithMany()
                .HasForeignKey(d => d.RankId)
                .HasConstraintName("CustomerRoleDetails_ibfk_3");

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("CustomerRoleDetails_ibfk_2");

            entity.HasOne(d => d.UserClientNavigation).WithMany()
                .HasForeignKey(d => d.UserClient)
                .HasConstraintName("CustomerRoleDetails_ibfk_1");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepId).HasName("PRIMARY");

            entity.ToTable("Department");

            entity.Property(e => e.DepId).HasColumnName("dep_id");
            entity.Property(e => e.DepName)
                .HasMaxLength(50)
                .HasColumnName("dep_name");
            entity.Property(e => e.Infor)
                .HasMaxLength(255)
                .HasColumnName("infor");
        });

        modelBuilder.Entity<DepartmentDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.BranchId, "branch_id");

            entity.HasIndex(e => e.DepId, "dep_id");

            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.DepId).HasColumnName("dep_id");
            entity.Property(e => e.Describe)
                .HasMaxLength(255)
                .HasColumnName("describe");
            entity.Property(e => e.TimeOfCreate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("time_of_create");

            entity.HasOne(d => d.Branch).WithMany()
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("DepartmentDetails_ibfk_1");

            entity.HasOne(d => d.Dep).WithMany()
                .HasForeignKey(d => d.DepId)
                .HasConstraintName("DepartmentDetails_ibfk_2");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Favorite");

            entity.HasIndex(e => new { e.UserClient, e.ProductId }, "idxFavorite_user_client_product_id").IsUnique();

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.ProductId)
                .HasMaxLength(10)
                .HasColumnName("product_id");
            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Favorite_ibfk_2");

            entity.HasOne(d => d.UserClientNavigation).WithMany()
                .HasForeignKey(d => d.UserClient)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Favorite_ibfk_1");
        });

        modelBuilder.Entity<Good>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.HasIndex(e => e.ProductId, "idx_product_id");

            entity.HasIndex(e => e.ProductName, "idx_product_name");

            entity.HasIndex(e => e.ProtyleId, "protyle_id");

            entity.Property(e => e.ProductId)
                .HasMaxLength(10)
                .HasColumnName("product_id");
            entity.Property(e => e.Characteristic)
                .HasMaxLength(255)
                .HasColumnName("characteristic");
            entity.Property(e => e.Details)
                .HasMaxLength(255)
                .HasColumnName("details");
            entity.Property(e => e.NumOfView)
                .HasDefaultValueSql("'0'")
                .HasColumnName("num_of_view");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.ProtyleId).HasColumnName("protyle_id");

            entity.HasOne(d => d.Protyle).WithMany(p => p.Goods)
                .HasForeignKey(d => d.ProtyleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Goods_ibfk_1");
        });

        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.HasKey(e => e.PrId).HasName("PRIMARY");

            entity.ToTable("GoodsReceipt");

            entity.HasIndex(e => e.PsId, "ps_id");

            entity.HasIndex(e => e.SupId, "sup_id");

            entity.HasIndex(e => e.UserEmp, "user_emp");

            entity.Property(e => e.PrId).HasColumnName("pr_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.PsId).HasColumnName("ps_id");
            entity.Property(e => e.SupId).HasColumnName("sup_id");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.TotalImport)
                .HasPrecision(10)
                .HasColumnName("total_import");
            entity.Property(e => e.TotalValueOfReceipt)
                .HasPrecision(10)
                .HasColumnName("total_value_of_receipt");
            entity.Property(e => e.TotalVat)
                .HasPrecision(10)
                .HasColumnName("total_VAT");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.Ps).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.PsId)
                .HasConstraintName("GoodsReceipt_ibfk_3");

            entity.HasOne(d => d.Sup).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.SupId)
                .HasConstraintName("GoodsReceipt_ibfk_1");

            entity.HasOne(d => d.UserEmpNavigation).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.UserEmp)
                .HasConstraintName("GoodsReceipt_ibfk_2");
        });

        modelBuilder.Entity<GoodsReceiptDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.PrId, "pr_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.PrId).HasColumnName("pr_id");
            entity.Property(e => e.ProductId)
                .HasMaxLength(18)
                .HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Pr).WithMany()
                .HasForeignKey(d => d.PrId)
                .HasConstraintName("GoodsReceiptDetails_ibfk_1");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("GoodsReceiptDetails_ibfk_2");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImgId).HasName("PRIMARY");

            entity.Property(e => e.ImgId).HasColumnName("img_id");
            entity.Property(e => e.PathImg)
                .HasMaxLength(100)
                .HasColumnName("path_img");
            entity.Property(e => e.PublicId)
                .HasMaxLength(100)
                .HasColumnName("public_id");
        });

        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PRIMARY");

            entity.ToTable("LoginHistory");

            entity.HasIndex(e => e.UserId, "index_user_id");

            entity.Property(e => e.LoginId).HasColumnName("login_id");
            entity.Property(e => e.Browser)
                .HasMaxLength(50)
                .HasColumnName("browser");
            entity.Property(e => e.Detail)
                .HasMaxLength(255)
                .HasColumnName("detail");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(100)
                .HasColumnName("device_id");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(50)
                .HasColumnName("device_name");
            entity.Property(e => e.Ipv4)
                .HasMaxLength(15)
                .HasColumnName("ipv4");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.LoginMethod)
                .HasMaxLength(20)
                .HasColumnName("login_method");
            entity.Property(e => e.LoginStatus)
                .HasMaxLength(20)
                .HasColumnName("login_status");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("login_time");
            entity.Property(e => e.LogoutTime)
                .HasColumnType("datetime")
                .HasColumnName("logout_time");
            entity.Property(e => e.OsName)
                .HasMaxLength(50)
                .HasColumnName("os_name");
            entity.Property(e => e.Time)
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.UserId)
                .HasMaxLength(18)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.LoginHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoginHistory_User");
        });

        modelBuilder.Entity<MessageBox>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MessageBox");

            entity.HasIndex(e => e.UserClient, "user_client");

            entity.HasIndex(e => e.UserEmp, "user_emp");

            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .HasColumnName("message");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasColumnName("_status");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.UserClientNavigation).WithMany()
                .HasForeignKey(d => d.UserClient)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("MessageBox_ibfk_2");

            entity.HasOne(d => d.UserEmpNavigation).WithMany()
                .HasForeignKey(d => d.UserEmp)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("MessageBox_ibfk_1");
        });

        modelBuilder.Entity<OauthProvider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("PRIMARY");

            entity.ToTable("OAuthProvider");

            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.ClientId)
                .HasMaxLength(100)
                .HasColumnName("client_id");
            entity.Property(e => e.ClientSecret)
                .HasMaxLength(100)
                .HasColumnName("client_secret");
            entity.Property(e => e.Enabled)
                .HasDefaultValueSql("'1'")
                .HasColumnName("enabled");
            entity.Property(e => e.OauthName)
                .HasMaxLength(20)
                .HasColumnName("oauth_name");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PmtId).HasName("PRIMARY");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.PmtId).HasColumnName("pmt_id");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(20)
                .HasColumnName("account_number");
            entity.Property(e => e.PmtName)
                .HasMaxLength(50)
                .HasColumnName("pmt_name");
        });

        modelBuilder.Entity<PaymentSlip>(entity =>
        {
            entity.HasKey(e => e.PsId).HasName("PRIMARY");

            entity.ToTable("PaymentSlip");

            entity.HasIndex(e => e.SupId, "sup_id");

            entity.HasIndex(e => e.UserEmp, "user_emp");

            entity.Property(e => e.PsId).HasColumnName("ps_id");
            entity.Property(e => e.Note)
                .HasMaxLength(15)
                .HasColumnName("note");
            entity.Property(e => e.PaymentAmount)
                .HasPrecision(10)
                .HasColumnName("payment_amount");
            entity.Property(e => e.SupId).HasColumnName("sup_id");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.Sup).WithMany(p => p.PaymentSlips)
                .HasForeignKey(d => d.SupId)
                .HasConstraintName("PaymentSlip_ibfk_1");

            entity.HasOne(d => d.UserEmpNavigation).WithMany(p => p.PaymentSlips)
                .HasForeignKey(d => d.UserEmp)
                .HasConstraintName("PaymentSlip_ibfk_2");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PRIMARY");

            entity.ToTable("Permission");

            entity.HasIndex(e => e.DepId, "dep_id");

            entity.HasIndex(e => e.WebId, "web_id");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.CanAdd).HasColumnName("can_add");
            entity.Property(e => e.CanDelete).HasColumnName("can_delete");
            entity.Property(e => e.CanModify).HasColumnName("can_modify");
            entity.Property(e => e.CanView).HasColumnName("can_view");
            entity.Property(e => e.DepId).HasColumnName("dep_id");
            entity.Property(e => e.Resource)
                .HasMaxLength(50)
                .HasColumnName("resource");
            entity.Property(e => e.WebId).HasColumnName("web_id");

            entity.HasOne(d => d.Dep).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.DepId)
                .HasConstraintName("Permission_ibfk_2");

            entity.HasOne(d => d.Web).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.WebId)
                .HasConstraintName("Permission_ibfk_1");
        });

        modelBuilder.Entity<PositionStaff>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PRIMARY");

            entity.ToTable("PositionStaff");

            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.AllowanceCoefficient)
                .HasDefaultValueSql("'0'")
                .HasColumnName("allowance_coefficient");
            entity.Property(e => e.PositionName)
                .HasMaxLength(50)
                .HasColumnName("position_name");
        });

        modelBuilder.Entity<ProductPhoto>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ProductPhoto");

            entity.HasIndex(e => e.ImgId, "img_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.ImgId).HasColumnName("img_id");
            entity.Property(e => e.ProductId)
                .HasMaxLength(10)
                .HasColumnName("product_id");

            entity.HasOne(d => d.Img).WithMany()
                .HasForeignKey(d => d.ImgId)
                .HasConstraintName("ProductPhoto_ibfk_1");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("ProductPhoto_ibfk_2");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProtyleId).HasName("PRIMARY");

            entity.ToTable("ProductType");

            entity.Property(e => e.ProtyleId).HasColumnName("protyle_id");
            entity.Property(e => e.AliasName)
                .HasMaxLength(50)
                .HasColumnName("alias_name");
            entity.Property(e => e.Details)
                .HasMaxLength(50)
                .HasColumnName("details");
            entity.Property(e => e.ProtyleName)
                .HasMaxLength(50)
                .HasColumnName("protyle_name");
        });

        modelBuilder.Entity<ProductTypeDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.ProtyleId, "protyle_id");

            entity.HasIndex(e => e.SupId, "sup_id");

            entity.Property(e => e.ProtyleId).HasColumnName("protyle_id");
            entity.Property(e => e.SupId).HasColumnName("sup_id");

            entity.HasOne(d => d.Protyle).WithMany()
                .HasForeignKey(d => d.ProtyleId)
                .HasConstraintName("ProductTypeDetails_ibfk_1");

            entity.HasOne(d => d.Sup).WithMany()
                .HasForeignKey(d => d.SupId)
                .HasConstraintName("ProductTypeDetails_ibfk_2");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromoId).HasName("PRIMARY");

            entity.ToTable("Promotion");

            entity.Property(e => e.PromoId).HasColumnName("promo_id");
            entity.Property(e => e.Discount)
                .HasDefaultValueSql("'0'")
                .HasColumnName("discount");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.PromoName)
                .HasMaxLength(50)
                .HasColumnName("promo_name");
            entity.Property(e => e.StartTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("start_time");
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.RankId).HasName("PRIMARY");

            entity.ToTable("Rank");

            entity.Property(e => e.RankId).HasColumnName("rank_id");
            entity.Property(e => e.Describe)
                .HasMaxLength(255)
                .HasColumnName("describe");
            entity.Property(e => e.RankName)
                .HasMaxLength(50)
                .HasColumnName("rank_name");
            entity.Property(e => e.RatingPoint)
                .HasColumnName("rating_point")
                .HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RefreshToken");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Expired)
                .HasColumnType("datetime")
                .HasColumnName("expired");
            entity.Property(e => e.IsRevoked)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_revoked");
            entity.Property(e => e.IsSureAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("is_sure_at");
            entity.Property(e => e.IsUse)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_use");
            entity.Property(e => e.Jwt)
                .HasMaxLength(255)
                .HasColumnName("jwt");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UserId)
                .HasMaxLength(18)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("RefreshToken_ibfk_1");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ProductId, "Reviews_ibfk_2");

            entity.HasIndex(e => new { e.UserClient, e.ProductId }, "idxReviews_user_client_product_id");

            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.ProductId)
                .HasMaxLength(18)
                .HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("_time");
            entity.Property(e => e.UserClient)
                .HasMaxLength(18)
                .HasColumnName("user_client");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Reviews_ibfk_2");

            entity.HasOne(d => d.UserClientNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserClient)
                .HasConstraintName("Reviews_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Describe)
                .HasMaxLength(100)
                .HasColumnName("describe");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PRIMARY");

            entity.ToTable("RolePermission");

            entity.HasIndex(e => e.PermissionId, "idx_role_permission_permission_id");

            entity.HasIndex(e => e.RoleId, "idx_role_permission_role_id");

            entity.Property(e => e.RolePermissionId).HasColumnName("role_permission_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("RolePermission_ibfk_2");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("RolePermission_ibfk_1");
        });

        modelBuilder.Entity<SalesPrice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SalesPrice");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => e.PromoId, "promo_id");

            entity.Property(e => e.NumOfProduct).HasColumnName("num_of_product");
            entity.Property(e => e.Price)
                .HasPrecision(10)
                .HasColumnName("price");
            entity.Property(e => e.ProductId)
                .HasMaxLength(10)
                .HasColumnName("product_id");
            entity.Property(e => e.PromoId).HasColumnName("promo_id");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("_time");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("SalesPrice_ibfk_1");

            entity.HasOne(d => d.Promo).WithMany()
                .HasForeignKey(d => d.PromoId)
                .HasConstraintName("SalesPrice_ibfk_2");
        });

        modelBuilder.Entity<ShippingBill>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ShippingBill");

            entity.HasIndex(e => e.BillId, "bill_id");

            entity.HasIndex(e => e.StatusId, "status_id");

            entity.Property(e => e.BillId)
                .HasMaxLength(15)
                .HasColumnName("bill_id");
            entity.Property(e => e.CurrentLocation)
                .HasMaxLength(255)
                .HasColumnName("CURRENT_location");
            entity.Property(e => e.Destination)
                .HasMaxLength(255)
                .HasColumnName("destination");
            entity.Property(e => e.Fee)
                .HasPrecision(10)
                .HasColumnName("fee");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Time)
                .HasColumnType("datetime")
                .HasColumnName("_time");

            entity.HasOne(d => d.Bill).WithMany()
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("ShippingBill_ibfk_2");

            entity.HasOne(d => d.Status).WithMany()
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("ShippingBill_ibfk_1");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.UserEmp).HasName("PRIMARY");

            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.UserEmpNavigation).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.UserEmp)
                .HasConstraintName("Staff_ibfk_1");
        });

        modelBuilder.Entity<StaffRoleDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.HasIndex(e => e.UserEmp, "user_emp");

            entity.Property(e => e.Describe)
                .HasMaxLength(255)
                .HasColumnName("describe");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("StaffRoleDetails_ibfk_2");

            entity.HasOne(d => d.UserEmpNavigation).WithMany()
                .HasForeignKey(d => d.UserEmp)
                .HasConstraintName("StaffRoleDetails_ibfk_1");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Detail)
                .HasMaxLength(255)
                .HasColumnName("detail");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupId).HasName("PRIMARY");

            entity.ToTable("Supplier");

            entity.Property(e => e.SupId).HasColumnName("sup_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.Detail)
                .HasMaxLength(255)
                .HasColumnName("detail");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.PhoneNum)
                .HasMaxLength(10)
                .HasColumnName("phone_num");
            entity.Property(e => e.SupName)
                .HasMaxLength(50)
                .HasColumnName("sup_name");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(20)
                .HasColumnName("tax_code");
        });

        modelBuilder.Entity<SupplierLogo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SupplierLogo");

            entity.HasIndex(e => e.ImgId, "img_id");

            entity.HasIndex(e => e.SupId, "sup_id");

            entity.Property(e => e.ImgId).HasColumnName("img_id");
            entity.Property(e => e.SupId).HasColumnName("sup_id");

            entity.HasOne(d => d.Img).WithMany()
                .HasForeignKey(d => d.ImgId)
                .HasConstraintName("SupplierLogo_ibfk_1");

            entity.HasOne(d => d.Sup).WithMany()
                .HasForeignKey(d => d.SupId)
                .HasConstraintName("SupplierLogo_ibfk_2");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PRIMARY");

            entity.ToTable("Topic");

            entity.HasIndex(e => e.UserEmp, "user_emp");

            entity.Property(e => e.TopicId).HasColumnName("topic_id");
            entity.Property(e => e.TopicName)
                .HasMaxLength(50)
                .HasColumnName("topic_name");
            entity.Property(e => e.UserEmp)
                .HasMaxLength(18)
                .HasColumnName("user_emp");

            entity.HasOne(d => d.UserEmpNavigation).WithMany(p => p.Topics)
                .HasForeignKey(d => d.UserEmp)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Topic_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.PhoneNum, "idx_phone_num").IsUnique();

            entity.HasIndex(e => e.UserName, "idx_user_name");

            entity.Property(e => e.UserId)
                .HasMaxLength(18)
                .HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email")
                .HasDefaultValueSql("'null'");
            entity.Property(e => e.IsBlock)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_block");
            entity.Property(e => e.IsDelete)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_delete");
            entity.Property(e => e.PassWord)
                .HasMaxLength(255)
                .HasColumnName("pass_word");
            entity.Property(e => e.PhoneNum)
                .HasMaxLength(10)
                .HasColumnName("phone_num")
                .HasDefaultValueSql("'null'");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<UserOauth>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserOauth");

            entity.HasIndex(e => e.ExternalId, "external_id").IsUnique();

            entity.HasIndex(e => e.ProviderId, "idx_user_oauth_provider_id");

            entity.HasIndex(e => e.UserId, "idx_user_oauth_user_id");

            entity.Property(e => e.AccessToken)
                .HasMaxLength(255)
                .HasColumnName("access_token");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("expiry_date");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(100)
                .HasColumnName("external_id");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .HasColumnName("refresh_token");
            entity.Property(e => e.UserId)
                .HasMaxLength(18)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Provider).WithMany()
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("UserOAuth_ibfk_2");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserOAuth_ibfk_1");
        });

        modelBuilder.Entity<UserPhotoDetail>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.ImgId, "img_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.ImgId).HasColumnName("img_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(18)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Img).WithMany()
                .HasForeignKey(d => d.ImgId)
                .HasConstraintName("UserPhotoDetails_ibfk_2");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserPhotoDetails_ibfk_1");
        });

        modelBuilder.Entity<WebSite>(entity =>
        {
            entity.HasKey(e => e.WebId).HasName("PRIMARY");

            entity.ToTable("WebSite");

            entity.Property(e => e.WebId).HasColumnName("web_id");
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .HasColumnName("url");
            entity.Property(e => e.WebName)
                .HasMaxLength(50)
                .HasColumnName("web_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
