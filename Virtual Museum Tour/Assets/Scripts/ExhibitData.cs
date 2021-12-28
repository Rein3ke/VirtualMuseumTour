using UnityEngine;

[CreateAssetMenu(fileName = "ExhibitData", menuName = "Exhibits/Create ExhibitData", order = 1)]
public class ExhibitData : ScriptableObject
{
    public string exhibitName;
    [TextArea]
    public string exhibitDescription;

    public AudioClip[] audioClips;
    public Texture2D[] images;
}
