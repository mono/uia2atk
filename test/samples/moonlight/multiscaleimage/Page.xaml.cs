using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MultiScaleImageSample
{
    public partial class Page : UserControl
    {
        Point lastMousePos = new Point();

        double _zoom = 1;
        bool mouseButtonPressed = false;
        bool mouseIsDragging = false;
        Point dragOffset;
        Point currentPosition;

        public double ZoomFactor
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        public Page()
        {
            InitializeComponent();

            //
            // We are setting the source here, but you should be able to set the Source property via
            //
            this.msi.Source = new DeepZoomImageTileSource(new Uri("imagebin/GeneratedImages/dzc_output.xml", UriKind.Relative));

            //
            // Firing an event when the MultiScaleImage is Loaded
            //
            this.msi.Loaded += new RoutedEventHandler(msi_Loaded);

            //
            // Firing an event when all of the images have been Loaded
            //
            this.msi.ImageOpenSucceeded += new RoutedEventHandler(msi_ImageOpenSucceeded);

            //
            // Handling all of the mouse and keyboard functionality
            //
            this.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (mouseButtonPressed)
                {
                    mouseIsDragging = true;
                }
                this.lastMousePos = e.GetPosition(this.msi);
            };

            this.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                mouseButtonPressed = true;
                mouseIsDragging = false;
                dragOffset = e.GetPosition(this);
                currentPosition = msi.ViewportOrigin;
            };

            this.msi.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                mouseIsDragging = false;
            };

            this.MouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs e)
            {
                mouseButtonPressed = false;
                if (mouseIsDragging == false)
                {
                    bool shiftDown = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

                    ZoomFactor = 2.0;
                    if (shiftDown) ZoomFactor = 0.5;
                    Zoom(ZoomFactor, this.lastMousePos);
                }
                mouseIsDragging = false;
            };

            this.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (mouseIsDragging)
                {
                    Point newOrigin = new Point();
                    newOrigin.X = currentPosition.X - (((e.GetPosition(msi).X - dragOffset.X) / msi.ActualWidth) * msi.ViewportWidth);
                    newOrigin.Y = currentPosition.Y - (((e.GetPosition(msi).Y - dragOffset.Y) / msi.ActualHeight) * msi.ViewportWidth);
                    msi.ViewportOrigin = newOrigin;
                }
            };

            new MouseWheelHelper(this).Moved += delegate(object sender, MouseWheelEventArgs e)
            {
                e.Handled = true;
                if (e.Delta > 0)
                    ZoomFactor = 1.2;
                else
                    ZoomFactor = .80;

                Zoom(ZoomFactor, this.lastMousePos);
            };
        }

        void msi_ImageOpenSucceeded(object sender, RoutedEventArgs e)
        {
            //If collection, this gets you a list of all of the MultiScaleSubImages
            //
            //foreach (MultiScaleSubImage subImage in msi.SubImages)
            //{
            //    // Do something
            //}
        }

        void msi_Loaded(object sender, RoutedEventArgs e)
        {
            // Hook up any events you want when the image has successfully been opened
        }

        public void Zoom(double zoom, Point pointToZoom)
        {
            Point logicalPoint = this.msi.ElementToLogicalPoint(pointToZoom);
            this.msi.ZoomAboutLogicalPoint(zoom, logicalPoint.X, logicalPoint.Y);
        }


        //*  Sample event handlerrs tied to the Click of event of various buttons for 
        //*  showing all images, zooming in, and zooming out!
        //* 
        private void ShowAllClick(object sender, RoutedEventArgs e)
        {
            this.msi.ViewportOrigin = new Point(0, 0);
            this.msi.ViewportWidth = 1;
            ZoomFactor = 1;
        }

        private void zoomInClick(object sender, RoutedEventArgs e)
        {
            Zoom(1.2, new Point(this.ActualWidth / 2, this.ActualHeight / 2));
        }

        private void zoomOutClick(object sender, RoutedEventArgs e)
        {
            Zoom(.8, new Point(this.ActualWidth / 2, this.ActualHeight / 2));
        }

    }
}