using System;
using System.Collections.Generic;
using System.Text;

namespace Basket.Match.BL
{
    public class Population
    {
        #region Data Members

        // Basic Params
        private int m_length;
        private int m_crossoverPoint;
        private int m_initialPopulationCount;
        private int m_populationLimit;
        private float m_mutationFreq;
        private float m_deathParam;
        private float m_reproduceParam;
        private int m_currentGeneration = 1;

        // Initial populaitons
        public List<BasketListGenome> m_genomes = new List<BasketListGenome>();
        public List<BasketListGenome> m_genomesResults = new List<BasketListGenome>();
        public List<BasketListGenome> m_genomesNextGen = new List<BasketListGenome>();
        public List<BasketListGenome> m_genomeFamily = new List<BasketListGenome>();

        #endregion

        #region Static Members

        // TODO: Change this to list later
        //public static List<BasketListGenome> IdialBaskets;
        public static BasketListGenome IdialBaskets;

        #endregion

        #region CTOR

        public Population(
            int length,
            int crossOverPoint,
            int initialPop,
            int popLimit,
            float mutationFreq,
            float deathParam,
            float reproductionParam,
            float[] weights)
        {
            this.m_length = length;
            this.m_crossoverPoint = crossOverPoint;
            this.m_initialPopulationCount = initialPop;
            this.m_populationLimit = popLimit;
            this.m_mutationFreq = mutationFreq;
            this.m_deathParam = deathParam;
            this.m_reproduceParam = reproductionParam;

            m_genomes = new List<BasketListGenome>();
            m_genomesResults = new List<BasketListGenome>();
            m_genomesNextGen = new List<BasketListGenome>();
            m_genomeFamily = new List<BasketListGenome>();

            //this.m_genomes = new List<BasketListGenome>();

            //for (int i = 0; i < this.m_initialPopulationCount; i++)
            //{
            //    BasketListGenome newGenome = new BasketListGenome(this.m_length);
            //    newGenome.CrossoverPoint = this.m_crossoverPoint;
            //    newGenome.FitnessFunction();
            //    newGenome.SetWeights(weights);

            //    this.m_genomes.Add(newGenome);
            //}
        }

        public Population(
            List<BasketListGenome> genomes,
            int length,
            int crossOverPoint,
            int initialPop,
            int popLimit,
            float mutationFreq,
            float deathParam,
            float reproductionParam,
            float[] weights) : this(length, crossOverPoint, initialPop, popLimit, mutationFreq, deathParam, reproductionParam, weights)
        {
            this.m_genomes = new List<BasketListGenome>();

            if (genomes != null)
            {
                this.m_genomes = genomes;
            }
            else
            {
                this.m_genomes = new List<BasketListGenome>();
            }

            this.m_genomesResults = new List<BasketListGenome>();
            this.m_genomesNextGen = new List<BasketListGenome>();
            this.m_genomeFamily = new List<BasketListGenome>();
        }

        #endregion

        #region Private Methods

        private void Mutate(Genome g) // OREN
        {
            if (ListGenomes.Seed.NextDouble() < this.m_mutationFreq)
            {
                g.Mutate();
            }
        }


        private List<BasketListGenome> DoCrossover(List<BasketListGenome> genes) // OREN
        {
            List<BasketListGenome> NewGeneration = new List<BasketListGenome>();
            int originalCount = genes.Count;
            int TotalScore = 0;

            // Sum total scores
            foreach (BasketListGenome CurrElement in genes)
            {
                TotalScore += (int)CurrElement.CurrentFitness;
            }

            // Take 50% of the genes and use
            // TODO: Need to check this loop
            while (NewGeneration.Count != (originalCount / 2))
            {
                Random r = new Random();
                int rInt = r.Next(0, TotalScore + 1);
                int SumScore = 0;

                // Sum curr score
                foreach (BasketListGenome CurrElement in genes)
                {
                    SumScore += (int)CurrElement.CurrentFitness;

                    // Check if we can add it.
                    if ((SumScore >= rInt) && (!NewGeneration.Contains(CurrElement)))
                    {
                        NewGeneration.Add(CurrElement);

                        break;
                    }
                }
            }

            foreach(BasketListGenome CurrToRemove in NewGeneration)
            {
                genes.Remove(CurrToRemove);
            }

            List<Tuple<BasketListGenome, BasketListGenome>> lstCrossoverList = new List<Tuple<BasketListGenome, BasketListGenome>>();

            for (int i=0; i < genes.Count-1; i+=2)
            {
                BasketListGenome CurrElement1 = genes[i];
                BasketListGenome CurrElement2 = genes[i + 1];

                BasketListGenome NewBasketGenome1 = CurrElement1.BasketCrossover(CurrElement2);
                BasketListGenome NewBasketGenome2 = CurrElement1.BasketCrossover(CurrElement2);

                NewGeneration.Add(NewBasketGenome1);
                NewGeneration.Add(NewBasketGenome2);
            }
            
            
            return NewGeneration;
        }

        private void SaveBestBasket()
        {
            BasketListGenome BestBasket = null;

            foreach (BasketListGenome basketGenome in this.m_genomes)
            {
                if (BestBasket == null)
                {
                    BestBasket = basketGenome;
                }
                else if (basketGenome != null)
                {
                    if (BestBasket.CurrentFitness < basketGenome.CurrentFitness)
                    {
                        BestBasket = basketGenome;
                    }
                }
            }

            IdialBaskets = BestBasket;
        }

        #endregion

        #region Public Methods

        public void NextGen()
        {
            this.m_currentGeneration++;

            // Save the best
            this.SaveBestBasket();

            List<BasketListGenome> lstGenomesToRemove = new List<BasketListGenome>();

            // Check which of the genomes we need to kill
            foreach (BasketListGenome g in this.m_genomes)
            {
                if (g.CanDie(this.m_deathParam))
                {
                    //this.m_genomes.Remove(g);
                    lstGenomesToRemove.Add(g);
                }
            }

            // Lior M: Add collection to remove all te genomes that need to remove
            this.m_genomes.RemoveAll(x => lstGenomesToRemove.Contains(x));

            if (this.m_genomesNextGen != null)
            {
                // Now reproduce
                this.m_genomesNextGen.Clear();
            }
            if (m_genomesResults != null)
            {
                // Now reproduce
                this.m_genomesResults.Clear();
            }

            // Check which genomes need to be reproduced
            foreach (BasketListGenome g in this.m_genomes)
            {
                if (g.CanReproduce(this.m_reproduceParam))
                {
                    this.m_genomesNextGen.Add(g);
                }
            }

            this.m_genomes = DoCrossover(this.m_genomesNextGen);

            //// Check for mutations
            //foreach (Genome g in this.m_genomes)
            //{
            //    Mutate(g);
            //}


        }



        #endregion
    }
}
