using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CI.TaskParallel;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class TestNettyUI : SingletonUtil<TestNettyUI>
{
    public IChannel channel;
    public InputField contentInputField;
    public InputField nameInputField;
    public InputField textInputField;
    public Button sendButton;

    // Start is called before the first frame update
    void Start()
    {
        UnityTask.InitialiseDispatcher();
        sendButton.onClick.AddListener(() =>
        {
            var nameStr = nameInputField.text;
            var textStr = textInputField.text;
            var byteBuf = PooledByteBufferAllocator.Default.Buffer();
            byteBuf.WriteShort(1);
            byteBuf.WriteInt(Encoding.UTF8.GetBytes(nameStr).Length);
            byteBuf.WriteString(nameStr, Encoding.UTF8);
            byteBuf.WriteInt(Encoding.UTF8.GetBytes(textStr).Length);
            byteBuf.WriteString(textStr, Encoding.UTF8);
            if (FindObjectsOfType<TestNetty>().Length > 0)
            {
                Debug.Log("Send TCP");
                channel.WriteAndFlushAsync(byteBuf).Wait();
                Debug.Log("Sent TCP");
            }

            if (FindObjectsOfType<TestNettyUdp>().Length > 0)
            {
                SendUDP(byteBuf);
            }
        });
    }

    async void SendUDP(IByteBuffer byteBuf)
    {
        Debug.Log("Send UDP");
        await channel.WriteAndFlushAsync(new DatagramPacket(byteBuf, new IPEndPoint(IPAddress.Loopback, 2887)));
        // await channel.WriteAndFlushAsync(new DatagramPacket(byteBuf, new DnsEndPoint("localhost", 2887)));
        Debug.Log("Sent UDP");
    }

    // Update is called once per frame
    void Update()
    {
    }
}