using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    private const string xmlApiFirstHalf = "http://api.openweathermap.org/data/2.5/weather?q=";
    [HideInInspector] public string city;
    private const string xmlApiSecondHalf = "&mode=xml&appid=b46d43390a158186789965ba6971d553";
    
    XmlDocument cityXmlData;

    public Material daySkybox;
    public Material cloudSkybox;

    int weatherCode;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCity("Orlando");

        RenderSettings.skybox = daySkybox;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CallAPI(string url, Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"network problem: {request.error}");
            }

            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"response error: {request.responseCode}");
            }

            else 
            {
                callback(request.downloadHandler.text);
            }
        }
    }

    private IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApiFirstHalf + city + xmlApiSecondHalf, callback);
    }

    public void OnXMLDataLoaded(string data)
    {
        cityXmlData = new XmlDocument();
        cityXmlData.LoadXml(data);

        XmlNode weatherNode = cityXmlData.SelectSingleNode("/current/weather");
        weatherCode = int.Parse(weatherNode.Attributes["number"].Value);

        Debug.Log(data);
        Debug.Log(weatherCode);
    }

    public void UpdateCity(string newCity)
    {
        city = newCity;

        StartCoroutine(GetWeatherXML(OnXMLDataLoaded));
    }

    public void DetermineSkybox(int weatherCode)
    {
        if (weatherCode >= 801 && weatherCode < 805)
        {

        }
    }
}
