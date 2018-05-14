using System;

namespace Basket.Common.Attribute
{

    public interface ICriteria
    {
        #region Methods

        float Calculate();

        #endregion
    }

    public class GeneralAttribute : System.Attribute, ICriteria
    {
        public float Calculate()
        {
            throw new NotImplementedException();
        }
    }

    public class PriceAttribute : GeneralAttribute, ICriteria
    {
        public float Calculate()
        {
            throw new NotImplementedException();
        }
    }

    public class GlutenFreeAttribute : GeneralAttribute, ICriteria
    {
        public float Calculate()
        {
            throw new NotImplementedException();
        }
    }

    public class OrganicAttribute : GeneralAttribute, ICriteria
    {
        public float Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
