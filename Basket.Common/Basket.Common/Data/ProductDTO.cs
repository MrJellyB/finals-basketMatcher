using Basket.Common.Attribute;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Basket.Common.Data
{
    public class ProductDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        
        public string PriceUpdateDate { get; set; }
        
        public long id { get; set; }
        
        public int ItemType { get; set; }
        
        public string name { get; set; }
        
        public string company { get; set; }
        
        public string createCountry { get; set; }
        
        public string ManufacturerItemDescription { get; set; }
        
        public string UnitQty { get; set; }
        
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float Quantity { get; set; }

        public int bIsWeighted { get; set; }

        public int? calories { get; set; }
        
        public string UnitOfMeasure { get; set; }
        
        public int QtyInPackage { get; set; }
        
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float price { get; set; }
        
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float UnitOfMeasurePrice { get; set; }
        
        public int AllowDiscount { get; set; }
        
        public int ItemStatus { get; set; }
        
        public int category { get; set; }

        public string categoryValue { get; set; }

        [BsonElement]
        public IList<OldPriceDTO> oldPriceArray { get; set; }

        [BsonElement]
        public IList<CommentsDTO> comments { get; set; }

        public bool VeganFriendly { get; set; }

        [GlutenFree]
        public bool GlutenFree { get; set; }

        [Organic]
        public bool Organic { get; set; }
        
        public string Kashrut { get; set; }

        [Price]
        public float ControlledPrice { get; set; }

        public string EfshariBari { get; set; }
    }

    public class OldPriceDTO
    {
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public double? curr { get; set; }

        [BsonRepresentation(BsonType.DateTime, AllowTruncation = true)]
        public DateTime createdTime { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
    }

    public class CommentsDTO
    {
        public string comment { get; set; }
        
        public int grade { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
    }
}
