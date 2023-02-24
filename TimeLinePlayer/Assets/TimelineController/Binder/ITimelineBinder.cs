using UnityEngine;

namespace TimelineControl
{
    // 暂时不考虑骨架重新加载的情况
    // 事件也应该在这处理
    public interface ITimelineBinder
    {
        void Init(TimelineControllerBase target);
        void Bind(string streamName, Object value);
        void Destroy();
    }
}
