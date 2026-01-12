using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность продажи виниловых пластинок в системе магазина.
    /// Хранит информацию о фактах продажи, включая дату и привязку к кассовому чеку.
    /// </summary>
    [Table("sales")]
    public class Sale
    {
        /// <summary>
        /// Задаёт id продажи.
        /// </summary>
        /// <value>Целочисленный идентификатор, автоматически генерируемый базой данных.</value>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Задаёт дату совершения продажи.
        /// </summary>
        /// <value>Объект <see cref="DateOnly"/>, представляющий дату продажи.</value>
        [Required]
        [Column("date")]
        [Display(Name = "Дата продажи")]
        public DateOnly Date { get; set; }

        /// <summary>
        /// Задаёт ссылку на кассовый чек или электронный документ, подтверждающий продажу.
        /// </summary>
        /// <value>Строка длиной до 255 символов, содержащая путь к документу.</value>
        [Required]
        [Column("receipt_link")]
        [StringLength(255)]
        [Display(Name = "Ссылка на чек")]
        public string InvoiceLink { get; set; }

        /// <summary>
        /// Задаёт список id пластинок, проданных в рамках данной продажи.
        /// </summary>
        /// <value>Список целочисленных идентификаторов пластинок.</value>
        [NotMapped]
        public List<int> RecordIds { get; set; } = [];

        /// <summary>
        /// Задаёт коллекцию пластинок, проданных в рамках данной продажи.
        /// </summary>
        /// <value>Список объектов <see cref="Record"/>, представляющих проданные виниловые пластинки.</value>
        [Display(Name = "Пластинки")]
        public List<Record> Records { get; set; } = [];
    }
}
