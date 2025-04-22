using HutongGames.PlayMaker;
using Passport;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TTSDK;
using Unity.Passport.Runtime;
using Unity.Passport.Runtime.UI;
using Unity.Passport.Sample.Scripts.Leaderboard;
using Unity.UOS.Auth;
using Unity.UOS.CloudSave;
using Unity.UOS.CloudSave.Exception;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Login : MonoBehaviour
{


    [Header("���а����")] public GameObject leaderboardPanel;
    private LeaderboardUIController _leaderboardUIController;
    public RawImage userAvatar;
    //[Tooltip("�˺���")] public TextMeshProUGUI userId;
    public Texture defaultAvatar;
    public Text currentRealmText;
    public Text personaName;
    public Text personaID;

    public string Userid;
    private Persona _currentPersona; //����ɾ��
    public string LoginID;//��¼ID
    public string userame;  //�����
    public string Topname; //������ 
    private string _currentRealm; // ��ǰѡ�е���

    bool isLoaginDy;

    public PlayMakerFSM Fsm;


    //��һ�� ������¼Ȩ������ ͬ��:������ֺ�ͷ��  ��ͬ��ʹ��Ĭ�����ֺ�ͷ��   
    //����С��Ϸ����ҪUOS��¼
    public void LoginDy()
    {

        TT.Login(OnLoginSuccessCallback, FailedCallback, true);

        void FailedCallback(string errMsg)
        {
            Debug.Log("DY --> ��¼ʧ��: " + errMsg);
        }
    }

    /// <summary>��¼�ɹ�</summary>
    /// <param name="code">��ʱ��¼ƾ֤, ��Ч�� 3 ���ӡ�����ͨ���ڷ������˵��� ��¼ƾ֤У��ӿ� ��ȡ openid �� session_key ����Ϣ��</param>
    /// <param name="anonymousCode">���ڱ�ʶ��ǰ�豸, ���۵�¼��񶼻᷵��, ��Ч�� 3 ����</param>
    /// <param name="isLogin">�ж��ڵ�ǰ APP(ͷ����������)�Ƿ��ڵ�¼״̬</param>
    void OnLoginSuccessCallback(string code, string anonymousCode, bool isLogin)
    {
        Debug.Log($"��¼�ɹ���code: {code}, anonymousCode:{anonymousCode}, isLogin{isLogin}");


        TT.GetUserInfo(OnGetScUserInfoSuccessCallback, OnGetScUserInfoFailedCallback);

        void OnGetScUserInfoSuccessCallback(ref TTUserInfo scUserInfo)
        {
            Debug.Log($"��¼�ɹ���ȡ�û���Ϣ�ɹ���nickName: {scUserInfo.nickName}");
            Debug.Log($"��¼�ɹ���ȡ�û���Ϣ�ɹ���avatarUrl: {scUserInfo.avatarUrl}");

            // DownSprite(scUserInfo.avatarUrl);
            StartCoroutine(LoadAvatar(scUserInfo.avatarUrl));
            personaName.text = scUserInfo.nickName;
            isLoaginDy = true;


            // Uoslogin(scUserInfo.nickName);
        }

        void OnGetScUserInfoFailedCallback(string errMsg)
        {
            Debug.Log($"��¼�ɹ���ȡ�û���Ϣʧ�ܣ�errMsg: {errMsg}");

            //   Uoslogin("���");
        }


    }


    //�ڶ��� UOS��¼  ʹ�õ���������ɵ�ID �Ͷ�����¼ʱ�������
    public async void Uoslogin(string name)
    {
        Debug.Log("UOS��ʼ����¼");
        _leaderboardUIController = leaderboardPanel.GetComponent<LeaderboardUIController>();
        await PassportFeatureSDK.Initialize();


        string userId = LoginID; // ��Ҫ��¼���ⲿϵͳ���û�Id      
        //string personaId = "1"; // ��ѡ, ��Ҫ��¼�� �ⲿϵͳ�Ľ�ɫID
        //string personaDisplayName = "���1"; // ��ѡ, ��Ҫ��¼�Ľ�ɫ���ǳơ�
        await AuthTokenManager.ExternalLogin(userId, null, name);

        _callback(PassportEvent.Completed);

    }

    private async void _callback(PassportEvent passportEvent)
    {
        switch (passportEvent)
        {

            case PassportEvent.Completed:
                Debug.Log("UOS��¼�ɹ�");
                //await GetPlayerInfo();   //��ȡOUS�������
                await CloudSave();       //��ʼ�����ݿ�
                GetRealm();             //��¼������

                break;
        }
    }


    //�޸�UOS����
    private async void Gaiming()
    {
        try
        {
            await PassportSDK.Identity.UpdatePersona(personaName.text);
            await PassportSDK.Identity.UpdateUserProfileInfo(personaName.text);
        }

        catch (PassportException e)
        {
            Debug.Log(e.Code);

            string log = e.ErrorMessage;

            bool ybool = log.Contains("��Ч������");
            if (ybool == true)
            {
                Debug.Log("������Ч!!!!����������");

            }
        }
    }

    //���OUS�����Ϣ
    private async Task GetPlayerInfo()
    {
        var userInfo = await PassportSDK.Identity.GetUserProfileInfo();
        //userId.text = $"{userInfo.Name}";

        if (userInfo.AvatarUrl != "")
        {
            StartCoroutine(LoadAvatar(userInfo.AvatarUrl));
            
        }
        else
        {
            userAvatar.texture = defaultAvatar;
        }
    }

    /// <summary>
    /// ��ȡ��Ӧ���µ���
    /// </summary>
    private async void GetRealm()
    {
        try
        {
            var list = await PassportSDK.Identity.GetRealms();


            if (!list.Any())//������������ bug ����
            {
                UIMessage.Show("�뵽��վ�Ͻ���������(Passport -> �����)", MessageType.Error);
                return;
            }

            _currentRealm = list[0].RealmID;
            currentRealmText.text = list[0].Name;
            StartGame();

        }
        catch (PassportException e)
        {
            Debug.Log(e.Code);
        }
    }


    /// <summary>
    /// ��ʼ��Ϸ��ѡ�н�ɫ�����û�н�ɫ��չʾ�½���ɫ���
    /// </summary>
    public async void StartGame()
    {

        var persona = await PassportSDK.Identity.GetPersonaByRealm(_currentRealm);

        if (persona != null)
        {

            await OnSelectPersona(persona);  //ѡ���ɫ

            await _leaderboardUIController.Init();    //��ʼ���а�
        }
        else
        {

            // չʾ�����½�ɫ���            
        }
    }

    /// <summary>
    /// ��¼�����һ��ѡ��ĳ����ɫ 
    /// </summary>
    /// <param name="persona"></param>
    private async Task OnSelectPersona(Persona persona)
    {       
        try
        {
            _currentPersona = persona;
            await PassportSDK.Identity.SelectPersona(persona.PersonaID);
            personaName.text = $"{persona.DisplayName}";
            personaID.text = $"��ɫ ID��{persona.PersonaID}";
            Userid = persona.PersonaID;
            FsmVariables.GlobalVariables.GetFsmString("Data_UID").Value = Userid;  //�û�id

        }
        catch (PassportException e)
        {
            Debug.Log(e.Code);
        }
    }

    //�ϴ��Լ��ķ���
    public void TopUpData(int Score)
    {
        TopUpDa(Score);
    }

    private async void TopUpDa(int Score)
    {


        Leaderboard.UpdateScoreResponse updatedScore = await PassportFeatureSDK.Leaderboard.UpdateScore(Topname, Score);
        // ��ȡ����óɼ����а񡹵�����
        Leaderboard.ListLeaderboardScoresResponse resp = await PassportFeatureSDK.Leaderboard.ListLeaderboardScores(Topname);
        foreach (Leaderboard.LeaderboardMemberScore score in resp.Scores)
        {
            // ��ӡ��ҵ����ơ��ɼ��Լ������ķּ�
            Debug.Log(score.DisplayName);
            Debug.Log(score.Score);
            // �˴�tier��Ӧ���а�ķּ����ã��磺��ҷ���Ϊ55��tier������������õķּ���ӦΪ����ͭ��
            Debug.Log(score.Tier);
        }

    }


    //��ʼ�����ݿ�
    private async Task CloudSave()
    {
        try
        {

            await CloudSaveSDK.InitializeAsync();
            Fsm.SendEvent("UosInitEnd");
        }
        catch (CloudSaveClientException e)
        {
            Debug.LogErrorFormat($"failed to initialize sdk, clientEx: {e}");
            throw;
        }
        catch (CloudSaveServerException e)
        {
            Debug.LogErrorFormat($"failed to initialize sdk, serverEx: {e}");
            throw;
        }

    }


    IEnumerator LoadAvatar(string url)
    {
        // ����ͷ��ͼƬ
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);

        yield return uwr.SendWebRequest(); // �ȴ��������

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("����UrlͼƬʧ�ܵ�ַ��" + url);

        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

            userAvatar.texture = texture;
         
        }

    }

    /// <summary>
    /// 0���������͡�1��ö������;
    ///     �������ͣ�0��������������Ϸ��ͨ�ط�����103�֡�105�֣���
    ///     ö�����ͣ�1�������ڶ�λ��Ϣ����ͭ����������
    /// </summary>
    private int rankDataType = 0;
    // ���а������ʶ--(Nullable)Ĭ��ֵΪdefault, ���ԣ�test
    private string zoneId = "default";

    /// <summary>
    /// �������а����
    /// </summary>
    public void SetImRankList(int rankValue)
    {

        if (!isLoaginDy)
        {
            LoginDy();
            Debug.Log("��Ҫ�ȵ�¼��ִ�и�������...");
            return;
        }
        Debug.Log("�������а����ݣ�" + rankValue);
        var paramJson = new TTSDK.UNBridgeLib.LitJson.JsonData
        {
            ["dataType"] = rankDataType,
            ["value"] = rankValue,
            //["priority"] = int.Parse(priority),
            ["zoneId"] = zoneId
        };
        Debug.Log($"SetImRankData param:{paramJson.ToJson()}");
        TT.SetImRankData(paramJson, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Debug.Log("�������а����ݳɹ�");
                GetImRankList();  //��ʾ���а�
            }
            else
            {
                Debug.Log("�������а����ݳɹ�");
            }
        });
    }

    /// <summary>
    /// ��ȡ���а��б����� API �� ���ݲ����Զ�������Ϸ�������а�
    /// </summary>
    public void GetImRankList()
    {
        // <param name="rankType">���������������ڣ�dayΪ����д�������������weekΪ��Ȼ�ܣ�monthΪ��Ȼ�£�allΪ����--(Require)</param>
        // <param name="dataType">�����������͵�������ö�����͵������޷�ͬʱ���������Ҫѡ��������Щ���͵�����--(Require)</param>
        // <param name="relationType">ѡ���չʾ��Χ��default: ���Ѽ��ܰ�չʾ��all�����ܰ�--(Nullable)</param>
        // <param name="suffix">���ݺ�׺�����չʾ��ʽΪ value + suffix����suffix�����֡�����չʾ 103�֡�104��--(Nullable)</param>
        // <param name="rankTitle">���а������İ�--(Nullable)</param>
        // <param name="zoneId">���а������ʶ--(Nullable)</param>
        // <param name="paramJson">���ϲ���ʹ��json��ʽ���룬����"{"rankType":"week","dataType":0,"relationType":"all","suffix":"��","rankTitle":"","zoneId":"default"}"</param>
        // <param name="action">�ص�����</param>

        var paramJson = new TTSDK.UNBridgeLib.LitJson.JsonData
        {
            ["rankType"] = RankType.day.ToString(),
            ["dataType"] = rankDataType,
            ["relationType"] = "default",
            ["suffix"] = "��",
            ["rankTitle"] = "�۷����а�",
            ["zoneId"] = zoneId,
        }; 
        Debug.Log($"�۷����а� GetImRankList param:{paramJson.ToJson()}");
        TT.GetImRankList(paramJson, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
            }
            else
            {
            }
        });

    }
    public enum RankType
    {
        // ��
        day,
        // ��Ȼ��
        week,
        // ��Ȼ��
        month,
        // ����
        all
    }

}
