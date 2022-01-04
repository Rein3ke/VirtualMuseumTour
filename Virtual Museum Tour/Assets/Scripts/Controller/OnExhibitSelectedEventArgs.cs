using System;

namespace Controller
{
    public class OnExhibitSelectedEventArgs: EventArgs
    {
        public Exhibit Exhibit { get; private set; }

        public OnExhibitSelectedEventArgs(Exhibit exhibit)
        {
            Exhibit = exhibit;
        }
    }
}