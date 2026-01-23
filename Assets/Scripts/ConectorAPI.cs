using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ConectorAPI : MonoBehaviour
{
    // URL de tu servidor Node.js (Asegúrate que el puerto 3001 es correcto)
    private string urlBase = "http://localhost:3001/api/unity/buscar?nombre=";

    // CAMBIO IMPORTANTE: Ahora devolvemos 'RespuestaAPI'
    public void BuscarEscuela(string nombre, Action<RespuestaAPI> alTerminar)
    {
        StartCoroutine(PeticionWeb(nombre, alTerminar));
    }

    IEnumerator PeticionWeb(string nombre, Action<RespuestaAPI> callback)
    {
        string urlFinal = urlBase + UnityWebRequest.EscapeURL(nombre);
        // Debug.Log("📞 Llamando a API: " + urlFinal);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(urlFinal))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Error API: " + webRequest.error);
            }
            else
            {
                string jsonRecibido = webRequest.downloadHandler.text;
                // Debug.Log("✅ JSON Recibido: " + jsonRecibido);

                try {
                    // Convertimos el JSON a nuestra nueva estructura maestra
                    RespuestaAPI respuesta = JsonUtility.FromJson<RespuestaAPI>(jsonRecibido);
                    callback(respuesta);
                }
                catch (Exception e) {
                    Debug.LogError("❌ Error leyendo JSON: " + e.Message);
                }
            }
        }
    }
}