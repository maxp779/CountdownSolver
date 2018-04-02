using System.Collections.Generic;
using System.Linq;

namespace CountdownSolver.Logic.NumbersGame
{
    public class OperandPermutator
    {
        private List<string> currentOperandPermutation;
        private List<string> allOperands;
        private int[] currentOperandIndexes;
        private int maxOperandIndex;

        public OperandPermutator(List<string> inputOperands, int numberOfOperandsRequired)
        {
            allOperands = new List<string>(inputOperands);
            currentOperandIndexes = new int[numberOfOperandsRequired];
            maxOperandIndex = inputOperands.Count-1;
        }

        bool increaseNextIndex = false;
        private bool firstExecution = true;
        public bool nextOperandPermutation()
        {
            do
            {
                //increase the last index unless first run
                if (!firstExecution)
                {
                    currentOperandIndexes[currentOperandIndexes.Length - 1] = currentOperandIndexes[currentOperandIndexes.Length - 1] + 1;
                }

                //start at the end of currentOperandIndexes
                for (int index = currentOperandIndexes.Length - 1; index >= 0; index--)
                {
                    //increaseNextIndex will have been set while the loop was on the PREVIOUS index to be actioned for THIS index
                    //so we dont use currentOperatorIndexes[index -1]
                    if (increaseNextIndex)
                    {
                        currentOperandIndexes[index] = currentOperandIndexes[index] + 1;
                        increaseNextIndex = false;
                    }

                    //if we have been through all possible combinations
                    if (index == 0 && currentOperandIndexes[index] == maxOperandIndex+1)
                    {
                        return false;
                        //currentOperandCount++ <-- try this
                        //currentOperatorIndexes = new int[maxOperators]; //back to [0,0,0,0]
                    }
                    else if (currentOperandIndexes[index] == maxOperandIndex+1)
                    {
                        currentOperandIndexes[index] = 0;
                        increaseNextIndex = true;
                    }
                }
                firstExecution = false;
            }
            while (containsDuplicates(currentOperandIndexes));

            return true;
        }

        /// <summary>
        /// Returns true if the input array contains duplicate values, false otherwise
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns></returns>
        private bool containsDuplicates(int[] inputArray)
        {
            IEnumerable<int> removedDuplicates = inputArray.Distinct();
            if(inputArray.Length != removedDuplicates.Count())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method turns the operand indexes into their int equivalents 
        /// e.g
        /// for an input operand array of [5,10,15,20]
        /// operator indexes [0,0,3,1] would return [5,5,20,10]
        /// </summary>
        /// <returns>a List containing the current operand permutation</returns>
        public List<string> getCurrentOperandPermutation()
        {
            currentOperandPermutation = new List<string>();
            int operandIndex;
            string operandValue;
            for (int index = 0; index < currentOperandIndexes.Length; index++)
            {
                operandIndex = currentOperandIndexes[index];
                operandValue = allOperands[operandIndex];
                currentOperandPermutation.Add(operandValue);
            }
            return currentOperandPermutation;
        }
    }


    //this method fucks around with the ORDER of the operands
    //could still be useful
    //public bool nextOperandPermutation()
    //    {
    //        //Find the largest index K such that a[k] < a[K + 1].If no such index exists, the permutation is the last permutation.
    //        int indexK = 0;
    //        bool indexKfound = false;

    //        for (int index = 0; index < currentOperandPermutation.Count; index++)
    //        {
    //            if (index != currentOperandPermutation.Count - 1)
    //            {
    //                if (currentOperandPermutation[index] < currentOperandPermutation[index + 1] && index > indexK)
    //                {
    //                    indexK = index;
    //                    indexKfound = true;
    //                }
    //            }
    //        }

    //        if (!indexKfound)
    //        {
    //            return indexKfound;
    //        }

    //        //Find the largest index L greater than K such that a[K] < a[L].
    //        int indexL = 0;
    //        for (int index = 0; index < currentOperandPermutation.Count; index++)
    //        {
    //            if (currentOperandPermutation[index] > currentOperandPermutation[indexK] && index > indexL)
    //            {
    //                indexL = index;
    //            }
    //        }

    //        //Swap the value of a[k] with that of a[L].
    //        int storeK = currentOperandPermutation[indexK];
    //        currentOperandPermutation[indexK] = currentOperandPermutation[indexL];
    //        currentOperandPermutation[indexL] = storeK;

    //        //Reverse the sequence from a[L + 1] up to and including the final element a[N].
    //        List<int> sequence = new List<int>();
    //        for (int index = indexL + 1; index < currentOperandPermutation.Count; index++)
    //        {
    //            sequence.Add(currentOperandPermutation[index]);
    //        }
    //        sequence.Reverse();

    //        int numbersIndex = indexL + 1;
    //        foreach (int aNumber in sequence)
    //        {
    //            currentOperandPermutation[numbersIndex] = aNumber;
    //            numbersIndex++;
    //        }
    //        return indexKfound;
    //    }

    //    public List<int> getCurrentOperands()
    //    {
    //        return currentOperandPermutation;
    //    }
    //}
}
