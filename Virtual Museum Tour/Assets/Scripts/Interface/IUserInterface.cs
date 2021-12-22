using System;

namespace Interface
{
    public interface IUserInterface
    {
        event EventHandler<OnVisibilityChangeEventArgs> OnVisibilityChange;
        bool IsVisible { get; set; }
        void ShowInterface();
        void HideInterface();
        void InvokeOnVisibilityChange(bool isVisible);
    }
}