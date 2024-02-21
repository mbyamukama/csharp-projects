using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetSalaryCalculator
{
    static class Program
    {
        static int [] GetNetSalary(int gross)
        {
            int tax = 0;
            int extraTax = 0;
            int nssf = 5 * gross / 100;
            if (gross > 235000 && gross < 335000)
            {
                tax = (int)(0.1 * (gross - 235000));
            }
            if (gross > 335000 && gross < 410000)
            {
                tax = 10000 + (int)(0.2 * (gross - 335000));
            }
            if (gross > 410000 && gross < 10000000)
            {
                tax = (int)(25000 + 0.3 * (gross - 410000));
            }
            if (gross > 10000000)
            {
                tax = (int)(25000 + 0.3 * (gross - 410000));
                extraTax = (int)(0.1 * (gross - 10000000));
            }
            int net = gross - tax - extraTax - nssf;
            return new int[] { tax, extraTax, net , nssf};
        }

        static void Main()
        {
            Console.Write("Enter 1 for NET SALARY and 2 FOR GROSS:\t");
            int option = Int32.Parse(Console.ReadLine());
            if(option==1)
            {
            Console.Write("Enter GROSS:\t");
            int gross = Int32.Parse(Console.ReadLine());
            
            Console.WriteLine("\nTAX:\t" + GetNetSalary(gross)[0]);
            Console.WriteLine("\nEXTRA TAX:\t" + GetNetSalary(gross)[1]);
            Console.WriteLine("\nNSSF:\t{0}\n", GetNetSalary(gross)[3]);
            Console.WriteLine("\nNET:\t{0}\n", GetNetSalary(gross)[2]);
            }
            else if(option==2)
            {
                Console.Write("Enter NET:\t");
                int net = Int32.Parse(Console.ReadLine());

                int grossEst = net * 10 / 7;
                while (Math.Abs(net - GetNetSalary(grossEst)[2]) > 100)
                {
                    grossEst -= 200;
                }
                Console.WriteLine("\nGROSS:\t{0}\n",grossEst);
            }
            Console.ReadLine();

        }
    }
}
