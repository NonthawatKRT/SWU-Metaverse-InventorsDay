using System;
using System.Collections;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;
using TMPro;

namespace PurrLobby
{
    public class ConnectedText : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private TMP_Text connectedText;
        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private GameObject loadingCamera;
        // [SerializeField] private QuestManager questManager;

        private void Awake()
        {
            networkManager.onClientConnectionState += OnConnectionState;
        }

        private void OnDestroy()
        {
            networkManager.onClientConnectionState -= OnConnectionState;
        }

        private void OnConnectionState(ConnectionState obj)
        {
            if (obj == ConnectionState.Connected)
            {
                StartCoroutine(ShowConnectedAndHideLoading());
            }
            else if (obj == ConnectionState.Disconnected)
            {
                StartCoroutine(TypewriterEffect("Loading..."));
            }
        }
        
        private IEnumerator ShowConnectedAndHideLoading()
        {
            yield return StartCoroutine(TypewriterEffect("Loaded"));
            
            Invoke("disabbleLoadingScreen", 1f);
        }

        private void disabbleLoadingScreen()
        {
            if (loadingCanvas != null)
                loadingCanvas.SetActive(false);
            if (loadingCamera != null)
                loadingCamera.SetActive(false);
            // if (questManager != null)
            //     questManager.AdvanceToNextQuest();
        }

        private WaitForSeconds _wait = new(0.1f);
        
        private IEnumerator TypewriterEffect(string newText)
        {
            while (connectedText.text.Length > 0)
            {
                connectedText.text = connectedText.text.Substring(0, connectedText.text.Length - 1);
                yield return _wait;
            }

            foreach (char c in newText)
            {
                connectedText.text += c;
                yield return _wait;
            }
        }
    }
}