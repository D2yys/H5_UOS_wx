using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Unity.Passport.Sample.Scripts
{
    public class TabController : MonoBehaviour
    {
        public GameObject tabFocus;
        public TextMeshProUGUI text;
        private readonly Color _inactiveColor = new Color(0.29f, 0.6745f, 0.9686f, 1);
        private readonly Color _activeColor = new Color(1, 1, 1, 1);
        
        public void SetStatus(bool active)
        {
            text.color = active ?  _activeColor: _inactiveColor;
            tabFocus.SetActive(active);
        }

        public void SetText(string str)
        {
            text.text = str;
        }
        
    }
}
