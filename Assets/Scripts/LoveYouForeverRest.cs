using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

public class LoveYouForeverRest
{
    private readonly string _baseUrl;
    // 字典里是正在加载的url和对应的AsyncOperation
    private static readonly Dictionary<string, UnityWebRequestAsyncOperation> CachingAsync = new Dictionary<string, UnityWebRequestAsyncOperation>();
    // 字典里是url和对应的文本缓存
    private static readonly Dictionary<string, string> TextCache = new Dictionary<string, string>();
    // 字典里是url和对应的上次缓存时间
    private static readonly Dictionary<string, long> CachingTime = new Dictionary<string, long>();
    // 默认的缓存超时时间
    private const long DefaultTimeout = 30000000;

    public LoveYouForeverRest(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    // 获得正在加载的数量
    public static int GetCachingCount()
    {
        return CachingAsync.Count;
    }

    // 清空缓存
    public static void ClearCache()
    {
        // 清空正在加载
        CachingAsync.Clear();
        // 清空文本缓存
        TextCache.Clear();
        // 清空上一次缓存时间
        CachingTime.Clear();
    }

    public void AddDanamku(AddDanmaku addDanmaku, Action<Result> onCompleted, Action<string, long, string> onError)
    {
        string body = JsonConvert.SerializeObject(addDanmaku);
        SendWebRequestText($"{_baseUrl}/loveyouforever/addDanmaku", resultText =>
        {
            var result = JsonConvert.DeserializeObject<Result>(resultText);
            onCompleted?.Invoke(result);
        }, onError, body);
    }

    public void ListDanmakus(string scene, Action<ListDanmakusResult> onCompleted, Action<string, long, string> onError)
    {
        SendWebRequestText($"{_baseUrl}/loveyouforever/listDanmakus/{scene}", resultText =>
        {
            var result = JsonConvert.DeserializeObject<ListDanmakusResult>(resultText);
            onCompleted?.Invoke(result);
        }, onError);
    }

    // 1.加载A，第一次的回调是onCompleted1，那我会告诉我正在加载A
    // 2.又加载A,第二次的回调是onCompleted2，我不去真的加载A，只要在完成的时候做我的onCompleted2就可以了
    // 3.那么加载完成会发生什么呢？首先调用一次onCompleted1，移除正在加载状态，调用一次onCompleted2
    public static void SendWebRequestText(string url, Action<string> onCompleted, Action<string, long, string> onError, string body = null, long timeout = DefaultTimeout)
    {
        // 是否正在加载
        if (CachingAsync.ContainsKey(url))
        {
            // 先拿到正在加载的请求
            var www = CachingAsync[url].webRequest;
            // 对正在加载的请求AsyncOperation加completed回调
            CachingAsync[url].completed += operation =>
            {
                // 因为已经不是首次加载，没必要在后续的回调中去掉正在加载状态，只需要调用OnComplete方法
                OnComplete(url, onCompleted, onError, www);
            };
        }
        else
        {
            // 是否不是POST(POST要传数据，不能缓存)， 是否没缓存过(上一次缓存时间不存在），是否上次缓存已过期（当前时间-上次缓存时间>超时时间）
            if (string.IsNullOrEmpty(body) || !CachingTime.ContainsKey(url) || DateTime.UtcNow.Ticks - CachingTime[url] > timeout)
            {
                // 如果有body为空或者空字符串，那么他是一个GET请求（不用传数据）；否则，他是一个POST请求（要传数据）
                var www = new UnityWebRequest(url, string.IsNullOrEmpty(body) ? "GET" : "POST");
                // 如果有body不为空或者空字符串，那么他是一个POST请求（要传数据）
                if (!string.IsNullOrEmpty(body))
                {
                    // 创建一个UploadHandlerRaw，参数是body的UTF-8的字节
                    UploadHandler uh = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                    // Content-Type为application/json
                    uh.contentType = "application/json";
                    // 赋值uh给www
                    www.uploadHandler = uh;
                }
                www.downloadHandler = new DownloadHandlerBuffer();

                // 把创建的请求发出去
                var request = www.SendWebRequest();
                // 记住我已经在加载了，并且把这个AsyncOperation保存起来，方便下一次加completed回调
                CachingAsync[url] = request;
                // 首次加载，对正在加载的请求AsyncOperation加completed回调
                request.completed += operation =>
                {
                    // 调用OnComplete方法
                    OnComplete(url, onCompleted, onError, www);
                    // 把表示正在加载的AsyncOperation干掉，根据url来移除，所以代表我已经不是正在加载了
                    CachingAsync.Remove(url);
                };
            }
            else // 是GET请求，并且缓存过，并且缓存没过期
            {
                // 直接对缓存的内容调用onCompleted
                onCompleted(TextCache[url]);
            }
        }
    }

    private static void OnComplete(string url, Action<string> onCompleted, Action<string, long, string> onError, UnityWebRequest www)
    {
        // 如果是错误
        if (www.isNetworkError || www.isHttpError)
        {
            // 调用onError的回调（url地址，返回码，错误信息）
            onError?.Invoke(www.url, www.responseCode, www.error);
        }
        else// 如果没错误
        {
            // 记住上一次缓存的时间
            CachingTime[url] = DateTime.UtcNow.Ticks;
            // 把请求返回的结果存到缓存里面
            TextCache[url] = www.downloadHandler.text;
            // 调用onCompleted回调（请求返回的结果）
            onCompleted?.Invoke(TextCache[url]);
        }
    }
}
