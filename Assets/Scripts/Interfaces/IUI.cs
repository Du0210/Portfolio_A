using System;
namespace HDU.Interface
{
    public interface IUI
    {

    }

    public interface IPopupUI
    {
        public bool IsCantCloseBack { get; set; }
        public Action<IPopupUI, bool> OnCloseRequeted { get; set; }
    }
}