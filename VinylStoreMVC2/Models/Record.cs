using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность виниловой пластинки в системе магазина.
    /// Хранит полную информацию о музыкальной записи, включая метаданные, цены и наличие.
    /// </summary>
    [Table("records")]
    public class Record
    {
        /// <summary>
        /// Задаёт id пластинки.
        /// </summary>
        /// <value>Целочисленный идентификатор, автоматически генерируемый базой данных.</value>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Задаёт название виниловой пластинки.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая название альбома.</value>
        [Column("name")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Название пластинки")]
        public string Name { get; set; }

        /// <summary>
        /// Задаёт id исполнителя, связанного с пластинкой.
        /// </summary>
        /// <value>Целочисленный идентификатор исполнителя.</value>
        [Column("artist_id")]
        [Required]
        [Display(Name = "Исполнитель")]
        public int ArtistId { get; set; }

        /// <summary>
        /// Задаёт объект исполнителя, связанного с пластинкой.
        /// </summary>
        /// <value>Объект <see cref="Artist"/>, представляющий исполнителя пластинки.</value>
        [Display(Name = "Исполнитель")]
        [ForeignKey("ArtistId")]
        public Artist? Artist { get; set; }

        /// <summary>
        /// Задаёт год выпуска пластинки.
        /// </summary>
        /// <value>Целое число в диапазоне от 1900 до 2100, представляющее год выпуска.</value>
        [Column("year")]
        [Range(1900, 2100)]
        [Display(Name = "Год выпуска")]
        public int Year { get; set; }

        /// <summary>
        /// Задаёт название лейбла, выпустившего пластинку.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая название лейбла.</value>
        [Column("label")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Лейбл")]
        public string Label { get; set; }

        /// <summary>
        /// Задаёт каталожный номер пластинки.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая уникальный каталожный номер.</value>
        [Column("catalog_number")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Каталожный номер")]
        public string CatalogNumber { get; set; }

        /// <summary>
        /// Задаёт текущую цену пластинки в рублях.
        /// </summary>
        /// <value>Неотрицательное целое число, представляющее цену в рублях.</value>
        [Column("current_price")]
        [Required]
        [Display(Name = "Текущая цена")]
        [Range(0, int.MaxValue)]
        public int CurrentPrice { get; set; }

        /// <summary>
        /// Задаёт количество экземпляров пластинки на складе.
        /// </summary>
        /// <value>Неотрицательное целое число, представляющее количество единиц товара на складе.</value>
        [Column("stock_quantity")]
        [Display(Name = "Количество на складе")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        /// <summary>
        /// Задаёт идентификатор статуса наличия пластинки.
        /// </summary>
        /// <value>Целочисленный идентификатор статуса (по умолчанию 2 - "Нет в наличии").</value>
        [Required]
        [Column("status_id")]
        [Display(Name = "Статус")]
        public int StatusId { get; set; } = 2;

        /// <summary>
        /// Задаёт объект статуса, связанный с пластинкой.
        /// </summary>
        /// <value>Объект <see cref="Status"/>, представляющий текущий статус наличия пластинки.</value>
        [Display(Name = "Статус")]
        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        /// <summary>
        /// Задаёт список идентификаторов жанров, к которым относится пластинка.
        /// </summary>
        /// <value>Список целочисленных идентификаторов жанров.</value>
        [NotMapped]
        public List<int> GenreIds { get; set; } = [];

        /// <summary>
        /// Задаёт коллекцию жанров, к которым относится пластинка.
        /// </summary>
        /// <value>Список объектов <see cref="Genre"/>, представляющих музыкальные жанры пластинки.</value>
        [Display(Name = "Жанры")]
        public List<Genre> Genres { get; set; } = [];

        /// <summary>
        /// Задаёт список идентификаторов поставок, в которые входила данная пластинка.
        /// </summary>
        /// <value>Список целочисленных идентификаторов поставок.</value>
        [NotMapped]
        public List<int> ShipmentIds { get; set; } = [];

        /// <summary>
        /// Задаёт коллекцию поставок, в которые входила данная пластинка.
        /// </summary>
        /// <value>Список объектов <see cref="Shipment"/>, представляющих поставки пластинки.</value>
        public List<Shipment> Shipments { get; set; } = [];

        /// <summary>
        /// Задаёт список идентификаторов продаж, в которые входила данная пластинка.
        /// </summary>
        /// <value>Список целочисленных идентификаторов продаж.</value>
        [NotMapped]
        public List<int> SaleIds { get; set; } = [];

        /// <summary>
        /// Задаёт коллекцию продаж, в которые входила данная пластинка.
        /// </summary>
        /// <value>Список объектов <see cref="Sale"/>, представляющих продажи пластинки.</value>
        public List<Sale> Sales { get; set; } = [];
    }
}
