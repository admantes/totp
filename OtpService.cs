using System;
using System.Security.Cryptography;
using System.Text;

using OtpNet;
namespace TOTPTester
{

    public static class TOTP
    {
        private static readonly char[] _digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
        private const int _mask = 31;
        private const int _shift = 5;

        public static string GetUri(string key, string user, string issuer)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(key); 
            string base32Key = ToBase32String(bytes);
             
            var uriString = new OtpUri(OtpType.Totp, base32Key, user, issuer).ToString();
             
            return uriString;
        }

        public static bool ValidateTotp(string key, string code)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(key);          
            
            //TOTP validation will access last and next 2 totp generated codes
            var window = new VerificationWindow(previous: 2, future: 2);
            var totp = new Totp(bytes);

            long timeWindowUsed;
            return totp.VerifyTotp(code, out timeWindowUsed, window);
        }

        public static string ToBase32String(byte[] data, bool padOutput = false)
        {
            return ToBase32String(data, 0, data.Length, padOutput);
        }

        public static string ToBase32String(byte[] data, int offset, int length, bool padOutput = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((offset + length) > data.Length)
                throw new ArgumentOutOfRangeException();

            if (length == 0)
                return "";

            // SHIFT is the number of bits per output character, so the length of the
            // output is the length of the input multiplied by 8/SHIFT, rounded up.
            // The computation below will fail, so don't do it.
            if (length >= (1 << 28))
                throw new ArgumentOutOfRangeException(nameof(data));

            var outputLength = (length * 8 + _shift - 1) / _shift;
            var result = new StringBuilder(outputLength);

            var last = offset + length;
            int buffer = data[offset++];
            var bitsLeft = 8;
            while (bitsLeft > 0 || offset < last)
            {
                if (bitsLeft < _shift)
                {
                    if (offset < last)
                    {
                        buffer <<= 8;
                        buffer |= (data[offset++] & 0xff);
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = _shift - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }
                int index = _mask & (buffer >> (bitsLeft - _shift));
                bitsLeft -= _shift;
                result.Append(_digits[index]);
            }
            if (padOutput)
            {
                int padding = 8 - (result.Length % 8);
                if (padding > 0) result.Append('=', padding == 8 ? 0 : padding);
            }
            return result.ToString();
        }

    }

}
