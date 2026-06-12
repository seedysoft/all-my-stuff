namespace Seedysoft.Libs.GasStationPrices.Extensions;

public static class ArrayExtensions
{
    // Source - https://stackoverflow.com/a/25995025
    // Posted by Pedro, modified by community. See post 'Timeline' for change history
    // Retrieved 2026-06-12, License - CC BY-SA 4.0
    public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
    {
        int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
        int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
        int numberOfRows = rowsLastIndex + 1;

        int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
        int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
        int numberOfColumns = columnsLastIndex + 1;

        var jaggedArray = new T[numberOfRows][];
        for (int row = rowsFirstIndex; row <= rowsLastIndex; row++)
        {
            jaggedArray[row] = new T[numberOfColumns];

            for (int col = columnsFirstIndex; col <= columnsLastIndex; col++)
            {
                jaggedArray[row][col] = twoDimensionalArray[row, col];
            }
        }

        return jaggedArray;
    }

    // Source - https://stackoverflow.com/a/26291720
    // Posted by Diligent Key Presser, modified by community. See post 'Timeline' for change history
    // Retrieved 2026-06-12, License - CC BY-SA 4.0
    public static T[,] To2D<T>(this T[][] source)
    {
        try
        {
            int FirstDim = source.Length;
            int SecondDim = source.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

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
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("The given jagged array is not rectangular.");
        }
    }
}
