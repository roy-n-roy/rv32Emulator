using System;
using System.Collections.Generic;
using System.Linq;

namespace RiscVCpu {

    /// <summary>
    /// bool配列をbyte配列に、byte配列をbool配列に変換します
    /// </summary>
    public static class BitByteConverter {
        #region ビット/バイト変換 メソッド定義

        /// <summary>指定したbool配列をbyte配列として返します</summary>
        public static IEnumerable<byte> GetBytes(IEnumerable<bool> bits, int startIndex, int count) {
            int i = 0;
            byte result = 0;

            foreach (bool bit in bits.Skip(startIndex).Take(count).ToArray()) {
                // 指定桁数について1を立てる
                if (bit) { result |= (byte)(1 << i); }

                if (i == 7) {
                    // 1バイト分で出力しビットカウント初期化
                    yield return result;
                    i = 0;
                    result = 0;
                } else {
                    i++;
                }
            }
            // 8ビットに足りない部分も出力
            if (i != 0) yield return result;
        }

        /// <summary>指定したbyte配列をbool配列として返します</summary>
        public static IEnumerable<bool> ToBits(IEnumerable<byte> bytes) {
            int i = 0;

            // 必要長の配列初期化
            bool[] result = new bool[bytes.Count() * 8];

            foreach (byte b in bytes) {
                foreach (int j in Enumerable.Range(0, 8)) {
                    // 1の場合、指定位置のboolをtrueにする
                    result[i * 8 + j] = (b & (1 << j)) > 0;

                }
                i++;
            }
            return result;
        }
        #endregion
    }
}
