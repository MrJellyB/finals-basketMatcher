using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Basket.Common.Data
{
    public class UserDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public int gender { get; set; }

        public string password { get; set; }

        public string userName { get; set; }

        public int userType { get; set; }

        [BsonElement]
        public ProfileDTO profile { get; set; }
    }

    public class ProfileDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonRepresentation(BsonType.DateTime, AllowTruncation = true)]
        public DateTime birthdate { get; set; }

        [BsonElement]
        public AddressDTO address { get; set; }

        [BsonElement]
        public PeopleAmountDTO peopleAmount { get; set; }

        [BsonElement]
        public PreferencesDTO preferences { get; set; }

        [BsonElement]
        public Avoidness avoidness { get; set; }
    }

    public class AddressDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string street { get; set; }

        public int? city { get; set; }
    }

    public class PeopleAmountDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int adults { get; set; }

        public int babies { get; set; }

        public int kids { get; set; }
    }

    public class PreferencesDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public bool kosher { get; set; }

        public bool vegan { get; set; }

        public bool veggie { get; set; }

        public bool organic { get; set; }
        // to push 
    }

    public class Avoidness
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public bool eggs { get; set; }

        public bool ful { get; set; }

        public bool gluten { get; set; }

        public bool milk { get; set; }

        public bool nuts { get; set; }

        public bool peanuts { get; set; }

        public bool soy { get; set; }
    }
}
