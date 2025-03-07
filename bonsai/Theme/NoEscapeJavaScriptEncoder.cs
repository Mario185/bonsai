using System;
using System.Buffers;
using System.Text.Encodings.Web;

namespace bonsai.Theme
{
  // Not used but saved for later usage
  public class NoEscapeJavaScriptEncoder : JavaScriptEncoder
  {
    public override int MaxOutputCharactersPerInputCharacter => 1;

    public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
    {
      numberOfCharactersWritten = 0;
      return false;
    }

    public override bool WillEncode(int unicodeScalar)
    {
      return false;
    }

    public override OperationStatus EncodeUtf8(ReadOnlySpan<byte> utf8Source, Span<byte> utf8Destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true)
    {
      utf8Source.CopyTo(utf8Destination);
      bytesConsumed = utf8Source.Length;
      bytesWritten = utf8Source.Length;
      return OperationStatus.Done;
    }

    public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
    {
      return -1;
    }

    public override OperationStatus Encode(ReadOnlySpan<char> source, Span<char> destination, out int charsConsumed, out int charsWritten, bool isFinalBlock)
    {
      source.CopyTo(destination);
      charsConsumed = source.Length;
      charsWritten = source.Length;
      return 0;
    }

    public override string Encode(string value)
    {
      return value;
    }
  }
}