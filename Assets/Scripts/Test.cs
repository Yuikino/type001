using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var loveYouForeverRest = new LoveYouForeverRest("http://seto.gicp.net");
        //var addDanmaku = new AddDanmaku();
        //addDanmaku.scene = "welcome";
        //addDanmaku.text = "wawa";
        //addDanmaku.time = 7;
        //loveYouForeverRest.AddDanamku(addDanmaku, result =>
        //{
        //    Debug.Log(result.status);
        //}, (url, responseCode, error) =>
        //{
        //    Debug.Log($"url: {url}, responseCode: {responseCode}, error: {error}");
        //});
        loveYouForeverRest.ListDanmakus("welcome", result =>
        {
            Debug.Log(result.status);
            foreach (var danmaku0 in result.list)
            {
                Debug.Log($"scene: {danmaku0.scene}, time: {danmaku0.time}, text: {danmaku0.text}");
            }
        }, (url, responseCode, error) =>
        {
            Debug.Log($"url: {url}, responseCode: {responseCode}, error: {error}");
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AAA(string url, long repsonseCode, string error)
    {

    }
}
