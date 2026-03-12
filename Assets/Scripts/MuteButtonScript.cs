using UnityEngine;

public class MuteButtonScript : MonoBehaviour
{
    public void ToggleGameMute()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            
            // Update Icon
            var img = GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                #if UNITY_EDITOR
                string path = AudioManager.Instance.IsMuted() ? "Assets/Images/exit.png" : "Assets/Images/pause.png";
                img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                #endif
            }
            
            Debug.Log("Mute toggled: " + AudioManager.Instance.IsMuted());
        }
    }
}
