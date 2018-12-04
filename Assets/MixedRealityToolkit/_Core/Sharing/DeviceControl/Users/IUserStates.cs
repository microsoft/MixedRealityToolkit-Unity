using Pixie.Core;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public interface IUserTransforms : IGameObject
    {
        bool GetTransform(TransformTypeEnum type, out Transform transform);
    }
}