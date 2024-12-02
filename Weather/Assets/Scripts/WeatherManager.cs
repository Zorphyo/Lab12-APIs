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

    public Material thunderstormSkybox;
    public Material drizzleSkybox;
    public Material rainSkybox;
    public Material snowSkybox;
    public Material clearSkyboxDay;
    public Material clearSkyboxNight;
    public Material cloudySkyboxDay;
    public Material cloudySkyboxNight;
    public Material defaultSkybox;

    int weatherCode;

    int timezone;
    int orlandoTimeZone = -18000;
    int timezoneDifference;

    DateTime currentOrlandoTime = DateTime.Now;
    DateTime selectedCityTime;

    public GameObject directionalLight;
    Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = directionalLight.GetComponent<Light>();
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

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApiFirstHalf + city + xmlApiSecondHalf, callback);
    }

    public void OnXMLDataLoaded(string data)
    {
        cityXmlData = new XmlDocument();
        cityXmlData.LoadXml(data);

        XmlNode weatherNode = cityXmlData.SelectSingleNode("/current/weather");
        weatherCode = int.Parse(weatherNode.Attributes["number"].Value);
        
        XmlNode cityNode = cityXmlData.SelectSingleNode("/current/city");
        XmlNode timezoneNode = cityNode.SelectSingleNode("timezone");
        timezone = int.Parse(timezoneNode.InnerText);

        Debug.Log(data);
        Debug.Log(weatherCode);

        DetermineTime(timezone);

        DetermineSkybox(weatherCode);
    }

    public void UpdateCity(string newCity)
    {
        city = newCity;

        StartCoroutine(GetWeatherXML(OnXMLDataLoaded));
    }

    public void DetermineTime(int timezone)
    {
        timezoneDifference = timezone - orlandoTimeZone;

        selectedCityTime = currentOrlandoTime.AddSeconds(timezoneDifference);

        Debug.Log(orlandoTimeZone);
        Debug.Log(timezone);
        Debug.Log(timezoneDifference);

        Debug.Log(currentOrlandoTime);
        Debug.Log(selectedCityTime);

        Debug.Log(selectedCityTime.Hour);

        ChangeLighting(selectedCityTime.Hour);
    }

    public void ChangeLighting(int hour)
    {
        if (hour >= 6 && hour <= 12)
        {
            light.intensity = hour / 12.0f;
        }

        else if (hour >= 13 && hour <= 18)
        {
            light.intensity = 1 - ((hour - 12) / 12.0f);
        }

        else 
        {
            light.intensity = 0;
        }
    }

    public void DetermineSkybox(int weatherCode)
    {
        if (weatherCode == 200 || weatherCode == 201 || weatherCode == 202 || weatherCode == 210 || weatherCode == 211 || weatherCode == 212 || weatherCode == 221 || weatherCode == 230 || weatherCode == 231 || weatherCode == 232)
        {
            RenderSettings.skybox = thunderstormSkybox;
        }

        else if (weatherCode == 300 || weatherCode == 301 || weatherCode == 302 || weatherCode == 310 || weatherCode == 311 || weatherCode == 312 || weatherCode == 313 || weatherCode == 314 || weatherCode == 321)
        {
            RenderSettings.skybox = drizzleSkybox;
        }

        else if (weatherCode == 500 || weatherCode == 501 || weatherCode == 502 || weatherCode == 503 || weatherCode == 504 || weatherCode == 511 || weatherCode == 520 || weatherCode == 521 || weatherCode == 522 || weatherCode == 531)
        {
            RenderSettings.skybox = rainSkybox;
        }

        else if(weatherCode == 600 || weatherCode == 601 || weatherCode == 602 || weatherCode == 611 || weatherCode == 612 || weatherCode == 613 || weatherCode == 615 || weatherCode == 616 || weatherCode == 620 || weatherCode == 621 || weatherCode == 622)
        {
            RenderSettings.skybox = snowSkybox;
        }

        else if (weatherCode == 800)
        {
            if (selectedCityTime.Hour >= 7 && selectedCityTime.Hour <= 18)
            {
                RenderSettings.skybox = clearSkyboxDay;
            }

            else 
            {
                RenderSettings.skybox = clearSkyboxNight;
            }    
        }

        else if (weatherCode == 801 || weatherCode == 802 || weatherCode == 803 || weatherCode == 804)
        {
            if (selectedCityTime.Hour >= 7 && selectedCityTime.Hour <= 18)
            {
                RenderSettings.skybox = cloudySkyboxDay;
            }

            else 
            {
                RenderSettings.skybox = cloudySkyboxNight;
            }  
        }

        else
        {
            RenderSettings.skybox = defaultSkybox;
        }
    }
}