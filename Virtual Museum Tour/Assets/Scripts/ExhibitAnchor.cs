using System.Collections.Generic;
using System.Linq;
using DollHouseView;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// The ExhibitAnchor class is used to instantiate exhibits. It stores the ID of the exhibit, as well as the instantiation scaling factor.
/// In addition, the ExhibitDirection vector can be used to determine the direction in which the exhibit should be oriented when instantiated.
/// This class is part of the ExhibitAnchor prefab and can be configured within the scene via the Inspector.
/// </summary>
public class ExhibitAnchor : MonoBehaviour, IPoi
{
	#region Serialized Fields

	/// <summary>
	/// ExhibitID which is used to assign an exhibit. Should contain the name of the Exhibit from the AssetBundle.
	/// </summary>
	[SerializeField] private string exhibitID;

	/// <summary>
	/// The scale factor used when instantiating the exhibit. Used for quick correction if the exhibit is not the desired size.
	/// </summary>
	[SerializeField] private float exhibitScale = 1f;

	/// <summary>
	/// A vector to influence the orientation of the exhibit afterwards. The exhibit looks at this vector after instantiation.
	/// </summary>
	[SerializeField] private Vector3 exhibitDirection;

	#endregion

	#region Properties

	/// <summary>
	/// Returns the direction in which the exhibit should be oriented after instantiation.
	/// </summary>
	public Vector3 ExhibitDirection => exhibitDirection;
	/// <summary>
	/// Returns the scale factor used when instantiating the exhibit.
	/// </summary>
	public float ExhibitScale => exhibitScale;


	/// <summary>
	/// Property for determining whether an exhibit is instantiated as a child element of ExhibitAnchor.
	/// </summary>
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

	public PointOfInterest Poi { get; private set; }

	#endregion

	#region Unity Methods

	/// <summary>
	/// If an ExhibitAnchor is instantiated, it is added to the static list of ExhibitAnchors.
	/// </summary>
	private void Awake()
	{
		ExhibitAnchors.Add(this);
	}

	/// <summary>
	/// If an ExhibitAnchor is destroyed, it is removed from the static list of ExhibitAnchors.
	/// </summary>
	private void OnDestroy()
	{
		ExhibitAnchors.Remove(this);
	}

	private void OnDrawGizmosSelected()
	{
		var position = transform.position;

		Gizmos.color = Color.red;
		Vector3 direction = (exhibitDirection - position).normalized;
		Gizmos.DrawRay(position, direction * Vector3.Distance(position, exhibitDirection));
		Gizmos.DrawSphere(exhibitDirection, 0.1f);
	}

	#endregion

	#region IPoi Implementation

	/// <summary>
	/// Instantiates the POI and configures it according to the ExhibitAnchor settings.
	/// </summary>
	public void InstantiatePoi()
	{
		if (Poi != null || !ContainsExhibit) return;

		Poi = Instantiate(Resources.Load<PointOfInterest>(PointOfInterest.PrefabPath), transform);
		Poi.IsClickable = false;
		Poi.PoiType = PoiType.Exhibit;
		Poi.HoverText = $"{nameof(ExhibitAnchor)}: {exhibitID}";
	}

	#endregion

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
}