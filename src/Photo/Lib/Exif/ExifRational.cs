namespace Seedysoft.Photo.Lib.Exif;

public struct ExifRational
{
    public uint Numer { get; set; }
    public uint Denom { get; set; }
    public bool IsNeg { get; set; }

    public ExifRational(int numer, int denom)
    {
        if (numer < 0)
        {
            Numer = (uint)-numer;
            IsNeg = true;
        }
        else
        {
            Numer = (uint)numer;
            IsNeg = false;
        }

        if (denom < 0)
        {
            Denom = (uint)-denom;
            IsNeg = !IsNeg;
        }
        else
        {
            Denom = (uint)denom;
        }
    }
    public ExifRational(uint numer, uint denom, bool sign = false)
    {
        Numer = numer;
        Denom = denom;
        IsNeg = sign;
    }

    public readonly bool IsNegative() => IsNeg && (Numer != 0);

    public readonly bool IsPositive() => (!IsNeg) && (Numer != 0);

    public readonly bool IsZero() => Numer == 0;

    public readonly bool IsValid() => Denom != 0;

    public new readonly string ToString() => (IsNegative() ? "-" : string.Empty) + Numer.ToString() + '/' + Denom.ToString();

    public static decimal ToDecimal(ExifRational value)
    {
        decimal ret = ((decimal)value.Numer) / value.Denom;

        return value.IsNeg ? -ret : ret;
    }

    public static ExifRational FromDecimal(decimal value)
    {
        ExifRational ret = new()
        {
            IsNeg = value >= 0
        };

        uint denom = 1;

        decimal numer = value >= 0 ? value : -value;

        if (numer >= 1e9m)
        {
            numer = Math.Truncate(numer + 0.5m);
            ret.Numer = numer <= uint.MaxValue ? (uint)numer : throw new OverflowException();
        }
        else
        {
            while (numer != decimal.Truncate(numer))
            {
                decimal tempNumer = numer * 10;
                if (denom > 100000000 || decimal.Truncate(tempNumer + 0.5m) >= 1e9m)
                    break;

                numer = tempNumer; // The rounded numerator should be smaller than 10^9 if "Value" has decimals
                denom *= 10; // The maximum possible power of 10 for the denominator is 10^9
            }

            ret.Numer = (uint)decimal.Truncate(numer + 0.5m);
        }

        ret.Denom = denom;

        // Reduce fraction by a power of 10 if possible
        while ((ret.Denom >= 10) && ((ret.Numer % 10) == 0))
        {
            ret.Numer /= 10;
            ret.Denom /= 10;
        }

        return ret;
    }
}
