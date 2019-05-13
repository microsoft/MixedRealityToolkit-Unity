// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteImage : RemoteComponent<Image>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedImage.ChangeType changeType = (SynchronizedImage.ChangeType)message.ReadByte();

            if (SynchronizedImage.HasFlag(changeType, SynchronizedImage.ChangeType.Data))
            {
                attachedComponent.overrideSprite = ImageService.Instance.GetSprite(message.ReadGuid());
                attachedComponent.sprite = ImageService.Instance.GetSprite(message.ReadGuid());
                attachedComponent.fillAmount = message.ReadSingle();
                attachedComponent.color = message.ReadColor();

                attachedComponent.alphaHitTestMinimumThreshold = message.ReadSingle();
                attachedComponent.fillOrigin = message.ReadInt32();
                attachedComponent.fillClockwise = message.ReadBoolean();
                attachedComponent.fillMethod = (Image.FillMethod)message.ReadByte();
                attachedComponent.fillCenter = message.ReadBoolean();
                attachedComponent.preserveAspect = message.ReadBoolean();
                attachedComponent.type = (Image.Type)message.ReadByte();
                attachedComponent.enabled = message.ReadBoolean();
            }
            if (SynchronizedImage.HasFlag(changeType, SynchronizedImage.ChangeType.Materials))
            {
                attachedComponent.material = MaterialPropertyAsset.ReadMaterials(message, null)?[0];
            }
            if (SynchronizedImage.HasFlag(changeType, SynchronizedImage.ChangeType.MaterialProperty))
            {
                int materialIndex = message.ReadInt32();
                MaterialPropertyAsset.Read(message, new Material[] { attachedComponent.material }, materialIndex);
            }
        }

        public void LerpRead(BinaryReader message, float lerpVal)
        {
            if (attachedComponent == null)
                return;

            SynchronizedImage.ChangeType changeType = (SynchronizedImage.ChangeType)message.ReadByte();

            //Only lerp messages with data changes on its own
            if (changeType == SynchronizedImage.ChangeType.Data)
            {
                attachedComponent.overrideSprite = ImageService.Instance.GetSprite(message.ReadGuid());
                attachedComponent.sprite = ImageService.Instance.GetSprite(message.ReadGuid());
                attachedComponent.fillAmount = Mathf.Lerp(attachedComponent.fillAmount, message.ReadSingle(), lerpVal);
                attachedComponent.color = Color.Lerp(attachedComponent.color, message.ReadColor(), lerpVal);

                //Dont read and lerp the rest of the data
                return;
            }
        }

    }
}