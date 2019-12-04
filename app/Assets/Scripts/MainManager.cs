using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Transform Model;
    public RectTransform canvas;

    public Renderer ModelMarker;
    public Renderer VideoMarker;
    public GameObject VideoPlayer;
    public RawImage VideoPanel;
    public Slider Rotating;
    public Slider Scaling;

    public Vector2 ScreenSizel;

    public AudioSource Audio;

    public void Start()
    {

    }

    public void Update()
    {
        ScreenSizel = new Vector2(Screen.width, Screen.height);
        if(VideoMarker.enabled)
        {
            VideoPlayer.SetActive(true);
        }
        else
        {
            VideoPlayer.SetActive(false);
            if (ModelMarker.enabled)
            {
                ModelRotating();
                ModelScaling();
                if (!Audio.isPlaying)
                {
                    Audio.Play();
                }
            }
            else
            {
                Audio.Stop();
            }
        }
        RescaleVideo();
    }

    public void ModelRotating()
    {
        Model.rotation = Quaternion.Euler(0, Rotating.value, 0);
    }

    public void ModelScaling()
    {
        Model.localScale = Vector3.one * Scaling.value;
    }

    public void RescaleVideo()
    {
        if (Screen.height > Screen.width)
            VideoPanel.rectTransform.sizeDelta = Vector2.one * canvas.sizeDelta.x;
        else
            VideoPanel.rectTransform.sizeDelta = Vector2.one * canvas.sizeDelta.y;
        Debug.Log(VideoPanel.rectTransform.sizeDelta);
    }


}
