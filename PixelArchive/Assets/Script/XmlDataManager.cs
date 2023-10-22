using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XmlDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // createXML();
        loadXml();
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
        XmlElement name = tempXml.CreateElement("Name");
        name.InnerText = "tempName";
        child.AppendChild(name);

        XmlElement lv = tempXml.CreateElement("Lv");
        lv.InnerText = "5";
        child.AppendChild(lv);

        XmlElement coin = tempXml.CreateElement("Coin");
        coin.InnerText = "10";
        child.AppendChild(coin);

        tempXml.Save("./Assets/Resources/CharacterInfo.xml");
    }

    void loadXml() {
        TextAsset textAsset = (TextAsset)Resources.Load("CharacterInfo");
        // if (textAsset == null) return;
        Debug.Log(textAsset);
        
        XmlDocument tempXml = new XmlDocument();
        tempXml.LoadXml(textAsset.text);
        // tempXml.LoadXml("./Assets/GameData/CharacterInfo.xml");

        XmlNodeList nodes = tempXml.SelectNodes("CharacterInfo/Character");

        foreach (XmlNode node in nodes) {
            Debug.Log("Name: " + node.SelectSingleNode("Name").InnerText);
            Debug.Log("Lv: " + node.SelectSingleNode("Lv").InnerText);
            Debug.Log("Coin: " + node.SelectSingleNode("Coin").InnerText);
        }
    }
}
