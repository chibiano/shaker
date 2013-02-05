﻿using System;
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

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;


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
        // String file = "sounds/Roland.wav";

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(MainPage_ManipulationStarted);

            _shakeDetect = new ShakeDetect();
            _shakeDetect.ShakeEvent += new EventHandler<EventArgs>(_shakeDetect_ShakeEvent);
            _shakeDetect.Start();
            stream = TitleContainer.OpenStream("sounds/shaker.wav");

            TouchPanel.EnabledGestures = GestureType.Tap;
            ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(MainPage_ManipulationCompleted);
        }


        private void MainPage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {

            String file = "sounds/down.wav";
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.Tap)
                {
                    //Do something
                    stream = TitleContainer.OpenStream(file);
                    effect = SoundEffect.FromStream(stream);
                    FrameworkDispatcher.Update();
                    _shakeDetect_ShakeEvent(sender, e);
                    effect.Play();
                   
                }
            }
        }




        
        void _shakeDetect_ShakeEvent(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    Storyboard shakeAnimation = Resources["ShakeAnimation"] as Storyboard;
                    shakeAnimation.Begin();
                    
                });
                
         
        }
      
    }
}