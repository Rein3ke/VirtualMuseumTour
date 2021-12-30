using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExhibitAnchor : MonoBehaviour
{
    public static List<ExhibitAnchor> ExhibitAnchors { get; } = new List<ExhibitAnchor>();

    [SerializeField] private string exhibitID;

    public string ExhibitID => exhibitID;

    private void Awake()
    {
        ExhibitAnchors.Add(this);
    }

    private void OnDestroy()
    {
        ExhibitAnchors.Remove(this);
    }
    
    public static ExhibitAnchor GetExhibitAnchor(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError($"GetExhibitAnchor: ID '{id}' can't be used.");
            return null;
        }
        
        return ExhibitAnchors.FirstOrDefault(anchor => anchor.ExhibitID.Equals(id));
    }
}
