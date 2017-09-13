// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of the pairing API to register to receive pairing event callbacks without
    /// having their classes inherit directly from PairingListener
    /// </summary>
    public class PairingAdapter : PairingListener
    {
        public event System.Action SuccessEvent;
        public event System.Action<PairingResult> FailureEvent;

        public PairingAdapter() { }

        public override void PairingConnectionSucceeded()
        {
            if (this.SuccessEvent != null)
            {
                this.SuccessEvent();
            }
        }

        public override void PairingConnectionFailed(PairingResult result)
        {
            if (this.FailureEvent != null)
            {
                this.FailureEvent(result);
            }
        }
    }
}
