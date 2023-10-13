
namespace Lab3;

static class Misc{
public static double ConvertBinaryToDouble(string binaryNumber)
{
    // Split the binary number into integer and fractional parts
    string[] parts = binaryNumber.Split('.');
    
    // Convert the integer part to decimal
    double integerPart = Convert.ToInt32(parts[0], 2);
    
    // Convert the fractional part to decimal
    double fractionalPart = 0;
    if (parts.Length > 1)
    {
        string fractionalBinary = parts[1];
        for (int i = 0; i < fractionalBinary.Length; i++)
        {
            if (fractionalBinary[i] == '1')
            {
                fractionalPart += 1.0 / Math.Pow(2, i + 1);
            }
        }
    }
    
    // Combine the integer and fractional parts
    double result = integerPart + fractionalPart;
    
    return result;
}
}