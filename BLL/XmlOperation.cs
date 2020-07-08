using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace BLL
{
    class XmlOperation
    {
        XmlDocument xDoc;
        public XmlOperation()
        {
            xDoc = new XmlDocument();
            loadLog();
        }
        public void loadLog()
        {
            xDoc.Load(Application.StartupPath + @"\productionLog.xml");
        }
        //只有当天的记录，如果没有该用户，返回0，然后创建，如果有返回相应的值
        public int loadDataByUser(string user)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            XmlElement root = xDoc.DocumentElement;
            //判断有没有当天的数据
            XmlElement nodeTime = (XmlElement)root.SelectSingleNode("//time[@data='" + today + "']");
            if (nodeTime == null)  //如果没有则创建
            {
                //先把现有的time节点删掉
                XmlNode nodeOldTime = root.SelectSingleNode("//time");
                root.RemoveChild(nodeOldTime);
                //添加今天的节点
                nodeTime = xDoc.CreateElement("time");
                nodeTime.SetAttribute("data", today);
                nodeTime.SetAttribute("totalProduction", "0");
                //添加该员工的节点
                XmlElement userNode = xDoc.CreateElement("user");
                userNode.SetAttribute("id", user);
                userNode.InnerText = "0";
                nodeTime.AppendChild(userNode);
                root.AppendChild(nodeTime);
                return 0;
            }
            else  //如果具有当天的数据
            {
                XmlElement nodeUser = (XmlElement)root.SelectSingleNode("//user[@id='" + user + "']");
                if (nodeUser == null)  //如果没有该用户
                {
                    //为该用户创建数据
                    nodeUser = xDoc.CreateElement("user");
                    nodeUser.SetAttribute("id", user);
                    nodeUser.InnerText = "0";
                    nodeTime.AppendChild(nodeUser);
                    return 0;
                }
                else
                {
                    return int.Parse(nodeUser.InnerText);
                }
            }
        }

        public int getTodayProduction()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            XmlElement root = xDoc.DocumentElement;
            //判断有没有当天的数据
            XmlElement nodeTime = (XmlElement)root.SelectSingleNode("//time[@data='" + today + "']");
            if (nodeTime == null)  //如果没有则创建
            {
                //先把现有的time节点删掉
                XmlNode nodeOldTime = root.SelectSingleNode("//time");
                root.RemoveChild(nodeOldTime);
                //添加今天的节点
                nodeTime = xDoc.CreateElement("time");
                nodeTime.SetAttribute("data", today);
                nodeTime.SetAttribute("totalProduction", "0");
                root.AppendChild(nodeTime);
                return 0;
            }
            else  //如果具有当天的数据
            {
                return int.Parse(nodeTime.GetAttribute("totalProduction"));
            }
        }

        public void setDataByUser(string usr, int number)
        {
            XmlElement root = xDoc.DocumentElement;
            XmlElement userNode = (XmlElement)root.SelectSingleNode("//user[@id='" + usr + "']");
            userNode.InnerText = number.ToString();
            //更新总数
            XmlNodeList nodeLists = root.SelectNodes("//user");
            int totalProduction = 0;
            foreach (XmlNode node in nodeLists)
            {
                totalProduction += int.Parse(node.InnerText);
            }
            XmlElement nodeTime = (XmlElement)root.SelectSingleNode("//time");
            nodeTime.SetAttribute("totalProduction", totalProduction.ToString());
        }
        public void Save()
        {
            xDoc.Save(Application.StartupPath + @"\productionLog.xml");
        }
    }
}
