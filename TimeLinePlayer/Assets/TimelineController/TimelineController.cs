using UnityEngine;
using UnityEngine.Playables;


namespace TimelineControl
{
    public class TimelineControllerBase
    {
        public GameObject GameObject { get; private set; }
        public ITimelineBinder Binder { get; private set; }
        public TimelineControllerError Error { get; private set; }
        public TimelineControllerState State { get; protected set; } = TimelineControllerState.None;
        public PlayableDirector Director { get; private set; }
        
        public void Init(GameObject gameObject, ITimelineBinder binder)
        {
            if (State != TimelineControllerState.None) return;

            Error = TimelineControllerError.None;
            State = TimelineControllerState.Idle;

            if (!gameObject)
            {
                Error = TimelineControllerError.AssetIsNull;
                goto EndPoint;
            }

            this.GameObject = gameObject;
            this.Binder = binder;
            this.Director = this.GameObject.GetComponent<PlayableDirector>();

            if (!this.Director)
            {
                Error = TimelineControllerError.PlayerDirectorMissing;
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
            if (Error != TimelineControllerError.None)
            {
                State = TimelineControllerState.Error;
            }
        }

        private void OnDirectorStopInternal(PlayableDirector director)
        {
            if (State != TimelineControllerState.Playing) return;
            State = TimelineControllerState.Idle;

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
    

    public class TimelineController<T> : TimelineControllerBase
    {
        protected T args;

        public void Play(T args)
        {
            if (State != TimelineControllerState.Idle) return;
            State = TimelineControllerState.Playing;
            this.args = args;

            Director.Play();
        }

        public void Stop()
        {
            if (State != TimelineControllerState.Playing) return;
            State = TimelineControllerState.Idle;
            
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

