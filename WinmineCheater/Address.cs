namespace WinmineCheater
{
    internal class Address
    {
        /// <summary>
        /// Column address.
        /// </summary>
        internal static readonly int COLUMN_ADDRESS = 0x01005334;
        /// <summary>
        /// Row address.
        /// </summary>
        internal static readonly int ROW_ADDRESS = 0x01005338;
        /// <summary>
        /// Mine start address.
        /// </summary>
        internal static readonly int MINE_START_ADDRESS = 0x01005361;
        /// <summary>
        /// Is enable "?" signature. 1: Enable, 0: Disable.
        /// </summary>
        internal static readonly int IS_ENABLE_QUESTION_MARK = 0x010056BC;
    }
}
