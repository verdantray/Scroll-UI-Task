using System;

namespace Core
{
    public enum DataType { Text, Image }
    
    /// <summary>
    /// A Struct contains Information for Scroll-UI-Element
    /// </summary>
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