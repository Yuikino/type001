using System.Text;
using CI.TaskParallel;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using UnityEngine;

public class TestHandlerUdp : SimpleChannelInboundHandler<DatagramPacket>
{
    public override void ChannelActive(IChannelHandlerContext context)
    {
        Debug.Log("ChannelActive");
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg)
    {
        Debug.Log("ChannelRead");
        var byteBuf = msg.Content;
        var opcode = byteBuf.ReadShort();
        Debug.Log(opcode);
        switch (opcode)
        {
            case 1:
                var nameLength = byteBuf.ReadInt();
                var name = byteBuf.ReadString(nameLength, Encoding.UTF8);
                var textLength = byteBuf.ReadInt();
                var text = byteBuf.ReadString(textLength, Encoding.UTF8);
                Debug.Log($"name: {name}, text: {text}");
                UnityTask.RunOnUIThread(() => { TestNettyUI.Instance.contentInputField.text += $"{name}: {text}\n"; });
                break;
        }
    }
}