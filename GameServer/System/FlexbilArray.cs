using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish
{
    public class FlexbilArray<T>
    {
        /// <summary>
        /// Entire Values
        /// </summary>
        private T[] Values = null;
        private int Length = 0;
        private int NextInsertIndex = 0;
        /// <summary>
        /// Setting Max Capacty to Array
        /// </summary>
        /// <param name="Ln"></param>
        public FlexbilArray(int Ln)
        {
            Values = new T[Ln];
            Length = Ln;
            NextInsertIndex = 0;
        }
        /// <summary>
        /// Creation Method
        /// </summary>
        public FlexbilArray()
        {
            Values = new T[0];
            Length = 0;
            NextInsertIndex = 0;
        }
        /// <summary>
        /// Adding Element To Array It's Size Increases automatically
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            if (NextInsertIndex == Length)
            {
                Length++;
                Array.Resize<T>(ref Values, Length);
            }
            Values[NextInsertIndex] = element;
            NextInsertIndex++;
        }
        /// <summary>
        /// Counting Elements In Array
        /// </summary>
        public int Count
        {
            get
            {
                return Values.Length;
            }
        }
        /// <summary>
        /// Getting Array Values From Existing Values
        /// </summary>
        /// <returns></returns>
        public T[] GetValues()
        {
            return Values;
        }
        /// <summary>
        /// Getting Element With It's Index
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public T GetElement(int Index)
        {
            if (Index > Values.Length)
                throw new Exception("Can't Get Uppder Value");
            return Values[Index];
        }
    }
}
