using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Data
{
    /// <summary>
    /// Контекст базы данных для приложения магазина виниловых пластинок.
    /// Определяет наборы сущностей и их отношения для хранения данных о музыке, продажах и поставках.
    /// </summary>
    public class ApplicationContext : DbContext
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ApplicationContext"/> с указанными параметрами.
        /// </summary>
        /// <param name="options">Параметры конфигурации контекста базы данных.</param>
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Задаёт набор сущностей исполнителей.
        /// </summary>
        public DbSet<Artist> Artists { get; set; }
        /// <summary>
        /// Задаёт набор сущностей музыкальных жанров.
        /// </summary>
        public DbSet<Genre> Genres { get; set; }
        /// <summary>
        /// Задаёт набор сущностей виниловых пластинок.
        /// </summary>
        public DbSet<Record> Records { get; set; }
        /// <summary>
        /// Задаёт набор сущностей статусов наличия пластинок.
        /// </summary>
        public DbSet<Status> Statuses { get; set; }
        /// <summary>
        /// Задаёт набор сущностей для связи многие-ко-многим между пластинками и жанрами.
        /// </summary>
        public DbSet<RecordGenre> RecordGenres { get; set; }
        /// <summary>
        /// Задаёт набор сущностей поставщиков пластинок.
        /// </summary>
        public DbSet<Supplier> Suppliers { get; set; }
        /// <summary>
        /// Задаёт набор сущностей поставок от поставщиков.
        /// </summary>
        public DbSet<Shipment> Shipments { get; set; }
        /// <summary>
        /// Задаёт набор сущностей для связи многие-ко-многим между поставками и пластинками.
        /// Хранит информацию о количестве пластинок в каждой поставке.
        /// </summary>
        public DbSet<ShipmentRecord> ShipmentRecords { get; set; }
        /// <summary>
        /// Задаёт набор сущностей продаж пластинок.
        /// </summary>
        public DbSet<Sale> Sales { get; set; }
        /// <summary>
        /// Задаёт набор сущностей для связи многие-ко-многим между продажами и пластинками.
        /// Хранит информацию о количестве и цене проданных пластинок.
        /// </summary>
        public DbSet<SaleRecord> SaleRecords { get; set; }

        /// <summary>
        /// Настраивает модель базы данных, включая отношения между сущностями и ограничения целостности.
        /// </summary>
        /// <param name="modelBuilder">Построитель модели для настройки сущностей и их отношений.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка отношения многие-ко-многим между пластинками и жанрами
            modelBuilder.Entity<Record>()
                .HasMany(e => e.Genres)
                .WithMany(e => e.Records)
                .UsingEntity<RecordGenre>(
                r => r.HasOne<Genre>().WithMany().HasForeignKey(e => e.GenreId).OnDelete(DeleteBehavior.Cascade),
                l => l.HasOne<Record>().WithMany().HasForeignKey(e => e.RecordId).OnDelete(DeleteBehavior.Cascade));

            // Настройка отношения многие-ко-многим между поставками и пластинками
            modelBuilder.Entity<Shipment>()
                .HasMany(e => e.Records)
                .WithMany(e => e.Shipments)
                .UsingEntity<ShipmentRecord>(
                r => r.HasOne<Record>().WithMany().HasForeignKey(e => e.RecordId).OnDelete(DeleteBehavior.Cascade),
                l => l.HasOne<Shipment>().WithMany().HasForeignKey(e => e.ShipmentId).OnDelete(DeleteBehavior.Cascade));

            // Настройка отношения многие-ко-многим между продажами и пластинками
            modelBuilder.Entity<Sale>()
                .HasMany(e => e.Records)
                .WithMany(e => e.Sales)
                .UsingEntity<SaleRecord>(
                 r => r.HasOne<Record>().WithMany().HasForeignKey(e => e.RecordId).OnDelete(DeleteBehavior.Cascade),
                l => l.HasOne<Sale>().WithMany().HasForeignKey(e => e.SaleId).OnDelete(DeleteBehavior.Cascade));
        }
    }
}
