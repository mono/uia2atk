using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ThumbSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        private void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double yAdjust = canvas.Height + e.VerticalChange;
            double xAdjust = canvas.Width + e.HorizontalChange;
            if ((xAdjust >= 0) && (yAdjust >= 0))
            {
                canvas.Width = xAdjust;
                canvas.Height = yAdjust;
                Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
                Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
                textBlock.Text = "Size: " + canvas.Width.ToString() + ", " + canvas.Height.ToString();
            }
        }

        private void thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            canvas.Background = new SolidColorBrush(Colors.Purple);
        }

        private void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            canvas.Background = new SolidColorBrush(Colors.Green);
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            textBlock.Width = canvas.Width;
            textBlock.Height = canvas.Height;
        }
    }
}
