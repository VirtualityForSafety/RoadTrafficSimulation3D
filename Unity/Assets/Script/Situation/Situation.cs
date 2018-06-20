using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class DefaultCamera
{
    public int cameraMode;

    public float height;
}

public class Car
{
    public int ID;
    public float speed;
    public Car() { }
    public Car(int i, float s) { ID = i; speed = s; }
}

public class Pedestrian
{
    public float walkingSpeed;
}

public class Situation
{
    [XmlAttribute("name")]
    public string name;

    public bool accident;

    public bool walkingPedestrian;

    [XmlElement("DefaultCamera")]
    public DefaultCamera defaultCamera;
    //*
    [XmlArray("Human")]
    [XmlArrayItem("Pedestrian")]
    public List<Pedestrian> pedestrians = new List<Pedestrian>();

    [XmlArray("Vehicle")]
    [XmlArrayItem("Car")]
    public List<Car> cars = new List<Car>();
    //*/

    public static Situation load(string fileName)
    {
        var serializer = new XmlSerializer(typeof(Situation));
        var stream = new FileStream(fileName, FileMode.Open);
        Situation situationContainer = serializer.Deserialize(stream) as Situation;
        stream.Close();
        return situationContainer;
    }

    public void write(string fileName)
    {
        var serializer = new XmlSerializer(typeof(Situation));
        using (var stream = new FileStream(fileName, FileMode.Create))
        {
            var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8);
            serializer.Serialize(xmlWriter, this);
        }
    }
}