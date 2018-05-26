using Microsoft.AspNetCore.Mvc;
using Basket.Common.Data;
using System.Collections.Generic;
using Basket.ServerSide;
using Basket.Match.BL;
using Basket.Common.Enums;
using System;
using Microsoft.AspNetCore.Cors;

namespace finals_basketMatch
{
    //[Produces("application/json")]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        #region Consts

        public const int NUMBER_OF_BASKETS = 100;
        public const int FROM_PRODUCTS = 10;
        public const int TO_PRODUCTS = 30;
        public const float MUTATION_PRECENT = 0.02f;
        public const int NUM_OF_GENERATION = 150;
        public const float MIN_FITNESS = 0;
        public const float MAX_FITNESS = 0;

        public int enumFitnessSize = Enum.GetNames(typeof(eFitnessFunctionParams)).Length;

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
            Program.storeDataLocally(db, p_strUserName);

            // step 1: generates 1000 baskets
            List<BasketDTO> listBaskets = this.db.GenerateRandomBasket(NUMBER_OF_BASKETS, FROM_PRODUCTS, TO_PRODUCTS);
            enumFitnessSize = Enum.GetNames(typeof(eFitnessFunctionParams)).Length;

            float[] Wights = setWeightsPerProfile(p_strUserName);

            Population AllPopulation = GenerateNewPopulation(p_strUserName, Wights, listBaskets);

            for (int i=0; i < NUM_OF_GENERATION; i++)
            {
                AllPopulation.NextGen();

                foreach (BasketListGenome Curr in AllPopulation.m_genomes)
                {
                    this.CalcFitness(Curr, Wights);
                }
            }

            Population.IdialBaskets.BasketObject.streetName = db.GetStoreByUser(p_strUserName).Address;

