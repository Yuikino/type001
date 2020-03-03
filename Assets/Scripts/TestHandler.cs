using System.Collections;
using System.Collections.Generic;
using System.Text;
using CI.TaskParallel;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using UnityEngine;

public class TestHandler : SimpleChannelInboundHandler<IByteBuffer>
{
    public override void ChannelActive(IChannelHandlerContext context)
    {
        Debug.Log("ChannelActive");
        TestNettyUI.Instance.channel = context.Channel;
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
    {
        Debug.Log("ChannelRead");
        var opcode = msg.ReadShort();
        Debug.Log(opcode);
        switch (opcode)
        {
            case 1:
                var nameLength = msg.ReadInt();
                var name = msg.ReadString(nameLength, Encoding.UTF8);
                var textLength = msg.ReadInt();
                var text = msg.ReadString(textLength, Encoding.UTF8);
                Debug.Log($"name: {name}, text: {text}");
                UnityTask.RunOnUIThread(() =>
                {
                    TestNettyUI.Instance.contentInputField.text += $"{name}: {text}\n";
                });
                break;
        }
    }
}