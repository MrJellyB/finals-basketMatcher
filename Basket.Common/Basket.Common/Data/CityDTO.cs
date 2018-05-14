using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Basket.Common.Data
{
    // clas
    public class CityDTO
    {
        [BsonId]
        public int _id { get; set; }

        public string cityName { get; set; }
    }
}
