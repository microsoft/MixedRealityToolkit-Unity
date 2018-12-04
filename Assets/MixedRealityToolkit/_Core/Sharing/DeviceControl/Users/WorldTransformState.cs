using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.DeviceControl
{
    [Serializable]
    public struct WorldTransformState : IItemState, IItemStateComparer<WorldTransformState>
    {
        public const float MaxObjectVelocity = 16f;

        public WorldTransformState(short key)
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

        public WorldTransformState(short key, TransformTypeEnum type, short userID)
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

        public bool IsDifferent(WorldTransformState from)
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

        public WorldTransformState Merge(WorldTransformState clientValue, WorldTransformState serverValue)
        {
            return serverValue;
        }

        public Vector3 Position
        {
            get
            {
                posValue.x = PosX;
                posValue.y = PosY;
                posValue.z = PosZ;
                return posValue;
            }
            set
            {
                PosX = value.x;
                PosY = value.y;
                PosZ = value.z;
            }
        }

        public Vector3 EulerAngles
        {
            get
            {
                rotValue.x = RotX;
                rotValue.y = RotY;
                rotValue.z = RotZ;
                return posValue;
            }
            set
            {
                RotX = value.x;
                RotY = value.y;
                RotZ = value.z;
            }
        }

        public float Velocity
        {
            get { return StateUtils.UShortValOut(Vel, MaxObjectVelocity); }
            set { Vel = StateUtils.UShortValIn(value, MaxObjectVelocity); }
        }

        #region raw values
        public float PosX;
        public float PosY;
        public float PosZ;

        public float RotX;
        public float RotY;
        public float RotZ;

        public ushort Vel;
        #endregion

        private static Vector3 posValue;
        private static Vector3 rotValue;

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}