using TimelineControl;
using UnityEngine;

public class TestTimelinePlayer : MonoBehaviour
{
    public GameObject asset;
    private TimelinePlayer<object> player;
    public Animator cube;
    public Animator sphere;
    
    void Start()
    {
        player = new TimelinePlayer<object>();
        player.Init(asset, new TimelineBinderBase());
    }

    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUILayout.Label($"{player.State}");
        
        if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.Height(100)))
        {
            player.Play(null);
        }
        
        if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(100)))
        {
            player.Stop();
        }
        
        if (GUILayout.Button("BindCube", GUILayout.Width(100), GUILayout.Height(100)))
        {
            player.Binder.Bind("_MoveTrack", cube);
        }
        
        if (GUILayout.Button("BindSphere", GUILayout.Width(100), GUILayout.Height(100)))
        {
            player.Binder.Bind("_MoveTrack", sphere);
        }
        
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.Destroy();
        }
    }
}
