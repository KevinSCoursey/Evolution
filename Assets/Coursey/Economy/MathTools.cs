using System;
using System.Threading;
using UnityEngine;
namespace Economy
{
    public static class MathTools
    {
        private const bool _debugThisClass = false;


        public static System.Random random;
        
        public static bool Initialize(int seed)
        {
            random = new System.Random(seed);
            return true;
        }

        public static int PseudoRandomInt()
        {
            return random.Next();
        }
        public static bool CoinFlip()
        {
            bool rand = random.Next(2) == 0;
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass)
            {
                Debug.Log($"Coin flip... {rand}");
            }
#pragma warning restore CS0162 // Unreachable code detected
            return rand;
        }
        public static int PseudoRandomIntExclusiveMax(int min, int max)
        {
            int rand = min >= max
                ? min
                : random.Next(min, max);
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass)
            {
                Debug.Log($"Random number (exclusive max) between {min} and {max} ... {rand}");
            }
#pragma warning restore CS0162 // Unreachable code detected
            return rand;
        }
        public static int PseudoRandomIntExcluding(int min, int max, int exclude)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass)
            {
                Debug.Log($"Random number (excluding {exclude}) from {min} to {max} excluding {max}");
            }
#pragma warning restore CS0162 // Unreachable code detected
            if (CoinFlip())
            {
                return PseudoRandomIntExclusiveMax(min, exclude);
            }
            else
            {
                return PseudoRandomIntExclusiveMax(exclude, max);
            }
        }
        public static float PseudoRandomFloat(float min, float max)
        {
            return min >= max
                ? min
                : (float)(random.NextDouble() * (max - min) + min);
        }
        public static int[,] PseudoRandomIntPairArray(int height, int min, int max, bool firstNumberAlwaysLarger = false)
        {
            string debugString = "The following Pseudo-Random Int-Pair Array was generated...\n";
            int[,] pseudoRandomIntPairArray = new int[height, 2];
            for (int y = 0; y < height; y ++)
            {
                pseudoRandomIntPairArray[y, 0] = PseudoRandomIntExclusiveMax(min, max);
                pseudoRandomIntPairArray[y, 1] = PseudoRandomIntExclusiveMax(min, max);
                if (firstNumberAlwaysLarger && pseudoRandomIntPairArray[y, 0] <= pseudoRandomIntPairArray[y, 1])
                {
                    while(pseudoRandomIntPairArray[y, 0] <= pseudoRandomIntPairArray[y, 1])
                    {
                        pseudoRandomIntPairArray[y, 0] = PseudoRandomIntExclusiveMax(min, max);
                        pseudoRandomIntPairArray[y, 1] = PseudoRandomIntExclusiveMax(min, max);
                    }
                }
                string.Join(debugString, y == height - 1 ? 
                    $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}]" : 
                    $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}], ");
                
                if(y < height - 1) debugString += $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}], ";
                else debugString += $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}]";

            }
            //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"{ debugString}");
#pragma warning restore CS0162 // Unreachable code detected
            return pseudoRandomIntPairArray;
        }

        public static int CalculateItemPurchasePrice(IEconomyItem economyItem, bool isSpecialized)
        {
            float markupFactor = 1.25f;
            int price = isSpecialized

                ? (int)(markupFactor * (economyItem.PriceDefault +
                80 * economyItem.MaxQuantityOfItem * economyItem.RarityInt /
                (Mathf.Log(economyItem.QuantityOfItem) * Mathf.Sin(economyItem.QuantityOfItem / (3 * economyItem.RarityInt)) + economyItem.QuantityOfItem)))

                : (int)(markupFactor * (economyItem.PriceDefault +
                80 * economyItem.MaxQuantityOfItem * economyItem.RarityInt /
                (Mathf.Log(economyItem.QuantityOfItem) * Mathf.Sin(economyItem.QuantityOfItem / (3 * economyItem.RarityInt)) + economyItem.QuantityOfItem)));

            return price;
        }
        public static int CalculateItemSalePrice(IEconomyItem economyItem, bool isSpecialized)
        {
            float markdownFactor = 0.75f;
            int price = isSpecialized

                ? (int)(markdownFactor * (economyItem.PriceDefault +
                80 * economyItem.MaxQuantityOfItem * economyItem.RarityInt /
                (Mathf.Log(economyItem.QuantityOfItem) * Mathf.Sin(economyItem.QuantityOfItem / (3 * economyItem.RarityInt)) + economyItem.QuantityOfItem)))

                : (int)(markdownFactor * (economyItem.PriceDefault +
                80 * economyItem.MaxQuantityOfItem * economyItem.RarityInt /
                (Mathf.Log(economyItem.QuantityOfItem) * Mathf.Sin(economyItem.QuantityOfItem / (3 * economyItem.RarityInt)) + economyItem.QuantityOfItem)));

            while(price >= economyItem.PurchasePrice)
            {
                price = (int)(price * 0.75);
            }

            return price;
        }
        public static float CalculateDistanceBetweenPoints(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2) + Mathf.Pow(z2 - z1, 2));
        }
        public static float CalculateDistanceBetweenPoints(float x1, float y1, float x2, float y2)
        {
            return Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2));
        }
    }
}