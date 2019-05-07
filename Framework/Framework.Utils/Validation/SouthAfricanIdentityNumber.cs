using System;
using System.Globalization;
using Framework.Utils.Extensions.String;

namespace Framework.Utils.Validation
{
    public static class SouthAfricanIdentityNumber
    {
        public static string Generate()
        {
            var rand = new Random();
            while (true)
            {
                var dd = (int) Math.Floor(rand.NextDouble()*31) + 1;
                var mm = (int) Math.Floor(rand.NextDouble()*12) + 1;
                var yy = (int) (92 - Math.Floor(rand.NextDouble()*50));
                var g = (int) Math.Floor(rand.NextDouble()*9999);

                if (dd >= 29)
                {
                    if (mm == 2)
                    {
                        dd = 28;
                    }
                    else if ((dd == 31) && (mm%2 == 1))
                    {
                        dd = 30;
                    }
                }

                for (var i = 80; i < 90; i++)
                {
                    var gen = Space(yy.ToString(), 2) + Space(mm.ToString(), 2) + Space(dd.ToString(), 2)
                              + Space(g.ToString(), 2) + Space(i.ToString(), 3);

                    if (IsValid(gen))
                    {
                        return gen;
                    }
                }
            }
        }

        public static bool Validate(string identityNumber)
        {
            identityNumber = identityNumber.Trim();
            if (identityNumber.IsNumeric())
            {
                return identityNumber.Length == 13 &&
                       GetIdentityNumberControlDigit(identityNumber).ToString().Equals(identityNumber.Substring(12, 1));
            }

            return false;
        }

        private static string Space(string n, int width)
        {
            while (n.Length < width)
            {
                n = "0" + n;
            }

            return n;
        }

        private static bool IsValid(string str)
        {
            var odd = 0;
            var evenComposite = string.Empty;
            var even = 0;

            for (var i = 0; i < str.Length - 1; i += 2)
            {
                odd += Convert.ToInt16(str.Substring(i, 1));
            }

            for (var i = 1; i < str.Length - 1; i += 2)
            {
                evenComposite += str.Substring(i, 1);
            }

            var evenCompositeValue = Convert.ToInt32(evenComposite)*2;

            for (var i = 0; i < evenCompositeValue.ToString().Length; i++)
            {
                even += Convert.ToInt16(evenCompositeValue.ToString().Substring(i, 1));
            }

            return Convert.ToInt16(str.Substring(str.Length - 1, 1)) == 10 - (even + odd)%10;
        }

        private static int GetIdentityNumberControlDigit(string idNumber)
        {
            int d;
            var a = 0;
            for (var i = 0; i < 6; i++)
            {
                a += int.Parse(idNumber[2*i].ToString());
            }

            var b = 0;
            for (var i = 0; i < 6; i++)
            {
                b = b*10 + int.Parse(idNumber[2*i + 1].ToString(CultureInfo.InvariantCulture));
            }

            b *= 2;
            var c = 0;
            do
            {
                c += b%10;
                b = b/10;
            } while (b > 0);

            c += a;
            d = 10 - c%10;
            if (d == 10)
            {
                d = 0;
            }

            return d;
        }
    }
}