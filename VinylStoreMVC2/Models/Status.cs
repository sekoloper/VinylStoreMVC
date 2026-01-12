using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность статуса наличия виниловых пластинок в системе магазина.
    /// Хранит информацию о доступности товара.
    /// </summary>
    [Table("statuses")]
    public class Status
    {
        /// <summary>
        /// Задаёт id статуса.
        /// </summary>
        /// <value>Целочисленный идентификатор, автоматически генерируемый базой данных.</value>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Задаёт название статуса наличия товара.
        /// </summary>
        /// <value>Строка длиной до 12 символов, содержащая название статуса.</value>
        [Required]
        [Column("name")]
        [StringLength(12)]
        [Display(Name = "Статус")]
        public string Name { get; set; }

        /// <summary>
        /// Задаёт список виниловых пластинок, имеющих данный статус.
        /// Представляет навигационное свойство для связи один-ко-многим с сущностью Record.
        /// </summary>
        /// <value>Список объектов <see cref="Record"/>, связанных с данным статусом.</value>
        public List<Record> Records { get; set; } = new();
    }
}
