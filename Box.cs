using System;
using System.Collections.Generic;
using System.Text;

namespace Boredbone.Utility
{
    public class Box<T>
    {
        public T Value { get; set; }

        public Box(T value){
            this.Value = value;
        }
    }
}
