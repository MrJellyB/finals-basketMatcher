using System;
using Basket.Common.Data;
using System.Reflection;
using System.Collections.Generic;
using Basket.ServerSide;
using Basket.Common.Attribute;

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

                int numOfProps = BasketListGenome.AttributePropertiesCount(typeof(ProductDTO));
                float[] result = new float[numOfProps];

                // Run over all items in a basket and with reflection 
                // and add all props with attrs to result
                foreach (var item in items)
                {
                    PropertyInfo[] props = BasketListGenome.GetProperties(item);
                    int j = 0;

                    // Run over all props and add only those who has our custom attributes
                    for (int i = 0; i < props.Length; i++)
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

        #region Static Methods

        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }

        // Return the number of props that are by our attributes
        private static int AttributePropertiesCount(Type type)
        {
            int result = 0;
            PropertyInfo[] props = GetProperties(type);

            foreach (PropertyInfo prop in props)
            {
                result += prop.GetCustomAttribute<GeneralAttribute>() != null ? 1 : 0;
            }

            return result;
        }

        #endregion
    }
}
