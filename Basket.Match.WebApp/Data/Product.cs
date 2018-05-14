using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Product
    {
        public string PriceUpdateDate { get; set; }
        public long id { get; set; }
        public int ItemType { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        public string createCountry { get; set; }
        public string ManufacturerItemDescription { get; set; }
        public string UnitQty { get; set; }
        public float Quantity { get; set; }
        public int bIsWeighted { get; set; }
        public string UnitOfMeasure { get; set; }
        public int QtyInPackage { get; set; }
        public float price { get; set; }
        public float UnitOfMeasurePrice { get; set; }
        public int AllowDiscount { get; set; }
        public int ItemStatus { get; set; }
        public int category { get; set; }
    }
}
