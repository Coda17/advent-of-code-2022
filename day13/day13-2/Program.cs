using day13_2;

var lines = File.ReadAllLines(args[0]);
const string decoderKeyStart = "[[2]]";
const string decoderKeyEnd = "[[6]]";
var packets = lines.Where(x => !string.IsNullOrEmpty(x)).Append(decoderKeyStart).Append(decoderKeyEnd).ToArray();
var ordered = packets.OrderBy(x => x, new PacketComparer()).ToList();

var start = ordered.IndexOf(decoderKeyStart) + 1;
var end = ordered.IndexOf(decoderKeyEnd) + 1;

Console.WriteLine($"Decoder key = {start} * {end} = {start * end}");