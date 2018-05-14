using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Basket.Common.Data
{
    public class StoreDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string Subchainid { get; set; }

        public string Storeid { get; set; }

        public string Bikoretno { get; set; }

        public string Storetype { get; set; }

        public string Chainname { get; set; }

        public string Subchainname { get; set; }

        public string Storename { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Zipcode { get; set; }
    }
}
