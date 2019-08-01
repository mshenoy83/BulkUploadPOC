using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

using Dapper.Contrib.Extensions;

namespace BulkUploadPOC
{


    [Dapper.Contrib.Extensions.Table("BulkTest")]
    public partial class BulkTest
    {
        [Dapper.Contrib.Extensions.Key]
        [StringLength(50)]
        public string Region { get; set; }

     
        [StringLength(50)]
        public string Country { get; set; }

       
        [StringLength(50)]
        public string Item_Type { get; set; }

       
        [StringLength(50)]
        public string Sales_Channel { get; set; }

    
        [StringLength(50)]
        public string Order_Priority { get; set; }


        public DateTime Order_Date { get; set; }

     
        public int Order_ID { get; set; }

    
        public DateTime Ship_Date { get; set; }

    
        public int Units_Sold { get; set; }

    
        public decimal Unit_Price { get; set; }

      
        public decimal Unit_Cost { get; set; }

    
        public decimal Total_Revenue { get; set; }

    
        public decimal Total_Cost { get; set; }

     
        public decimal Total_Profit { get; set; }
    }
}
