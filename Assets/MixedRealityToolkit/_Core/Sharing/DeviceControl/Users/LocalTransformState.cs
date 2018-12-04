using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.DeviceControl
{
    [Serializable]
    public struct LocalTransformState : IItemState, IItemStateComparer<LocalTransformState>
    {
        public const float MaxPositionRange = 50f;
        public const float MaxObjectVelocity = 16f;

        public LocalTransformState(short key)
        {
            ItemID = key;
            Type = TransformTypeEnum.None;
            UserID = -1;

            RotX = 0;
            RotY = 0;
            RotZ = 0;

            PosX = 0;
            PosY = 0;
            PosZ = 0;

            Vel = 0;
        }

        public LocalTransformState(short key, TransformTypeEnum type, short userID)
        {
            ItemID = key;
            Type = type;
            UserID = userID;

            RotX = 0;
            RotY = 0;
            RotZ = 0;

            PosX = 0;
            PosY = 0;
            PosZ = 0;

            Vel = 0;
        }

        short IItemState.Key { get { return ItemID; } }

        public short ItemID;
        public TransformTypeEnum Type;
        public short UserID;

        public bool IsDifferent(LocalTransformState from)
        {
            return ItemID != from.ItemID
                || UserID != from.UserID

                || RotX != from.RotX
                || RotY != from.RotY
                || RotZ != from.RotZ

                || PosX != from.PosX
                || PosY != from.PosY
                || PosZ != from.PosZ

                || Vel != from.Vel;
        }

        public LocalTransformState Merge(LocalTransformState clientValue, LocalTransformState serverValue)
        {
            return serverValue;
        }

        public Vector3 LocalPosition
        {
            get { return StateUtils.ShortPosOut(PosX, PosY,PosZ, MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out PosX, out PosY, out PosZ, MaxPositionRange); }
        }

        public Vector3 LocalEulerAngles
        {
            get { return StateUtils.ByteRotOut(RotX, RotY, RotZ); }
            set { StateUtils.ByteRotIn(value, out RotX, out RotY, out RotZ); }
        }

        public float Velocity
        {
            get { return StateUtils.UShortValOut(Vel, MaxObjectVelocity); }
            set { Vel = StateUtils.UShortValIn(value, MaxObjectVelocity); }
        }

        #region raw values
        public byte RotX;
        public byte RotY;
        public byte RotZ;

        public short PosX;
        public short PosY;
        public short PosZ;

        public ushort Vel;
        #endregion

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}