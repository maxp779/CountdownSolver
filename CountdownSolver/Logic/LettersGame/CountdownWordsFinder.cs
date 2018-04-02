using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CountdownSolver.Logic.LettersGame
{
    public class CountdownWordsFinder
    {
        private static ICollection<string> dictionary;
        IProducerConsumerCollection<string> dictionaryBag;
        private ICollection<char> letters;
        private IProducerConsumerCollection<string> wordsFound = new ConcurrentBag<string>();

        public static void PopulateWordList()
        {
            dictionary = File.ReadLines(@"Wordlist\words_alpha.txt").ToList().AsReadOnly();
        }

        public ICollection<string> FindAllWords(string inputLetters)
        {
            if (inputLetters != null)
            {
                letters = inputLetters.ToList().AsReadOnly();
                StartThreads();
                return wordsFound.ToList();
            }
            else
            {
                return wordsFound.ToList();
            }
        }

        private void StartThreads()
        {
            int processorCount = Environment.ProcessorCount;
            dictionaryBag = new ConcurrentBag<string>(dictionary);
            List<Thread> threadList = new List<Thread>();

            for (int count = 0; count < processorCount; count++)
            {        
                WordFinderThread currentWordFinder = new WordFinderThread(ref dictionaryBag, ref wordsFound, ref letters, ref dictionary);
                threadList.Add(new Thread(() => currentWordFinder.start()));         
            }

            foreach (Thread aThread in threadList)
            {
                aThread.Start();
            }

            foreach (Thread aThread in threadList)
            {
                aThread.Join();
            }
        }
    }
}
