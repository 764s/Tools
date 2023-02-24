using UnityEngine;
using UnityEngine.Playables;


namespace TimelineControl
{
    public class TimelinePlayerBase
    {

    }

    public static class TimelinePlayerUtil
    {
        public static string TimelineColor(this string str, string color)
        {
            return $"<color={color}>{str}</color>";
        }

        public static string TimelineColor(this string str, Color color)
        {
            return TimelineColor(str, ColorUtility.ToHtmlStringRGB(color));
        }
    }

    public interface ITimelineEventListener
    {
        void OnError(TimelinePlayerError error);
    }

// 暂时不考虑骨架重新加载的情况
    public interface ITimelineBinder
    {
        bool IsBinderTarget(ITimelineBinderTarget target);
        ITimelineBinderTarget CreateBinderTarget(PlayableDirector director);
        void DestroyBindTarget(ITimelineBinderTarget binderTarget);
        bool Bind(ITimelineBinderTarget target);

    }

    public interface ITimelineBinderTarget
    {
        void Init(TimelinePlayerBase timelinePlayer);
        void Destroy();
    }

    public enum TimelinePlayerError
    {
        None,
        AssetIsNull,
        PlayerDirectorMissing,
        BindFailed,
    }

    public enum TimePlayerState
    {
        None,
        Error,
        Idle,
        Playing,
    }

    public class TimelinePlayer<T> : TimelinePlayerBase
    {
        protected GameObject gameObject;
        protected T args;
        protected ITimelineEventListener eventListener;
        private ITimelineBinder binder;

        private PlayableDirector director;
        private TimelinePlayerError error;

        private ITimelineBinderTarget binderTarget;

        public TimePlayerState State { get; private set; } = TimePlayerState.None;

        public void Init(GameObject gameObject, ITimelineEventListener eventListener, ITimelineBinder binder)
        {
            if (State != TimePlayerState.None) return;

            error = TimelinePlayerError.None;
            State = TimePlayerState.Idle;

            if (!gameObject)
            {
                error = TimelinePlayerError.AssetIsNull;
                goto EndPoint;
            }

            this.gameObject = gameObject;
            this.eventListener = eventListener;
            this.binder = binder;
            this.director = this.gameObject.GetComponent<PlayableDirector>();

            if (!this.director)
            {
                error = TimelinePlayerError.PlayerDirectorMissing;
                goto EndPoint;
            }

            if (director.playOnAwake)
            {
                director.playOnAwake = false;
                Debug.Log($"{GetType().Name} 已禁用playOnAwake {gameObject.name}");
            }

            director.stopped -= OnDirectorStop;
            director.stopped += OnDirectorStop;

            if (binder != null)
            {
                if (binderTarget != null || !binder.IsBinderTarget(binderTarget))
                {
                    binder.DestroyBindTarget(binderTarget);
                    binderTarget = null;
                }

                if (binderTarget == null)
                {
                    binderTarget = binder.CreateBinderTarget(director);
                    binderTarget.Init(this);
                }

                if (!binder.Bind(binderTarget))
                {
                    error = TimelinePlayerError.BindFailed;
                    goto EndPoint;
                }
            }
            else
            {
                binderTarget = null;
            }

            EndPoint:
            if (error != TimelinePlayerError.None)
            {
                State = TimePlayerState.Error;
                eventListener?.OnError(error);
            }
        }

        public void Play(T args)
        {
            if (State != TimePlayerState.Idle) return;
            State = TimePlayerState.Playing;
            this.args = args;

            director.Play();
        }

        public void Stop()
        {
            if (State != TimePlayerState.Playing) return;
            State = TimePlayerState.Idle;
            director.Stop();
            OnStop();
        }

        private void OnStop()
        {

        }

        private void OnDirectorStop(PlayableDirector director)
        {
            if (State != TimePlayerState.Playing) return;

            Debug.Log($"{GetType().Name} OnDirectorStop");
            State = TimePlayerState.Idle;
            OnStop();
        }

        public void Destroy()
        {
            Stop();
            
            if (director)
            {
                director.stopped -= OnDirectorStop;
            }
            
            if (binderTarget != null)
            {
                binderTarget.Destroy();
                binderTarget = null;
            }
        }
    }
}

