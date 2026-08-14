using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class GoogleSheetsService : MonoBehaviour
    {
        private const string WebAppURL = "https://script.google.com/macros/s/AKfycbyrKqkmzZeJTnVnXOgzg9rrHcBRm286hUHOj3AcsKV7pswcvq0PxUcpt78PCdpE2udX_g/exec";

        public void SendPlayerData(string playerName, string email, string phohe, Action<bool> onComplete = null)
        {
            StartCoroutine(SendDataCoroutine(playerName, email, phohe, "", "", onComplete));
        }

        public void SendPlayerDataWithResult(string playerName, string email, string phone, string gameResult, string prizeWon, Action<bool> onComplete)
        {
            StartCoroutine(SendDataCoroutine(playerName, email, phone, gameResult, prizeWon, onComplete));
        }

        private IEnumerator SendDataCoroutine(string playerName, string email, string phone, string gameResult, string prizeWon, Action<bool> onComplete)
        {
            PlayerData data = new PlayerData
            {
                name = playerName,
                email = email,
                phone = phone,
                gameResult = gameResult,
                prizeWon = prizeWon,
            };

            var jsonData = JsonUtility.ToJson(data);
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            using var request = new UnityWebRequest(WebAppURL, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result ==  UnityWebRequest.Result.Success)
                onComplete?.Invoke(true);
            else
            {
                onComplete?.Invoke(false);
            }
        }

        [Serializable]
        private class PlayerData
        {
            public string name;
            public string email;
            public string phone;
            public string gameResult;
            public string prizeWon;
        }
    }
}
