using System;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using UnityEngine;

public class TestNetty : MonoBehaviour
{
    private IEventLoopGroup workerGroup;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            workerGroup = new MultithreadEventLoopGroup();
            Bootstrap b = new Bootstrap()
                .Group(workerGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    channel.Pipeline
                        .AddLast(new LengthFieldBasedFrameDecoder(short.MaxValue, 0, 2, 0, 2))
                        .AddLast(new LengthFieldPrepender(2))
                        .AddLast(new TestHandler());
                }));
            b.ConnectAsync("localhost", 9527);
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