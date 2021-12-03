using UnityEngine;

public class ExhibitAnchor : MonoBehaviour
{
    [SerializeField] private string exhibitID;
    [SerializeField] private ExhibitData exhibitData;

    public ExhibitData ExhibitData
    {
        get => exhibitData;
        set => exhibitData = value;
    }

    public string ExhibitID => exhibitID;
    
}