            return Ok(Population.IdialBaskets.BasketObject);
        }

        public float[] setWeightsPerProfile(String strUserName)
        {
            float[] fltWeights = new float[enumFitnessSize];
            UserDTO currUser = db.GetUserDTOByUserName(strUserName);

            if ((currUser == null) || (currUser.profile == null))
            { 
                for (int i = 0; i < enumFitnessSize; i++)
                {
                    fltWeights[i] = 0f;
                }

                fltWeights[(int)eFitnessFunctionParams.price] = 1;
            }
            else
            {

                // Price considerations are common to all kinds of profile
                fltWeights[(int)eFitnessFunctionParams.price] = (float)0.9;
                fltWeights[(int)eFitnessFunctionParams.WasInLastBasket] = (float)0.8;

                // Kosher considerations are also common to all kinds of profile
                if (currUser.profile.preferences.kosher == true)
                {
                    fltWeights[(int)eFitnessFunctionParams.Kashrut] = 1;
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.Kashrut] = (float)0.5;
                }

                // Check organic preference 
                if (currUser.profile.preferences.organic == true)
                {
                    fltWeights[(int)eFitnessFunctionParams.Organic] = (float)1;
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.Organic] = (float)0.2;
                }

                // Check avoidness to milk: If has no avoidness, put 0.6 and then if the user is 
                // vegan, put lower score
                if (currUser.profile.avoidness.milk == false)
                {
                    fltWeights[(int)eFitnessFunctionParams.IsDairy] = (float)0.6;
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.IsDairy] = 0;
                }

                // Parve considerations are common to non unique baskets - if they are unique,
                // the score will be changed later
                fltWeights[(int)eFitnessFunctionParams.IsParve] = (float)0.5;

                // Check gluten avoidness   
                if (currUser.profile.avoidness.gluten == true)
                {
                    fltWeights[(int)eFitnessFunctionParams.GlutenFree] = (float)1;
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.GlutenFree] = (float)0.5;
                }

                // Check soy avoidness   
                if (currUser.profile.avoidness.soy == true)
                {
                    fltWeights[(int)eFitnessFunctionParams.IsSoy] = (float)0;
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.IsSoy] = (float)0.5;
                }

                // Check grmas - the bigger the fanily, the bigger the grams
                if (currUser.profile.peopleAmount.adults >= 2)
                {
                    fltWeights[(int)eFitnessFunctionParams.GramAmount] = (float)0.4;

                    if (currUser.profile.peopleAmount.kids >= 1 ||
                        currUser.profile.peopleAmount.babies >= 1)
                    {
                        fltWeights[(int)eFitnessFunctionParams.GramAmount] = (float)0.6;
                    }
                    else if (currUser.profile.peopleAmount.kids > 2 ||
                            currUser.profile.peopleAmount.babies > 2)
                    {
                        fltWeights[(int)eFitnessFunctionParams.GramAmount] = (float)0.8;
                    }
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.GramAmount] = (float)0.2;
                }

                // Get user profile - Define weights by preferences
                if (currUser.profile.preferences.veggie == true)
                {
                    fltWeights[(int)eFitnessFunctionParams.IsMeat] = 0;
                    fltWeights[(int)eFitnessFunctionParams.IsParve] = (float)0.8;
                    fltWeights[(int)eFitnessFunctionParams.IsDairy] = (float)0.8;
                    fltWeights[(int)eFitnessFunctionParams.VeganFriendly] = (float)0.6;
                    fltWeights[(int)eFitnessFunctionParams.Vegetarian] = (float)1;
                }
                else if (currUser.profile.preferences.vegan == true)
                {
                    fltWeights[(int)eFitnessFunctionParams.IsMeat] = 0;
                    fltWeights[(int)eFitnessFunctionParams.IsParve] = (float)0.9;
                    fltWeights[(int)eFitnessFunctionParams.IsDairy] = (float)0;
                    fltWeights[(int)eFitnessFunctionParams.VeganFriendly] = (float)1;
                    fltWeights[(int)eFitnessFunctionParams.Vegetarian] = (float)0.8;
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.IsMeat] = (float)0.5;
                    fltWeights[(int)eFitnessFunctionParams.VeganFriendly] = (float)0.3;
                    fltWeights[(int)eFitnessFunctionParams.Vegetarian] = (float)0.5;
                }

                // Check if there's a need to baby products
                if (currUser.profile.peopleAmount.babies > 0)
                {
                    fltWeights[(int)eFitnessFunctionParams.IsBabyProduct] = (float)1;
                }
                else
                {
                    fltWeights[(int)eFitnessFunctionParams.Vegetarian] = (float)0;
                }
            }

            return fltWeights;
        }

        private Population GenerateNewPopulation(string p_strUserName, float[] Wights, List<BasketDTO> listBaskets)
        {
            List<BasketListGenome> Generation = new List<BasketListGenome>();

            foreach (BasketDTO CurrBasket in listBaskets)
            {
                BasketListGenome BasketGenomObject = this.GetBasketGenomeWithFitness(CurrBasket, p_strUserName, Wights);
                Generation.Add(BasketGenomObject);
            }

            Population.IdialBaskets = null;

            return new Population(
                        Generation, 
                        NUMBER_OF_BASKETS,
                        FROM_PRODUCTS / 2, 
                        NUMBER_OF_BASKETS, 
                        NUMBER_OF_BASKETS,
                        MUTATION_PRECENT, 
                        MIN_FITNESS, 
                        MAX_FITNESS, 
                        Wights);
        }

        private BasketListGenome GetBasketGenomeWithFitness(BasketDTO CurrBasket, string p_strUserName, float[] Wights)
        {
            BasketListGenome BasketGenomObject = new BasketListGenome(CurrBasket, (FROM_PRODUCTS / 2));

            if (p_strUserName != null)
            {
                BasketGenomObject.UserName = p_strUserName;
            }

            this.CalcFitness(BasketGenomObject, Wights);

            return BasketGenomObject;
        }

        private BasketListGenome CalcFitness(BasketListGenome BasketGenomObject, float[] Wights)
        {
            // init the matrix
            float[][] Matrix = new float[BasketGenomObject.BasketObject.basketItems.Count][];


            for (int i = 0; i < Matrix.Length; i++)
            {
                Matrix[i] = new float[enumFitnessSize];
            }

            BasketGenomObject.MakeBasketMatrix(ref Matrix);
            float[] Params = BasketGenomObject.GetBasketNormalizedParams(Matrix);

            float Grade = BasketGenomObject.FitnessFunction(Params, Wights);

            return BasketGenomObject;
        }
     }
}
