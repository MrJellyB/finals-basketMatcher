using System;
using System.Collections.Generic;
using Basket.Common.Data;

namespace Basket.ServerSide
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMongoDB db = ConnectionMongoDB.GetInstance();
            db.InitMongoClient(ConnectionMongoDB.URL);
            //db.GenerateRandomBasket(1000, 1, 30);
            GetDataCheck(db);
        }

        private static void GetDataCheck(ConnectionMongoDB db)
        {
            ProductDTO currProduct2 = db.GetProductDTOByProductId(212);
            ProductDTO currProduct = db.GetProductDTOByProductId(34000196173);
            ProductDTO currProduct3 = db.GetProductDTOByProductId(3147699100639);
            List<BasketDTO> baskets = db.GetAllBasketsDTO();
            List<GenderDTO> genders = db.GetAllGenders();
            
            List<ProductDTO> products = db.GetAllProductDTO();
            List<CategoryDTO> category = db.GetAllCategoriesDTO();
            List<CityDTO> cities = db.GetAllCitiesDTO();
            UserDTO user = db.GetUserDTOByUserName("liormiz");
            List<UserDTO> users = db.GetAllUsersDTO(); //test
        }
    }
}
