using UnityEngine;

public class ExhibitAnchor : MonoBehaviour
{
    [SerializeField] private string exhibitID;
    [SerializeField] private ExhibitData exhibitData;
    [SerializeField] private AudioClip[] audioClips;

    public ExhibitData ExhibitData
    {
        get => exhibitData;
        set
        {
            if (exhibitID.Equals(value.exhibitName))
            {
                if (exhibitData == null)
                {
                    exhibitData = value;
                    LoadAudioClipsFromExhibitData();
                }
                else
                {
                    Debug.LogWarning($"ExhibitAnchor [{exhibitID}]: ExhibitData already set.");
                }
            }
            else
            {
                Debug.LogWarning($"ExhibitAnchor [{exhibitID}]: Can't add given ExhibitData. IDs doesn't match!");
            }
        } 
    }

    private void LoadAudioClipsFromExhibitData()
    {
        if (exhibitData != null && exhibitData.audioClips.Length > 0)
        {
            audioClips = exhibitData.audioClips;
        }
        else
        {
            return;
        }
    }

    public string ExhibitID => exhibitID;
    
}
