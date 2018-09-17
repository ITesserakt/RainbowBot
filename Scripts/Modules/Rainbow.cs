using System;
using System.Timers;
using Discord;

namespace Informatics.Scripts.Modules {
    public class Rainbow {
        private          Color  _currentColor;
        private          Color  _targetColor = Color.DarkRed;
        private readonly Random _rnd         = new Random();
        private          IRole  _role;
        private          float  _stepAccum;
        public           float  Step  = 0.1f;
        public           int    Delay = 100;
        private readonly Timer  _timer;

        public Rainbow() {
            _timer = new Timer {
                AutoReset = true,
                Enabled = false,
                Interval = Delay
            };
            _timer.Elapsed += TimerOnTick;
        }

        public void Start(IRole role) {
            _role = role;
            _timer.Stop();
            _timer.Start();
        }

        private async void TimerOnTick(object sender, ElapsedEventArgs e) {
            if (_stepAccum >= 1) {
                _stepAccum = 0;
                int r = _rnd.Next(256), b = _rnd.Next(256), g = _rnd.Next(256);

                _currentColor = _targetColor;
                _targetColor = new Color(r, g, b);
            }

            var mixR = (int) (_currentColor.R * (1 - _stepAccum) + _targetColor.R * _stepAccum);
            var mixG = (int) (_currentColor.G * (1 - _stepAccum) + _targetColor.G * _stepAccum);
            var mixB = (int) (_currentColor.B * (1 - _stepAccum) + _targetColor.B * _stepAccum);
            _stepAccum += Step;
            await _role.ModifyAsync(props => props.Color = new Color(mixR, mixG, mixB));
        }

        public void Stop() {
            _timer.Stop();
        }
    }
}