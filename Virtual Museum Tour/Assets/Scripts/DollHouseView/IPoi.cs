namespace DollHouseView
{
    /// <summary>
    /// Interface to be implemented by classes that can be represented by a point of interest.
    /// </summary>
    public interface IPoi
    {
        /// <summary>
        /// Used to store the point of interest object that is representative of the implementing class.
        /// This property is assigned when a point of interest is created.
        /// </summary>
        PointOfInterest Poi { get; }
        
        /// <summary>
        /// Creates a point of interest.
        /// Called multiple times from an EventAssetBundleLoaded event.
        /// </summary>
        void InstantiatePoi();
    }
}