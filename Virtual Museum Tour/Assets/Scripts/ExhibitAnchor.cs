using System.Collections.Generic;
using System.Linq;
using DollHouseView;
using UnityEngine;

public class ExhibitAnchor : MonoBehaviour, IPoi
{
    /// <summary>
    /// ExhibitID which is used to assign an exhibit. Should have the name of the Exhibit from the AssetBundle.
    /// </summary>
    [SerializeField] private string exhibitID;

    /// <summary>
    /// Static list to access all ExhibitAnchors.
    /// </summary>
    public static List<ExhibitAnchor> ExhibitAnchors { get; } = new List<ExhibitAnchor>();

    /// <summary>
    /// Public accessor for exhibitID.
    /// </summary>
    public string ExhibitID => exhibitID;

    /// <summary>
    /// If an ExhibitAnchor is destroyed it will be removed from the list.
    /// </summary>
    private void Awake()
    {
        ExhibitAnchors.Add(this);
    }

    /// <summary>
    /// If an ExhibitAnchor is destroyed it will be removed from the list.
    /// </summary>
    private void OnDestroy()
    {
        ExhibitAnchors.Remove(this);
    }

    public PointOfInterest Poi { get; private set; }

    public void InstantiatePoi()
    {
        if (Poi != null) return;

        Poi = Instantiate(Resources.Load<PointOfInterest>(PointOfInterest.PrefabPath), transform);
        Poi.IsClickable = false;
        Poi.PoiType = PoiType.Exhibit;
        Poi.HoverText = $"{nameof(ExhibitAnchor)}: {exhibitID}";
    }

    /// <summary>
    /// Returns an ExhibitAnchor if it could be found via the exhibit ID.
    /// </summary>
    /// <param name="id">ID of the exhibit.</param>
    /// <returns>The ExhibitAnchor found or null.</returns>
    public static ExhibitAnchor GetExhibitAnchor(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError($"{nameof(GetExhibitAnchor)}: ID '{id}' can't be accepted.");
            return null;
        }
        
        return ExhibitAnchors.FirstOrDefault(anchor => anchor.ExhibitID.Equals(id));
    }
}
