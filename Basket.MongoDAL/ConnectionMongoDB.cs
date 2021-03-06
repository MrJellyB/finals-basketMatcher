﻿using Basket.Common.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Basket.ServerSide
{
    public class ConnectionMongoDB
    {
        #region Consts

        // private const string URL = "mongodb://localhost";
        // private const string URL = "mongodb://localhost:27017";
        public const string URL = "mongodb://11:22@193.106.55.172:8888/test";
        public const string PRODUCT_NAME_COLLECTION = "product";
        public const string GENDER_NAME_COLLECTION = "gender";
        public const string CATEGORY_NAME_COLLECTION = "category";
        public const string BASKET_NAME_COLLECTION = "basket";
        public const string CITY_NAME_COLLECTION = "city";
        public const string USERS_NAME_COLLECTION = "users";
        public const string STORE_NAME_COLLECTION = "store";

        #endregion

        #region C'Tors

        private ConnectionMongoDB()
        {
        }

        #endregion

        #region Properties
        private static ConnectionMongoDB s_intance;

        private static readonly Random getrandom = new Random();
        public MongoClient client { get; set; }
        public MongoServer server { get; set; }
        public MongoDatabase database { get; set; }
        public MongoCollection<GenderDTO> genderCollection { get; set; }
        public MongoCollection<ProductDTO> productCollection { get; set; }

        public MongoCollection<CategoryDTO> categoryCollection { get; set; }

        public MongoCollection<BasketDTO> basketCollection { get; set; }

        public MongoCollection<CityDTO> cityCollection { get; set; }

        public MongoCollection<UserDTO> userCollection { get; set; }

        public MongoCollection<StoreDTO> storeCollection { get; set; }

        public Dictionary<long, ProductDTO> Products { get; set; }

        public List<long> ProductsKeys { get; set; }

        public List<StoreDTO> Stores { get; set; }

        public List<CityDTO> Cities { get; set; }

        public List<long> BarkodListIds { get; set; }

        public Dictionary<string, List<BasketDTO>> OldBasketsPerUser { get; set; }

        #endregion

        #region Methods

        public void InitMongoClient(string connectionString)
        {
            this.client = new MongoClient(connectionString);
            this.server = client.GetServer();
            this.database = server.GetDatabase("test");

            // define the collections
            this.GetCollectionsValues();

            // get collection values
            this.GetData();
        }

        public void GetCollectionsValues()
        {
            this.productCollection = database.GetCollection<ProductDTO>(PRODUCT_NAME_COLLECTION);
            this.genderCollection = database.GetCollection<GenderDTO>(GENDER_NAME_COLLECTION);
            this.categoryCollection = database.GetCollection<CategoryDTO>(CATEGORY_NAME_COLLECTION);
            this.basketCollection = database.GetCollection<BasketDTO>(BASKET_NAME_COLLECTION);
            this.cityCollection = database.GetCollection<CityDTO>(CITY_NAME_COLLECTION);
            this.storeCollection = database.GetCollection<StoreDTO>(STORE_NAME_COLLECTION);
            this.userCollection = database.GetCollection<UserDTO>(USERS_NAME_COLLECTION);
        }

        public void GetData()
        {
            if (this.Stores == null)
            {
                this.Stores = this.GetAllStores();
            }

            if (this.Cities == null)
            {
                this.Cities = this.GetAllCitiesDTO();
            }

            if (this.Products == null)
            {
                this.Products = new Dictionary<long, ProductDTO>();
                this.ProductsKeys = new List<long>();

                List<ProductDTO> allProducts = this.GetAllProductDTO();
                foreach (var product in allProducts)
                {
                    this.Products.Add(product.id, product);
                    this.ProductsKeys.Add(product.id);
                }
            }

            if (this.OldBasketsPerUser == null)
            {
                this.OldBasketsPerUser = new Dictionary<string, List<BasketDTO>>();
                List<BasketDTO> allBaskets = this.GetAllBasketsDTO();

                foreach (var basket in allBaskets)
                {
                    if (!OldBasketsPerUser.ContainsKey(basket.userName))
                        OldBasketsPerUser.Add(basket.userName, new List<BasketDTO>());

                    OldBasketsPerUser[basket.userName].Add(basket);
                }
            }
        }

        public void queryOnProduct()
        {
            List<GenderDTO> dataGender = genderCollection.AsQueryable<GenderDTO>().ToList();
            List<ProductDTO> dataProduct = productCollection.AsQueryable<ProductDTO>().ToList();
        }

        public List<ProductDTO> GetAllProductDTO()
        {
            List<ProductDTO> dataProduct = productCollection.AsQueryable<ProductDTO>().ToList();
            return dataProduct;
        }
        public ProductDTO GetProductDTOByProductId(long p_productId)
        {
            ProductDTO dataProduct = productCollection.AsQueryable<ProductDTO>().Where(x => x.id == p_productId).FirstOrDefault();
            return dataProduct;
        }

        public ProductDTO GetProductDTONotFromDBByProductId(long p_productId)
        {
            //return this.Products.Where(x => x.id == p_productId).FirstOrDefault();
            return this.Products[p_productId];
        }

        public BasketItemsDTO GetRandomProduct()
        {
            //List<BasketDTO> RandomBasket = this.GenerateRandomBasket(1, 1, 1);
            //return RandomBasket.FirstOrDefault().basketItems.FirstOrDefault();
            int nProductIndex = this.GetRandomNumber(0, this.ProductsKeys.Count - 1);
            ProductDTO randomProduct = this.Products[this.ProductsKeys[nProductIndex]];
            BasketItemsDTO item = new BasketItemsDTO();
            item.id = randomProduct.id;
            item.image = "";
            item.name = randomProduct.name;
            item.price = randomProduct.price;
            item.amount = this.GetRandomNumber(1, 3);

            return item;
        }

        public List<GenderDTO> GetAllGenders()
        {
            List<GenderDTO> data = genderCollection.AsQueryable<GenderDTO>().ToList();
            return data;
        }

        public GenderDTO GetGenderDTOByProductId(long p_genderId)
        {
            GenderDTO dataProduct = genderCollection.AsQueryable<GenderDTO>().Where(x => x.id == p_genderId).FirstOrDefault();
            return dataProduct;
        }

        public StoreDTO GetStoreByCity(string p_strCity)
        {
            List<StoreDTO> lstStores = this.Stores.Where(x => x.City == p_strCity).ToList();
            return lstStores.FirstOrDefault();
        }

        public StoreDTO GetStoreByUser(string strUserName)
        {
            StoreDTO strToReturn = null;
            UserDTO userUser = this.GetUserDTOByUserName(strUserName);
            if (userUser != null &&
                userUser.profile != null &&
                userUser.profile.address != null)
            {
                int? cityCode = userUser.profile.address.city;
                if (cityCode.HasValue)
                {
                    CityDTO city = this.Cities.Where(x => x._id == cityCode.Value).FirstOrDefault();
                    if (city != null)
                    {
                        strToReturn = this.GetStoreByCity(city.cityName);
                    }
                }
            }

            if (strToReturn == null)
            {
                strToReturn = this.Stores.FirstOrDefault();
            }

            return strToReturn;
        }

        public CategoryDTO GetCategoryDTOById(long p_category)
        {
            CategoryDTO category = categoryCollection.AsQueryable<CategoryDTO>().Where(x => x.id == p_category).FirstOrDefault();
            return category;
        }

        public List<CategoryDTO> GetAllCategoriesDTO()
        {
            List<CategoryDTO> data = categoryCollection.AsQueryable<CategoryDTO>().ToList();
            return data;
        }

        public List<BasketDTO> GetAllBasketsDTO()
        {
            List<BasketDTO> data = basketCollection.AsQueryable<BasketDTO>().ToList();
            return data;
        }

        public List<BasketDTO> GetListBasketByUserName(string p_strUserName)
        {
            //List<BasketDTO> data = basketCollection.AsQueryable<BasketDTO>().Where(x => x.userName == p_strUserName).ToList();
            ///return data;
            ///
            if (this.OldBasketsPerUser.Keys.Contains(p_strUserName))
                return this.OldBasketsPerUser[p_strUserName];

            return new List<BasketDTO>();
        }

        public List<CityDTO> GetAllCitiesDTO()
        {
            List<CityDTO> data = cityCollection.AsQueryable<CityDTO>().ToList();
            return data;
        }

        public List<UserDTO> GetAllUsersDTO()
        {
            List<UserDTO> data = userCollection.AsQueryable<UserDTO>().ToList();
            return data;
        }

        public UserDTO GetUserDTOByUserName(string strUserName)
        {
            UserDTO data = userCollection.AsQueryable<UserDTO>().Where(x => x.userName == strUserName).FirstOrDefault();
            return data;
        }

        public List<StoreDTO> GetAllStores()
        {
            List<StoreDTO> data = storeCollection.AsQueryable<StoreDTO>().ToList();
            return data;
        }

        public float GetMaxPrice()
        {
            float max = 0;
            ProductDTO currProduct = null;

            if (this.Products != null)
            {
                currProduct = this.Products.Values.OrderByDescending(x => x.price).FirstOrDefault();
            }
            if (currProduct != null)
            {
                max = currProduct.price;
            }

            return max;
        }


        public List<BasketDTO> GenerateRandomBasket(int p_nNumberOfBasket, int p_nFromNumberOfProduct, int p_nToNumberOfProducts)
        {
            List<BasketDTO> basketList = new List<BasketDTO>();
            float fTotalPrice = 0;
            for (int nIndex = 0; nIndex < p_nNumberOfBasket; nIndex++)
            {
                fTotalPrice = 0;
                BasketDTO basket = new BasketDTO();
                basket.basketItems = new List<BasketItemsDTO>();
                basket.id = 0;

                int nCountOfProducts = this.GetRandomNumber(p_nFromNumberOfProduct, p_nToNumberOfProducts);

                for (int jIndex = 0; jIndex < nCountOfProducts; jIndex++)
                {
                    int nProductIndex = this.GetRandomNumber(0, this.ProductsKeys.Count - 1);
                    ProductDTO currProduct = this.Products[this.ProductsKeys[nProductIndex]];
                    BasketItemsDTO item = new BasketItemsDTO();
                    item.id = currProduct.id;
                    item.image = "";
                    item.name = currProduct.name;
                    item.price = currProduct.price;
                    item.amount = this.GetRandomNumber(1, 3);
                    fTotalPrice = fTotalPrice + (item.price * item.amount);
                    basket.basketItems.Add(item);
                }

                basket.totalPrice = fTotalPrice;
                StoreDTO store = this.Stores[this.GetRandomNumber(0, this.Stores.Count - 1)];
                basket.streetName = store.Address + " " + store.City;

                basketList.Add(basket);
            }

            return basketList;
        }

        public int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }

        #endregion

        #region Static Methods

        public static ConnectionMongoDB GetInstance()
        {
            // TODO: add a mutex here to avoid thread collision

            if (ConnectionMongoDB.s_intance == null)
            {
                ConnectionMongoDB.s_intance = new ConnectionMongoDB();
            }

            return ConnectionMongoDB.s_intance;
        }

        #endregion
    }
}
