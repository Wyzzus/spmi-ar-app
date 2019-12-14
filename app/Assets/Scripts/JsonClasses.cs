using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppJson
{
    [System.Serializable]
    public class SpinInfo
    {
        public Spin spin;
    }
    [System.Serializable]
    public class Spin
    {
        public string id;
        public string title;
        public string caption;
        public int frame_count;
        public int key_frame;
        public bool reverse;
        public Creator creator;
        public Account account;
    }

    public class Creator
    {
        public string username;
        public string logo;
        public object url;
    }

    public class Account
    {
        public Plan plan;
    }

    public class Plan
    {
        public bool unbranded;
    }

}


