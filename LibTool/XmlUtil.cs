using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace DaiChong.Lib.Util
{
    public class XmlUtil
    {
        private XPathDocument docNav = null;
        public XPathNavigator Nav { get; set; }
        public XmlUtil(string xmlPath) : this(xmlPath, true)
        {

        }
        public XmlUtil(XPathDocument doc)
        {
            this.docNav = doc;
            Nav = docNav.CreateNavigator();
        }

        public XmlUtil(string xml, bool isPath)
        {
            if (isPath)
            {
                this.docNav = new XPathDocument(xml);
            }
            else
            {
                using (StringReader stream = new StringReader(xml))
                {
                    this.docNav = new XPathDocument(stream);
                }
            }
            Nav = docNav.CreateNavigator();
        }

        public string GetString(string xpath, string attr)
        {
            string ret = string.Empty;
            XPathNodeIterator ite = Nav.Select(xpath);
            if (ite.MoveNext())
            {
                ret = ite.Current.GetAttribute(attr, string.Empty);
            }
            return ret;
        }

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static T Deserialize<T> (string xml)  where T : class
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                return xmldes.Deserialize(sr) as T;
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream) where T : class
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return xmldes.Deserialize(stream) as T;
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(object obj,bool omitXmlDeclaration = true)
        {
            string str = string.Empty;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlWriterSettings settings = new XmlWriterSettings();
            //去除顶部的<xml>
            settings.OmitXmlDeclaration = omitXmlDeclaration;

            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriter writer = XmlWriter.Create(stream, settings);

                XmlSerializer xml = new XmlSerializer(obj.GetType());
                xml.Serialize(writer, obj, ns);

                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                str = sr.ReadToEnd();
            }
            return str;
        }

        public static string Serializer(object obj, XmlWriterSettings settings)
        {
            string str = string.Empty;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
 
            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriter writer = XmlWriter.Create(stream, settings);

                XmlSerializer xml = new XmlSerializer(obj.GetType());
                xml.Serialize(writer, obj, ns);

                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                str = sr.ReadToEnd();
            }
            return str;
        }

        #endregion
    }
}
