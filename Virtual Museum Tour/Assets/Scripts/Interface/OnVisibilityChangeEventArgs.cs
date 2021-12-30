using System;

namespace Interface
{
    public class OnVisibilityChangeEventArgs : EventArgs
    {
        public bool IsVisible { get; private set; }

        public OnVisibilityChangeEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}