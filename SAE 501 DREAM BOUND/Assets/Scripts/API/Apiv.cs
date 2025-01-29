using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class Apiv : MonoBehaviour
{
    public string apiUpdateZoneUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/update-zone";


    private static readonly string AesKey = "uB9xG2vLq5Z7w8NfT4mJh1YKr3PcXs6d";

    private static readonly string AesIV = "1234567890123456";

    // Encrypts a given plaintext using AES encryption.
    private string EncryptAES(string plainText)
    {
        Debug.Log("Début du chiffrement AES.");
        try
        {
            using (Aes aesAlg = Aes.Create())
            {

                byte[] key = new byte[32]; // 256 bits
                byte[] iv = new byte[16];  // 128 bits


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
                    Debug.Log("Chiffrement réussi : " + encrypted);
                    return encrypted;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Erreur de chiffrement: " + ex.Message);
            return null;
        }
    }

    // Sends a request to update the current zone using an authentication token.
    public void SendZoneUpdate(string zoneName)
    {
        string authToken = PlayerPrefs.GetString("authToken", "");
        if (string.IsNullOrEmpty(authToken))
        {
            Debug.Log("Token d'authentification introuvable !");
            return;
        }
        StartCoroutine(SendZoneRequest(zoneName, authToken));
    }

    // Coroutine that encrypts the zone name and sends a POST request to the API.
    IEnumerator SendZoneRequest(string zoneName, string authToken)
    {
        string encryptedZone = EncryptAES(zoneName);
        if (encryptedZone == null)
        {
            Debug.LogError("Échec du chiffrement de la zone");
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
            Debug.Log("Zone mise à jour avec succès : " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("URL de la requête: " + apiUpdateZoneUrl);
            Debug.Log("Données envoyées: " + jsonData);
            Debug.Log("Erreur lors de la mise à jour de la zone : " + request.error);
        }
    }

    // Data structure for sending encrypted zone information and authentication token.
    [System.Serializable]
    public class ZoneData
    {
        public string token;
        public string zoneAtteinte;
        public ZoneData(string zoneAtteinte, string token)
        {
            this.zoneAtteinte = zoneAtteinte;
            this.token = token;
        }
    }
}