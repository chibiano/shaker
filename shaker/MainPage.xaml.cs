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
using Microsoft.Phone.Controls;
using System.Windows.Resources;
using Microsoft.Xna.Framework;

using Microsoft.Devices.Sensors;

using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace shaker
{
    public partial class MainPage : PhoneApplicationPage
    {

        //Accelerometer accelerometer;
        // private Vector3 _lastreading;
        private ShakeDetect _shakeDetect;
        Stream stream;
        SoundEffect effect;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _shakeDetect = new ShakeDetect();
            _shakeDetect.ShakeEvent += new EventHandler<EventArgs>(_shakeDetect_ShakeEvent);
            _shakeDetect.Start();
            stream = TitleContainer.OpenStream("sounds/shaker.wav");

            /*
                        if (!Accelerometer.IsSupported)
                        {
                            statusTextBlock.Text = "device does not support accelerometer";
                            startButton.IsEnabled = false;
                            stopButton.IsEnabled = false;
                        }
            */
        }

        void _shakeDetect_ShakeEvent(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                Storyboard shakeAnimation = Resources["ShakeAnimation"] as Storyboard;
                shakeAnimation.Begin();

            });

            /* stream = TitleContainer.OpenStream("sounds/shaker.wav");
             effect = SoundEffect.FromStream(stream);
             FrameworkDispatcher.Update();
             effect.Play();*/


        }




        /*
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (accelerometer == null)
            {
                accelerometer = new Accelerometer();
                accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(60);
                accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
            }

            try
            {
                statusTextBlock.Text = "starting accelerometer";
                accelerometer.Start();
            }
            catch (InvalidOperationException ex)
            {
                statusTextBlock.Text = "unable to start accelerometer";
            }
        }

        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }



        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            statusTextBlock.Text = "getting data";

            Vector3 acceleration = accelerometerReading.Acceleration;

            if (_lastreading != null)
            {
                if (shaked(_lastreading, acceleration))
                {
                    try
                    {
                        statusTextBlock.Text = "shake shake shake";
                    }
                    catch (InvalidOperationException ex)
                    {
                        statusTextBlock.Text = "unable to shake";
                    }
                }
            }
            _lastreading = acceleration;

            //numeric values
            xTextBlock.Text = "X: " + acceleration.X.ToString("0.00");
            yTextBlock.Text = "Y: " + acceleration.Y.ToString("0.00");
            zTextBlock.Text = "Z: " + acceleration.Z.ToString("0.00");

            //graphical values
            xLine.X2 = xLine.X1 + acceleration.X * 200;
            yLine.Y2 = yLine.Y1 - acceleration.Y * 200;
            zLine.X2 = zLine.X1 - acceleration.Z * 100;
            zLine.Y2 = zLine.Y1 + acceleration.Z * 100;
        }

        private bool shaked(Vector3 _lastreading, Vector3 acceleration)
        {
            double deltaY = Math.Abs(acceleration.Y) + Math.Abs(_lastreading.Y);
            if (deltaY > 1.8)
                return true;
            else
                return false;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (accelerometer != null)
            {
                accelerometer.Stop();
                statusTextBlock.Text = "acceleromater stopped";
            }
        }
          */
    }
}