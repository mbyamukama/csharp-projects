using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AppExtensions.Numeric
{
    /// <summary>
    /// Generates random numbers with high entropy
    /// </summary>
    public class Random
    {
        RNGCryptoServiceProvider rng;
        private List<byte> randNums = null;
        public int index;
        public Random()
        {
            Initialize();   
        }
        private void Initialize()
        {
            index = 0;
            rng = new RNGCryptoServiceProvider();
            byte[] num = new byte[10000];
            rng.GetBytes(num);
            randNums = num.ToList();
        }
        /// <summary>
        /// Gets the next random number
        /// </summary>
        /// <param name="min"> the inclusive lower bound for the random number</param>
        /// <param name="max">the inclusive upper bound for the random number</param>
        /// <returns></returns>
        public int Next(int min, int max)
        {
            int num = 0;
            bool found = false;
            while (index < randNums.Capacity & found != true)
            {
                if (max > 255)
                {
                    num = (int)(randNums[index] * max / 255);
                }
                else { num = randNums[index]; }
                if (num < min | num > max)
                {
                    index++;
                }
                else
                {
                    found = true;
                    index++;  //will start at next value if method is recalled
                }
                if (index >= randNums.Capacity)
                {
                    Initialize(); //reset the index to 0
                }
            }
            return num;
        }
        /// <summary>
        /// generates an array of random numbers
        /// </summary>
        /// <param name="arraySize">the size of the array</param>
        /// <param name="min">the inclusive minima of the generated numbers</param>
        /// <param name="max">the inclusive maxima of the generated numbers</param>
        /// <returns></returns>
        public T [] RandomArray<T>(int arraySize, int min, int max)
        {
            T [] array = new T[arraySize];
            for (int j = 0; j < arraySize; j++)
            {
                array[j] = (T)Convert.ChangeType(Next(min, max), typeof(T));
            }
            return array;
        }
        /// <summary>
        /// returns a random sample of an array
        /// </summary>
        /// <param name="array">the array to be sampled</param>
        /// <param name="sampleSize">the size of the sample</param>
        /// <returns></returns>
        public byte[] RandomSample(byte[] array, int sampleSize, int maxVal)
        {
            int arrLen = array.Length;
            byte[] sample = new byte[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                sample[i] = (byte)(array[(byte)Next(0, array.Length - 1)] * maxVal / 255);
            }
            return sample;

        }
        /// <summary>
        /// returns a random string of concatenated two-digit byte values based on a source string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string RandomString(string s, int sampleSize)
        {
            byte[] nameb = ASCIIEncoding.ASCII.GetBytes(s);
            string str = "";
            byte[] rsample = RandomSample(nameb, sampleSize, Next(41, 99));
            foreach (byte b in rsample)
            {
                str +=b;
            }
            return str;
        }
        public string RandomNumericString(int sampleSize)
        {
            string x = "";
            for (int i = 0; i < sampleSize; i++)
            {
                x += Next(0, 9);
            }
            return x;
        }
    }
}
