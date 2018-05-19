using Microsoft.AspNetCore.Mvc;
using Basket.Common.Data;
using System.Collections.Generic;
using Basket.ServerSide;
using Basket.Match.BL;
using Basket.Common.Enums;
using System;

namespace finals_basketMatch
{
    //[Produces("application/json")]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        #region Consts

        public const int NUMBER_OF_BASKETS = 1000;
        public const int FROM_PRODUCTS = 1;
        public const int TO_PRODUCTS = 30;

        #endregion

        #region C'Tors

        public MainController()
        {
            this.db = ConnectionMongoDB.GetInstance();
            this.db.InitMongoClient(ConnectionMongoDB.URL);
        }

        #endregion

        #region Properties

        public ConnectionMongoDB db { get; set; }

        #endregion

        [HttpGet("{p_strUserName}", Name = "GetUltimateBasketByUser")]
        public IActionResult GetUltimateBasketByUser(string p_strUserName)
        {
            // step 1: generates 1000 baskets
            List<BasketDTO> listBaskets = this.db.GenerateRandomBasket(NUMBER_OF_BASKETS, FROM_PRODUCTS, TO_PRODUCTS);

                // TODO: *** CHANGE TO SAPIR FUNCION ***
                float[] Wights = new float[13];
                for (int i = 0; i < 13; i++)
                {
                    Wights[i] = 1f;
                }
                // *** CHANGE TO SAPIR FUNCION ***

            List<BasketListGenome> Generation = new List<BasketListGenome>();

            foreach(BasketDTO CurrBasket in listBaskets)
            {
                BasketListGenome BasketGenomObject = new BasketListGenome(CurrBasket);
                //BasketGenomObject.BasketObject = CurrBasket;
                BasketGenomObject.UserName = p_strUserName;

                // init the matrix
                float[][] Matrix = new float[listBaskets[0].GetBasketItemCount()][];

                int enumFitnessSize = Enum.GetNames(typeof(eFitnessFunctionParams)).Length;
                for (int i = 0; i < Matrix.Length; i++)
                {
                    Matrix[i] = new float[enumFitnessSize];
                }

                BasketGenomObject.MakeBasketMatrix(ref Matrix);
                float[] Params = BasketGenomObject.GetBasketNormalizedParams(Matrix);

                float Grade = BasketGenomObject.FitnessFunction(Params, Wights);

                Generation.Add(BasketGenomObject);
            }

            Population AllPopulation = new Population(Generation, 1000, 500, 1000, 1000, 0.02f, 30, 70, Wights);
            

            return Ok(null);
        }
    }
}
