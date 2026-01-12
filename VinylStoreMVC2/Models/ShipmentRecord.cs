using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность связи между поставками и виниловыми пластинками.
    /// Используется для реализации отношения многие-ко-многим между Shipment и Record.
    /// Хранит информацию о количестве каждой пластинки в конкретной поставке.
    /// </summary>
    [Table("shipment_records")]
    public class ShipmentRecord
    {
        /// <summary>
        /// Задаёт id, к которой относится запись.
        /// </summary>
        /// <value>Целочисленный идентификатор поставки.</value>
        [Column("shipment_id")]
        public int ShipmentId { get; set; }

        /// <summary>
        /// Задаёт id виниловой пластинки, поставленной в рамках данной поставки.
        /// </summary>
        /// <value>Целочисленный идентификатор пластинки.</value>
        [Column("record_id")]
        public int RecordId { get; set; }

        /// <summary>
        /// Задаёт количество экземпляров пластинки, поставленных в данной поставке.
        /// </summary>
        /// <value>Положительное целое число, представляющее количество поставленных единиц.</value>
        [Column("quantity")]
        public int Quantity { get; set; }
    }
}
