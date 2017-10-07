using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinwheelLib;

namespace Pinwheel
{
    class Program
    {
        static String[] grid1 = new String[] {
                @"   w)  wb)b w w",
                @"w w) w)b bwww v",
                @"vwb  bwv w \/w)",
                @"  v b) b)  bw)w",
                @"   bw b bb v  v",
                @" bbwwbv vb)wb) ",
                @"bv \/wb   w b)b",
                @"v  b  vw)  w b ",
                @" b b) w)wb) b) ",
                @" b) w b)w) b b)",
                @" b bvwwbbbwbb  ",
                @"w)bv \/\/wv\/b ",
                @"w w    w w)  vw",
                @"  ww) b  bb) w)",
                @"w)v  ww  v  w) "
            };

        static string[] grid10 = new string[]
        {
            @" w)   bw)ww    ",
            @"ww) bb)b \/w) w",
            @" wbbv   bw   wv",
            @"wv\/w)b)b w)w  ",
            @" b wbw b b)  w)",
            @"b)bb vwv b bb)b",
            @"w)\/b)bw v vbb ",
            @"ww) bw) w   \/b",
            @"v wb bww wwwb) ",
            @" w) bb)vw\/vw)b",
            @" w w bbbv w)b) ",
            @" w vw v b b)bb ",
            @"w)  v  bb b vww",
            @" ww b b\/wb bb ",
            @"w\/ w)   bvbww)",
        };

        static void Main(string[] args)
        {
            pwCell.Initialize(grid10);
            pwCell.Dump();
            int i = 1;
            while (i > 0)
            {
                i = pwCell.Process();
                Console.WriteLine($"Did a line and had {i} changes");
            }
            pwCell.Dump();
            pwCell.DumpFill();
        }
    }
}
