namespace BulkUploadPOC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SalesRecords_Test")]
    public partial class SalesRecords_Test
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Region { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(50)]
        public string Item_Type { get; set; }

        [Required]
        [StringLength(50)]
        public string Sales_Channel { get; set; }

        [Required]
        [StringLength(50)]
        public string Order_Priority { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Order_Date { get; set; }

        public int Order_ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Ship_Date { get; set; }

        public int Units_Sold { get; set; }

        public decimal Unit_Price { get; set; }

        public decimal Unit_Cost { get; set; }

        public decimal Total_Revenue { get; set; }

        public decimal Total_Cost { get; set; }

        public decimal Total_Profit { get; set; }
    }
}
