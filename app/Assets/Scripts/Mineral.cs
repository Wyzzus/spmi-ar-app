using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mineral : MonoBehaviour
{
    public string Name;
    public string ID;
    public string Width;
    public string Height;
    public AudioClip sound;

    public string GetPartOfUrl()
    {
        return Width + "." + Height;
    }
}
