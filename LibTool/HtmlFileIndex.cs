using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DaiChong.Lib.Util
{
    [Serializable]
    public class HtmlFileIndex
    {
        public Dictionary<string, string> Data { get; set; }
        public DateTime ProcessTime { get; set; }
        public HtmlFiles HtmlFileCollection { get; set; }
    }

    [Serializable]
    public class HtmlFiles : CollectionBase
    {
        public int Add(HtmlFile value)
        {
            return base.InnerList.Add(value);
        }

        public HtmlFile this[int idx]
        {
            get { return (HtmlFile)base.InnerList[idx]; }
            set { base.InnerList[idx] = value; }
        }
    }

    [Serializable]
    public class HtmlFile
    {
        public Dictionary<string, string> Data { get; set; }
        public DateTime SaveTime { get; set; }
        public List<Parameter> Parameters { get; set; }
    }

    [Serializable]
    public class Parameter
    {
        [XmlAttribute("key")]
        public string Key { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

}
