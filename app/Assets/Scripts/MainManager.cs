using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AppJson;
using UnityEngine.Networking;

public class MainManager : MonoBehaviour
{
    public Transform Model;
    public GameObject canvas;

    public Renderer VideoMarker;
    public RawImage Viewer;

    public GameObject Downloader;
    public GameObject View;

    public Slider Rotator;
    public Slider ProgressBar;
    public bool ImageFound = false;

    public string ID = "dr9zr08t3olv";
    public string JsonUrl;

    public string ImageTemplateUrl = "http://spins0.arqspin.com/";
    public SpinInfo Info;

    public Texture[] Textures;
    public int Code = 0;

    public float progress = 0;
    public bool Debug = true;

    public void CreateJsonUrl()
    {
        JsonUrl = "http://spins0.arqspin.com/" + ID + "/spin.json";
    }

    public void Start()
    {
        CreateJsonUrl();
    }

    public void Update()
    {
        if(VideoMarker.enabled)
        {
            ImageFound = true;
            DownloadImages();
        }

        if(ImageFound)
        {
            canvas.SetActive(true);
            switch (Code)
            {
                default:
                case 0: //Default
                    Downloader.SetActive(false);
                    View.SetActive(false);
                    break;
                case 1: //Start Connection Process
                    Downloader.SetActive(false);
                    View.SetActive(false);
                    break;
                case 2: //Error
                    Downloader.SetActive(false);
                    View.SetActive(false);
                    break;
                case 3: //Start Dowload Process
                    Downloader.SetActive(true);
                    ProgressBar.value = progress;
                    View.SetActive(false);
                    break;
                case 4: //Success
                    Downloader.SetActive(false);
                    ProcessImages();
                    View.SetActive(true);
                    break;
            }
        }
        else
        {
            canvas.SetActive(false);
        }

        
    }

    public void OpenClose()
    {
        ImageFound = !ImageFound;
        canvas.SetActive(false);
        Reset();
    }

    public void Reset()
    {
        StopAllCoroutines();
        Textures = null;
        Code = 0;
        Viewer.texture = null;
    }

    public void ProcessImages()
    {
        if(Textures[(int)Rotator.value] != null)
        {
            Viewer.texture = Textures[(int)Rotator.value];
        }
    }

    public void DownloadImages()
    {
        if(Code == 0)
        {
            StartCoroutine(Images());
        }
    }

    public IEnumerator Images()
    {
        Code = 1;
        using(UnityWebRequest webRequest = UnityWebRequest.Get(JsonUrl))
        {
            yield return webRequest.SendWebRequest();
            Info = JsonUtility.FromJson<SpinInfo>(webRequest.downloadHandler.text);
        }

        int ImageCount = Info.spin.frame_count;
        Textures = new Texture[ImageCount];

        for(int i = 1; i <= ImageCount; i++)
        {
            progress = i / (float)ImageCount;
            string number = formatNumber(i);
            string imageUrl = ImageTemplateUrl + ID.ToString() + "/380.380/" + number + ".jpg";
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Code = 2;
                }
                else
                {
                    Textures[i - 1] = DownloadHandlerTexture.GetContent(webRequest);
                    Code = 3;
                }
            }
        }
        Rotator.maxValue = Textures.Length - 1;
        Code = 4;
    }

    public string formatNumber(int n)
    {
        if (n < 10)
        {
            return "00" + n.ToString();
        }
        else if(n < 100)
        {
            return "0" + n.ToString();
        }
        else
        {
            return n.ToString();
        }
    }
}
