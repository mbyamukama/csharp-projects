namespace TopTalTests
{
    internal class Program
    {

        public static string[] solution(string S, string[] B)
        {
            double s = Convert.ToDouble(S);
            List<double> depts = new List<double>();
            List<double> R = new List<double>(depts.Count);
            foreach ( string item in B)
            {
                depts.Add(Convert.ToDouble(item));
            }
            List<double> deptsCopy = new List<double>(depts);

            while (depts.Count > 0)
            {
                //get the highest
                double currMax = depts.Max();
                double v = Math.Truncate(100 * (s * currMax / depts.Sum())) / 100;
                int index = deptsCopy.IndexOf(depts.Max());
                //add to R
                R.Insert(index, v);
                //set remainder
                s = s - v;
                //remove it
                depts.Remove(currMax);
            }
            List<string> Rs = new List<string>();
            foreach(double item in R)
            {
                Rs.Add(item.ToString("F"));
            }
            return Rs.ToArray();
        }
        static void Main(string[] args)
        {
            string [] d = solution("300.01", new string[] { "300.00","200.00","100.00"});
            foreach(string i in d)
            {
                Console.Write(i + ",");
            }
        }
    }
}