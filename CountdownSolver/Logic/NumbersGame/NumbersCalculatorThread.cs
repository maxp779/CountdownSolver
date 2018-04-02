using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CountdownSolver.Logic.NumbersGame
{
    public class NumbersCalculatorThread
    {
        //passed in externally
        IProducerConsumerCollection<string> outputInfixSolutions;
        IProducerConsumerCollection<string> outputPostfixSolutions;
        IProducerConsumerCollection<List<String>> operandList;
        int target;

        //created in this thread
        List<List<string>> currentPostfixSolutions;
        List<string> currentClosestCalculation;
        int currentClosestTarget;
        bool findMoreSolutions = true;
        int solutionsLimit;

        public NumbersCalculatorThread(ref IProducerConsumerCollection<List<String>> operandList, ref IProducerConsumerCollection<string> infixSolutionsCollection, ref IProducerConsumerCollection<string> postfixSolutionsCollection, 
            string target, int solutionsLimit = -1)
        {
            this.operandList = operandList;
            this.outputInfixSolutions = infixSolutionsCollection;
            this.outputPostfixSolutions = postfixSolutionsCollection;

            this.target = int.Parse(target);
            currentPostfixSolutions = new List<List<string>>();
            this.solutionsLimit = solutionsLimit;
        }

        public void Start()
        {
            IterateThroughOperands();
        }

        /// <summary>
        /// Iterate through each operand permutation
        /// </summary>
        private void IterateThroughOperands()
        {
            List<string> currentOperandList;
            while (operandList.TryTake(out currentOperandList))
            {
                IterateThroughAllOperators(currentOperandList);
            }
            ConvertToInfix(currentPostfixSolutions, outputInfixSolutions);
        }

        /// <summary>
        /// Iterate through each operator set with the current operand permutation. For each operator List generated IterateThroughAllCalculations() is called.
        /// </summary>
        /// <param name="currentOperandList"></param>
        private void IterateThroughAllOperators(ICollection<string> currentOperandList)
        {
            OperatorPermutator operatorPermutator = new OperatorPermutator(currentOperandList.Count-1); //if 6 operands we need 5 operators etc so always -1
            while (operatorPermutator.NextOperatorPermutation() && findMoreSolutions)
            {
                ICollection<string> currentOperators = operatorPermutator.GetCurrentOperatorPermutation();
                IterateThroughAllCalculations(currentOperandList, currentOperators);
            }
        }

        /// <summary>
        /// Iterate through each possible valid calculation permutation with the current operator and operand permutations. For each permutation SolveCalculation() is called
        /// to ascertain if this calculation hits the target number.
        /// </summary>
        /// <param name="currentOperandList"></param>
        /// <param name="currentOperators"></param>
        private void IterateThroughAllCalculations(ICollection<string> currentOperandList, ICollection<string> currentOperators)
        {
            CalculationPermutator calculationPermutator = new CalculationPermutator(currentOperandList, currentOperators);
            while (calculationPermutator.nextCalculationPermutation() && findMoreSolutions)
            {
                List<string> currentCalculation = calculationPermutator.getCurrentCalculationPermutation();
                SolveCalculation(currentCalculation);
            }
            
            //for performance reasons it is best to limit the amount of solutions to find in cases where processor count is low
            if (currentPostfixSolutions.Count >= solutionsLimit && solutionsLimit != -1)
            {
                findMoreSolutions = false;
            }
        }

        /// <summary>
        /// Calculate if the current calculation permutation hits the target or not.
        /// If it does not, check to see if it beats the current closest calculation.
        /// </summary>
        /// <param name="currentCalculationStack"></param>
        private void SolveCalculation(List<string> currentCalculation)
        {
            currentCalculation.Reverse();
            Stack<string> currentCalculationStack = new Stack<string>(currentCalculation);
            Stack<int> currentInts = new Stack<int>();

            bool calculating = true;
            bool validCalculation = true;
            while (calculating && validCalculation)
            {
                if (currentCalculationStack.Count == 0 && currentInts.Count == 1)
                {
                    calculating = false;
                }
                else
                {
                    string currentStackString = currentCalculationStack.Pop();
                    int currentStackNumber;
                    if (int.TryParse(currentStackString, out currentStackNumber))
                    {
                        currentInts.Push(currentStackNumber);
                    }
                    else if (currentStackString == "+")
                    {
                        Add(currentInts);
                    }
                    else if (currentStackString == "-")
                    {
                        //it is invalid if negative numbers are used at any point
                        validCalculation = Subtract(currentInts);
                    }
                    else if (currentStackString == "*")
                    {
                        Multiply(currentInts);
                    }
                    else if (currentStackString == "/")
                    {
                        //it is invalid if the numbers do not divide evenly
                        validCalculation = Divide(currentInts);
                    }
                }
            }

            //result will be the final element in the stack
            if (validCalculation)
            {
                //int result = int.Parse(currentCalculationStack.Pop());
                int result = currentInts.Pop();
                if (result == target)
                {
                    currentCalculation.Reverse();
                    currentPostfixSolutions.Add(currentCalculation);
                    outputPostfixSolutions.TryAdd(ListToString(currentCalculation));
                }
                else //it does not hit the target so check if it beats the current closest calculation
                {
                    //Math.Abs removes the issue with negative numbers
                    int currentDifference = target - result;
                    int closestDifference = target - currentClosestTarget;
                    if (Math.Abs(currentDifference) < Math.Abs(closestDifference) || currentClosestTarget == 0)
                    {
                        currentClosestTarget = result;
                        currentCalculation.Reverse();
                        currentClosestCalculation = currentCalculation;
                    }
                }
            }
        }

        private void Add(Stack<int> currentInts)
        {
            int secondOperand = currentInts.Pop();
            int firstOperand = currentInts.Pop();
            int result = firstOperand + secondOperand;
            currentInts.Push(result);
        }
        private bool Subtract(Stack<int> currentInts)
        {
            int secondOperand = currentInts.Pop();
            int firstOperand = currentInts.Pop();
            int result = firstOperand - secondOperand;
            if (result >= 0)
            {
                currentInts.Push(result);
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Multiply(Stack<int> currentInts)
        {
            int secondOperand = currentInts.Pop();
            int firstOperand = currentInts.Pop();
            int result = firstOperand * secondOperand;
            currentInts.Push(result);
        }
        private bool Divide(Stack<int> currentInts)
        {
            int secondOperand = currentInts.Pop();
            int firstOperand = currentInts.Pop();

            if (firstOperand == 0 || secondOperand == 0)
            {
                return false;
            }

            if (firstOperand % secondOperand == 0)
            {
                int result = firstOperand / secondOperand;
                currentInts.Push(result);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ConvertToInfix(List<List<string>> postfixSolutions, IProducerConsumerCollection<string> infixSolutions)
        {
            //List<string> infixSolutions = new List<string>();
            foreach (List<string> postfixSolution in postfixSolutions)
            {
                Stack<string> infixStack = new Stack<string>();
                for (int index = 0; index < postfixSolution.Count; index++)
                {
                    string currentElement = postfixSolution.ElementAt(index);
                    if (!IsOperator(currentElement))
                    {
                        infixStack.Push(currentElement);
                    }
                    else
                    {
                        if (infixStack.Count < 2)
                        {
                            //invalid postfix expression
                        }
                        else
                        {
                            string first = infixStack.Pop();
                            string second = infixStack.Pop();
                            string expression = "(" + second + " " + currentElement + " " + first + ")";
                            infixStack.Push(expression);
                        }
                    }
                }
                if (infixStack.Count == 1)
                {
                    string finalExpression = infixStack.Pop();
                    finalExpression = finalExpression.Remove(0, 1);
                    finalExpression = finalExpression.Remove(finalExpression.Length - 1, 1);
                    infixSolutions.TryAdd(finalExpression);
                }
                else
                {
                    //invalid postfix expression
                }
            }
        }

        private string[] operators = new string[] { "+", "-", "*", "/" };
        private bool IsOperator(string input)
        {
            return operators.Contains(input);
        }

        private string ListToString(List<string> input)
        {
            StringBuilder output = new StringBuilder();

            foreach(string aString in input)
            {
                output.Append(aString);
            }
            return output.ToString();
        }
    }
}
