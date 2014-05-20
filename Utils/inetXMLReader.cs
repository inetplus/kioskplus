using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Drawing;

namespace kioskplus
{
    class inetXMLReader
    {
        private XmlReader inetXML = null;

        public Boolean readXMLFile(String asFile, String asClassName, ref inetRegionCtrl argRegion)
        {
            Boolean lbClassFound = false;
            Boolean lbClassNameFound = false;
            Boolean lbClassEnde = false;
            Boolean lbErsteDaten = false;
            int liWert = 0;
            Point[] tmpPoint;
            Rectangle tmpRect;
            String tmpText;
            char[] cSplitt={',',';'};
            String[] s;
            int[] iint;
            Boolean lbReadUserTextPos = false, lbReadTimeTextPos = false;
            if (File.Exists(asFile) == false)
                return false;

            inetXML = XmlReader.Create(asFile);
            inetRegionCtrl localRegionControl = new inetRegionCtrl();
            inetRegion tmpRegion = null;

            while (inetXML.Read())
            {
                switch (inetXML.NodeType)
                {
                    case XmlNodeType.Element:

                        switch(inetXML.Name)
                        {
                            case "class_name":
                                if (inetXML.Name == "class_name")
                                {
                                    lbClassNameFound = true;
                                }
                                break;
                            case "class_color":
                                liWert = 1;
                                break;
                            case "class_bg":
                                liWert = 2;
                                break;
                            case "name":
                                liWert = 3;
                                break;
                            case "onmouseover":
                                liWert = 4;
                                break;
                            case "onmouseout":
                                liWert = 5;
                                break;
                            case "rect":
                                liWert = 6;
                                break;
                            case "poly":
                                liWert = 7;
                                break;
                            case "text":
                                liWert = 8;
                                break;
                            case "txt_usr_color":
                                liWert = 9;
                                break;
                            case "txt_usr_pos":
                                liWert = 10;
                                break;
                            case "txt_usr_bold":
                                liWert = 11;
                                break;
                            case "time_pos":
                                liWert = 12;
                                break;
                        }
                       
                        break;
                    case XmlNodeType.Text:

                        if (lbClassFound)
                        {
                            switch (liWert)
                            {
                                case 1:
                                    
                                    break;
                                case 2:

                                    break;
                                case 3:
                                    tmpRegion.SetName(inetXML.Value);
                                    break;
                                case 4:
                                    tmpRegion.SetMausOver(inetXML.Value);
                                    break;
                                case 5:
                                    tmpRegion.SetMausOut(inetXML.Value);
                                    break;
                                case 6:// Rect 
                                    tmpText = inetXML.Value;
                                    if (!tmpText.Equals(String.Empty))
                                    {
                                        tmpText = tmpText.Trim();
                                       
                                        s = tmpText.Split(cSplitt);
                                        if (s.Length <= 4)
                                        {
                                            tmpRect = new Rectangle(Int32.Parse(s[0]), Int32.Parse(s[0]), Int32.Parse(s[0]), Int32.Parse(s[0]));
                                            tmpRegion.AddRectangle(tmpRect);
                                        }
                                    }
                                    break;
                                case 7: // Poly
                                    tmpText = inetXML.Value;
                                    if (!tmpText.Equals(String.Empty))
                                    {
                                        tmpText = tmpText.Trim();
                                        s = tmpText.Split(cSplitt);

                                        iint = null;
                                        iint = new int[s.Length];
                                        int index = 0;
                                        foreach (String sr in s)
                                        {
                                            iint[index] = Int32.Parse(sr);
                                            index++;
                                        }
                                        tmpPoint = new Point[(iint.Length/2)];
                                        int index1 = 0;
                                        for (index = 0; index < iint.Length; index=index+2)
                                        {
                                            tmpPoint[index1].X = iint[index];
                                            tmpPoint[index1].Y = iint[index + 1];
                                            index1++;
                                        }

                                        tmpRegion.AddPolyGon(tmpPoint);

                                    }

                                    break;
                                case 8: // Text
                                    tmpRegion.SetText(inetXML.Value);
                                    break;
                                case 9: // Color
                                    {  
                                        int tmp;
                                        if (Int32.TryParse(inetXML.Value, out tmp))
                                            localRegionControl.SetUserColor(tmp);
                                        
                                        break;
                                    }
                                case 10: // User_POS
                                    if (!lbReadUserTextPos)
                                    {
                                        char[] c = { ',' };
                                        String[] sTmp = inetXML.Value.Split(c);
                                        if (sTmp.Length == 4)
                                        {
                                            try
                                            {
                                                Point p = new Point(Int32.Parse(sTmp[0]), Int32.Parse(sTmp[1]));
                                                Size size = new Size(Int32.Parse(sTmp[2]), Int32.Parse(sTmp[3]));
                                                localRegionControl.SetUserTextPos(new Rectangle(p, size));
                                            }
                                            catch (Exception e)
                                            { }
                                        }
                                        lbReadUserTextPos = true;
                                    }
                                    break;
                                case 11: // Bold?

                                    break;
                                case 12: // TIME_POS
                                    {
                                        if (!lbReadTimeTextPos)
                                        {
                                            char[] c = { ',' };
                                            String[] sTmp = inetXML.Value.Split(c);
                                            if (sTmp.Length == 4)
                                            {
                                                try
                                                {
                                                    Point p = new Point(Int32.Parse(sTmp[0]), Int32.Parse(sTmp[1]));
                                                    Size size = new Size(Int32.Parse(sTmp[2]), Int32.Parse(sTmp[3]));
                                                    localRegionControl.SetTimeTextPos(new Rectangle(p, size));
                                                }
                                                catch (Exception e)
                                                { }
                                            }
                                            lbReadTimeTextPos = true;
                                        }
                                        break;
                                    }

                            }

                        }
                        
                        if (lbClassNameFound)
                        {
                            if (inetXML.Value.Equals(asClassName))
                            {
                                lbClassFound = true;
                                tmpRegion = new inetRegion();
                            }
                        }

                       
                        break;

                    case XmlNodeType.EndElement:

                        if (inetXML.Name == "iskin_row")
                        {
                            if (lbClassFound)
                                localRegionControl.SetRegion(tmpRegion);
                          
                            lbClassFound = false;
                            lbClassNameFound = false;
                            liWert = 0;

                        }
                        break;

                }
            }

            argRegion = localRegionControl;

            return true;
        }
    }
}
