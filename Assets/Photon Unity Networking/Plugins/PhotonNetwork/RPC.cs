
#pragma warning disable 1587
/// \file
/// <summary>Reimplements a RPC Attribute, as it's no longer in all versions of the UnityEngine assembly.</summary>
#pragma warning restore 1587

using System;

/// <summary>Replacement for RPC attribute with different name. Used to flag methods as remote-callable.</summary>
public class PunRPC : Attribute
{
}