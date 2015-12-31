using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonalib
{
    enum CellValue { Nothing, Unset, Filled, Empty, Unknown};
    public class Row
    {
        private int _width; // Width of grid row
        private int _cwidth; // Minimum width of constraints (including gaps)
        private int _count; // Count of constraints (not including gaps)
        private int[] _constraints;
        private CellValue[] _contents;
        public void Fill(int i) { _contents[i] = CellValue.Filled; } // Does not check for valid - SimpleStep will do that.
        public void Empty(int i) { _contents[i] = CellValue.Empty; }
        public bool IsComplete() { return _contents.FirstOrDefault(i => CellValue.Unset == i) == CellValue.Nothing; }
        public Row(int width, int[] constraints)
        {
            _width = width;
            _count = constraints.Count();
            _contents = Enumerable.Repeat(CellValue.Unset, width).ToArray(); // Why I can't just  "= new CellValue[width] {Unset}"
            _constraints = (int[])constraints.Clone();
            _cwidth = _constraints.Sum() + _count - 1;
        }
        public void SimpleStep()
        {
            // TODO: return array of column numbers affected?
            // Apply constraints with offsets iteratively until we can assign blocks.
            // So, we're taking something like "1 1 2" on a width of 10 and trying (blobs & offsets shown):
            // X-X-XX---- (0 1 1) X-X--XX--- (0 1 2) X-X---XX-- (0 1 3) X-X----XX- (0 1 4) X-X-----XX (0 1 5)
            // X--X-XX--- (0 2 1) X--X--XX-- (0 2 2) X--X---XX- (0 2 3) X--X----XX (0 2 4)
            // X---X-XX-- X---X--XX- X---X---XX
            // X----X-XX- X----X--XX
            // X-----X-XX
            // -X-X-XX--- -X-X--XX-- -X-X---XX- -X-X----XX
            // -X--X-XX-- -X--X--XX- -X--X---XX
            // -X---X-XX- -X---X--XX
            // -X----X-XX
            // --X-X-XX-- --X-X--XX- --X-X---XX
            // --X--X-XX- --X--X--XX
            // --X---X-XX
            // ---X-X-XX- ---X-X--XX
            // ---X--X-XX
            // ----X-X-XX
            int[] offsets = new int[_count];
            offsets[0] = 0; // Only the first offset can be 0.
            for (int i = 1; i < _count; ++i) offsets[i] = 1;
            offsets[_count - 1] = 0; // Slippery trick. Set the last offset to zero, so we can pre-increment.
            CellValue[] imagined = (CellValue[])Enumerable.Repeat(CellValue.Unset, _width).ToArray(); // Clear row
            while (offsets[0] < _width - _cwidth)
            {
                // Pre-increment, so we can call "continue" at the end - this means our initial state must be "-1".
                for (int j = _count - 1; j >= 0; j--)
                {
                    offsets[j]++;
                    if (offsets.Sum() + _cwidth - (_count-1) <= _width)
                        break; 
                    offsets[j] = 1;
                }
                StringBuilder strout = new StringBuilder();
                for (int j=0;j< _count;j++)
                {
                    strout.Append('-', offsets[j]);
                    strout.Append('X', _constraints[j]);
                }
                strout.Append('-', _width - (_cwidth - (_count-1) + offsets.Sum()));
                Debug.WriteLine("Evaluating row: {0}",strout.ToString());
                // Evaluate current offset pattern for validity (can it co-exist with the current _contents)?
                bool bValid = true;
                int l = 0;
                for (int j = 0; j < _count; j++)
                {
                    for (int k = 0; k < offsets[j]; k++)
                    {
                        // If fixed value is Filled, it's invalid.
                        if (_contents[l] == CellValue.Filled)
                            bValid = false;
                        l++;
                    }
                    for (int k = 0; k < _constraints[j]; k++)
                    {
                        // If fixed value is Empty, it's invalid.
                        if (_contents[l] == CellValue.Empty)
                            bValid = false;
                        l++;
                    }
                }
                Debug.WriteLine(bValid ? "Valid" : "Invalid");
            }
        }
    }
}
