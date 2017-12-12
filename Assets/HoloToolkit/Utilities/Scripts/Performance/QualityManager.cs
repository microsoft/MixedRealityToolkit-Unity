using UnityEngine;

namespace HoloToolkit.Unity
{
    public class QualityManager : Singleton<QualityManager>
    {
        [SerializeField]
        private int currentQualityLevel = 0;
        private string qLKey = "qualityLevel";

        private string qualityLevelName = "default";
        public string QualityLevelName { get { return qualityLevelName; } }

        protected override void InitializeInternal()
        {
            currentQualityLevel = QualitySettings.GetQualityLevel();
            qualityLevelName = QualitySettings.names[currentQualityLevel];
        }

        void Update()
        {
            var newQualityLevel = currentQualityLevel;

            if (AdaptivePerformance.Instance.EnableDebugKeys)
            {
                if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus) || Input.GetKeyUp(KeyCode.Comma))
                {
                    newQualityLevel--;
                }

                if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus) || Input.GetKeyUp(KeyCode.Period))
                {
                    newQualityLevel++;
                }
            }

            SaveQualityLevel(newQualityLevel);
        }

        public void SaveQualityLevel(int newLevel)
        {
            var fixedQualityLevel = Mathf.Clamp(newLevel, 0, QualitySettings.names.Length - 1);

            if (fixedQualityLevel == currentQualityLevel)
            {
                return;
            }

            currentQualityLevel = fixedQualityLevel;

            PlayerPrefs.SetInt(qLKey, currentQualityLevel);
            PlayerPrefs.Save();

            Debug.Log("Setting quality level to " + QualitySettings.names[currentQualityLevel]);
            QualitySettings.SetQualityLevel(currentQualityLevel);
            qualityLevelName = QualitySettings.names[currentQualityLevel];
        }
    }
}
