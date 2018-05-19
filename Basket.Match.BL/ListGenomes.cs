using Basket.Common.Data;
using System;
using System.Collections.Generic;

namespace Basket.Match.BL
{
    public class ListGenomes : Genome
    {

        #region Data Members

        private static Random s_seed;
        public static Random Seed
        {
            get { return s_seed;}
            set { s_seed = value;}
        }
        
        private int m_min;
        public int Min
        {
            get { return m_min;}
            set { m_min = value;}
        }
        
        private int m_max;
        public int Max
        {
            get { return m_max;}
            set { m_max = value;}
        }
        
        protected IList<BasketItemsDTO> m_list;
        public IList<BasketItemsDTO> List
        {
            get { return m_list;}
            set { m_list = value;}
        }
        

        #endregion

        #region CTOR

        public ListGenomes()
        {

        }

        //public ListGenomes(long length, int min, int max) 
        //{
        //    this.Length = length;
        //    this.Min = min;
        //    this.Max = max;
        //    this.m_list = new List<BasketItemsDTO>();

        //    // Generate the gene list
        //    for (int i = 0; i < length; i++)
        //    {
        //        float nextVal = GenerateRandGeneVal(min,max);
        //        this.m_list.Add(nextVal);
        //    }
        //}
        public ListGenomes(Genome g, int min, int max) 
        {
            this.Length = ((ListGenomes)g).List.Count;
            this.Min = min;
            this.Max = max;
            this.m_list = ((ListGenomes)g).List;
        }

        public ListGenomes(IList<BasketItemsDTO> list, int min, int max)
        {
            this.Length = list.Count;
            this.Min = min;
            this.Max = max;
            this.m_list = list;
        }

        private float GenerateRandGeneVal(int min, int max)
        {
            return (float)(min + s_seed.NextDouble() * (max - min));
        }

        #endregion

        #region Private Methods

        // Exchanges gene values between two genes according to the CrossingPoint
        private void CrossoverGeneValues(ref ListGenomes geneOne, ref ListGenomes geneTwo)
        {
            for (int i = 0; i < this.CrossoverPoint; i++)
            {
                geneOne.List[i] = geneTwo.List[i];
            }

            for (int i = this.CrossoverPoint; i < Length; i++)
            {
                geneTwo.List[i] = geneOne.List[i];
            }
        }

        #endregion

        #region Public Methods

        public override Genome Crossover(Genome g)
        {
            // Generate the firstGene as copy of g and the second one as copy of this genome
            ListGenomes originalGenome = (ListGenomes)g;
            ListGenomes firstGene = new ListGenomes(g, originalGenome.Min, originalGenome.Max);
            ListGenomes secondGene = new ListGenomes((Genome)this, originalGenome.Min, originalGenome.Max);
            
            CrossoverGeneValues(ref firstGene, ref secondGene);

            // Take the better gene that made out from the crossover, randomly
            // TODO: Maybe change this check to fitness check
            if (s_seed.Next(2) == 1)
            {
                return firstGene;
            }
            else
            {
                return secondGene;
            }
        }

        public override double FitnessFunction()
        {
            // TODO: Add the fitness function here based on the basket weights
            return 1;
        }

        public override void Initiate()
        {
        }

        /// <summary>
        /// Choose a random gene value and assign a random generated value to it
        /// </summary>
        public override void Mutate()
        {
            //this.List[Seed.Next((int)this.Length)] = Seed.Next(this.Min, this.Max);
        }
        
        public override bool CanDie(float fitness)
        {
            if (this.CurrentFitness <= fitness)
            {
                return true;
            }

            return false;
        }

        public override bool CanReproduce(float fitness)
        {
            if (this.CurrentFitness > fitness)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
