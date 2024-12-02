using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Billboard : MonoBehaviour
{
    public string webImage;

    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();

        StartCoroutine(GetDownloadedImage(OnImageLoaded));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DownloadImage(Action<Texture2D> callback) { 

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(webImage);
        yield return request.SendWebRequest();
        callback(DownloadHandlerTexture.GetContent(request));

    }

    public IEnumerator GetDownloadedImage(Action<Texture2D> callback)
    {
        return DownloadImage(callback);
    }

    public void OnImageLoaded(Texture2D image)
    {
        renderer.material.SetTexture("_MainTex", image);
    }
}
