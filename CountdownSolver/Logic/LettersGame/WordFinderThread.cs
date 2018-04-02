using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace CountdownSolver.Logic.LettersGame
{
    public class WordFinderThread
    {
        private IProducerConsumerCollection<string> wordsFound;
        private ICollection<char> letters;
        private IProducerConsumerCollection<string> dictionaryBag;

        public WordFinderThread(ref IProducerConsumerCollection<string> dictionaryBag, ref IProducerConsumerCollection<string> wordsFound, ref ICollection<char> letters, ref ICollection<string> dictionary)
        {
            this.wordsFound = wordsFound;
            this.letters = letters;
            this.dictionaryBag = dictionaryBag;
        }

        public void start()
        {
            bool addWord;
            char[] currentWordArray;
            string currentWord;
            while (dictionaryBag.TryTake(out currentWord))
            {
                currentWordArray = currentWord.ToCharArray();
                List<char> currentLetters = letters.ToList();

                addWord = true;

                foreach (char currentLetter in currentWordArray)
                {
                    if (currentLetters.Contains(currentLetter))
                    {
                        currentLetters.Remove(currentLetter);
                    }
                    else
                    {
                        addWord = false;
                        break;
                    }
                }

                if (addWord)
                {
                    wordsFound.TryAdd(currentWord);
                }
            }
        }
    }
}
