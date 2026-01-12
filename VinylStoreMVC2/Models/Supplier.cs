using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность поставщика виниловых пластинок в системе магазина.
    /// Хранит информацию о компаниях и контактных лицах, поставляющих товары.
    /// </summary>
    [Table("suppliers")]
    public class Supplier
    {
        /// <summary>
        /// Задаёт id поставщика.
        /// </summary>
        /// <value>Целочисленный идентификатор, автоматически генерируемый базой данных.</value>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Задаёт название компании-поставщика.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая наименование поставщика.</value>
        [Column("name")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Наименование поставщика")]
        public string Name { get; set; }

        /// <summary>
        /// Задаёт имя контактного лица в компании-поставщике.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая имя контактного лица.</value>
        [Column("contact_person")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Контактное лицо")]
        public string ContactPerson { get; set; }

        /// <summary>
        /// Задаёт номер телефона для связи с поставщиком.
        /// </summary>
        /// <value>Строка длиной до 100 символов, содержащая номер телефона.</value>
        [Column("phone_number")]
        [Required]
        [StringLength(100)]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Задаёт список поставок, осуществлённых данным поставщиком.
        /// Представляет навигационное свойство для связи один-ко-многим с сущностью Shipment.
        /// </summary>
        /// <value>Список объектов <see cref="Shipment"/>, связанных с данным поставщиком.</value>
        public List<Shipment> Shipments { get; set; } = new();
    }
}
