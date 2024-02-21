using System;

namespace SCNatwestDecisionMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            int indicativeRate = 4966, natwestRate = 5113, initialAmount = 100000, TTFee = 0000;
            string verdict = "transfer";

            Console.WriteLine("AMOUNT\t\tCAN WDRAW\tCAN TX\t\tVERDICT");
            while (initialAmount<3000000)
            {
                double canWithdraw = Math.Round(initialAmount*1.0 / natwestRate,2);
                double canTransfer = Math.Round((initialAmount - TTFee)*1.0 / indicativeRate, 2);

                if (canTransfer > canWithdraw) verdict = "transfer";
                else verdict = "withdraw";

                Console.WriteLine(initialAmount + "\t\t" + canWithdraw + "\t\t" + canTransfer + "\t\t" + verdict);
                initialAmount += 20000;

                
            }
            Console.ReadLine();
        }
    }
}
