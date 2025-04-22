using Passport;
using UnityEngine;
using TMPro;

namespace Unity.Passport.Sample.Scripts
{
    public class PersonaItem : MonoBehaviour
    {
        public TextMeshProUGUI personaName;
    
        public TextMeshProUGUI id;
        private Persona _persona;

        public void Reset()
        {
            personaName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            id = transform.Find("ID").GetComponent<TextMeshProUGUI>();
        }
        
        public void Set(Persona persona)
        {
            personaName.text = $"昵称：{persona.DisplayName}";
            id.text = $"PID：{persona.PersonaID}";
            _persona = persona;
        }

        public void Select()
        {
            // DemoUIController.SelectPersona.Invoke(_persona);
            // UIController.GetComponent<DemoUIController>().OnSelectedPersona();
        }
    }
}