using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AppJson;
using UnityEngine.Networking;
using UnityEngine.Video;
using Vuforia;

public class MainManager : MonoBehaviour
{
    public Transform Model;
    public GameObject canvas;

    public Animator anim;
    public AudioSource Sound;
    public VideoPlayer VideoPlayer;

    public Renderer[] Markers;

    public Renderer VideoMarker;
    public RawImage Viewer;
    public UnityEngine.UI.Image ViewerSprite;

    public ScrollRect Zoomer;

    public RectTransform viewPortScreen;

    public GameObject Downloader;
    public GameObject View;
    public GameObject VideoView;

    public GameObject FindMessage;

    public Slider Rotator;
    public Slider ProgressBar;
    public bool ImageFound = false;

    public string ID = "dr9zr08t3olv";
    public string JsonUrl;

    public string ImageTemplateUrl = "https://spins0.arqspin.com/";
    public SpinInfo Info;

    public List<Texture> Textures = new List<Texture>();
    public List<Sprite> Sprites = new List<Sprite>();
    public int Code = 0;

    public float progress = 0;
    public bool DebugMode = true;
    public Renderer activeImage = null;

    public Renderer currentImage;

    public bool Zoom;
    public Text ZoomText;

    public GameObject Confirmation;

    public Text MineralName;

    public string CreateJsonUrl(string id)
    {
        return "http://spins0.arqspin.com/" + id + "/spin.json";
    }

    public void Start()
    {

    }

    public void ShowFindMessage()
    {
        if (activeImage || VideoMarker.enabled)
            FindMessage.SetActive(false);
        else
            FindMessage.SetActive(true);
    }

    public void GetMineral()
    {
        Zoom = true;
        Zooming();
        //ImageFound = true;
        if (Textures.Count == 0)
        {
            DownloadImages(activeImage.GetComponentInParent<Mineral>());

        }
        View.SetActive(true);
        StartCoroutine(StartSound());
        Downloader.SetActive(true);
    }

    public IEnumerator StartSound()
    {
        while(progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log(progress);
        }
        Sound.clip = currentImage.GetComponentInParent<Mineral>().sound;
        Sound.Play();
    }


    public void Update()
    {
        activeImage = GetFoundImage();

        ShowFindMessage();

        if(!activeImage)
        {
            Code = 0;
        }

        if (activeImage && activeImage.enabled)
        {
            ImageFound = true;
            if(Code < 3 && !View.activeSelf)
                DownloadImages(activeImage.GetComponentInParent<Mineral>());

            if(activeImage != currentImage)
            {
                Code = 0;
            }
            
            MineralName.text = activeImage.GetComponentInParent<Mineral>().Name;
            anim.SetInteger("Watch", 1);
        }
        else if(VideoMarker.enabled)
        {
            anim.SetInteger("Watch", 2);
        }
        else
        {
            anim.SetInteger("Watch", 0);
            if(Code == 4)
                StopAllCoroutines();
            //if(!View.activeSelf)
            //    Textures.Clear();
        }

        if (ImageFound)
        {
            canvas.SetActive(true);
            if(progress > 0.9f)
                Downloader.SetActive(false);
            else
                Downloader.SetActive(true);

            switch (Code)
            {
                default:
                case 0: //Default
                    break;
                case 1: //Start Connection Process
                    break;
                case 2: //Error
                    break;
                case 3: //Start Dowload Process
                    ProgressBar.value = progress;
                    break;
                case 4: //Success
                    break;
            }
        }
        else
        {
            //canvas.SetActive(false);
        }
        if(Textures.Count > 0)
        {
            ProcessImages();
        }

        if (Zoom)
        {
            float w = (Screen.height > Screen.width) ? viewPortScreen.rect.width * 2 : viewPortScreen.rect.height;
            Viewer.rectTransform.sizeDelta = new Vector2(w, w);
            //Viewer.rectTransform.anchoredPosition = new Vector2(0, 0);
            ZoomText.text = "Отдалить";
            Zoomer.enabled = true;
        }
        else
        {
            float w = (Screen.height > Screen.width) ? viewPortScreen.rect.width : viewPortScreen.rect.height / 2;
            Viewer.rectTransform.sizeDelta = new Vector2(w, w);
            //Viewer.rectTransform.anchoredPosition = new Vector2(0, 0);
            ZoomText.text = "Приблизить";
            Zoomer.enabled = false;
        }

    }

    public Renderer GetFoundImage()
    {
        foreach(Renderer r in Markers)
        {
            if (r.enabled)
                return r;
        }
        return null;
    }

    public void OpenVideo()
    {
        VideoPlayer.Play();
        VideoView.SetActive(true);
    }

    public void CloseViewer()
    {
        View.SetActive(false);
        Sound.Pause();
        Sound.clip = null;
        StopAllCoroutines();
        //Textures.Clear();
        //if (activeImage && activeImage.enabled)
        //    DownloadImages(activeImage.GetComponentInParent<Mineral>());
    }

    public void CloseVideo()
    {
        VideoPlayer.Stop();
        VideoView.SetActive(false);
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
        //Textures.Clear();
        Viewer.texture = null;
    }

    public void ProcessImages()
    {
        if((int)Rotator.value < Textures.Count && Textures[(int)Rotator.value] != null)
        {
            Viewer.texture = Textures[(int)Rotator.value];
        }
    }

    public void DownloadImages(Mineral mineral)
    {
        Textures.Clear();
        ProgressBar.value = 1;
        if (Code == 0)
        {
            StartCoroutine(Images(mineral));
        }
    }

    public void ConfirmExit()
    {
        Confirmation.SetActive(true);
    }

    public void NoExit()
    {
        Confirmation.SetActive(false);
    }

    public void YesExit()
    {
        Application.Quit();
    }

    public IEnumerator Images(Mineral mineral)
    {
        currentImage = activeImage;
        Code = 1;
        JsonUrl = CreateJsonUrl(mineral.ID);
        using(UnityWebRequest webRequest = UnityWebRequest.Get(JsonUrl))
        {
            yield return webRequest.SendWebRequest();
            Info = JsonUtility.FromJson<SpinInfo>(webRequest.downloadHandler.text);
        }

        int scale = 2;
        int ImageCount = Info.spin.frame_count;

        for(int i = 1; i <= ImageCount; i+=scale)
        {
            progress = i / (float)ImageCount;
            string number = formatNumber(i);
            string imageUrl = ImageTemplateUrl + mineral.ID + "/" + mineral.GetPartOfUrl() + "/" + number + ".jpg";
            if(DebugMode)
                Debug.Log(imageUrl);
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Code = 2;
                }
                else
                {
                    var tex = DownloadHandlerTexture.GetContent(webRequest);
                    Textures.Add(tex);
                    Code = 3;
                }
            }
        }
        Rotator.maxValue = Textures.Count - 1;
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

    public void Zooming()
    {
        Zoom = !Zoom;

        if (Zoom)
        {
            Viewer.rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            Viewer.rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        Debug.Log(Screen.height + " | " + Screen.width);
    }
}
