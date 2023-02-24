using UnityEngine;
using UnityEngine.Playables;


namespace TimelineControl
{
    public class TimelinePlayerBase
    {
        public GameObject GameObject { get; private set; }
        public ITimelineBinder Binder { get; private set; }
        public TimelinePlayerError Error { get; private set; }
        public TimePlayerState State { get; protected set; } = TimePlayerState.None;
        public PlayableDirector Director { get; private set; }
        
        public void Init(GameObject gameObject, ITimelineBinder binder)
        {
            if (State != TimePlayerState.None) return;

            Error = TimelinePlayerError.None;
            State = TimePlayerState.Idle;

            if (!gameObject)
            {
                Error = TimelinePlayerError.AssetIsNull;
                goto EndPoint;
            }

            this.GameObject = gameObject;
            this.Binder = binder;
            this.Director = this.GameObject.GetComponent<PlayableDirector>();

            if (!this.Director)
            {
                Error = TimelinePlayerError.PlayerDirectorMissing;
                goto EndPoint;
            }

            if (Director.playOnAwake)
            {
                Director.playOnAwake = false;
                Debug.Log($"{GetType().Name} 已禁用playOnAwake {gameObject.name}");
            }

            Director.stopped -= OnDirectorStopInternal;
            Director.stopped += OnDirectorStopInternal;

            this.Binder = binder;
            this.Binder?.Init(this);

            EndPoint:
            if (Error != TimelinePlayerError.None)
            {
                State = TimePlayerState.Error;
            }
        }

        private void OnDirectorStopInternal(PlayableDirector director)
        {
            if (State != TimePlayerState.Playing) return;
            State = TimePlayerState.Idle;

            OnDirectorStop();
        }
        
        protected virtual void OnDirectorStop()
        {

        }

        public void Destroy()
        {
            OnDestroy();
        }

        protected void OnDestroy()
        {
            if (Binder != null)
            {
                Binder.Destroy();
                Binder = null;
            }
            
            if (Director)
            {
                Director.stopped -= OnDirectorStopInternal;
                Director = null;
            }
        }
    }
    

    public class TimelinePlayer<T> : TimelinePlayerBase
    {
        protected T args;

        public void Play(T args)
        {
            if (State != TimePlayerState.Idle) return;
            State = TimePlayerState.Playing;
            this.args = args;

            Director.Play();
        }

        public void Stop()
        {
            if (State != TimePlayerState.Playing) return;
            State = TimePlayerState.Idle;
            
            Director.Stop();
            OnStop();
        }

        protected override void OnDirectorStop()
        {
            base.OnDirectorStop();
            OnStop();
        }

        private void OnStop()
        {
            
        }
    }
}

