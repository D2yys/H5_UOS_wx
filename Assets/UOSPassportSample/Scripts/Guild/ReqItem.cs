using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Unity.Passport.Sample.Scripts
{
    public class ReqItem : MonoBehaviour
    {
        public TextMeshProUGUI personaName;
        private Guild.GuildRequest _req;
        public TextMeshProUGUI requestMessage;

        public void Init(Guild.GuildRequest req)
        {
            personaName.text = req.PersonaId;
            _req = req;
            requestMessage.text = req.Message;
        }

        public void HandleApprove()
        {
           GuildUIController.OnApproveGuildRequest.Invoke(_req);
        }

        public void HandleReject()
        {
            GuildUIController.OnRejectGuildRequest.Invoke(_req);
        }
    }
}