using System;
using System.Security.Cryptography;
using PinBlock.Internal;

namespace PinBlock;

public sealed class PinBlockEncoder : IDisposable
{
    private readonly TripleDES tripleDES;
    private readonly ICryptoTransform encryptor;

    public PinBlockEncoder(string key)
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
        this.encryptor = this.tripleDES.CreateEncryptor();
    }

    public void Dispose()
    {
        this.encryptor?.Dispose();
        this.tripleDES?.Dispose();
    }

    public string Encode(string pan, string pin)
    {
        if (string.IsNullOrEmpty(pan))
        {
            throw new ArgumentNullException(nameof(pan));
        }

        if (string.IsNullOrEmpty(pin))
        {
            throw new ArgumentNullException(nameof(pin));
        }

        if (pan.Length < 12)
        {
            throw new ArgumentException("The pan length need to be at least 12 symbols.");
        }

        if (pin.Length < 4)
        {
            throw new ArgumentException("The pin length need to be at least 4 symbols.");
        }

        string pinLenHead = pin.Length.ToString().PadLeft(2, '0') + pin;
        string pinData = pinLenHead.PadRight(16, 'F');
        byte[] bPin = pinData.ToByteArray();
        string panPart = pan.ExtractPanAccountNumberPart();
        string panData = panPart.PadLeft(16, '0');
        byte[] bPan = panData.ToByteArray();

        byte[] pinblock = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            pinblock[i] = (byte)(bPin[i] ^ bPan[i]);
        }

        byte[] encryptedPinblock = this.encryptor.TransformFinalBlock(pinblock, 0, pinblock.Length);

        return encryptedPinblock
            .FromByteArray()
            .ToUpperInvariant();
    }
}
