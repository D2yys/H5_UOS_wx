using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using Passport;
using Unity.Passport.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Passport.Runtime.Model;
using static System.Text.Encoding;
using Convert = System.Convert;
using Enum = System.Enum;


namespace Unity.Passport.Sample.Scripts
{
    
    public class NetworkUtils
{
    protected internal static async Task<UnityWebRequest> RequestWithBody(string url, string data,
        string appID = "", string appSecret = "", string authorizationToken = "", string requestMethod = "POST")
    {
        UnityWebRequest uwr = new UnityWebRequest(url, requestMethod);

        if (!string.IsNullOrEmpty(authorizationToken))
        {
            uwr.SetRequestHeader("Authorization", authorizationToken);
        }

        uwr.disposeDownloadHandlerOnDispose = true;
        uwr.disposeUploadHandlerOnDispose = true;
        uwr.uploadHandler = new UploadHandlerRaw(UTF8.GetBytes(data));
        uwr.SetRequestHeader("content-type", "application/json;charset=utf-8");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.timeout = 3;

        if (!string.IsNullOrEmpty(appID))
        {
            SetNonce(appID, appSecret, ref uwr);
        }


        var sendOperation = uwr.SendWebRequest();
        while (!sendOperation.isDone)
        {
            await Task.Yield();
        }

        return uwr;
    }
    protected internal static string HexString(byte[] data)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            builder.Append(data[i].ToString("X2").ToLower());
        }

