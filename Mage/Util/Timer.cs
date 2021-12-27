
using System;

namespace Mage
{
    public class Timer
    {
        public float ElapsedTime { get; private set; }
        public float ExpireTime { get; set; }
        public event EventHandler OnExpire;

        public Timer() { }
        public Timer(float expireTime) { this.ExpireTime = expireTime; }
        public Timer(float expireTime, EventHandler onExpire)
        {
            this.ExpireTime = expireTime;
            OnExpire += onExpire;
        }

        public void Update(float delta)
        {
            ElapsedTime += delta;
            if (ElapsedTime >= ExpireTime)
            {
                ElapsedTime = 0;
                OnExpire?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Reset()
        {
            ElapsedTime = 0;
        }
    }
}