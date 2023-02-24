using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace TimelineControl
{
    public class BindEntry
    {
        public Object value;
        public Func<Object> funcValue;
        public readonly List<Object> keyList = new List<Object>();
    }
    
    public class TimelineBinderBase : ITimelineBinder
    {
        private TimelineControllerBase controller;
        private Dictionary<string, BindEntry> dic;
        public void Init(TimelineControllerBase controller)
        {
            this.controller = controller;
        }

        public void Bind(string streamName, Object value)
        {
            var bindings = controller.Director.playableAsset.outputs;

            if (dic == null) dic = new Dictionary<string, BindEntry>();
            if (!dic.TryGetValue(streamName, out var entry))
            {
                entry = new BindEntry {};

                foreach (var playableBinding in bindings)
                {
                    if ((playableBinding.streamName != streamName)) continue;
                    entry.keyList.Add(playableBinding.sourceObject);
                }
                
                dic[streamName] = entry;
            }

            entry.value = value;

            ApplyBind(entry);
        }

        public void Rebind()
        {
            if (dic == null || dic.Count == 0) return;
            
            foreach (var kv in dic)
            {
                ApplyBind(kv.Value);
            }
        }

        private void ApplyBind(BindEntry entry)
        {
            var value = entry.value;
            if (!value)
            {
                if (entry.funcValue != null)
                {
                    value = entry.funcValue.Invoke();
                }
            }
            
            for (var i = 0; i < entry.keyList.Count; i++)
            {
                controller.Director.SetGenericBinding(entry.keyList[i], value);
            }
        }

        public void Destroy()
        {
            controller = null;
            dic?.Clear();
            
        }
    }
}

