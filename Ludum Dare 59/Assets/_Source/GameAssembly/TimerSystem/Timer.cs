using System;
using UnityEngine;

namespace TimerSystem
{
    public class Timer
    {
        private bool _ticking = true;
        
        public bool IsPaused => !_ticking;

        public short ID { get; private set; }
        public float TimeLeft { get; private set; }

        public event Action<Timer> OnTimeElapsed;

        public Timer(float time)
        {
            TimeLeft = time;
        }

        public void Tick()
        {
            if(!_ticking)
                return;
            
            TimeLeft -= Time.deltaTime;

            if (TimeLeft > 0)
                return;
            
            OnTimeElapsed?.Invoke(this);
            OnTimeElapsed = null;
        }

        public void SetID(short id)
        {
            ID = id;
        }

        public void Pause() => _ticking = false;

        public void Unpause() => _ticking = true;
    }
}