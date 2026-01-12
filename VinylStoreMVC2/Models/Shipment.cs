using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность поставки виниловых пластинок от поставщика в систему магазина.
    /// Хранит информацию о поступлениях товара, включая дату и привязку к накладной.
    /// </summary>
    [Table("shipments")]
    public class Shipment
    {
        /// <summary>
        /// Задаёт id поставки.
        /// </summary>
        /// <value>Целочисленный идентификатор, автоматически генерируемый базой данных.</value>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Задаёт id поставщика, осуществившего данную поставку.
        /// </summary>
        /// <value>Целочисленный идентификатор поставщика.</value>
        [Required]
        [Column("supplier_id")]
        [Display(Name = "Поставщик")]
        public int SupplierId { get; set; }

        /// <summary>
        /// Задаёт объект поставщика, связанного с данной поставкой.
        /// </summary>
        /// <value>Объект <see cref="Supplier"/>, представляющий компанию-поставщика.</value>
        [Display(Name = "Поставщик")]
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        /// <summary>
        /// Задаёт дату осуществления поставки.
        /// </summary>
        /// <value>Объект <see cref="DateOnly"/>, представляющий дату поставки.</value>
        [Required]
        [Column("date")]
        [Display(Name = "Дата поставки")]
        public DateOnly Date {  get; set; }

        /// <summary>
        /// Задаёт ссылку на товарную накладную.
        /// </summary>
        /// <value>Строка длиной до 255 символов, содержащая путь к документу.</value>
        [Required]
        [Column("invoice_link")]
        [StringLength(255)]
        [Display(Name = "Ссылка на накладную")]
        public string InvoiceLink { get; set; }

        /// <summary>
        /// Задаёт список идентификаторов пластинок, поступивших в рамках данной поставки.
        /// </summary>
        /// <value>Список целочисленных идентификаторов пластинок.</value>
        [NotMapped]
        public List<int> RecordIds { get; set; } = [];

        /// <summary>
        /// Задаёт коллекцию пластинок, поступивших в рамках данной поставки.
        /// </summary>
        /// <value>Список объектов <see cref="Record"/>, представляющих поставленные виниловые пластинки.</value>
        [Display(Name = "Пластинки")]
        public List<Record> Records { get; set; } = [];
    }
}
