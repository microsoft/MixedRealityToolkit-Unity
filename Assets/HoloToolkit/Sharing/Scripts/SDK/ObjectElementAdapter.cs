//
// Copyright (C) Microsoft. All rights reserved.
//

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of ObjectElements to register to receive event callbacks without
    /// having their classes inherit directly from ObjectElementListener
    /// </summary>
    public class ObjectElementAdapter : ObjectElementListener
    {
        public event System.Action<long, int> IntChangedEvent;
        public event System.Action<long, float> FloatChangedEvent;
        public event System.Action<long, XString> StringChangedEvent;
        public event System.Action<Element> ElementAddedEvent;
        public event System.Action<Element> ElementDeletedEvent;

        public ObjectElementAdapter() { }

        public override void OnIntElementChanged(long elementID, int newValue)
        {
            Profile.BeginRange("OnIntElementChanged");
            if (this.IntChangedEvent != null)
            {
                this.IntChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        public override void OnFloatElementChanged(long elementID, float newValue)
        {
            Profile.BeginRange("OnFloatElementChanged");
            if (this.FloatChangedEvent != null)
            {
                this.FloatChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        public override void OnStringElementChanged(long elementID, XString newValue)
        {
            Profile.BeginRange("OnStringElementChanged");
            if (this.StringChangedEvent != null)
            {
                this.StringChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        public override void OnElementAdded(Element element)
        {
            Profile.BeginRange("OnElementAdded");
            if (this.ElementAddedEvent != null)
            {
                this.ElementAddedEvent(element);
            }
            Profile.EndRange();
        }

        public override void OnElementDeleted(Element element)
        {
            Profile.BeginRange("OnElementDeleted");
            if (this.ElementDeletedEvent != null)
            {
                this.ElementDeletedEvent(element);
            }
            Profile.EndRange();
        }
    }
}
