using System;
using System.Diagnostics;

class Program
{
    const int N = 100;
    const long maxMoney = (long)1e14 + 1;
    static long[,] f = new long[N, N];
    static long money;
    static int rem;

    static bool Ask(long x)
    {
        Console.WriteLine("? " + x);
        Debug.Assert(x < maxMoney && --rem >= 0);

        string response = Console.ReadLine();
        if (response[0] == 'L')
        {
            money += x;
            return true;
        }
        else
        {
            money -= x;
            Debug.Assert(money >= 0);
            return false;
        }
    }

    static void InitializeTable()
    {
        
        for (int a = 0; a < N - 1; a++)
        {
            for (int k = 0; k < N - 1; k++)
            {
                if (a == 0)
                {
                    f[a, k] = 1;
                }
                else if (k == 0)
                {
                    f[a, k] = f[a - 1, k + 1];
                }
                else
                {
                    f[a, k] = f[a - 1, k - 1] + f[a - 1, k + 1];
                }
            }
            if (f[a, 0] >= maxMoney) break;
        }
    }


    static void SearchAndSolve()
    {
        long nowL = 1, nowR = maxMoney;
        rem = N;
        money = 1;

        
        while (nowL < maxMoney && Ask(nowL))
        {
            nowL <<= 1; 
            
        }

        if (money == 0)
        {
            nowR = nowL;
        }

        nowL >>= 1; 

        if (nowL == 0)
        {
            Console.WriteLine("! 0");
            return;
        }

        int a = 0, k = 0;

        while (f[a, k] < nowR - nowL)
        {
            a++;
        }

        while (nowR - nowL > 1)
        {
            Debug.Assert(a >= 0 && k >= 0);
            Debug.Assert(f[a, k] >= nowR - nowL);

            while (money < (nowR - nowL) + nowL * k)
            {
                Ask(nowL);
            }

            long bestMid = nowL + (k != 0 ? f[a - 1, k - 1] : 0);
            bestMid = Math.Min(bestMid, nowR - 1);

            if (Ask(bestMid))
            {
                a--;
                k++;
                nowL = bestMid;
            }
            else
            {
                a--;
                k--;
                nowR = bestMid;
            }
        }

        Console.WriteLine("! " + nowL);
    }

    static void Main(string[] args)
    {
        
        InitializeTable();

        
        int testCount = int.Parse(Console.ReadLine());

        for (int testCase = 0; testCase < testCount; testCase++)
        {
            
            SearchAndSolve();
        }
    }
}
