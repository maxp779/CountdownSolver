using System.Collections.Generic;
using System.Linq;

namespace CountdownSolver.Logic.NumbersGame
{
    public class OperatorPermutator
    {
        private ICollection<string> operators = new List<string> { "+", "-", "*", "/" };
        private ICollection<string> operatorPermutation;

        private int[] currentOperatorIndexes;
        private int operatorCount;
        bool firstExecution = true;

        //we will use a base 4 number system since there are always 4 operators
        private int maxOperatorIndex = 3;

        public OperatorPermutator(int numberOfOperatorsRequired)
        {
            operatorCount = numberOfOperatorsRequired;
            currentOperatorIndexes = new int[numberOfOperatorsRequired];
        }


        bool increaseNextIndex = false;
        /// <summary>
        /// Gets the next operator permutation e.g if the current one is
        /// [+,+,+] then [+,+,-] will be the next one.
        /// 
        /// This method accounts for the initial first permutation and will not
        /// skip it.
        /// </summary>
        /// <returns>true if there are more operator permutations, false if all have been iterated through</returns>
        public bool NextOperatorPermutation()
        {
            //increase the last index unless first run
            if (!firstExecution)
            {
                currentOperatorIndexes[currentOperatorIndexes.Length - 1] = currentOperatorIndexes[currentOperatorIndexes.Length - 1] + 1;
            }

            for (int index = currentOperatorIndexes.Length - 1; index >= 0; index--)
            {
                //increaseNextIndex will have been set while the loop was on the PREVIOUS index to be actioned for THIS index
                //so we dont use currentOperatorIndexes[index -1]
                if (increaseNextIndex)
                {
                    currentOperatorIndexes[index] = currentOperatorIndexes[index] + 1;
                    increaseNextIndex = false;
                }

                //if we have been through all possible combinations
                if (index == 0 && currentOperatorIndexes[index] == maxOperatorIndex + 1)
                {
                    return false;
                }
                else if (currentOperatorIndexes[index] == (maxOperatorIndex+1))
                {
                    currentOperatorIndexes[index] = 0;
                    increaseNextIndex = true;
                }
            }

            firstExecution = false;
            return true;
        }

        /// <summary>
        /// This method turns the operator indexes into their char equivalents 
        /// e.g
        /// operator indexes [0,0,3] would return [+,+,/]
        /// </summary>
        /// <returns>a List containing the current operator permutation e.g [+,+,/]</returns>
        public ICollection<string> GetCurrentOperatorPermutation()
        {
            operatorPermutation = new List<string>();
            int operatorIndex;
            string operatorValue;
            for (int index = 0; index < currentOperatorIndexes.Length; index++)
            {
                operatorIndex = currentOperatorIndexes[index];
                operatorValue = operators.ElementAt(operatorIndex);
                operatorPermutation.Add(operatorValue);
            }
            return operatorPermutation;
        }
    }
}
