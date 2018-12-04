using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class NextDemo : MonoBehaviour
    {
        [SerializeField]
        private Text demoTitle;
        [SerializeField]
        private List<int> skipScenes = new List<int>();

        public void OnClickPrevDemo()
        {
            int nextSceneIndex = GetPrevScene(SceneManager.GetActiveScene().buildIndex);

            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();

            SceneManager.LoadScene(nextSceneIndex, LoadSceneMode.Single);
        }

        public void OnClickNextDemo()
        {
            int nextSceneIndex = GetNextScene(SceneManager.GetActiveScene().buildIndex);           

            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();

            SceneManager.LoadScene(nextSceneIndex, LoadSceneMode.Single);
        }

        public void OnClickReloadDemo()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();

            SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Single);
        }

        private int GetPrevScene(int currentIndex, int depth = 0)
        {
            if (depth > 10)
                throw new System.Exception("Something's gone wrong when searching for prev scene.");

            int nextSceneIndex = currentIndex - 1;

            if (nextSceneIndex < 0)
                nextSceneIndex = 0;

            if (skipScenes.Contains(nextSceneIndex))
                return GetPrevScene(nextSceneIndex, depth + 1);

            return nextSceneIndex;
        }

        private int GetNextScene(int currentIndex, int depth = 0)
        {
            if (depth > 10)
                throw new System.Exception("Something's gone wrong when searching for next scene.");

            int nextSceneIndex = currentIndex + 1;

            if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
                nextSceneIndex = 0;

            if (skipScenes.Contains(nextSceneIndex))
                return GetNextScene(nextSceneIndex, depth + 1);

            return nextSceneIndex;
        }

        private void OnEnable()
        {
            demoTitle.text = SceneManager.GetActiveScene().name;
        }
    }
}