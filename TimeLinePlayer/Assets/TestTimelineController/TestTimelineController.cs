using TimelineControl;
using UnityEngine;

public class TestTimelineController : MonoBehaviour
{
    public GameObject asset;
    private TimelineController<object> controller;
    public Animator cube;
    public Animator sphere;
    
    void Start()
    {
        controller = new TimelineController<object>();
        controller.Init(asset, new TimelineBinderBase());
    }

    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUILayout.Label($"{controller.State}");
        
        if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.Height(100)))
        {
            controller.Play(null);
        }
        
        if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(100)))
        {
            controller.Stop();
        }
        
        if (GUILayout.Button("BindCube", GUILayout.Width(100), GUILayout.Height(100)))
        {
            controller.Binder.Bind("_MoveTrack", cube);
        }
        
        if (GUILayout.Button("BindSphere", GUILayout.Width(100), GUILayout.Height(100)))
        {
            controller.Binder.Bind("_MoveTrack", sphere);
        }
        
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.Destroy();
        }
    }
}
