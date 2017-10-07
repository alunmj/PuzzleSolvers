using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nonalib;
using System.Threading;

namespace Nonatest
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] constraingH = new string[]
            {
/*                "8",
                "4 12 3",
                "2 1 14 1 2",
                "1 1 15 1 1",
                "1 3 11 4 3 1",

                "1 1 1 11 3 1 1 1",
                "1 1 11 3 1 1",
                "1 1 8 2 1 2",
                "2 2 6 4 4 2 1",
                "1 1 4 2 4 1 2",

                "5 4 1 2 4",
                "5 5 1 1 4",
                "5 3 3 1 4",
                "5 3 2 2 3 5",
                "5 7 2 2 2 5",

                "5 6 4 3 6",
                "5 8 4 6",
                "14 12",
                "15 12",
                "16 12",

                "30",
                "29",
                "5 15 5 3",
                "3 14 3 5",
                "13 3 1",

                "13 17",
                "13 2 1 2 2 1 1",
                "23 2 3 2 1",
                "1 4 2 2 1",
                "2 8 1 3 5 2",

                "1 4 2 1 4 3",
                "2 1 1 6 1 1 5 3 1",
                "1 3 4 1 4 7 1",
                "2 1 1 2 4 3 1",
                "1 3 3 1 4 1 2",

                "1 2 2 2 2 1 4 1 2",
                "1 2 2 1 2 6 3",
                "2 6 2 3 7",
                "2 2 3 2",
                "12 14"*/
                "5",
                "7",
                "7",
                "1 1 2 2",
                "1 3 3",
                "3 2 4",
                "2 4 4",
                "1 2 1",
                "1 1 3 1",
                "2 1 7",
                "6 2",
                "1 2 1",
                "2 1 1 1 2",
                "2 4 2 1",
                "2 4 1 1 2 1",
                "6 4 5",
                "4 3 1 5",
                "2 1 2 1 1 1",
                "4 4 6",
                "4"
            };
            int[][] constraintH = new int[constraingH.Count()][];
            for (int i = 0; i < constraingH.Count(); i++)
                constraintH[i] = constraingH[i].Split(new char[] { ' ', ',' }).Select(x => int.Parse(x)).ToArray();
            String[] constraingV = new string[]
            {
                /*"7 3",
                "2 12 3",
                "1 12 3",
                "1 13 3 3",
                "5 14 1 3 2 2",

                "1 16 1 1 1 1 2",
                "5 7 1 6 1",
                "7 1 2 2 1",
                "8 2 1 1 2 1",
                "4 10 3 1 2 1 1",

                "9 15 1 2 1 1",
                "10 15 1 2 1 1",
                "12 14 1 2 1 1",
                "8 3 14 1 1 2 1",
                "9 4 12 2 2 1",

                "8 1 10 6 1",
                "9 1 9 1 2",
                "7 2 2 8 2 2",
                "7 2 2 1 8 3",
                "7 1 2 1 8",

                "6 1 2 8 4",
                "4 3 2 7 1 2 2",
                "4 2 2 8 1 1 1",
                "6 4 11 9 2",
                "9 8 4 3 1",

                "5 8 1 3 2 2",
                "8 1 3 3 1",
                "7 1 3 3 1",
                "8 1 2 2 1",
                "8 3 3 3 1",

                "5 9 1 2 2 2 1",
                "1 13 4 2 3 2 1",
                "4 10 6 1 6 1",
                "1 9 4 1 1 2 1 1",
                "1 7 2 3 1 1 1 1",

                "2 3 4 2 2 2 1",
                "6 1 2 1 1 1",
                "3 2 2 1",
                "1 2 2 2",
                "10 1"*/
                "5",
                "6",
                "3",
                "3 2",
                "2 2",
                "4 3 2",
                "2 6 2",
                "2 2 1 1",
                "4 2 3 1 2 1",
                "3 1 3 5",
                "4 1 3 2 1",
                "3 2 3 4 2",
                "6 3 1 2",
                "4 4 1",
                "3 2 3 1",
                "2 4 4",
                "6 2 2 1",
                "3 5",
                "2 2 1",
                "2"
            };
            int[][] constraintV = new int[constraingV.Count()][];
            for (int i = 0; i < constraingV.Count(); i++)
                constraintV[i] = constraingV[i].Split(new char[] { ' ', ',' }).Select(x => int.Parse(x)).ToArray();
            /* 001                new int[][] {
                                new int[]{4 },
                                new int[]{1,2 },
                                new int[]{3,7 },
                                new int[]{1,1,1,1,1 },
                                new int[] {1,1,1,3 },
                                new int[] {2,2,2,3 },
                                new int[] {8,1,2 },
                                new int[] {5,1,3,2 },
                                new int[] {9,1 },
                                new int[] {2,1,1,2,1 },
                                new int[] {4,1,2,1 },
                                new int[] {10,1 },
                                new int[] {1,1,2 },
                                new int[] {13 },
                                new int[] {8 }
                        };
            {
                new int[] {6 },
                new int[] {1,3,3 },
                new int[] {3,2,3,2,},
                new int[] {6,2 },
                new int[] {8,2,2 },
                new int[] {1,1,2,4,2 },
                new int[] {1,1,3,1,2 },
                new int[] {1,1,2,1,1,2 },
                new int[] {6,5,2 },
                new int[] {2,1,3,4 },
                new int[] { 4,5,1 },
                new int[] {1,3,1 },
                new int[] {2,1 },
                new int[] {2,2 },
                new int[] {6}
            };
                        */
            Row[] rows = new Row[constraintH.Count()];
            Row r;
            Parallel.For(0, rows.Count(), (i) =>
            {
                rows[i] = new Row(constraintV.Count(), constraintH[i]);
            });
            bool bIncomplete = true;
            int changes = 1;
            int iterations = 0;
            while (bIncomplete && changes != 0)
            {
                iterations++;
                changes = 0;
                Parallel.For(0, rows.Count(), (i) =>
                  {
                      Interlocked.Add(ref changes, rows[i].SimpleStep());
                  });
                bIncomplete = false;
                rows = Row.Transpose(constraintV, rows);
                Parallel.For(0, rows.Count(), (i) =>
                 {
                     Interlocked.Add(ref changes, rows[i].SimpleStep());
                     if (!rows[i].IsComplete())
                         bIncomplete = true;
                 });
                rows = Row.Transpose(constraintH, rows);
            }
            if (changes == 0) Console.WriteLine("Stuck!");
            Console.WriteLine("{0} iterations", iterations);
            foreach (Row row in rows)
                Console.WriteLine(row.ToString());
        }
    }
}
