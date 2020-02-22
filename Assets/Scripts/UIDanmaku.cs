using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIDanmaku : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Text text;
    public float speed;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            rectTransform.position = new Vector3(rectTransform.position.x - speed * Time.deltaTime, rectTransform.position.y, rectTransform.position.z);
        }
    }
}
