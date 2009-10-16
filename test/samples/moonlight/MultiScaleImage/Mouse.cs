using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MultiScaleImageSample
{
    public class Mouse
    {
        public Point DownPosition
        {
            get;
            set;
        }
        public Point UpPosition
        {
            get;
            set;
        }
        public Point MovePosition
        {
            get;
            set;
        }
        public Point UpInterval
        {
            get
            {
                return new Point((DownPosition.X - UpPosition.X) / 1000, (DownPosition.Y - UpPosition.Y) / 1000);
            }
        }
    }
}
