using System;
using System.Security.Cryptography;
using PinBlock.Internal;

namespace PinBlock;

public sealed class PinBlockDecoder : IDisposable
{
    private readonly TripleDES tripleDES;
    private readonly ICryptoTransform decryptor;

    public PinBlockDecoder(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        key = key.Trim();
        if (key.Length != 32)
        {
            throw new ArgumentException($"The Key has to be 32 symbols.");
        }

        this.tripleDES = TripleDES.Create();
        this.tripleDES.KeySize = 128;
        this.tripleDES.Padding = PaddingMode.None;
        this.tripleDES.Mode = CipherMode.ECB;
        this.tripleDES.Key = key.ToByteArray();

        this.decryptor = tripleDES.CreateDecryptor();
    }

    public void Dispose()
    {
        this.decryptor?.Dispose();
        this.tripleDES?.Dispose();
    }

    public string Decode(string pinBlock, string pan)
    {
        if (string.IsNullOrEmpty(pan))
        {
            throw new ArgumentNullException(nameof(pan));
        }

        if (string.IsNullOrEmpty(pinBlock))
        {
            throw new ArgumentNullException(nameof(pinBlock));
        }

        if (pan.Length < 12)
        {
            throw new ArgumentException("The pan length need to be at least 12 symbols.");
        }

        byte[] bExtendedPinBlock = pinBlock.ToByteArray();
        string panPart = pan.ExtractPanAccountNumberPart();
        string panData = panPart.PadLeft(16, '0');
        byte[] bPan = panData.ToByteArray();

        byte[] decryptedPinBlock = this.decryptor.TransformFinalBlock(bExtendedPinBlock, 0, bExtendedPinBlock.Length);

        byte[] bPin = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            bPin[i] = (byte)(decryptedPinBlock[i] ^ bPan[i]);
        }

        string pinData = bPin.FromByteArray();
        int pinLen = Convert.ToInt32(pinData.Substring(0, 2));
        return pinData.Substring(2, pinLen);
    }
}
