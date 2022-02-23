namespace DollHouseView
{
    public interface IPoi
    {
        PointOfInterest Poi { get; }
        
        void InstantiatePoi();
    }
}