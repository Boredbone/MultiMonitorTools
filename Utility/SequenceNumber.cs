using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Boredbone.Utility
{
    /// <summary>
    /// 文字列を連番で並べ替えるためのオブジェクト
    /// </summary>
    public struct SequenceNumber : IComparable
    {
        private string preName;
        private string numberString;
        private string postName;
        private string extension;
        private int number;

        private static Regex regex = new Regex(@"\d+", RegexOptions.RightToLeft);

        public SequenceNumber(string str)
        {
            //var sname = Path.GetFileNameWithoutExtension(str);
            extension = Path.GetExtension(str);
            var name = str.Substring(0, str.Length - extension.Length);


            numberString = regex.Match(name).ToString();
            Int32.TryParse(numberString, out this.number);

            var sp = regex.Split(name, 2);


            if (sp.Length > 0)
            {
                preName = sp[0];
            }
            else
            {
                preName = "";
            }

            if (sp.Length > 2)
            {
                //postName = string.Join("", Enumerable.Range(1, sp.Length - 1).Select(x => sp[x]));
                postName = string.Join("", sp.Skip(1));
            }
            else if (sp.Length > 1)
            {
                postName = sp[1];
            }
            else
            {
                postName = "";
            }
        }


        public int CompareTo(object obj)
        {
            var other = (SequenceNumber)obj;

            var result = this.preName.CompareTo(other.preName);
            if (result != 0)
            {
                return result;
            }

            result = this.number.CompareTo(other.number);
            if (result != 0)
            {
                return result;
            }

            result = this.numberString.CompareTo(other.numberString);
            if (result != 0)
            {
                return result;
            }

            result = this.postName.CompareTo(other.postName);
            if (result != 0)
            {
                return result;
            }

            result = this.extension.CompareTo(other.extension);
            if (result != 0)
            {
                return result;
            }
            return 0;
        }

        public override string ToString()
        {
            return preName + "<" + number.ToString() + ">" + postName + extension;
        }
    }
}
