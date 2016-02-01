using System;

namespace HoloToolkit.XTools
{
    /// <summary>
    /// Allows users of ObjectElements to register to receive event callbacks without
    /// having their classes inherit directly from ObjectElementListener
    /// </summary>
    public class ObjectElementAdapter : ObjectElementListener
    {
        public event Action<long, int> IntChangedEvent;
        public event Action<long, float> FloatChangedEvent;
        public event Action<long, XString> StringChangedEvent;
        public event Action<Element> ElementAddedEvent;
        public event Action<Element> ElementDeletedEvent;

        public ObjectElementAdapter()
        {

        }

        public override void OnIntElementChanged(long elementID, int newValue)
        {
            XTools.Profile.BeginRange("OnIntElementChanged");
            if (this.IntChangedEvent != null)
            {
                this.IntChangedEvent(elementID, newValue);
            }
            XTools.Profile.EndRange();
        }

        public override void OnFloatElementChanged(long elementID, float newValue)
        {
            XTools.Profile.BeginRange("OnFloatElementChanged");
            if (this.FloatChangedEvent != null)
            {
                this.FloatChangedEvent(elementID, newValue);
            }
            XTools.Profile.EndRange();
        }

        public override void OnStringElementChanged(long elementID, XString newValue)
        {
            XTools.Profile.BeginRange("OnStringElementChanged");
            if (this.StringChangedEvent != null)
            {
                this.StringChangedEvent(elementID, newValue);
            }
            XTools.Profile.EndRange();
        }

        public override void OnElementAdded(Element element)
        {
            XTools.Profile.BeginRange("OnElementAdded");
            if (this.ElementAddedEvent != null)
            {
                this.ElementAddedEvent(element);
            }
            XTools.Profile.EndRange();
        }

        public override void OnElementDeleted(Element element)
        {
            XTools.Profile.BeginRange("OnElementDeleted");
            if (this.ElementDeletedEvent != null)
            {
                this.ElementDeletedEvent(element);
            }
            XTools.Profile.EndRange();
        }
    }
}
