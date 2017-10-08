using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinwheelLib
{
    public class pwCell
    {
        public enum directions { up, right, down, left, none, multi };
        static pwCell[,] cells;
        static int changesThisPass;
        static int height, width;
        private bool[] border = new bool[4] { false, false, false, false };
        private directions associates = directions.multi; // This cell associates with multiple cells.
        private bool filled = false;
        class Point
        {
            public float x, y;
            public override string ToString()
            {
                return string.Format($"{x},{y}");
            }
            public static bool operator ==(Point a, Point b) { return a.x == b.x && a.y == b.y; }
            public static bool operator !=(Point a, Point b) { return a.x != b.x || a.y != b.y; }
        };
        private Point centerPoint = invalid; // invalid.
        private Point myPoint = invalid;
        static private Point invalid = new Point { x = -1, y = -1 }; // invalid.
        private pwCell InDirection(directions dir)
        {
            switch (dir)
            {
                case directions.up:
                    if (myPoint.y == 0) return null;
                    return cells[(int)myPoint.x, (int)myPoint.y - 1];
                case directions.right:
                    if (myPoint.x == width - 1) return null;
                    return cells[(int)myPoint.x + 1, (int)myPoint.y];
                case directions.down:
                    if (myPoint.y == height - 1) return null;
                    return cells[(int)myPoint.x, (int)myPoint.y + 1];
                case directions.left:
                    if (myPoint.x == 0) return null;
                    return cells[(int)myPoint.x - 1, (int)myPoint.y];
                default:
                    return null;
            }
        }
        private void SetBorder(directions dir)
        {
            if (!IsBorder(dir))
                changesThisPass++;
            border[(int)dir] = true;
            var otherSide = InDirection(dir);
            if (otherSide != null)
                otherSide.border[(int)(((int)dir + 2) % 4)] = true;
        }
        public bool IsBorder(directions dir) { return border[(int)dir]; }
        static void Create(int x, int y, char cell)
        {
            pwCell c = cells[x, y] = new pwCell();
            c.myPoint = new Point { x = x, y = y };
            if (x == 0)
                c.SetBorder(directions.left);
            if (y == 0)
                c.SetBorder(directions.up);
            if (x == width - 1)
                c.SetBorder(directions.right);
            if (y == height - 1)
                c.SetBorder(directions.down);
            switch (cell)
            {
                case 'w':
                    c.associates = directions.none; // Associates with itself.
                    c.centerPoint = c.myPoint;
                    c.filled = false;
                    break;
                case 'b':
                    c.associates = directions.none; // Associates with itself.
                    c.centerPoint = c.myPoint;
                    c.filled = true;
                    break;
                case ')':
                    c.associates = directions.left;
                    c.centerPoint = new Point { x = x - .5f, y = y };
                    c.filled = cells[x - 1, y].filled;

                    cells[x - 1, y].associates = directions.right;
                    cells[x - 1, y].centerPoint = c.centerPoint;
                    break;
                case 'v':
                case 'V':
                    c.associates = directions.up;
                    c.centerPoint = new Point { x = x, y = y - .5f };
                    c.filled = cells[x, y - 1].filled;

                    cells[x, y - 1].associates = directions.down;
                    cells[x, y - 1].centerPoint = c.centerPoint;
                    break;
                case '\\': // bottom left of a four.
                    c.associates = directions.up;
                    c.centerPoint = new Point { x = x + .5f, y = y - .5f };
                    c.filled = cells[x, y - 1].filled;

                    cells[x, y - 1].associates = directions.right;
                    cells[x, y - 1].centerPoint = c.centerPoint;
                    break;
                case '/':  // bottom right of a four.
                           // bottom right
                    c.associates = directions.left;
                    c.centerPoint = new Point { x = x - .5f, y = y - .5f };
                    c.filled = cells[x, y - 1].filled;

                    cells[x, y - 1].associates = directions.down;
                    cells[x, y - 1].centerPoint = c.centerPoint;
                    break;
            }
        }
        static void ValConfirm(bool condition, string reason)
        {
            if (!condition)
            {
                throw new Exception(reason);
            }
        }
        static void Validate()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //  Verify borders match.
                    if (i == 0)
                        ValConfirm(cells[i, j].IsBorder(directions.left), "left hand edge missing");
                    else
                        ValConfirm(cells[i - 1, j].IsBorder(directions.right) == cells[i, j].IsBorder(directions.left), "disagree about horizontal borders");
                    if (j == 0)
                        ValConfirm(cells[i, j].IsBorder(directions.up), "top edge missing");
                    else
                        ValConfirm(cells[i, j - 1].IsBorder(directions.down) == cells[i, j].IsBorder(directions.up), "disagree about vertical borders");
                    if (i == width - 1)
                        ValConfirm(cells[i, j].IsBorder(directions.right), "right hand edge missing");
                    if (j == height - 1)
                        ValConfirm(cells[i, j].IsBorder(directions.down), "bottom edge missing");
                }
            }
        }
        static Point Symmetry(Point point, Point center)
        {
            // Returns the symmetric point of "point" about the "center"
            Point newPoint = new Point { x = center.x + center.x - point.x, y = center.y + center.y - point.y };
            if (newPoint.x < 0 || newPoint.x >= width || newPoint.y < 0 || newPoint.y >= height)
                throw new Exception("You can't access a symmetry point outside of the puzzle, thanks.");
            return newPoint;
        }
        public static void Initialize(String[] grid)
        {
            width = grid[0].Length;
            height = grid.Length;
            cells = new pwCell[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    pwCell.Create(i, j, grid[j][i]);
                }
            pwCell.Validate();
        }
        public static void Initialize(int[,] dotArray)
        {
            width = 1 + (dotArray.GetUpperBound(0)) / 2;
            height = 1 + (dotArray.GetUpperBound(1)) / 2;
            cells = new pwCell[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    pwCell c = cells[x, y] = new pwCell();
                    c.myPoint = new Point { x = x, y = y };
                    if (x == 0)
                        c.SetBorder(directions.left);
                    if (y == 0)
                        c.SetBorder(directions.up);
                    if (x == width - 1)
                        c.SetBorder(directions.right);
                    if (y == height - 1)
                        c.SetBorder(directions.down);
                }
            for (int i = 0; i <= dotArray.GetUpperBound(0); i++)
                for (int j = 0; j <= dotArray.GetUpperBound(1); j++)
                {
                    if (dotArray[i, j] == 0) continue;
                    Point centerPoint = new Point() { x = (float)i / 2.0f, y = (float)j / 2.0f };
                    pwCell c;
                    int il, ih, jl, jh;
                    if (i % 2 == 0)
                        ih = il = i / 2;
                    else
                        ih = 1 + (il = (i - 1) / 2);
                    if (j % 2 == 0)
                        jh = jl = j / 2;
                    else
                        jh = 1 + (jl = (j - 1) / 2);
                    for (int ii = il; ii <= ih; ii++)
                        for (int ij = jl; ij <= jh; ij++)
                        {
                            c = cells[ii, ij];
                            c.centerPoint = centerPoint;
                            c.filled = (dotArray[i, j] == 2);
                            c.associates = directions.none; // Associates with itself.
                            if (ii == il && ij == jl && il != ih)
                                c.associates = directions.right;
                            if (ii == ih && ij == jh && il != ih)
                                c.associates = directions.left;
                            if (ij == jl && ii == ih && jl != jh)
                                c.associates = directions.down;
                            if (ij == jh && ii == il && jl != jh)
                                c.associates = directions.up;
                        }
                }
        }


        public static void Dump()
        {
            Debug.Write('+');
            for (int i = 0; i < width; i++)
                Debug.Write("__");
            Debug.WriteLine("");
            for (int j = 0; j < height; j++)
            {
                Debug.Write('|');
                for (int i = 0; i < width; i++)
                {
                    if (cells[i, j].IsBorder(directions.down))
                        Debug.Write('_');
                    else
                        Debug.Write(' ');
                    if (cells[i, j].IsBorder(directions.right))
                        Debug.Write('|');
                    else Debug.Write(' ');
                }
                Debug.WriteLine("");
            }
        }
        public static void DumpFill()
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (cells[i, j].filled)
                        Debug.Write("**");
                    else if (cells[i, j].centerPoint != invalid)
                        Debug.Write("..");
                    else
                        Debug.Write(" @");
                }
                Debug.WriteLine("");
            }
        }
        private static void copyBorders()
        {
            // For all cells associated with a symmetry point, copy their association and borders.
            foreach (var cell in cells)
            {
                if (cell.centerPoint != invalid)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (cell.IsBorder((directions)k))
                        {
                            Point opposite = Symmetry(cell.myPoint, cell.centerPoint);
                            pwCell newCell = cells[(int)opposite.x, (int)opposite.y];
                            directions oppside = (directions)((k + 2) % 4);
                            if (!newCell.IsBorder(oppside))
                            {
                                Debug.WriteLine($"Copying border from {cell.myPoint} to {opposite}.");
                                newCell.SetBorder(oppside);
                            }
                        }
                    }
                }
            }

        }
        private static void separateOwnedCells()
        {
            // Find every un-bordered boundary between two cells owned by different symmetry points.
            // Only need to check right and down.
            foreach (var cell in cells)
            {
                pwCell newCell;
                if (cell.myPoint.x < width - 1)
                {
                    newCell = cells[(int)cell.myPoint.x + 1, (int)cell.myPoint.y];
                    if (newCell.centerPoint != invalid && cell.centerPoint != invalid)
                    {
                        if (cell.centerPoint != newCell.centerPoint)
                            if (!cell.IsBorder(directions.right))
                            {
                                Debug.WriteLine($"Separating owned cells {cell.myPoint} and right");
                                cell.SetBorder(directions.right);
                            }
                    }
                }
                if (cell.myPoint.y < height - 1)
                {
                    newCell = cells[(int)cell.myPoint.x, (int)cell.myPoint.y + 1];
                    if (newCell.centerPoint != invalid && cell.centerPoint != invalid)
                    {
                        if (cell.centerPoint != newCell.centerPoint)
                        {
                            if (!cell.IsBorder(directions.down))
                            {
                                Debug.WriteLine($"Separating owned cells {cell.myPoint} and down");
                                cell.SetBorder(directions.down);
                            }
                        }
                    }
                }
            }
        }
        private static void findConstrainedPoints()
        {
            // for each unassociated cell, if it can only reach cells of a single symmetry point, the cell must be associated with that symmetry point.
            // This code only looks one cell away.
            foreach (var cell in cells)
            {
                if (cell.centerPoint != invalid) continue;
                Point putativePoint = invalid;
                for (int k = 0; k < 4; k++)
                {
                    if (cell.IsBorder((directions)k)) continue;
                    pwCell newCell = cell.InDirection((directions)k);
                    if (newCell.associates == (directions)((k + 2) % 4)) continue; // if associates to this one, it might as well be a border!
                    if (newCell == null) throw new Exception("You can't have a null cell the other side of a non-border"); // Shouldn't ever happen.
                    if (newCell.centerPoint != invalid)
                    {
                        if (putativePoint == invalid)
                            putativePoint = newCell.centerPoint;
                        else if (putativePoint != newCell.centerPoint)
                        {
                            putativePoint = invalid;
                            break; // Can't set the center point to anything specific.
                        }
                    }
                    else
                    {
                        // If it connects with an undecided cell, it's undecided.
                        putativePoint = invalid;
                        break;
                    }
                }
                if (putativePoint != invalid)
                {
                    Debug.WriteLine($"Setting cell at {cell.myPoint} to associate with {putativePoint}");
                    cell.centerPoint = putativePoint;
                    cell.filled = cells[(int)putativePoint.x, (int)putativePoint.y].filled;
                    Point oppSide = Symmetry(cell.myPoint, putativePoint);
                    pwCell oppCell = cells[(int)oppSide.x, (int)oppSide.y];
                    if (oppCell.centerPoint != invalid && oppCell.centerPoint != putativePoint)
                        throw new Exception("Can't associate symmetry point that's already associated elsewhere!");
                    oppCell.centerPoint = putativePoint;
                    oppCell.filled = cells[(int)putativePoint.x, (int)putativePoint.y].filled;
                    changesThisPass++;
                }
            }
        }
        private static void associateDeadEnds()
        {
            // For each cell, if it's a dead end (borders on three sides), make sure that the association points out, so we can trim it.
            foreach (var cell in cells)
            {
                directions dirOut = directions.multi;
                if (cell.associates != directions.multi) continue; // Already associated!
                for (int k = 0; k < 4; k++)
                {
                    if (cell.IsBorder((directions)k)) continue;
                    pwCell newCell = cell.InDirection((directions)k);
                    if (newCell.associates == (directions)((k + 2) % 4)) continue; // if associates to this one, it might as well be a border!
                    if (dirOut != directions.multi)
                    {
                        dirOut = directions.multi;
                        break;
                    }
                    dirOut = (directions)k;
                }
                if (dirOut != directions.multi)
                {
                    cell.associates = dirOut;
                    Debug.WriteLine($"Associating dead-end cell at {cell.myPoint} to direction {dirOut}");
                    changesThisPass++;
                }
            }
        }
        public static int Process()
        {
            changesThisPass = 0;
            copyBorders();
            separateOwnedCells();
            findConstrainedPoints();
            associateDeadEnds();
            return changesThisPass;
        }
    }
}
