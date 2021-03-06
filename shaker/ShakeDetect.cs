﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework;

namespace shaker
{
    public class ShakeDetect
    {
        private Accelerometer _accelerometer = null;
        object SyncRoot = new object();
        private int _minimumShake;
        ShakeRecord[] _shakeRecordList;
        private int _shakeRecordIndex = 0;
        private const double MinAccelMagnitude = 1.3;
        private const double MinAccelMagnitudeSquared = MinAccelMagnitude * MinAccelMagnitude;
        private static readonly TimeSpan MinShakeTime = TimeSpan.FromMilliseconds(50);
        private static readonly TimeSpan minTimeEachShake = TimeSpan.FromMilliseconds(10);
        private DateTimeOffset lastShakeTime;
        Stream stream;
        SoundEffect effect;


        public event EventHandler<EventArgs> ShakeEvent = null;
        public Object a;

        protected void OnShakeEvent()
        {
            if (ShakeEvent != null)
                ShakeEvent(this, new EventArgs());

        }


        [Flags]
        public enum Direction
        {
            None = 0,
            North = 1,
            South = 2,
            East = 8,
            West = 4,
            NorthWest = North | West,
            SouthWest = South | West,
            SouthEast = South | East,
            NorthEast = North | East
        };

        public struct ShakeRecord
        {
            public Direction ShakeDirection;
            public DateTime EventTime;
        }

        public ShakeDetect()
            : this(2)
        {
        }

        public ShakeDetect(int minShakes)
        {
            _minimumShake = minShakes;
            _shakeRecordList = new ShakeRecord[minShakes];
            lastShakeTime = DateTimeOffset.Now;
        }

        public void Start()
        {
            lock (SyncRoot)
            {
                if (_accelerometer == null)
                {
                    _accelerometer = new Accelerometer();
                    _accelerometer.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(_accelerometer_ReadingChanged);
                    _accelerometer.Start();
                }
            }
        }

        public void Stop()
        {
            lock (SyncRoot)
            {
                if (_accelerometer != null)
                {
                    _accelerometer.Stop();
                    _accelerometer.ReadingChanged -= _accelerometer_ReadingChanged;
                    _accelerometer = null;
                }
            }
        }

        Direction DegreesToDirection(double direction)
        {
            if (direction < 0)
            { 
                direction += 360; 
            }
            
            if ((direction >= 337.5) || (direction <= 22.5))
                return Direction.North;
            if ((direction <= 67.5))
                return Direction.NorthEast;
            if (direction <= 112.5)
                return Direction.East;
            if (direction <= 157.5)
                return Direction.SouthEast;
            if (direction <= 202.5)
                return Direction.South;
            if (direction <= 247.5)
                return Direction.SouthWest;
            if (direction <= 292.5)
                return Direction.West;
            return Direction.NorthWest;
        }

        void CheckForShakes(Direction direction)
        {
            int startIndex = (_shakeRecordIndex - 1);
            if (startIndex < 0)
                startIndex = _minimumShake - 1;
            int endIndex = _shakeRecordIndex;

            if ((_shakeRecordList[endIndex].EventTime.Subtract(_shakeRecordList[startIndex].EventTime)) <= MinShakeTime)
            {
                lastShakeTime = DateTimeOffset.Now;
                String file = "sounds/shaker.wav";

                if (direction == Direction.North)
                    file = ("sounds/up.wav");

                else if (direction == Direction.NorthEast)
                    file = ("sounds/up.wav");

                else if (direction == Direction.East)
                    file = ("sounds/right.wav");

                else if (direction == Direction.SouthEast)
                    file = ("sounds/right.wav");

                else if (direction == Direction.South)
                    file = ("sounds/down.wav");

                else if (direction == Direction.SouthWest)
                    file = ("sounds/down.wav");

                else if (direction == Direction.West)
                    file = ("sounds/left.wav");

                else if (direction == Direction.NorthWest)
                    file = ("sounds/left.wav");





                stream = TitleContainer.OpenStream(file);



                effect = SoundEffect.FromStream(stream);

                FrameworkDispatcher.Update();
                effect.Play();

                
                
                

                OnShakeEvent();
            }
        }

        void _accelerometer_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            DateTimeOffset eventTime = e.Timestamp;


            TimeSpan timeDiff = eventTime - lastShakeTime;

            
            if (timeDiff> MinShakeTime)

            {
               
                if ((e.X * e.X + e.Y * e.Y )> MinAccelMagnitudeSquared)
                {

                    
                    double degrees =( 180.0 * Math.Atan2(e.X, e.Y) / Math.PI);

                    Direction direction = DegreesToDirection(degrees);

                     //if ((direction & _shakeRecordList[_shakeRecordIndex].ShakeDirection) != Direction.None)
                      // return;
                    ShakeRecord record = new ShakeRecord();
                    record.EventTime = DateTime.Now;
                    record.ShakeDirection = direction;
                    _shakeRecordIndex = (_shakeRecordIndex + 1) % _minimumShake;
                    _shakeRecordList[_shakeRecordIndex] = record;

                    CheckForShakes(direction);
                }
            }
        }

    }
}
