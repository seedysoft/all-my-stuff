namespace Seedysoft.Libs.Travel.Extensions;

public static class ArrayExtensions
{
    // Source - https://stackoverflow.com/a/25995025
    // Posted by Pedro, modified by community. See post 'Timeline' for change history
    // Retrieved 2026-06-12, License - CC BY-SA 4.0
    public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
    {
        int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
        int colsLastIndex = twoDimensionalArray.GetUpperBound(1);

        var jaggedArray = new T[(rowsLastIndex + 1)][];
        for (int row = twoDimensionalArray.GetLowerBound(0); row <= rowsLastIndex; row++)
        {
            jaggedArray[row] = new T[(colsLastIndex + 1)];

            for (int col = twoDimensionalArray.GetLowerBound(1); col <= colsLastIndex; col++)
            {
                jaggedArray[row][col] = twoDimensionalArray[row, col];
            }
        }

        return jaggedArray;
    }

    // Source - https://stackoverflow.com/a/26291720
    // Posted by Diligent Key Presser, modified by community. See post 'Timeline' for change history
    // Retrieved 2026-06-12, License - CC BY-SA 4.0
    public static T[,] To2D<T>(this T[][]? source)
    {
        if (source == null)
            return new T[0, 0];

        // throws InvalidOperationException if source is not rectangular
        int SecondDim = source.GroupBy(static row => row.Length).Single().Key;
        int FirstDim = source.Length;

        var result = new T[FirstDim, SecondDim];
        for (int row = 0; row < FirstDim; ++row)
        {
            for (int col = 0; col < SecondDim; ++col)
            {
                result[row, col] = source[row][col];
            }
        }

        return result;
    }
}
