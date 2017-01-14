// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of ObjectElements to register to receive event callbacks without
    /// having their classes inherit directly from ObjectElementListener
    /// </summary>
    public class ObjectElementAdapter : ObjectElementListener
    {
        public event System.Action<long, bool> BoolChangedEvent;
        public event System.Action<long, int> IntChangedEvent;
        public event System.Action<long, long> LongChangedEvent;
        public event System.Action<long, float> FloatChangedEvent;
        public event System.Action<long, double> DoubleChangedEvent;
        public event System.Action<long, XString> StringChangedEvent;
        public event System.Action<Element> ElementAddedEvent;
        public event System.Action<Element> ElementDeletedEvent;

        /// <summary>
        /// Initializes a new instance of <see cref="ObjectElementAdapter"/>.
        /// </summary>
        public ObjectElementAdapter() { }

        /// <summary>
        /// Throws the <see cref="IntChangedEvent"/>.
        /// </summary>
        /// <param name="elementID">The Elements id.</param>
        /// <param name="newValue">The new int value.</param>
        public override void OnIntElementChanged(long elementID, int newValue)
        {
            Profile.BeginRange("OnIntElementChanged");
            if (this.IntChangedEvent != null)
            {
                this.IntChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        /// <summary>
        /// Throws the <see cref="DoubleChangedEvent"/>.
        /// </summary>
        /// <param name="elementID">The Elements id.</param>
        /// <param name="newValue">The new double value.</param>
        public override void OnDoubleElementChanged(long elementID, double newValue)
        {
            Profile.BeginRange("OnDoubleElementChanged");
            if (this.DoubleChangedEvent != null)
            {
                this.DoubleChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        /// <summary>
        /// Throws the <see cref="FloatChangedEvent"/>.
        /// </summary>
        /// <param name="elementID">The Elements id.</param>
        /// <param name="newValue">The new float value.</param>
        public override void OnFloatElementChanged(long elementID, float newValue)
        {
            Profile.BeginRange("OnFloatElementChanged");
            if (this.FloatChangedEvent != null)
            {
                this.FloatChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        /// <summary>
        /// Throws the <see cref="LongChangedEvent"/>.
        /// </summary>
        /// <param name="elementID">The Elements id.</param>
        /// <param name="newValue">The new long value.</param>
        public override void OnLongElementChanged(long elementID, long newValue)
        {
            Profile.BeginRange("OnLongElementChanged");
            if (this.LongChangedEvent != null)
            {
                this.LongChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        /// <summary>
        /// Throws the <see cref="StringChangedEvent"/>.
        /// </summary>
        /// <param name="elementID">The Elements id.</param>
        /// <param name="newValue">The new string value.</param>
        public override void OnStringElementChanged(long elementID, XString newValue)
        {
            Profile.BeginRange("OnStringElementChanged");
            if (this.StringChangedEvent != null)
            {
                this.StringChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        /// <summary>
        /// Throws the <see cref="BoolChangedEvent"/>.
        /// </summary>
        /// <param name="elementID">The Elements id.</param>
        /// <param name="newValue">The new bool value</param>
        public override void OnBoolElementChanged(long elementID, bool newValue)
        {
            Profile.BeginRange("OnBoolElementChanged");
            if (this.BoolChangedEvent != null)
            {
                this.BoolChangedEvent(elementID, newValue);
            }
            Profile.EndRange();
        }

        /// <summary>
        /// Throws the <see cref="ElementAddedEvent"/>.
        /// </summary>
        /// <param name="element">The new Element.</param>
        public override void OnElementAdded(Element element)
        {
            Profile.BeginRange("OnElementAdded");
            if (this.ElementAddedEvent != null)
            {
                this.ElementAddedEvent(element);
            }
            Profile.EndRange();
        }

        /// <summary>
        /// Throws the <see cref="ElementDeletedEvent"/>.
        /// </summary>
        /// <param name="element">The deleted Element.</param>
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