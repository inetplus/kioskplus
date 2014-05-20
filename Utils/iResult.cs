using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace kioskplus.Utils
{

    [XmlRoot("ip")]
    [Serializable]
    public class ReqResult
    {
        [XmlElement()]
        public String result
        { get; set; }
    }
}
