using Basket.Common.Data;
using System;

namespace Basket.Common.Attribute
{

    public abstract class GeneralAttribute : System.Attribute
    {
        public virtual float Val(object val)
        {
            return (val == null) ? 1f : 0f;
        }

        public abstract float Calc(ProductDTO productOne, ProductDTO productTwo);
    }

    public class PriceAttribute : GeneralAttribute
    {
        public override float Calc(ProductDTO productOne, ProductDTO productTwo)
        {
            return productOne.price + productTwo.price;
        }

        public override float Val(object val)
        {
            float toReturn = (float)val;
            // I know this is stupid, its just for the 
            // clean iteration in PopulateMatrix in BasketListGenome
            return toReturn;
        }


    }

    public class GlutenFreeAttribute : GeneralAttribute
    {
        //public override float Val(object val)
        //{
        //    float toReturn = (bool)val;
        //    // I know this is stupid, its just for the 
        //    // clean iteration in PopulateMatrix in BasketListGenome
        //    return toReturn.;
        //}
        public override float Calc(ProductDTO productOne, ProductDTO productTwo)
        {
            // Meanwhile its only 
            return (float)(Convert.ToInt32(productOne.GlutenFree) * Convert.ToInt32(productTwo.GlutenFree));
        }
    }

    public class OrganicAttribute : GeneralAttribute
    {
        //public float Val(bool val)
        //{
        //    throw new NotImplementedException();
        //}
        public override float Calc(ProductDTO productOne, ProductDTO productTwo)
        {
            return (float)(Convert.ToInt32(productOne.Organic) * Convert.ToInt32(productTwo.Organic));
        }
    }
}
