namespace TopTalTests
{
    internal class Program
    {

        public static int solution(int X, int[] B, int Z)
        {
            // write your code in C# 6.0 with .NET 4.5 (Mono)
            //get sum of B= downloaded bytes
            try
            {
                int downloaded = 0;
                for (int i = 0; i < B.Length; i++)
                {
                    downloaded += B[i];
                }
                int remaining = X - downloaded;
                if (remaining == 0)
                {
                    return 0;
                }
                else
                {
                    //average download rate over last Z minutes. Get last Z elements
                    int startIndex = B.Length - Z;
                    double average = 0;
                    for (int i = startIndex; i < B.Length; i++)
                    {
                        average += B[i];
                    }
                    average = average * 1.0 / Z;

                    return (int)Math.Ceiling(remaining / average);
                }
            }
            catch
            {
                return -1;
            }
        }
    
        static void Main(string[] args)
        {
            int d = solution(100, new int [] { 10,6,6,8}, 2);
            Console.WriteLine(d);
        }
    }
}