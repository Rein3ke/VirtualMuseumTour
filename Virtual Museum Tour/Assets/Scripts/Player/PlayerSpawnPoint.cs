using DollHouseView;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Player
{
    /// <summary>
    /// This class is used in the PlayerSpawnPoint prefab.
    /// It defines a point where the player can spawn.
    /// This class also inherits from the IPoi interface, which displays a Icon in the doll house view.
    /// </summary>
    public class PlayerSpawnPoint : MonoBehaviour, IPoi
    {
        #region Serialized Fields

        /// <summary>
        /// The name of the PlayerSpawnPoint that will be set in the editor.
        /// </summary>
        [SerializeField] private string playerSpawnName;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the name of the PlayerSpawnPoint.
        /// </summary>
        public string PlayerSpawnName => playerSpawnName;
        /// <summary>
        /// Instance of the POI object initialized and declared in InstantiatePoi().
        /// </summary>
        public PointOfInterest Poi { get; private set; }

        #endregion

        #region IPoi Implementation

        /// <summary>
        /// The PoiController uses the IPoi interface to initialize and declare a POI object via this method.
        /// Here a POI prefab is instantiated and the properties of the POI object are set.
        /// </summary>
        public void InstantiatePoi()
        {
            if (Poi != null) return;
        
            Poi = Instantiate(Resources.Load<PointOfInterest>(PointOfInterest.PrefabPath), transform);
            Poi.IsClickable = true;
            Poi.PoiType = PoiType.PlayerSpawnPoint;
            Poi.HoverText = $"{nameof(PlayerSpawnPoint)}: {playerSpawnName}";
            Poi.OnClick += PoiOnClick;
        }

        #endregion

        /// <summary>
        /// Handles the click event of the POI object.
        /// A teleport request is sent to the PlayerSpawnController via the event system.
        /// </summary>
        private void PoiOnClick()
        {
            EventManager.TriggerEvent(EventType.EventTeleportRequest, new EventParam
            {
                EventPlayerSpawnPoint = this
            });
        }
    }
}
