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
using Microsoft.Devices.Sensors;

namespace shaker
{
    public class ShakeDetect
    {
        private Accelerometer _accelerometer = null;
        object SyncRoot = new object();
        private int _minimumShake;
        ShakeRecord[] _shakeRecordList;
        private int _shakeRecordIndex = 0;
        private const double MinAccelMagnitude = 1.2;
        private const double MinAccelMagnitudeSquared = MinAccelMagnitude * MinAccelMagnitude;
        private static readonly TimeSpan MinShakeTime = TimeSpan.FromMilliseconds(500);

        public event EventHandler<EventArgs> ShakeEvent = null;

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

        void CheckForShakes()
        {
            int startIndex = (_shakeRecordIndex - 1);
            if (startIndex < 0)
                startIndex = _minimumShake - 1;
            int endIndex = _shakeRecordIndex;

            if ((_shakeRecordList[endIndex].EventTime.Subtract(_shakeRecordList[startIndex].EventTime)) <= MinShakeTime)
            {
                OnShakeEvent();
            }
        }

        void _accelerometer_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            if ((e.X * e.X + e.Y * e.Y) > MinAccelMagnitudeSquared)
            {
                double degrees = 180.0 * Math.Atan2(e.Y, e.X) / Math.PI;
                Direction direction = DegreesToDirection(degrees);

                if ((direction & _shakeRecordList[_shakeRecordIndex].ShakeDirection) != Direction.None)
                    return;
                ShakeRecord record = new ShakeRecord();
                record.EventTime = DateTime.Now;
                record.ShakeDirection = direction;
                _shakeRecordIndex = (_shakeRecordIndex + 1) % _minimumShake;
                _shakeRecordList[_shakeRecordIndex] = record;

                CheckForShakes();
            }
        }

    }
}
