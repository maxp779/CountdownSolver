using System.Collections.Generic;
using System.Linq;

namespace CountdownSolver.Logic.NumbersGame
{
    public class CalculationPermutator
    {
        ICollection<string> operands;
        ICollection<string> operators;
        List<string> currentCalculation;

        public CalculationPermutator(ICollection<string> inputOperands, ICollection<string> inputOperators)
        {
            this.operands = new List<string>(inputOperands);
            this.operators = new List<string>(inputOperators);
            this.currentCalculation = new List<string>(inputOperands);
            foreach(string anOperator in inputOperators)
            {
                currentCalculation.Add(anOperator);
            }
        }

        bool firstExecution = true;
        int currentOperatorToMove = 1; //1= first 2=second etc
        public bool nextCalculationPermutation()
        {
            if(currentCalculation.Count >= 4)
            {

            }

            /**
             * this is done so an initial permutation such as
             * 5 10 15 + +
             * will be returned once
             **/
            if (firstExecution)
            {
                firstExecution = false;
                return true;
            }

            /**
             * the final operator must remain at the END of the permutation
             * if this is the currentOperatorToMove then either it is the ONLY operator
             * e.g 5 10 + (this has no other permutations)
             * or all other operators that can be moved have been moved so we are finished with this
             * operand/operator set
             **/
            //if (currentOperatorToMove == operators.Count)
            //{
            //    return false;
            //}

            //find the operator
            int currentIndex = 0;
            int currentOperatorCount = 0;
            while (currentOperatorCount != currentOperatorToMove)
            {
                string currentElement = currentCalculation.ElementAt(currentIndex);

                //if we have an operator
                if (isOperator(currentElement))
                {
                    currentOperatorCount++;
                    //if it is the correct operator to move
                    if (currentOperatorCount == currentOperatorToMove)
                    {
                        //switch it with the operand to the left IF it does not invalidate the calculation
                        if(isValidSwitch(currentIndex))
                        {
                            switchCalculationElements(currentIndex, currentIndex - 1);
                        }
                        else if(currentOperatorToMove == operators.Count)//we are at the final operator which cannot be moved
                        {
                            return false;
                        }
                        else //the current operator has been moved as far as possible, time to move the next one
                        {
                            currentOperatorToMove++;
                        }
                    }
                }
                currentIndex++;
            }
            return true;
        }

        private void switchCalculationElements(int firstIndex, int secondIndex)
        {
            string store = currentCalculation.ElementAt(firstIndex);
            currentCalculation[firstIndex] = currentCalculation[secondIndex];
            currentCalculation[secondIndex] = store;
        }

        private bool isValidSwitch(int operatorIndex)
        {
            int operandCount = 0;
            int operatorCount = 0;
            for(int index = 0; index <= operatorIndex; index++)
            {
                string currentElement = currentCalculation.ElementAt(index);
                if(isOperator(currentElement))
                {
                    operatorCount++;
                }
                else
                {
                    operandCount++;
                }
            }        
            //if operandCount WAS equal to operatorCount+1 then a switch would invalidate this permutation
            return operandCount != (operatorCount + 1);
        }

        private bool isOperator(string aString)
        {
            return operators.Contains(aString);
        }

        public List<string> getCurrentCalculationPermutation()
        {
            return new List<string>(currentCalculation);
        }
    }
}
