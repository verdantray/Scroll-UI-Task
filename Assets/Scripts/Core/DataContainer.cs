using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public enum DataType { Text, Image }
    
    [Serializable]
    public struct DataContainer
    {
        public DataType type;
        public string content;

        public override string ToString()
        {
            return $"{type} : {content}";
        }
    }
}