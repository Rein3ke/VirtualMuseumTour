using UnityEngine;
using com.rein3ke.virtualtour.core;

/// <summary>
/// Exhibit data object. Stores the anchor GameObject as well as the exhibit's name, description, AudioClip and Textures.
/// Also, a property is used to automatically retrieve data from a specific ExhibitData object.
/// </summary>
public class Exhibit
{
	#region Properties

	/// <summary>
	/// The anchor GameObject which is used to position the exhibit.
	/// </summary>
	public GameObject Anchor { get; set; }
	/// <summary>
	/// The name or ID of an exhibit. Used to assign a matching ExhibitAnchor.
	/// </summary>
	public string Name { get; set; }
	/// <summary>
	/// The asset to be instantiated.
	/// </summary>
	public GameObject Asset { get; set; }
	/// <summary>
	/// The ExhibitData object, which contains the description text, audio clips and textures.
	/// </summary>
	public ExhibitData ExhibitData
	{
		get => _exhibitData;
		set // If the ExhibitData object is changed, the new data is automatically applied to the exhibit.
		{
			_exhibitData = value;
			Description = _exhibitData.exhibitDescription;
			AudioClips = _exhibitData.audioClips;
			Textures = _exhibitData.images;
		}
	}

	/// <summary>
	/// Text of the exhibit description. Used in the detail view.
	/// </summary>
	public string Description { get; private set; }
	/// <summary>
	/// List of audio clips. Are loaded into the audio dropdown of the details view.
	/// </summary>
	public AudioClip[] AudioClips { get; private set; }
	/// <summary>
	/// List of all textures (currently not used).
	/// </summary>
	public Texture2D[] Textures { get; private set; }

	#endregion

	#region Members

	private ExhibitData _exhibitData;

	#endregion
}