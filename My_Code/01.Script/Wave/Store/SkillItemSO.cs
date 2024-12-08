using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "SO/SkillItem")]
public class SkillItemSO : ScriptableObject
{
    [OnValueChanged("ChangeAssetName"), Delayed]
    public string skillTypeName;

    [OnValueChanged("ChangeAssetName"), Delayed]
    public int keyIdx = -1;

    [OnValueChanged("ChangeAssetName"), Delayed] [TextArea]
    public string skillDescription;

    public VideoClip videoClip;

    private string newName;

#if UNITY_EDITOR

    private void ChangeAssetName()
    {
        if (string.IsNullOrEmpty(skillTypeName)) return;
        newName = skillTypeName;
        string assetPath = AssetDatabase.GetAssetPath(this);
        AssetDatabase.RenameAsset(assetPath, $"{newName} (KeyCode_{KeyAlpha(keyIdx)})");
        AssetDatabase.SaveAssets();
    }
#endif


    private string KeyAlpha(int idx)
    {
        switch (idx)
        {
            case 0:
                return "Q";
            case 1:
                return "E";
            case 2:
                return "R";
            default:
                return "Passive";
        }
    }
}