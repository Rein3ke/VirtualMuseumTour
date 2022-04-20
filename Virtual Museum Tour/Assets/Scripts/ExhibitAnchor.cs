using System.Collections.Generic;
using System.Linq;
using DollHouseView;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ExhibitAnchor : MonoBehaviour, IPoi
{
    /// <summary>
    /// ExhibitID which is used to assign an exhibit. Should have the name of the Exhibit from the AssetBundle.
    /// </summary>
    [SerializeField] private string exhibitID;
    [SerializeField] private float exhibitScale = 1f;
    [SerializeField] private Vector3 exhibitDirection;
    
    public Vector3 ExhibitDirection => exhibitDirection;
    public float ExhibitScale => exhibitScale;

    public bool ContainsExhibit
    {
        get
        {
            var exhibit = transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("Exhibit"));
            return exhibit != null;
        }
    }

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
        if (Poi != null || !ContainsExhibit) return;

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
        
        var exhibitAnchors = ExhibitAnchors.Where(e => !string.IsNullOrWhiteSpace(e.exhibitID)).ToList();      
        return exhibitAnchors.FirstOrDefault(anchor => anchor.ExhibitID.Equals(id));
    }

    private void OnValidate()
    {
        // exhibitDirection = new Vector3(exhibitDirection.x, Mathf.Clamp(exhibitDirection.y, -1f, 2f), exhibitDirection.z);
    }

    private void OnDrawGizmosSelected()
    {
        var position = transform.position;
        
        Gizmos.color = Color.red;
        Vector3 direction = (exhibitDirection - position).normalized;
        Gizmos.DrawRay(position, direction * Vector3.Distance(position, exhibitDirection));
        Gizmos.DrawSphere(exhibitDirection, 0.1f);
    }
}
