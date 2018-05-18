using System;
using Basket.Common.Data;
using System.Reflection;
using System.Collections.Generic;
using Basket.ServerSide;
using Basket.Common.Attribute;
using Basket.Common.Enums;
using System.Linq;

namespace Basket.Match.BL
{
    public class BasketListGenome : ListGenomes
    {
        #region Consts

        const int TRUE = 1;
        const int FALSE = 0;

        #endregion

        #region Data Members

        private double[] m_weights;
        private BasketDTO m_basket;

        #endregion

        #region Properties

        public string UserName
        {
            get { return UserName; }
            private set { UserName = value; }
        }


        public double[] Weights
        {
            get { return m_weights; }
            private set { m_weights = value; }
        }

        // TODO: Return here a matrix containing the values of the basket's items
        public float[] Basket
        {
            get
            {
                IList<BasketItemsDTO> items = this.m_basket.basketItems;
                ConnectionMongoDB mongo = ConnectionMongoDB.GetInstance();

                int numOfProps = BasketListGenome.GetProperties(typeof(ProductDTO)).Count;
                float[] result = new float[numOfProps];

                // Run over all items in a basket and with reflection 
                // and add all props with attrs to result
                foreach (var item in items)
                {
                    List<PropertyInfo> props = BasketListGenome.GetProperties(item);
                    int j = 0;

                    // Run over all props and add only those who has our custom attributes
                    for (int i = 0; i < props.Count; i++)
                    {
                        GeneralAttribute relatedAttr = props[i].GetCustomAttribute<GeneralAttribute>();

                        if (relatedAttr != null)
                        {
                            //result[j++] = 
                        }
                    }
                }

               return result;
            }
            //set { m_basket = value; }
        }

        #endregion

        #region CTOR

        public BasketListGenome(int length) : base(length, 0, 1)
        {
            this.m_weights = new double[length];
            this.m_weights[0] = 1;

            // Default weights
            for (int i = 1; i < m_weights.Length; i++)
            {
                this.m_weights[i] = 0;
            }
        }

        #endregion

        #region Public Methods

        public void SetWeights(float[] weights)
        {
            this.m_weights = new double[weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                SetWeight(weights[i], i);
            }
        }

        public void SetWeight(float weight, int valNum)
        {
            if (weight > 1 || weight < 0)
            {
                throw new Exception("the weight must be between 1 and 0");
            }

            this.m_weights[valNum] = weight; 
        }

        public override double FitnessFunction()
        {
            double productToReturn = 1f;
            for (int i = 0; i < this.Length; i++)
            {
                // TODO: Compare those genes against IdialBaskets in Population class
                // to find the minimum
                productToReturn *= (this.m_weights[i] * (double)this.m_list[i]);
            } 

            return productToReturn;
        }



