
using NBitcoin;
using System;
using System.Numerics;

namespace KeyNBitcoin
{
  public class Program
  {
    public static void Main(string[] args)
    {
      byte[] bytes = new BigInteger(934157136952).ToByteArray();
      Array.Resize<byte>(ref bytes, 32);
      Key key = new Key(bytes);
      Console.WriteLine(key.PubKey);
    }
  }
}