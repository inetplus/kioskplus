using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace kioskplus
{
    public class inetRegionCtrl
    {
        private int textUserColor = 0;
        private String backGround = "";
        private Boolean textUserBold = false;
        private Rectangle userPos;
        private Rectangle timePos;
        private ArrayList mRegions = new ArrayList();
      
        public int GetUserColor()
        {
            return textUserColor;
        }
        public void SetUserBold(Boolean abBold)
        {
            textUserBold = abBold;
        }

        public String GetBackGround()
        {
            return backGround;
        }
        public void SetBackGround(String aString)
        {
            backGround = aString;
        }

        public Boolean isUserBold()
        {
            return textUserBold;
        }

        public void SetUserColor(int aiColor)
        {
            textUserColor = aiColor;
        }

        public Rectangle GetUserTextPos()
        {
            return userPos;
        }
        public void SetUserTextPos(Rectangle aRect)
        {
            userPos = aRect;
        }

        public Rectangle GetTimeTextPos()
        {
            return timePos;
        }
        public void SetTimeTextPos(Rectangle aRect)
        {
            timePos = aRect;
        }

        public inetRegion GetRegion(Point point)
        {
           inetRegion regionTemp = null;

            foreach(object tmp in mRegions)
            {
                if (tmp.GetType() == typeof(inetRegion))
                {
                    regionTemp = (inetRegion)tmp;
                    if (regionTemp.isPointRgn(point))
                        return regionTemp;
                }
            }

            regionTemp = null;
            return regionTemp;
        }

        public int SetRegion(inetRegion argRegion)
        {
            return mRegions.Add(argRegion);
        }

    }

    public class inetRegion
    {
        private GraphicsPath graphics = null;
        private String name = "";
        private String onMausOver = "";
        private String onMausOut = "";
        private String text = "";

        public inetRegion()
        {
            graphics = new GraphicsPath();
        }

        public void AddPolyGon(Point[] points)
        {
            graphics.AddPolygon(points);
        }
        public void AddRectangle(Rectangle rect)
        {
            graphics.AddRectangle(rect);
        }
        public Boolean isPointRgn(Point point)
        {
            return graphics.IsVisible(point);
        }

        public void SetName(String aString)
        {
            name = aString;
        }

        public String GetName()
        {
            return name;
        }

        public void SetMausOver(String aString)
        {
            onMausOver = aString;
        }

        public String GetMausOver()
        {
            return onMausOver;
        }

        public void SetMausOut(String aString)
        {
            onMausOut = aString;
        }

        public String GetMausOut()
        {
            return onMausOut;
        }

        public void SetText(String aString)
        {
            text = aString;
        }

        public String GetText()
        {
            return text;
        }
    }

}
