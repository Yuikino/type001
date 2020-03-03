using System;
using System.Net;
using System.Net.Sockets;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using UnityEngine;

public class TestNettyUdp : MonoBehaviour
{
    private IEventLoopGroup workerGroup;

    // Start is called before the first frame update
    void Start()
    {
        StartUdp();
    }

    async void StartUdp()
    {
        try
        {
            workerGroup = new MultithreadEventLoopGroup();
            Bootstrap b = new Bootstrap()
                .Group(workerGroup)
                .ChannelFactory(() => new SocketDatagramChannel(AddressFamily.InterNetwork))
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new ActionChannelInitializer<IDatagramChannel>(channel =>
                {
                    channel.Pipeline
                        .AddLast(new TestHandlerUdp());
                }));
            var ch = await b.BindAsync(IPEndPoint.MinPort);
            TestNettyUI.Instance.channel = ch;
            IByteBuffer byteBuf = PooledByteBufferAllocator.Default.Buffer();
            byteBuf.WriteShort(0);
            DatagramPacket datagramPacket = new DatagramPacket(byteBuf, new IPEndPoint(IPAddress.Loopback, 2887));
            // DatagramPacket datagramPacket = new DatagramPacket(byteBuf, new DnsEndPoint("localhost", 2887));
            await ch.WriteAndFlushAsync(datagramPacket);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}