        return builder.ToString();
    }

    protected internal static long GetUnixTimeStampSeconds(DateTime dt)
    {
        DateTime dateStart = new DateTime(1970, 1, 1, 0, 0, 0);
        return Convert.ToInt64((dt - dateStart).TotalSeconds);
    }
    protected internal static byte[] Sha256(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        using (SHA256 mySHA256 = SHA256.Create())
        {
            byte[] hash = mySHA256.ComputeHash(bytes);
            return hash;
        }
    }

    private static void SetNonce(string appID, string appSecret, ref UnityWebRequest uwr)
    {
        string nonce = Guid.NewGuid().ToString();
        long timestamp = GetUnixTimeStampSeconds(DateTime.UtcNow);
        string tokenContent = $"{appID}:{appSecret}:{timestamp}:{nonce}";
        string token = HexString(Sha256(tokenContent));
        uwr.SetRequestHeader("X-Timestamp", $"{timestamp}");
        uwr.SetRequestHeader("X-Nonce", nonce);
        uwr.SetRequestHeader("X-Nonce-Token", token);
        uwr.SetRequestHeader("X-AppID", appID);
    }

    protected internal static async Task<UnityWebRequest> GetRequest(string url, string appID = "",
        string appSecret = "",
        string authorizationToken = "")
    {
        UnityWebRequest uwr = new UnityWebRequest(url, "GET");
        if (!string.IsNullOrEmpty(authorizationToken))
        {
            uwr.SetRequestHeader("Authorization", authorizationToken);
        }

        uwr.disposeDownloadHandlerOnDispose = true;
        uwr.disposeUploadHandlerOnDispose = true;
        uwr.SetRequestHeader("content-type", "application/json;charset=utf-8");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.timeout = 3;

        if (!string.IsNullOrEmpty(appID))
        {
            SetNonce(appID, appSecret, ref uwr);
        }


        var sendOperation = uwr.SendWebRequest();
        while (!sendOperation.isDone)
        {
            await Task.Yield();
        }

        return uwr;
    }

    protected internal static async Task<UnityWebRequest> DeleteRequest(string url, string appID = "",
        string appSecret = "",
        string authorizationToken = "")
    {
        UnityWebRequest uwr = new UnityWebRequest(url, "DELETE");
        if (!string.IsNullOrEmpty(authorizationToken))
        {
            uwr.SetRequestHeader("Authorization", authorizationToken);
        }

        uwr.disposeDownloadHandlerOnDispose = true;
        uwr.disposeUploadHandlerOnDispose = true;
        uwr.SetRequestHeader("content-type", "application/json;charset=utf-8");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.timeout = 3;

        if (!string.IsNullOrEmpty(appID))
        {
            SetNonce(appID, appSecret, ref uwr);
        }


        var sendOperation = uwr.SendWebRequest();
        while (!sendOperation.isDone)
        {
            await Task.Yield();
        }

        return uwr;
    }
}
    
    public class MockNetworkUtils
    {
        private const string fallbackMsg = "Unknown Error";
        public static Action<string, string> OnAccessTokenRefreshed;

        private static async Task<T> PassportRequestWithDataInternal<T>(string url, string data,
            string appID, string appSecret, string authorizationToken, bool canRetry, string requestMethod = "POST")
            where T : IMessage<T>, new()
        {
            using (UnityWebRequest uwr =
                   await NetworkUtils.RequestWithBody(url, data, appID, appSecret, authorizationToken,
                       requestMethod: requestMethod))
            {
                try
                {
                    if (uwr.error == null)
                    {
                        if (uwr.responseCode == (long)HttpStatusCode.NoContent)
                        {
                            return new T();
                        }

                        var settings = JsonParser.Settings.Default.WithIgnoreUnknownFields(true);
                        return new JsonParser(settings).Parse<T>(uwr.downloadHandler.text);
                    }

                    var errorMessage = ParseErrorMessage(uwr);

                    throw new PassportException(errorMessage.Code, errorMessage.Message);
                }
                catch (Exception e)
                {
                    switch (e)
                    {
                        case PassportException:
                            throw;
                    }

                    Debug.Log(e);
                    throw new PassportException(ErrorCode.Unknown, e.Message);
                }
            }
        }


        protected internal static async Task<T> PassportPostRequest<T>(string url, string data,
            string appID, string appSecret, string authorizationToken = "") where T : IMessage<T>, new()
        {
            return await PassportRequestWithDataInternal<T>(url, data, appID, appSecret, authorizationToken, true);
        }

        protected internal static async Task<T> PassportPutRequest<T>(string url, string data,
            string appID, string appSecret, string authorizationToken = "") where T : IMessage<T>, new()
        {
            return await PassportRequestWithDataInternal<T>(url, data, appID, appSecret, authorizationToken, true,
                requestMethod: "PUT");
        }

        protected internal static async Task<T> PassportGetRequestInternal<T>(string url,
            string appID, string appSecret, string authorizationToken, bool canRetry,
            Action<PassportException> exceptionHandler = null) where T : IMessage<T>, new()
        {
            using (UnityWebRequest uwr =
                   await NetworkUtils.GetRequest(url, appID, appSecret, authorizationToken))
            {
                try
                {
                    if (uwr.error == null)
                    {
                        if (uwr.responseCode == (long)HttpStatusCode.NoContent)
                        {
                            return new T();
                        }

                        var settings = JsonParser.Settings.Default.WithIgnoreUnknownFields(true);
                        return new JsonParser(settings).Parse<T>(uwr.downloadHandler.text);
                    }

                    var errorMessage = ParseErrorMessage(uwr);

                    throw new PassportException(errorMessage.Code, errorMessage.Message);
                }
                catch (Exception e)
                {
                    switch (e)
                    {
                        case PassportException exception:
                            if (exceptionHandler == null)
                            {
                                throw;
                            }

                            exceptionHandler(exception);
                            return new T();
                    }

                    Debug.Log(e);
                    if (exceptionHandler == null)
                    {
                        throw new PassportException(ErrorCode.Unknown, e.Message);
                    }

                    exceptionHandler(new PassportException(ErrorCode.Unknown, e.Message));
                    return new T();
                }
            }
        }

        protected internal static async Task<T> PassportDeleteRequestInternal<T>(string url,
            string appID, string appSecret, string authorizationToken, bool canRetry,
            Action<PassportException> exceptionHandler = null) where T : IMessage<T>, new()
        {
            using (UnityWebRequest uwr =
                   await NetworkUtils.DeleteRequest(url, appID, appSecret, authorizationToken))
            {
                try
                {
                    if (uwr.error == null)
                    {
                        if (uwr.responseCode == (long)HttpStatusCode.NoContent)
                        {
                            return new T();
                        }

                        var settings = JsonParser.Settings.Default.WithIgnoreUnknownFields(true);
                        return new JsonParser(settings).Parse<T>(uwr.downloadHandler.text);
                    }

                    var errorMessage = ParseErrorMessage(uwr);

                    throw new PassportException(errorMessage.Code, errorMessage.Message);
                }
                catch (Exception e)
                {
                    switch (e)
                    {
                        case PassportException exception:
                            if (exceptionHandler == null)
                            {
                                throw;
                            }

                            exceptionHandler(exception);
                            return new T();
                    }

                    Debug.Log(e);
                    if (exceptionHandler == null)
                    {
                        throw new PassportException(ErrorCode.Unknown, e.Message);
                    }

                    exceptionHandler(new PassportException(ErrorCode.Unknown, e.Message));
                    return new T();
                }
            }
        }

        protected internal static async Task<T> PassportGetRequest<T>(string url,
            string appID, string appSecret, string authorizationToken = "",
            Action<PassportException> exceptionHandler = null) where T : IMessage<T>, new()
        {
            return await PassportGetRequestInternal<T>(url, appID, appSecret, authorizationToken, true,
                exceptionHandler);
        }

        protected internal static async Task<T> PassportDeleteRequest<T>(string url,
            string appID, string appSecret, string authorizationToken = "",
            Action<PassportException> exceptionHandler = null) where T : IMessage<T>, new()
        {
            return await PassportDeleteRequestInternal<T>(url, appID, appSecret, authorizationToken, true,
                exceptionHandler);
        }
        

        protected internal static PassportErrorMessage ParseErrorMessage(UnityWebRequest request)
        {
            ErrorCode code = ErrorCode.Default;
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                return new PassportErrorMessage()
                    { Code = ErrorCode.SdkerrorNetworkError, Message = request.error };
            }

            PassportCustomError customError;
            try
            {
                customError = JsonConvert.DeserializeObject<PassportCustomError>(request.downloadHandler.text);
                code = CastToPassportErrorCode(customError);
                var Message = customError.Message;

                return new PassportErrorMessage()
                    { Code = code, Message = Message };
            }
            catch (Exception e)
            {
                Debug.Log(e);
                switch (e)
                {
                    case PassportException:
                        throw;
                }

                Debug.Log(e);
                return new PassportErrorMessage() { Code = ErrorCode.Unknown, Message = fallbackMsg };
            }
        }

        private static bool ShouldRefreshToken(PassportErrorMessage message)
        {
            return message.Code == ErrorCode.UnauthenticatedPassportAccessTokenExpired;
        }

        protected static internal ErrorCode CastToPassportErrorCode(PassportCustomError customError)
        {
            if (Enum.IsDefined(typeof(ErrorCode), customError.DetailCode) && customError.DetailCode != 0)
            {
                return (ErrorCode)customError.DetailCode;
            }

            return (ErrorCode)(customError.Code * 10000);
        }
        
    }
}