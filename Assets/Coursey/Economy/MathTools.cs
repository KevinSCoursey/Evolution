using System;
using System.Threading;
using UnityEngine;
namespace Economy
{
    public static class MathTools
    {
        private const bool _debugThisClass = true;


        public static System.Random random;
        
        public static int PseudoRandomInt()
        {
            return random.Next();
        }
        public static int PseudoRandomInt(int min, int max)
        {
            Thread.Sleep(random.Next(1, random.Next(1,10)));//probably totally unnecessary

            return random.Next(min, max);
        }
        public static int[,] PseudoRandomIntPairArray(int height, int min, int max, bool firstNumberAlwaysLarger = false)
        {
            string debugString = "The following Pseudo-Random Int-Pair Array was generated...\n";
            int[,] pseudoRandomIntPairArray = new int[height, 2];
            for (int y = 0; y < height; y ++)
            {
                pseudoRandomIntPairArray[y, 0] = PseudoRandomInt(min, max);
                pseudoRandomIntPairArray[y, 1] = PseudoRandomInt(min, max);
                if (firstNumberAlwaysLarger && pseudoRandomIntPairArray[y, 0] <= pseudoRandomIntPairArray[y, 1])
                {
                    while(pseudoRandomIntPairArray[y, 0] <= pseudoRandomIntPairArray[y, 1])
                    {
                        pseudoRandomIntPairArray[y, 0] = PseudoRandomInt(min, max);
                        pseudoRandomIntPairArray[y, 1] = PseudoRandomInt(min, max);
                    }
                }
                /*string.Join(debugString, y == height - 1 ? 
                    $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}]" : 
                    $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}], ");
                */
                if(y < height - 1)
                {
                    debugString += $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}], ";
                }
                else
                {
                    debugString += $"[{pseudoRandomIntPairArray[y, 0]}][{pseudoRandomIntPairArray[y, 1]}]";
                }

            }
            //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"{ debugString}");
#pragma warning restore CS0162 // Unreachable code detected
            return pseudoRandomIntPairArray;
        }
    }
}
