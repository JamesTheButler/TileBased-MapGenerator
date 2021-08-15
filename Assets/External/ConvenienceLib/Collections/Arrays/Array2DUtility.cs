namespace Convenience.Collections.Arrays {
    public static class Array2DUtility {
        /// <summary>
        /// Creates and returns an array.
        /// </summary>
        /// <param name="fillValue">[Optional] Initial value</param>
        public static T[,] CreateArray<T>(int length0, int length1, T fillValue = default) {
            var arr = new T[length0, length1];

            if (!fillValue.Equals(default(T))) { FillArray(arr, fillValue); }

            return arr;
        }

        /// <summary>
        /// Fills given the array with the given value.
        /// </summary>
        public static void FillArray<T>(T[,] array, T value) {
            for (int i = 0; i < array.GetLength(0); i++) {
                for (int j = 0; j < array.GetLength(1); j++) {
                    array[i, j] = value;
                }
            }
        }

        /// <summary>
        /// Fills a row in the given the array with the given value.
        /// </summary>
        public static void FillRow<T>(T[,] array, int rowId, T value) {
            if (rowId >= array.GetLength(1)) { return; }

            for (int i = 0; i < array.GetLength(0); i++) {
                array[i, rowId] = value;
            }
        }

        /// <summary>
        /// Fills a column in the given the array with the given value.
        /// </summary>
        public static void FillColumn<T>(T[,] array, int columnId, T value) {
            if (columnId >= array.GetLength(0)) { return; }

            for (int j = 0; j < array.GetLength(1); j++) {
                array[columnId, j] = value;
            }
        }

        //TODO add to lib
        public static void MaskedFill<T>(T[,] array, T value, bool[,] mask) {
            var len0 = System.Math.Min(array.GetLength(0), mask.GetLength(0));
            var len1 = System.Math.Min(array.GetLength(1), mask.GetLength(1));

            for (int i = 0; i < len0; i++) {
                for (int j = 0; j < len1; j++) {
                    if (mask[i, j]) { array[i, j] = value; }
                }
            }
        }

        public static string ToString<T>(T[,] array) {
            string s = "";
            for (int i = 0; i < array.GetLength(0); i++) {
                for (int j = 0; j < array.GetLength(1); j++) {
                    s += array[i, j].ToString() + "\t";
                }
                s += "\n";
            }
            return s;
        }
    }
}
