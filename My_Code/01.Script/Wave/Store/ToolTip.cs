using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptText;
    [SerializeField] private VideoPlayer videoPlayer;

    public void SetToolTip(string name, string des, VideoClip videoClip)
    {
        nameText.text = name;
        descriptText.text = des;
        videoPlayer.clip = videoClip;
    }
}
