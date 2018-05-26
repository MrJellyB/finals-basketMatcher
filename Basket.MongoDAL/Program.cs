using System;
using System.Collections.Generic;
using Basket.Common.Data;

namespace Basket.ServerSide
{
    public class Program
    {
        static void Main(string[] args)
        {
            ConnectionMongoDB db = ConnectionMongoDB.GetInstance();
            db.InitMongoClient(ConnectionMongoDB.URL);
            //db.GenerateRandomBasket(1000, 1, 30);
            //GetDataCheck(db);
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
            float max = db.GetMaxPrice();
            StoreDTO strStore = db.GetStoreByUser("ABCD");  
        }

        private static Dictionary<long, ProductDTO> _products;
        public static Dictionary<long, ProductDTO> Prodcuts
        {
            get
            {
                if (_products == null)
                {
                    _products = new Dictionary<long, ProductDTO>();
                }

                return _products;
            }
            set
            {
                _products = value;
            }
        }

        private static Dictionary<string, List<BasketDTO>> _oldBasketsPerUser;
        public static Dictionary<string, List<BasketDTO>> OldBasketsPerUser
        {
            get
            {
                if (_oldBasketsPerUser == null)
                {
                    _oldBasketsPerUser = new Dictionary<string, List<BasketDTO>>();
                }

                return _oldBasketsPerUser;
            }
            set
            {
                _oldBasketsPerUser = value;
            }
        }

        public static void storeDataLocally(ConnectionMongoDB db, string username)
        {
            if (Prodcuts.Count == 0)
            {
                List<ProductDTO> allProducts = db.GetAllProductDTO();
                foreach (var product in allProducts)
                    Prodcuts.Add(product.id, product);
            }

            if (OldBasketsPerUser.ContainsKey(username))
                OldBasketsPerUser[username] = db.GetListBasketByUserName(username);
            else
                OldBasketsPerUser.Add(username, db.GetListBasketByUserName(username));
        }

        private static readonly Random getrandom = new Random();

        public static BasketItemsDTO GetRandomProduct()
        {
            lock (getrandom) // synchronize
            {
                ProductDTO randomProduct = Prodcuts[getrandom.Next(0, Prodcuts.Count - 1)];
                BasketItemsDTO item = new BasketItemsDTO();
                item.id = randomProduct.id;
                item.image = "";
                item.name = randomProduct.name;
                item.price = randomProduct.price;
                item.amount = getrandom.Next(1, 3);

                return item;
            }
        }
    }
}
