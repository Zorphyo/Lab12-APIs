using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedImages : MonoBehaviour
{
    public List<string> imageURLs = new List<string>();
    public List<Texture2D> images = new List<Texture2D>();

    public static LoadedImages instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
