using System.ComponentModel.DataAnnotations.Schema;

namespace VinylStoreMVC.Models
{
    /// <summary>
    /// Представляет сущность связи между продажами и виниловыми пластинками.
    /// Используется для реализации отношения многие-ко-многим между Sale и Record.
    /// Хранит информацию о количестве и цене каждой пластинки в конкретной продаже.
    /// </summary>
    [Table("sale_records")]
    public class SaleRecord
    {
        /// <summary>
        /// Задаёт id продажи, к которой относится запись.
        /// </summary>
        /// <value>Целочисленный идентификатор продажи.</value>
        [Column("sale_id")]
        public int SaleId { get; set; }

        /// <summary>
        /// Задаёт id виниловой пластинки, проданной в рамках данной продажи.
        /// </summary>
        /// <value>Целочисленный идентификатор пластинки.</value>
        [Column("record_id")]
        public int RecordId { get; set; }

        /// <summary>
        /// Задаёт количество экземпляров пластинки, проданных в данной продаже.
        /// </summary>
        /// <value>Положительное целое число, представляющее количество проданных единиц.</value>
        [Column("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Задаёт цену за один экземпляр пластинки на момент продажи.
        /// </summary>
        /// <value>Целое число, представляющее цену в рублях.</value>
        [Column("price")]
        public int Price { get; set; }
    }
}
