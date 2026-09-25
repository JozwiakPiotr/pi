using Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Desk> Desks => Set<Desk>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Asset> Assets => Set<Asset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var room = modelBuilder.Entity<Room>();
        room.ToTable("rooms");
        room.HasKey(x => x.Id);
        room.HasIndex(x => new { x.Number, x.Level }).IsUnique();
        room.Property(x => x.Number)
            .HasMaxLength(32)
            .IsRequired();
        room.Property(x => x.Level)
            .IsRequired();

        var feature = modelBuilder.Entity<Feature>();
        feature.ToTable("features");
        feature.HasKey(x => x.Id);
        feature.HasIndex(x => x.Name).IsUnique();
        feature.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        var asset = modelBuilder.Entity<Asset>();
        asset.ToTable("assets");
        asset.HasKey(x => x.Id);
        asset.HasIndex(x => x.Name).IsUnique();
        asset.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        var desk = modelBuilder.Entity<Desk>();
        desk.ToTable("desks");
        desk.HasKey(x => x.Id);
        desk.HasIndex(x => new { x.Number, x.RoomId }).IsUnique();

        desk.Property(x => x.Number)
            .HasMaxLength(32)
            .IsRequired();

        desk.HasOne(x => x.Room)
            .WithMany(x => x.Desks)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        desk.Property(x => x.State)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        desk.Property(x => x.CreatedAt)
            .IsRequired();

        desk.Property(x => x.UpdatedAt);

        desk.Navigation(x => x.Features)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        desk.Navigation(x => x.Assets)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        var deskFeature = modelBuilder.Entity<DeskFeature>();
        deskFeature.ToTable("desk_features");
        deskFeature.HasKey(x => new { x.DeskId, x.FeatureId });
        deskFeature.HasOne(x => x.Desk)
            .WithMany(x => x.Features)
            .HasForeignKey(x => x.DeskId)
            .OnDelete(DeleteBehavior.Cascade);
        deskFeature.HasOne(x => x.Feature)
            .WithMany(x => x.Desks)
            .HasForeignKey(x => x.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);

        var deskAsset = modelBuilder.Entity<DeskAsset>();
        deskAsset.ToTable("desk_assets");
        deskAsset.HasKey(x => new { x.DeskId, x.AssetId });
        deskAsset.Property(x => x.Count)
            .IsRequired();
        deskAsset.HasOne(x => x.Desk)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.DeskId)
            .OnDelete(DeleteBehavior.Cascade);
        deskAsset.HasOne(x => x.Asset)
            .WithMany(x => x.Desks)
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}