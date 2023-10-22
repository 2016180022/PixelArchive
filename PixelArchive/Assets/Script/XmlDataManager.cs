using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XmlDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        createXML();
    }

    void createXML() {
        XmlDocument tempXml = new XmlDocument();
        //xml 기본 선언
        tempXml.AppendChild(tempXml.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        //루트 노드 생성
        XmlNode root = tempXml.CreateNode(XmlNodeType.Element, "CharacterInfo", string.Empty);
        tempXml.AppendChild(root);

        //자식 노드 생성
        XmlNode child = tempXml.CreateNode(XmlNodeType.Element, "Character", string.Empty);
        root.AppendChild(child);

        //자식 노드에 들어갈 속성 생성
        XmlElement name = tempXml.CreateElement("name");
        name.InnerText = "tempName";
        child.AppendChild(name);

        XmlElement lv = tempXml.CreateElement("lv");
        lv.InnerText = "5";
        child.AppendChild(lv);

        XmlElement coin = tempXml.CreateElement("coin");
        coin.InnerText = "10";
        child.AppendChild(coin);

        tempXml.Save("./Assets/GameData/CharacterInfo.xml");
    }
}
