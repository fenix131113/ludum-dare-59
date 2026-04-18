using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

namespace TimerSystem
{
    public class TimersHandler : ITickable
    {
        private readonly Dictionary<short, Timer> _timers = new();

        public void Tick()
        {
            if (_timers.Count == 0)
                return;

            foreach (var pair in _timers.ToList())
            {
                if (!_timers.TryGetValue(pair.Key, out var timer))
                    continue;
                
                timer.Tick();
            }
        }
        
        private void OnTimeElapsed(Timer timer) => _timers.Remove(timer.ID);

        public Timer GetTimer(short id) => _timers.GetValueOrDefault(id);

        public bool IsTimerValid(short id) => _timers.ContainsKey(id);

        public void RegisterTimer(Timer timer)
        {
            short id = -1;
            
            for (var i = 0; i <= short.MaxValue; i++)
            {
                if(_timers.ContainsKey((short)i))
                    continue;

                id = (short)i;
                break;
            }
            
            if (timer == null || _timers.ContainsValue(timer) || id == -1)
                return;

            timer.SetID(id);
            _timers[id] = timer;
            BindTimer(timer);
        }

        public void UnregisterTimer(short id)
        {
            if (!_timers.TryGetValue(id, out var temp))
                return;
            
            if(_timers.Remove(id))
                ExposeTimer(temp);
        }

        public void UnregisterTimer(Timer timer)
        {
            if (timer == null || !_timers.ContainsValue(timer))
                return;

            var key = _timers.First(x => x.Value == timer).Key;
            var temp = _timers[key];
            
            if(_timers.Remove(key))
                ExposeTimer(temp);
        }

        private void BindTimer(Timer timer)
        {
            timer.OnTimeElapsed += OnTimeElapsed;
        }

        private void ExposeTimer(Timer timer)
        {
            timer.OnTimeElapsed -= OnTimeElapsed;
        }
    }
}
