using UnityEngine;

public class Exhibit
{ 
    public GameObject Anchor { get; set; }
    public string Name { get; set; }
    public GameObject Asset { get; set; }

    public ExhibitData ExhibitData
    {
        get => _exhibitData;
        set
        {
            _exhibitData = value;
            Description = _exhibitData.exhibitDescription;
            AudioClips = _exhibitData.audioClips;
        }
    }
    private ExhibitData _exhibitData;
    
    public string Description { get; private set; }
    public AudioClip[] AudioClips { get; private set; }
    public Texture[] Textures { get; private set; }
}