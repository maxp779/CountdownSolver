using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CountdownSolver.Logic.NumbersGame
{
    public class CountdownNumbersCalculatorBruteForce
    {
        //exact calculations is for a future feature where close calculations are stored separately and delivered to the client
        //if no exact calculations are found that meet the target
        private static IProducerConsumerCollection<Stack<string>> exactCalculations;
        private List<string> operands;
        private string target;
        bool findAllSolutions;
        IProducerConsumerCollection<List<string>> operandLists;
        IProducerConsumerCollection<string> infixSolutions = new ConcurrentBag<string>();
        IProducerConsumerCollection<string> postfixSolutions = new ConcurrentBag<string>();

        public CountdownNumbersCalculatorBruteForce(List<string> operands, string target, bool findAllSolutions = true)
        {
            this.operands = operands;
            this.target = target;
            this.findAllSolutions = findAllSolutions;
        }

        public List<string> Calculate()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //collect all operand permutations
            operandLists = GetAllOperandPermutations();
            
            //create list of threads to deal with operand permutations
            List<Thread> threadList = CreateThreadList();

            //start each thread
            foreach (Thread aThread in threadList)
            {
                aThread.Start();
            }

            //wait for each thread to finish
            foreach (Thread aThread in threadList)
            {
                aThread.Join();
            }
            var elapsedMs = watch.ElapsedMilliseconds;
            return infixSolutions.ToList<string>();
        }

        private List<Thread> CreateThreadList()
        {
            int solutionsLimit = -1;
            if (!findAllSolutions)
            {
                solutionsLimit = CalculateSolutionsLimit();
            }

            int processorCount = Environment.ProcessorCount;
            int range = operandLists.Count / processorCount;
            List<Thread> threadList = new List<Thread>();
            int threadNumber = 0;
            for (int count = 0; count < processorCount; count++)
            {
                NumbersCalculatorThread currentNumbersCalculatorThread = new NumbersCalculatorThread(ref operandLists, ref infixSolutions, ref postfixSolutions, target, solutionsLimit);
                Thread thread = new Thread(delegate ()
                {
                    currentNumbersCalculatorThread.Start();
                });
                thread.Name = "NumbersCalculatorThread:" + threadNumber;
                threadList.Add(thread);
                threadNumber++;
            }
            return threadList;
        }

        private IProducerConsumerCollection<List<string>> GetAllOperandPermutations()
        {
            List<List<string>> operandLists = new List<List<string>>();
            OperandPermutator operandPermutator;
            for (int operandListSize = 2; operandListSize <= operands.Count; operandListSize++)
            {
                operandPermutator = new OperandPermutator(operands, operandListSize);

                while (operandPermutator.nextOperandPermutation())
                {
                    operandLists.Add(operandPermutator.getCurrentOperandPermutation());
                }
            }

            //remove duplicates, this is possible if duplicate numbers were supplied i.e [100, 10, 36, 36, 4, 9]
            IProducerConsumerCollection<List<string>> operandListsNoDuplicates = new ConcurrentBag<List<string>>();
            bool addList;
            foreach(List<string> outerList in operandLists)
            {
                addList = true;
                foreach (List<string> innerList in operandListsNoDuplicates)
                {
                    if(outerList.SequenceEqual(innerList))
                    {
                        addList = false;
                    }
                }

                if(addList)
                {
                    operandListsNoDuplicates.TryAdd(outerList);
                }
            }
            return operandListsNoDuplicates;
        }

        private int CalculateSolutionsLimit()
        {
            int processorCount = Environment.ProcessorCount;
            if(processorCount < 4)
            {
                return 10;
            }
            else if(processorCount < 8)
            {
                return 20;
            }
            else if(processorCount < 16)
            {
                return 30;
            }
            return -1; //-1 = find all solutions        
        }
    }
}
