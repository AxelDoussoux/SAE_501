using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class Apiv : MonoBehaviour
{
    public string apiUpdateZoneUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/update-zone";

    // 32-character (256-bit) AES key
    private static readonly string AesKey = "uB9xG2vLq5Z7w8NfT4mJh1YKr3PcXs6d";
    // 16-character (128-bit) IV
    private static readonly string AesIV = "1234567890123456";

    // Encrypts a given string using AES encryption
    private string EncryptAES(string plainText)
    {
        Debug.Log("Starting AES encryption.");
        try
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = new byte[32];
                byte[] iv = new byte[16];

                byte[] keyBytes = Encoding.UTF8.GetBytes(AesKey);
                byte[] ivBytes = Encoding.UTF8.GetBytes(AesIV);

                System.Array.Copy(keyBytes, key, System.Math.Min(keyBytes.Length, key.Length));
                System.Array.Copy(ivBytes, iv, System.Math.Min(ivBytes.Length, iv.Length));

                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    string encrypted = System.Convert.ToBase64String(msEncrypt.ToArray());
                    Debug.Log("Encryption successful: " + encrypted);
                    return encrypted;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Encryption error: " + ex.Message);
            return null;
        }
    }

    // Sends a request to update the zone
    public void SendZoneUpdate(string zoneName)
    {
        string authToken = PlayerPrefs.GetString("authToken", "");
        if (string.IsNullOrEmpty(authToken))
        {
            Debug.Log("Authentication token not found!");
            return;
        }
        StartCoroutine(SendZoneRequest(zoneName, authToken));
    }

    // Coroutine to send the zone update request
    IEnumerator SendZoneRequest(string zoneName, string authToken)
    {
        string encryptedZone = EncryptAES(zoneName);
        if (encryptedZone == null)
        {
            Debug.LogError("Zone encryption failed");
            yield break;
        }

        ZoneData zoneData = new ZoneData(encryptedZone, authToken);
        string jsonData = JsonUtility.ToJson(zoneData);

        UnityWebRequest request = new UnityWebRequest(apiUpdateZoneUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Zone successfully updated: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Request URL: " + apiUpdateZoneUrl);
            Debug.Log("Sent data: " + jsonData);
            Debug.Log("Error updating zone: " + request.error);
        }
    }

    [System.Serializable]
    public class ZoneData
    {
        public string token;
        public string zoneReached;
        public ZoneData(string zoneReached, string token)
        {
            this.zoneReached = zoneReached;
            this.token = token;
        }
    }
}