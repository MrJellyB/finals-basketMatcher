using System;
using Basket.Common.Data;
using System.Reflection;
using System.Collections.Generic;
using Basket.ServerSide;
using Basket.Common.Attribute;
using Basket.Common.Enums;

namespace Basket.Match.BL
{
    public class BasketListGenome : ListGenomes
    {
        #region Data Members

        private double[] m_weights;
        private BasketDTO m_basket;

        #endregion

        #region Properties

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

        #endregion

        #region Private Methods

        // Make a values matrix out of the basket
        private float[][] MakeBasketMatrix()
        {
            int numItems = this.m_basket.basketItems.Count;
            int numOfProps = BasketListGenome.GetProperties(typeof(ProductDTO)).Count;
            float[][] MatToReturn = new float[numItems][];

            InitEmptyMatrix(ref MatToReturn);

            return MatToReturn;
        }

        public float Normalize(float p_originalValue,float p_min, float p_max, float p_fNewMax, float p_fNewMin, bool p_bIsDesc)
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
                // List<PropertyInfo> props = BasketListGenome.GetProperties(productItem);

                mat[i][(int)eFitnessFunctionParams.price] = productItem.price;

                // for (int j = 0; j < props.Count; j++)
                // {
                //     object value = props[j].GetValue(productItem);
                //     mat[i][j] = props[j].GetCustomAttribute<GeneralAttribute>().Val(value);
                // }
            }
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
