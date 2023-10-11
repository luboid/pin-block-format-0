using System;
using System.Globalization;
using System.Linq;

namespace PinBlock.Internal;

internal static class StringExtensions
{
    public static string ExtractPanAccountNumberPart(this string accountNumber)
    {
        string accountNumberPart;
        if (accountNumber?.Length > 12)
        {
            accountNumberPart = accountNumber.Substring(accountNumber.Length - 13, 12);
        }
        else
        {
            accountNumberPart = accountNumber;
        }

        return accountNumberPart ?? string.Empty;
    }

    public static string FromByteArray(this byte[] input)
    {
        if (input == null)
        {
            return default;
        }

        return string.Join(string.Empty, input.Select(item => item.ToString("X2")));
    }

    public static byte[] ToByteArray(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length % 2 == 1)
        {
            text = "0" + text;
        }

        try
        {
            byte[] result = new byte[text.Length / 2];
            for (int index = 0; index < text.Length; index += 2)
            {
                result[index / 2] = byte.Parse(text.Substring(index, 2), NumberStyles.HexNumber);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Can't convert `{text}` to byte array.", ex);
        }
    }

    public static byte[] Xor(this string operator1, string operator2)
    {
        if (string.IsNullOrEmpty(operator1))
        {
            throw new ArgumentNullException(nameof(operator1));
        }

        if (string.IsNullOrEmpty(operator2))
        {
            throw new ArgumentNullException(nameof(operator2));
        }

        if (operator1.Length != operator2.Length)
        {
            throw new ArgumentException($"{nameof(operator1)} and {nameof(operator2)} have different length.");
        }

        byte[] op1 = operator1.ToByteArray();
        byte[] op2 = operator2.ToByteArray();

        byte[] result = new byte[op1.Length];
        for (int index = 0; index < op1.Length; index++)
        {
            result[index] = (byte)(op1[index] ^ op2[index]);
        }

        return result;
    }
}
