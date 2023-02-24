using TimelineControl;
using UnityEngine;

public class TestTimelinePlayer : MonoBehaviour
{
    public GameObject asset;
    private TimelinePlayer<object> player;
    
    void Start()
    {
        player = new TimelinePlayer<object>();
        player.Init(asset, null, null);
        player.Play(null);
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
    }
}
