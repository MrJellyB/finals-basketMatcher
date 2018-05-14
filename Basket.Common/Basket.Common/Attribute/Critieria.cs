using System;

namespace Basket.Common.Attribute
{

    public class GeneralAttribute : System.Attribute
    {
        public virtual float Val(object val)
        {
            return (val == null) ? 1f : 0f;
        }
    }

    public class PriceAttribute : GeneralAttribute
    {
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
    }

    public class OrganicAttribute : GeneralAttribute
    {
        //public float Val(bool val)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
