//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System;

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// Used to markup SyncPrimitives within a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SyncDataAttribute : Attribute
    {
        public string CustomFieldName;
    }

    /// <summary>
    /// Used to markup SyncObject classes, so that they properly get instantiated 
    /// when using a hierarchical data model that inherits from SyncObject.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SyncDataClassAttribute : Attribute
    {
        public string CustomClassName;
    }
}