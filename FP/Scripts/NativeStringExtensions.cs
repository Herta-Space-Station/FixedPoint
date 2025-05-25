using NativeCollections;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     Provides extension methods for NativeString to append various Herta math types.
    /// </summary>
    public static class NativeStringExtensions
    {
        /// <summary>
        ///     Appends an FP value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The FP value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, FP v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an FPMatrix2x2 value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The FPMatrix2x2 value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, FPMatrix2x2 v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an FPMatrix3x3 value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The FPMatrix3x3 value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, FPMatrix3x3 v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an FPMatrix4x4 value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The FPMatrix4x4 value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, FPMatrix4x4 v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an FPQuaternion value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The FPQuaternion value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, FPQuaternion v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an FPVector2 value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The FPVector2 value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, FPVector2 v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an FPVector3 value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The FPVector3 value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, FPVector3 v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an IntVector2 value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The IntVector2 value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, IntVector2 v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Appends an IntVector3 value to the NativeString if formatting succeeds.
        /// </summary>
        /// <param name="builder">The NativeString builder to append to.</param>
        /// <param name="v">The IntVector3 value to append.</param>
        /// <returns>True if formatting and appending succeeded, false otherwise.</returns>
        public static bool AppendFormattable(ref this NativeString builder, IntVector3 v)
        {
            if (v.TryFormat(builder.Space, out int charsWritten))
            {
                builder.Advance(charsWritten);
                return true;
            }

            return false;
        }
    }
}