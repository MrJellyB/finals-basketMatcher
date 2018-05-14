using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Basket.Common.Data
{
    public class CategoryDTO
    {
        [BsonId]
        public int _id { get; set; }

        public string name { get; set; }

        public int id { get; set; }
    }
}
