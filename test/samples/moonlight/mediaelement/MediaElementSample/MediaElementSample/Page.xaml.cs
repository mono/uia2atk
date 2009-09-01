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
using System.Windows.Threading;

namespace MediaElementSample
{
    public partial class Page : UserControl
    {
        private DispatcherTimer timer;
        public Page()
        {
            InitializeComponent();
            btnPlayPause.Checked += new RoutedEventHandler(btnPlayPause_Checked);
            btnPlayPause.Unchecked += new RoutedEventHandler(btnPlayPause_Unchecked);

            VideoElement.CurrentStateChanged += new RoutedEventHandler(VideoElement_CurrentStateChanged);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += new EventHandler(timer_Tick);
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (VideoElement.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                txtVideoPosition.Text =
                    string.Format("{0:00}:{1:00}", VideoElement.Position.Minutes, VideoElement.Position.Seconds);

                sliderScrubber.Value = VideoElement.Position.TotalSeconds /
                    VideoElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        void VideoElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (VideoElement.CurrentState == MediaElementState.Playing)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }

        void btnPlayPause_Unchecked(object sender, RoutedEventArgs e)
        {
            VideoElement.Play();
            btnPlayPause.Content = "Pause";
        }

        private void btnPlayPause_Checked(object sender, RoutedEventArgs e)
        {
            VideoElement.Pause();
            btnPlayPause.Content = "Play";
        }
    }
}

