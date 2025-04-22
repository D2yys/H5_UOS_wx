using Passport;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts
{
    public class RealmItem : MonoBehaviour
    {
        public TextMeshProUGUI realmName;
        public List<Sprite> spriteList;
        // public TextMeshProUGUI id;
        public Image realmImage;
        private Realm _realm;
        public GameObject focus;

        public void Reset()
        {
            realmName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            realmImage = transform.Find("Item").gameObject.GetComponent<Image>();
            focus = transform.Find("Focus").gameObject;
            // id = transform.Find("ID").GetComponent<TextMeshProUGUI>();
        }
        
        public void Set(Realm realm)
        {
            realmName.text = realm.Name;
            // id.text = realm.RealmID;
            _realm = realm;
            // 随机获取一个图标
            var index = Random.Range(0, spriteList.Count - 1);
            Sprite sprite = spriteList[index];
            realmImage.sprite = sprite;
        }

        public void Select()
        {
            DemoUIController.SelectRealm.Invoke(_realm);
        }

    }
}