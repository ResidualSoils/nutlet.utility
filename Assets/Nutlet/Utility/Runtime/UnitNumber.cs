using System;
using System.Collections.Generic;
using System.Text;

namespace Nutlet.Utility
{
    /// <summary> 提供方便的数字的缩写表示，例如1.2K或者2.34m </summary>
    public struct UnitNumber
    {
        private static readonly List<char> _unitUpper = new List<char>() {'K', 'M', 'B'};
        private static readonly List<char> _unitLower = new List<char>() {'k', 'm', 'b'};
        private static readonly int _defaultDigit = 2;
        private static readonly bool _defaultCase = true;
        
        [ThreadStatic] 
        private static StringBuilder _builder;
        private static StringBuilder Builder => _builder ??= new StringBuilder();
        
        /// <summary> 原数字 </summary>
        public int Value { get; set; }
        
        public UnitNumber(int value)
        {
            Value = value;
        }

        /// <summary>
        /// 尝试将字符串转换为UnitNumber
        /// </summary>
        /// <param name="input"> 输入字符串 </param>
        /// <param name="result"> 转换结果 </param>
        /// <returns> 是否转换成功 </returns>
        public static bool TryParseUnitNumber(string input, out UnitNumber result)
        {
            result = new UnitNumber(0);

            if (int.TryParse(input, out var value))
            {
                result = new UnitNumber(value);
                return true;
            }
            
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Check if any character in the suffixes matches the last character of 'input'
            // 检查后缀中是否存在'input'中最后一个字符
            var unitIndex = _unitUpper.IndexOf(input[^1]);
            if (unitIndex == -1)
                unitIndex = _unitLower.IndexOf(input[^1]);
            if (unitIndex == -1)
                return false;
            
            if (double.TryParse(input.Remove(input.Length - 1), out var num))
            {
                var m = (unitIndex + 1) * 1000;
                result = new UnitNumber((int)(num * m));
                return true;
            }

            return false;
        }

        public override string ToString() => ToString(_defaultDigit, _defaultCase);
        
        /// <param name="digit"> 小数点后位数 </param>
        public string ToString(int digit) => ToString(digit, _defaultCase);
        
        /// <param name="isUpper"> 是否大写 </param>
        public string ToString(bool isUpper) => ToString(_defaultDigit, isUpper);

        /// <param name="digit"> 小数点后位数 </param>
        /// <param name="isUpper"> 是否大写 </param>
        public string ToString(int digit, bool isUpper)
        {
            var negative = Value < 0 ? 1 : 0;
            var row = Value.ToString();
            var unitIndex = (row.Length - negative - 1) / 3 - 1;

            if (unitIndex < 0)
                return row;
            
            var pointIndex = row.Length % 3 + negative;
            var length = negative + pointIndex + 1 + digit + 1;
            var unit = isUpper ? _unitUpper : _unitLower;

            Builder.Clear();
            for (var i = 0; i < length; i++)
            {
                if (i < pointIndex)
                    Builder.Append(row[i]);
                else if (i == pointIndex)
                    Builder.Append('.');
                else if (i == length - 1)
                    Builder.Append(unit[unitIndex]);
                else
                    Builder.Append(row[i + 1]);
            }

            return Builder.ToString();
        }
    }
}