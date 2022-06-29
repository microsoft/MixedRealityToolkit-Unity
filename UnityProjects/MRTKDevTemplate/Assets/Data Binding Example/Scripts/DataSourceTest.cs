// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Simple test data source that programmatically changes variables in a data source.
    /// </summary>
    /// <remarks>
    /// Using a simple <key,value> store, it's possible to separate data from view
    /// to simplify the integration of generic view prefabs that are populated from
    /// external information.
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Data Binding/Data Source Test")]
    public class DataSourceTest : DataSourceGOBase
    {
        private float _deltaSeconds;
        private int _nextOneSecondTarget;
        private int _nextFiveSecondTarget;
        private bool _styleYellow;
        private int _status = 0;
        private int _score = 100;

        [SerializeField]
        [Tooltip("Open status sprite for statusSprite keypath")]
        private Sprite statusSpriteOpen;

        [SerializeField]
        [Tooltip("Pending status sprite for statusSprite keypath")]
        private Sprite statusSpritePending;

        [SerializeField]
        [Tooltip("Cancelled status sprite for statusSprite keypath")]
        private Sprite statusSpriteCancelled;

        [SerializeField]
        [Tooltip("In Progress status sprite for statusSprite keypath")]
        private Sprite statusSpriteInProgress;

        [SerializeField]
        [Tooltip("Completed status sprite for statusSprite keypath")]
        private Sprite statusSpriteCompleted;

        private System.Random _random = new System.Random();

        private string[] _statusText = { "open", "pending", "cancelled", "inprogress", "completed" };
        private string[] _firstNames = { "Cora", "Elise", "April", "Libby", "Alexandra", "Shania", "Sana", "Iqra", "Mya", "Hazel",
                                    "Aiden", "Tanya", "Marvin", "Leonardo", "Alfred", "Juan", "Herman", "Umar", "Dewey" };
        private string[] _lastNames = { "Hershberger", "Cagle", "Scanlon", "Dowdy", "McMurray", "Garber", "Robins", "Taggert", "Ammons",
                                    "Fajardo", "Mercutio", "Petrunich", "Guzenski", "Zatara"};

        protected override void InitializeDataSource()
        {
            InitializeData();
            _deltaSeconds = 0;
            _nextOneSecondTarget = 0;
        }

        private void Update()
        {
            Sprite[] statusSprites = { statusSpriteOpen, statusSpritePending, statusSpriteCancelled, statusSpriteInProgress, statusSpriteCompleted };

            _deltaSeconds += Time.deltaTime;
            int tenthsOfSeconds = (int)(_deltaSeconds * 10.0);

            DataChangeSetBegin();

            if (tenthsOfSeconds > _nextFiveSecondTarget)
            {
                if (_styleYellow)
                {
                    SetValue("stylesheet", "standard");
                }
                else
                {
                    SetValue("stylesheet", "yellow");
                }
                _styleYellow = !_styleYellow;
                _nextFiveSecondTarget += 50;

                SetValue("firstname", _firstNames[_random.Next(0, _firstNames.Length)]);
                SetValue("lastname", _lastNames[_random.Next(0, _lastNames.Length)]);
            }

            if (tenthsOfSeconds > _nextOneSecondTarget)
            {
                _nextOneSecondTarget += 10;
                SetValue("score", ++_score);

                if ((++_status % _statusText.Length) == 0)
                {
                    _status = 0;
                }

                SetValue("status.name", _statusText[_status]);
                SetValue("status.icon", statusSprites[_status]);
            }

            DataChangeSetEnd();
        }

        private void InitializeData()
        {
            DataChangeSetBegin();
            SetValue("status.icon", statusSpriteOpen);
            SetValue("status.name", "open");
            SetValue("firstname", _firstNames[0]);
            SetValue("lastname", _lastNames[0]);
            SetValue("score", 123);
            SetValue("stylesheet", "standard");
            DataChangeSetEnd();
        }
    }
}
