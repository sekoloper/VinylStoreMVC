using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность исполнителя в системе магазина виниловых пластинок.
    /// </summary>
    [Table("artists")]
    public class Artist
    {
        /// <summary>
        /// Задаёт id исполнителя.
        /// </summary>
        /// <value>Целочисленный идентификатор, автоматически генерируемый базой данных.</value>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Задаёт имя исполнителя.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая имя исполнителя.</value>
        [Column("name")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Название исполнителя")]
        public string Name { get; set; }

        /// <summary>
        /// Задаёт коллекцию виниловых пластинок, принадлежащих данному исполнителю.
        /// Представляет навигационное свойство для связи один-ко-многим с сущностью Record.
        /// </summary>
        /// <value>Список объектов <see cref="Record"/>, связанных с данным исполнителем.</value>
        [NotMapped]
        public List<Record> Records { get; set; } = new();
    }
}
