using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PinGUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const double squareSize = 25;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void widthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(widthBox.Text);
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

        private void blankCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint ptpt = e.GetCurrentPoint((UIElement) sender);
            
            Point pt = ptpt.RawPosition;

            pt.X = Math.Round(pt.X);
            pt.Y = Math.Round(pt.Y);

            if (pt.X < 0.0 || pt.X > (double)blankCanvas.Width)
            {
                return;
            }
            if (pt.Y < 0.0 || pt.Y > (double)blankCanvas.Height)
            {
                return;
            }
            double halfSize = squareSize / 2.0;
            // Find nearest quarter square corner to the cursor.
            Point roundedPt = new Point(
                halfSize * Math.Round(pt.X / halfSize), 
                halfSize * Math.Round(pt.Y / halfSize));

            if (roundedPt.X <= 0.0 || roundedPt.X >= blankCanvas.Width) return; // no dots on the edge!
            if (roundedPt.Y <= 0.0 || roundedPt.Y >= blankCanvas.Height) return; // no dots on the edge!

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

        private void blankCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
