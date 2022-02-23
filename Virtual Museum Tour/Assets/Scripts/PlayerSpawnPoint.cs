using DollHouseView;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour, IPoi
{
    [SerializeField] private string playerSpawnName;

    public string PlayerSpawnName => playerSpawnName;
    public PointOfInterest Poi { get; private set; }

    public void InstantiatePoi()
    {
        if (Poi != null) return;
        
        Poi = Instantiate(Resources.Load<PointOfInterest>(PointOfInterest.PrefabPath), transform);
        Poi.IsClickable = true;
        Poi.PoiType = PoiType.PlayerSpawnPoint;
        Poi.OnClick += PoiOnClick;
    }

    private static void PoiOnClick()
    {
        Debug.Log("POI clicked!");
    }
}
