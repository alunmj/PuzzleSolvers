using PinwheelLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PinGUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class iPoint { public int x, y; };
    public sealed partial class MainPage : Page
    {
        private const double squareSize = 25;
        int[,] dotArray = new int[7,7];
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void widthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(widthBox.Text);
                dotArray = new int[width+width-1, dotArray.GetUpperBound(1)+1];
                blankCanvas.Width = squareSize * width;
            }
            catch (FormatException ex)
            {

            }
        }

        private void heightBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int height = Convert.ToInt32(heightBox.Text);
                dotArray = new int[dotArray.GetUpperBound(0) + 1, height+height-1];
                blankCanvas.Height = squareSize * height;
            }
            catch (FormatException ex) { }
        }

        private void blankCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PathGeometry grid = new PathGeometry();
                        
            for (double x = 0; x <= blankCanvas.Width; x += squareSize)
            {
                PathFigure fig = new PathFigure();
                fig.StartPoint = new Point(x, 0);
                LineSegment pols = new LineSegment();
                pols.Point = (new Point(x, blankCanvas.Height));
                fig.Segments.Add(pols);
                grid.Figures.Add(fig);
            }

            for (double y = 0; y <= blankCanvas.Height; y += squareSize)
            {
                PathFigure fig = new PathFigure();
                fig.StartPoint = new Point(0, y);
                LineSegment pols = new LineSegment();
                pols.Point = (new Point(blankCanvas.Width, y));
                fig.Segments.Add(pols);
                grid.Figures.Add(fig);
            }

            paf.Data = grid;
        }

        private Point RoundedPt(iPoint iPt)
        {
            Point error = new Point(-1, -1);
            Point roundedPoint ;

            double halfSize = squareSize / 2.0;

            // Find nearest quarter square corner to the cursor.
            roundedPoint = new Point(
                halfSize *(double)iPt.x,
                halfSize * (double)iPt.y);

            if ((roundedPoint.X <= 0.0 || roundedPoint.X >= blankCanvas.Width) ||
                (roundedPoint.Y <= 0.0 || roundedPoint.Y >= blankCanvas.Height))
            {
                return error; // no dots on the edge!
            }

            return roundedPoint;
        }

        private void blankCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptpt = e.GetCurrentPoint((UIElement) sender);
            
            Point roundedPt = RoundedPt(FixedPt(ptpt.Position));


            // Center the circle there, by making the left & top off by half the width / height.
            Canvas.SetLeft(cursorEllipse, roundedPt.X - (cursorEllipse.Width / 2.0));
            Canvas.SetTop(cursorEllipse, roundedPt.Y - (cursorEllipse.Height / 2.0));

            e.Handled = true;
        }

        private void blankCanvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            cursorEllipse.Visibility = Visibility.Visible;
        }

        private void blankCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            cursorEllipse.Visibility = Visibility.Collapsed;
        }

        private iPoint FixedPt(Point dPt)
        {
            iPoint error = new PinGUI.iPoint() { x = -1, y = -1 };
            double halfSize = squareSize / 2.0;
            int width = (int)(blankCanvas.Width / halfSize);
            int height = (int)(blankCanvas.Height / halfSize);
            // Find nearest quarter square corner to the cursor.
            iPoint pt = new iPoint()
            {
                x = (int)Math.Round(dPt.X / halfSize),
                y = (int)Math.Round(dPt.Y / halfSize)
            };

            if (pt.x <= 0 || pt.y <= 0 || pt.x >= blankCanvas.Width || pt.y >= blankCanvas.Height)
                return error;

            return pt;
        }

        private void blankCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptpt = e.GetCurrentPoint((UIElement)sender);
            Point roundedPt = RoundedPt(FixedPt(ptpt.Position));

            iPoint wherePt = FixedPt(roundedPt);
            if (wherePt.x < 0 || wherePt.y < 0) return;

            int color = dotArray[wherePt.x-1, wherePt.y-1];
            Ellipse newCircle;
            Point offsetPt;
            // If I did a custom control for the dots, I could do without all this hit-test malarkey, but this isn't that important.
            var ttv = blankCanvas.TransformToVisual(Window.Current.Content);
            offsetPt = ttv.TransformPoint(roundedPt);
            var hitResults = VisualTreeHelper.FindElementsInHostCoordinates(offsetPt, blankCanvas);
            switch (color)
            {
                case 0: // empty
                    // Make sure there's no dot next to this one, because that would never work.
                    for (int i = -1; i < 2; i++)
                        for (int j = -1; j < 2; j++)
                            if ((i + wherePt.x -1 >=0 && i+wherePt.x-1<=dotArray.GetUpperBound(0)) &&
                                (j+wherePt.y-1>=0 && j+wherePt.y-1<=dotArray.GetUpperBound(1)) &&
                                dotArray[i + wherePt.x - 1, j + wherePt.y - 1] != 0) return;
                    newCircle = new Ellipse();
                    newCircle.Width = newCircle.Height = cursorEllipse.Width;
                    // New circle will be white with black border.
                    newCircle.Stroke = new SolidColorBrush(Colors.Black);
                    newCircle.Fill = new SolidColorBrush(Colors.Wheat);
                    Canvas.SetLeft(newCircle, roundedPt.X - newCircle.Width / 2.0);
                    Canvas.SetTop(newCircle, roundedPt.Y - newCircle.Height / 2.0);
                    blankCanvas.Children.Add(newCircle);
                    color = 1;
                    break;
                case 1: // white circle
                    if (hitResults.Any())
                    {
                        newCircle = hitResults.First() as Ellipse;
                        newCircle.Fill = new SolidColorBrush(Colors.Black);
                        color = 2;
                    }
                    break;
                case 2: // black circle
                    if (hitResults.Any())
                    {
                        newCircle = hitResults.First() as Ellipse;
                        blankCanvas.Children.Remove(newCircle);
                        color = 0;
                    }
                    break;
            }
            dotArray[wherePt.x - 1, wherePt.y - 1] = color;
        }

        private void calcButton_Click(object sender, RoutedEventArgs e)
        {
            pwCell.Initialize(dotArray);
            pwCell.Dump();
            while (pwCell.Process() != 0) ;
            pwCell.Dump();
            pwCell.DumpFill();
        }
    }
}
