using System.Globalization;
using System.Text.RegularExpressions;
using HubbingHome.Api.Options;

namespace HubbingHome.Api.Services;

/// <summary>
/// Daikinエアコン向けBroadLink送信コマンドを生成
/// </summary>
internal static partial class DaikinAirConditionerBroadlinkCommandGenerator
{
    private const int CoolSecondFrameModeByte = 0x39;
    private const int HeatSecondFrameModeByte = 0x49;

    /// <summary>
    /// Home Assistant BroadLinkのb64直接送信用コマンドを生成
    /// </summary>
    public static string Generate(string commandName, DaikinAirConditionerCodeOptions options)
    {
        var match = DaikinTemperatureCommandRegex().Match(commandName);
        if (!match.Success)
        {
            throw new RemoteCommandValidationException("Daikin air conditioner command is not supported");
        }

        var mode = match.Groups["mode"].Value;
        var wholeDegrees = int.Parse(match.Groups["whole"].Value, CultureInfo.InvariantCulture);
        var halfDegrees = match.Groups["half"].Value == "5" ? 0.5 : 0;
        var temperature = wholeDegrees + halfDegrees;
        var temperatureByte = checked((byte)(temperature * 2));

        var firstFrameModeByte = mode == "heat"
            ? checked((byte)options.HeatFirstFrameModeByte)
            : checked((byte)options.CoolFirstFrameModeByte);
        var secondFrameModeByte = mode == "heat"
            ? HeatSecondFrameModeByte
            : CoolSecondFrameModeByte;

        var firstFrame = CreateFirstFrame(firstFrameModeByte);
        var secondFrame = CreateSecondFrame((byte)secondFrameModeByte, temperatureByte);

        return $"b64:{Convert.ToBase64String(CreateBroadlinkPacket(firstFrame, secondFrame))}";
    }

    private static byte[] CreateFirstFrame(byte modeByte)
    {
        var frame = new byte[]
        {
            0x11, 0xDA, 0x27, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x03,
            0x00, 0x00, modeByte, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };
        frame[^1] = CalculateChecksum(frame.AsSpan(0, frame.Length - 1));
        return frame;
    }

    private static byte[] CreateSecondFrame(byte modeByte, byte temperatureByte)
    {
        var frame = new byte[]
        {
            0x11, 0xDA, 0x27, 0x00, 0x00, modeByte, temperatureByte, 0x00, 0xA0, 0x00,
            0x00, 0x06, 0x60, 0x00, 0x00, 0xC3, 0x00, 0x00, 0x00,
        };
        frame[^1] = CalculateChecksum(frame.AsSpan(0, frame.Length - 1));
        return frame;
    }

    private static byte CalculateChecksum(ReadOnlySpan<byte> bytes)
    {
        var sum = 0;
        foreach (var value in bytes)
        {
            sum += value;
        }

        return (byte)(sum & 0xFF);
    }

    private static byte[] CreateBroadlinkPacket(byte[] firstFrame, byte[] secondFrame)
    {
        var durations = new List<int>
        {
            17, 11, 16, 12, 16, 12, 15, 13, 15, 12, 16,
            797, 114, 53,
        };

        AddFrameDurations(durations, firstFrame);
        durations.AddRange([1118, 114, 53]);
        AddFrameDurations(durations, secondFrame);
        durations.Add(3333);

        var body = EncodeBroadlinkDurations(durations);
        var packet = new byte[4 + body.Length];
        packet[0] = 0x26;
        packet[1] = 0x00;
        packet[2] = (byte)(body.Length & 0xFF);
        packet[3] = (byte)(body.Length >> 8);
        body.CopyTo(packet.AsSpan(4));
        return packet;
    }

    private static void AddFrameDurations(List<int> durations, byte[] frame)
    {
        foreach (var value in frame)
        {
            for (var bit = 0; bit < 8; bit++)
            {
                durations.Add(16);
                durations.Add(((value >> bit) & 1) == 1 ? 40 : 12);
            }
        }

        durations.Add(16);
    }

    private static byte[] EncodeBroadlinkDurations(IEnumerable<int> durations)
    {
        var bytes = new List<byte>();
        foreach (var duration in durations)
        {
            if (duration > byte.MaxValue)
            {
                bytes.Add(0x00);
                bytes.Add((byte)(duration >> 8));
                bytes.Add((byte)(duration & 0xFF));
            }
            else
            {
                bytes.Add((byte)duration);
            }
        }

        return [.. bytes];
    }

    [GeneratedRegex("^(?<mode>cool|heat)_(?<whole>\\d{2})_(?<half>[05])$", RegexOptions.CultureInvariant)]
    private static partial Regex DaikinTemperatureCommandRegex();
}
