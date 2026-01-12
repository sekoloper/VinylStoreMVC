using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность музыкального жанра в системе магазина виниловых пластинок.
    /// </summary>
    [Table("genres")]
    public class Genre
    {
        /// <summary>
        /// Задаёт id жанра.
        /// </summary>
        /// <value>Целочисленный идентификатор, автоматически генерируемый базой данных.</value>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Задаёт название музыкального жанра.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая название жанра (например, "Рок", "Джаз", "Классика").</value>
        [Column("name")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Название жанра")]
        public string Name { get; set; }

        /// <summary>
        /// Задаёт коллекцию виниловых пластинок, принадлежащих к данному жанру.
        /// Представляет навигационное свойство для связи многие-ко-многим с сущностью Record.
        /// </summary>
        /// <value>Список объектов <see cref="Record"/>, связанных с данным жанром.</value>
        public List<Record> Records { get; set; } = [];
    }
}
