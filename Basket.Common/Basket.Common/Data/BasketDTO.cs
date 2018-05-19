using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Basket.Common.Data
{
    [KnownType(typeof(BasketItemsDTO))]
    public class BasketDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement]
        //[BsonIgnore]
        public IList<BasketItemsDTO> basketItems { get; set; }

        [BsonRepresentation(BsonType.DateTime, AllowTruncation = true)]
        public DateTime createdTime { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public double totalPrice { get; set; }

        public string streetName { get; set; }

        public string userName { get; set; }

        public int id { get; set; }

        public int GetBasketItemCount()
        {
            if (this.basketItems != null)
            {
                return this.basketItems.Count;
            }
            return 0;
        }
    }

    public class BasketItemsDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonRepresentation(BsonType.Int64, AllowTruncation = true)]
        public long id { get; set; }
        
        public string name { get; set; }
        
        public string image { get; set; }
        
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float price { get; set; }
        
        public int amount { get; set; }
        
    }
}