        public float[] GetBasketParamsArray(float[][] mat)
        {
            int rows = mat.GetLength(0);
            int cols = mat.GetLength(1);
            float[] Params = new float[cols];

            Params[(int)eFitnessFunctionParams.price] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.price, eActionOnParams.Sum);
            Params[(int)eFitnessFunctionParams.IsMeat] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.IsMeat, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.VeganFriendly] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.VeganFriendly, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.Kashrut] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.Kashrut, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.Organic] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.Organic, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.GlutenFree] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.GlutenFree, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.IsDairy] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.IsDairy, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.IsParve] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.IsParve, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.Vegetarian] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.Vegetarian, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.IsSoy] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.IsSoy, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.WasInLastBasket] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.WasInLastBasket, eActionOnParams.Percent);
            Params[(int)eFitnessFunctionParams.GramAmount] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.GramAmount, eActionOnParams.Sum);
            Params[(int)eFitnessFunctionParams.IsBabyProduct] = this.GetBasketParamsValues(mat, rows, eFitnessFunctionParams.IsBabyProduct, eActionOnParams.Percent);

            return Params;
        }

        public float[] NormalizeAllParams(float[] Params, int NumberOfProducts)
        {
            float[] result = new float[Params.Length];

            // TODO

            return result;
        }

        private float GetBasketParamsValues(float[][] p_mat, int p_rows ,eFitnessFunctionParams p_params, eActionOnParams p_actionOnParams)
        {
            float result = 0;

            switch (p_actionOnParams)
            {
                case (eActionOnParams.Sum):
                    {
                        float sum = 0;

                        for (int i = 0; i < p_rows; i++)
                        {
                            sum += p_mat[i][(int)p_params];
                        }

                        result = sum;

                        break;
                    }
                case (eActionOnParams.Percent):
                    {
                        float sum = 0;

                        for (int i = 0; i < p_rows; i++)
                        {
                            sum += p_mat[i][(int)p_params];
                        }

                        result = (sum / p_rows);

                        break;
                    }
                default:
                    {

                        break;
                    }
            }

            return result;
        }

        #endregion

        #region Private Methods

        private float[] InvokeAndReturnMatrixCalculations()
        {

            // TODO: Invoke here every parameter (column) in the matrix and
            // put it a one dimentional array
            return new float[1];
        }

        // Make a values matrix out of the basket
        private float[][] MakeBasketMatrix()
        {
            int numItems = this.m_basket.basketItems.Count;
            int numOfProps = BasketListGenome.GetProperties(typeof(ProductDTO)).Count;
            float[][] MatToReturn = new float[numItems][];

            InitEmptyMatrix(ref MatToReturn);

            PopulateMatrix(ref MatToReturn);

            return MatToReturn;
        }

        private float Normalize(float p_originalValue,float p_min, float p_max, float p_fNewMax, float p_fNewMin, bool p_bIsDesc)
        {
            float normalValue = 0f;

            normalValue = (((p_originalValue - p_min) / (p_max - p_min)) * (p_fNewMax - p_fNewMin)) + p_fNewMin;

            // in case that the category is price and etc...
            if (p_bIsDesc)
            {
                normalValue = p_fNewMax - normalValue;
            }

            return normalValue;
        }

        private void InitEmptyMatrix(ref float[][] mat)
        {
            List<PropertyInfo> props = BasketListGenome.GetProperties(typeof(ProductDTO));
            int numOfProps = props.Count;

            for (int i = 0; i < mat.Length; i++)
            {
                mat[i] = new float[numOfProps];

                for (int j = 0; j < mat[i].Length; j++)
                {
                    mat[i][j] = 0f;
                }
            }
        }

        /// <summary>
        /// Populate the "mat" matrix with values according to the properties
        /// </summary>
        /// <param name="mat">the matrix to manipulate</param>
        private void PopulateMatrix(ref float[][] mat)
        {
            ConnectionMongoDB db = ConnectionMongoDB.GetInstance();
            IList<BasketItemsDTO> products = this.m_basket.basketItems;
            
            for (int i = 0; i < mat.Length; i++)
            {
                ProductDTO productItem = db.GetProductDTOByProductId(products[i].id);

                FillProduct(mat, i, productItem, db);
            }
        }

        private void FillProduct(float[][] mat, int i, ProductDTO productItem, ConnectionMongoDB DbConnection)
        {
            mat[i][(int)eFitnessFunctionParams.price] = productItem.price;
            mat[i][(int)eFitnessFunctionParams.Organic] = productItem.Organic == true ? TRUE : FALSE;
            mat[i][(int)eFitnessFunctionParams.VeganFriendly] = productItem.VeganFriendly == true ? TRUE : FALSE;

            mat[i][(int)eFitnessFunctionParams.IsMeat] = productItem.category == (int)eCategory.MeatAndFish ? TRUE : FALSE;
            mat[i][(int)eFitnessFunctionParams.IsDairy] = productItem.category == (int)eCategory.MilkAndEggs ? TRUE : FALSE;
            mat[i][(int)eFitnessFunctionParams.IsBabyProduct] = productItem.category == (int)eCategory.Babies ? TRUE : FALSE;

            // whenever the product is kosher
            if (productItem.Kashrut != null && productItem.Kashrut != string.Empty)
            {
                mat[i][(int)eFitnessFunctionParams.Kashrut] = TRUE;
            }
            else
            {
                mat[i][(int)eFitnessFunctionParams.Kashrut] = FALSE;
            }

            // whenever the product is parve
            if (productItem.category != (int)eCategory.MeatAndFish && productItem.category != (int)eCategory.MilkAndEggs)
            {
                mat[i][(int)eFitnessFunctionParams.IsParve] = TRUE;
            }
            else
            {
                mat[i][(int)eFitnessFunctionParams.IsParve] = FALSE;
            }

            // whenever the product is Vegetarian
            if (productItem.category != (int)eCategory.MeatAndFish)
            {
                mat[i][(int)eFitnessFunctionParams.Vegetarian] = TRUE;
            }
            else
            {
                mat[i][(int)eFitnessFunctionParams.Vegetarian] = FALSE;
            }

            // whenever the product is Vegetarian
            if (productItem.name.Contains("סויה") ||
                productItem.name.ToLower().Contains("soy") ||
                productItem.ManufacturerItemDescription.Contains("סויה") ||
                productItem.ManufacturerItemDescription.ToLower().Contains("soy"))
            {
                mat[i][(int)eFitnessFunctionParams.Vegetarian] = TRUE;
            }
            else
            {
                mat[i][(int)eFitnessFunctionParams.Vegetarian] = FALSE;
            }

            List<BasketDTO> allBaskets = DbConnection.GetListBasketByUserName(this.UserName);
            //BasketDTO current = allBaskets.OrderByDescending(x => x.createdTime).FirstOrDefault();

            int productCounter = 0;

            foreach (BasketDTO CurrBasket in allBaskets)
            {
                foreach (BasketItemsDTO CurrProduct in CurrBasket.basketItems)
                {
                    if (CurrProduct.id == productItem.id)
                    {
                        productCounter++;

                        break;
                    }
                }
            }

            mat[i][(int)eFitnessFunctionParams.WasInLastBasket] = (float)((float)productCounter) / ((float)allBaskets.Count);
        }

        private List<string> GetParams()
        {
            return new List<string> { "price", "" };
        }

        #endregion

        #region Static Methods

        private static List<PropertyInfo> GetProperties(object obj)
        {
            PropertyInfo[] props = obj.GetType().GetProperties();
            return AttrProperties(props);
        }

        private static List<PropertyInfo> GetProperties(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            return AttrProperties(props);
        }

        private static List<PropertyInfo> AttrProperties(PropertyInfo[] props)
        {
            List<PropertyInfo> listToReturn = new List<PropertyInfo>();

            for (int i = 0; i < props.Length; i++)
            {
                // Add only those with our attributes
                if (props[i].GetCustomAttribute<GeneralAttribute>() != null)
                {
                    listToReturn.Add(props[i]);
                }

            }

            return listToReturn;
        }

        #endregion
    }
}
