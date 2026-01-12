// Models/RecordGenre.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность связи между виниловыми пластинками и музыкальными жанрами.
    /// Используется для реализации отношения многие-ко-многим между Record и Genre.
    /// </summary>
    [Table("record_genres")]
    public class RecordGenre
    {
        /// <summary>
        /// Задаёт id виниловой пластинки, связанной с жанром.
        /// </summary>
        /// <value>Целочисленный идентификатор пластинки.</value>
        [Column("record_id")]
        public int RecordId { get; set; }

        /// <summary>
        /// Задаёт id музыкального жанра, связанного с пластинкой.
        /// </summary>
        /// <value>Целочисленный идентификатор жанра.</value>
        [Column("genre_id")]
        public int GenreId { get; set; }
    }
}