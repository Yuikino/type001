using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoDanmaku : MonoBehaviour
{
    public Transform laneParent;
    public UIDanmaku uiDanmakuPrefab;
    public Button pauseButton;
    public InputField danmakuInput;

    private VideoPlayer videoPlayer;
    // 为什么不用Queue？因为Queue不能插入到最前面，那么到时候发送弹幕的时候不好处理
    private List<Danmaku> danmakuList = new List<Danmaku>();
    private List<Transform> laneList = new List<Transform>();

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        
        var loveYouForeverRest = new LoveYouForeverRest("http://localhost:8080");
        loveYouForeverRest.ListDanmakus("welcome", result =>
        {
            if (result.status == "ok")
            {
                danmakuList.AddRange(result.list);
                videoPlayer.Play();
            }
        }, (url, responseCode, error) =>
        {
            Debug.Log($"url: {url}, responseCode: {responseCode}, error: {error}");
        });
        
        for (int i = 0; i < laneParent.childCount; i++)
        {
            laneList.Add(laneParent.GetChild(i));
        }
        pauseButton.onClick.AddListener(() =>
        {
            videoPlayer.Pause();
        });
        danmakuInput.onEndEdit.AddListener(text =>
        {
            AddDanmaku addDanmaku = new AddDanmaku();
            addDanmaku.text = text;
            addDanmaku.time = (float)videoPlayer.clockTime;
            addDanmaku.scene = "welcome";
            loveYouForeverRest.AddDanamku(addDanmaku, result =>
            {
                if (result.status == "ok")
                {
                    Danmaku danmaku = new Danmaku();
                    danmaku.text = addDanmaku.text;
                    danmaku.time = addDanmaku.time;
                    danmakuList.Insert(0, danmaku);
                    danmakuInput.text = "";
                }
            }, null);
        });
    }


    void Update()
    {
        // 获得当前视频时间
        var time = videoPlayer.clockTime;
        int toRemove = 0;
        foreach (var danmaku in danmakuList)
        {
            // 如果弹幕的时间小于当前时间，代表要被显示
            if (danmaku.time < time)
            {
                // 显示弹幕
                ShowDanmaku(danmaku);
                // 移除弹幕
                toRemove++;
            }
            else// 如果弹幕时间大于等于当前时间，代表不用被显示，调用break中断循环
            {
                break;
            }
        }
        // 遍历一下处理掉多少条弹幕就删掉多少条
        for (int i = 0; i < toRemove; i++)
        {
            // 每次删掉第一条
            danmakuList.RemoveAt(0);
        }
    }

    void ShowDanmaku(Danmaku danmaku)
    {
        UIDanmaku uiDanmaku = Instantiate(uiDanmakuPrefab);
        uiDanmaku.text.text = danmaku.text;
        uiDanmaku.videoPlayer = videoPlayer;
        uiDanmaku.transform.SetParent(laneList[Random.Range(0, laneList.Count - 1)]);
        uiDanmaku.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
