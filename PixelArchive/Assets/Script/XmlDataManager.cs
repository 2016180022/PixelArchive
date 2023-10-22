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
        // saveOverlapXml();
    }

    void createXML() {
        XmlDocument tempXml = new XmlDocument();
        //xml 기본 선언
        tempXml.AppendChild(tempXml.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        //루트 노드 생성
        XmlNode root = tempXml.CreateNode(XmlNodeType.Element, "CharacterInfo", string.Empty);
        tempXml.AppendChild(root);

        for (int i = 0; i < 3; i++) {
            //자식 노드 생성
            XmlNode child = tempXml.CreateNode(XmlNodeType.Element, "Character", string.Empty);
            root.AppendChild(child);

            //자식 노드에 들어갈 속성 생성
            XmlElement name = tempXml.CreateElement("EnemyName");
            name.InnerText = "tempName";
            child.AppendChild(name);

            XmlElement type = tempXml.CreateElement("EnemyType");
            type.InnerText = "tempType";
            child.AppendChild(type);

            XmlElement lv = tempXml.CreateElement("Health");
            lv.InnerText = "1";
            child.AppendChild(lv);

            XmlElement coin = tempXml.CreateElement("Speed");
            coin.InnerText = "1";
            child.AppendChild(coin);

            XmlElement range = tempXml.CreateElement("Range");
            range.InnerText = "1";
            child.AppendChild(range);
        }
        
        tempXml.Save("./Assets/Resources/CharacterInfo.xml");
    }

    void loadXml() {
        TextAsset textAsset = (TextAsset)Resources.Load("CharacterInfo");
        // if (textAsset == null) return;
        // Debug.Log(textAsset);
        
        XmlDocument tempXml = new XmlDocument();
        tempXml.LoadXml(textAsset.text);

        XmlNodeList nodes = tempXml.SelectNodes("CharacterInfo/Character");

        foreach (XmlNode node in nodes) {
            Debug.Log("EnemyName: " + node.SelectSingleNode("EnemyName").InnerText);
            Debug.Log("EnemyType: " + node.SelectSingleNode("EnemyType").InnerText);
            Debug.Log("Health: " + node.SelectSingleNode("Health").InnerText);
            Debug.Log("Speed: " + node.SelectSingleNode("Speed").InnerText);
            Debug.Log("Range: " + node.SelectSingleNode("Range").InnerText);
        }
        
    }

    void saveOverlapXml() {
        TextAsset textAsset = (TextAsset)Resources.Load("CharacterInfo");
        XmlDocument tempXml = new XmlDocument();
        tempXml.LoadXml(textAsset.text);

        XmlNodeList nodes = tempXml.SelectNodes("CharacterInfo/Character");
        XmlNode tempChracter = nodes[0];

        tempChracter.SelectSingleNode("Name").InnerText = "temp";
        tempChracter.SelectSingleNode("Lv").InnerText = "1";
        tempChracter.SelectSingleNode("Coin").InnerText = "1";

        tempXml.Save("./Assets/Resources/CharacterInfo.xml");
    }
}
