using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts
{
    public class Tutorial : MonoBehaviour
    {
        public List<GameObject> introductionList;
        public GameObject skipButton;
        private void Start()
        {
            for (int i = 0; i < introductionList.Count; i += 1)
            {
                var item = introductionList[i];
                var ii = i;
                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    item.SetActive(false);
                    if (ii < introductionList.Count - 1)
                    {
                        introductionList[ii + 1].SetActive(true);
                    }
                    else
                    {
                        skipButton.SetActive(false);
                    }
                });
            }
            skipButton.GetComponent<Button>().onClick.AddListener(SkipTutorial);
        }

        public void SkipTutorial()
        {
            for (int i = 0; i < introductionList.Count; i += 1)
            {
                introductionList[i].SetActive(false);
            }
            skipButton.SetActive(false);
        }

    }
}