using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;

[XmlRoot("Situation")]
public class SituationBuilder : MonoBehaviour {
    public Toggle toggleAccident;
    public Toggle toggleWalking;
    public InputField inputWalking;
    public InputField inputCar;
    public Dropdown dropCameraMode;
    public InputField inputNumber;
    public InputField inputOther;
    public InputField inputSituationName;
    public InputField inputCameraHeight;
    public Text textIssuedSituation;
    public GameObject globalCamera;
    public Slider cameraHeightSlider;
    public Camera observationCamera;

    private string issuedSituationFileName = "IssuedSituation.txt";

    public Situation situationContainer;

    private string fileSavePath;

	// Use this for initialization
	void Start () {
        fileSavePath = Application.dataPath + "/../../Testset/Situation/";
        string issuedSituationFilePath = System.IO.File.ReadAllText(fileSavePath + issuedSituationFileName);
        textIssuedSituation.text = issuedSituationFilePath;
        situationContainer = Situation.load(fileSavePath + issuedSituationFilePath);
        if (situationContainer==null)
        {
            textIssuedSituation.text = "File loading error : please check " + fileSavePath;
            Debug.Log("ERROR : error with XML file!");
        }
        adjustSituationParameter();
    }

    private void Update()
    {
        adjustScene();
    }

    public void adjustValueFromSlider(float value)
    {
        situationContainer.defaultCamera.height = value;
    }

    void adjustSituationParameter()
    {
        adjustParam2UI();
        adjustParam2Scene();
    }

    void adjustParam2UI()
    {
        toggleAccident.isOn = situationContainer.accident;
        toggleWalking.isOn = situationContainer.walkingPedestrian;
        inputWalking.text = situationContainer.pedestrians[0].walkingSpeed.ToString();
        dropCameraMode.value = situationContainer.defaultCamera.cameraMode;
        inputNumber.text = (situationContainer.cars.Count - 1).ToString();
        inputCar.text = situationContainer.cars[0].speed.ToString();
        inputCameraHeight.text = situationContainer.defaultCamera.height.ToString();
        if ((situationContainer.cars.Count - 1) > 0)
        {
            inputOther.text = situationContainer.cars[1].speed.ToString();
        }
        inputSituationName.text = situationContainer.name;
    }

    void adjustParam2Scene()
    {
        cameraHeightSlider.value = situationContainer.defaultCamera.height;
        //rccCamera.ChangeCamera(situationContainer.defaultCamera.cameraMode);
        //rccCamera.initialCameraMode = situationContainer.defaultCamera.cameraMode;
    }

    void adjustScene()
    {
        globalCamera.transform.position = new Vector3(globalCamera.transform.position.x, situationContainer.defaultCamera.height, globalCamera.transform.position.z);
    }

    void obtainSituationParameter()
    {
        situationContainer.accident = toggleAccident.isOn;
        situationContainer.walkingPedestrian = toggleWalking.isOn;
        situationContainer.pedestrians[0].walkingSpeed = float.Parse(inputWalking.text);
        situationContainer.defaultCamera.cameraMode = dropCameraMode.value;
        situationContainer.cars.Clear();
        for (int i = 0; int.Parse(inputNumber.text)+1 > i ; i++)
        {
            if (i == 0)
                situationContainer.cars.Add(new Car(i, float.Parse(inputCar.text)));
            else
                situationContainer.cars.Add(new Car(i, float.Parse(inputOther.text)));
        }
        situationContainer.defaultCamera.height = float.Parse(inputCameraHeight.text);
        situationContainer.name = inputSituationName.text;
    }

    public void build()
    {
        obtainSituationParameter();

        string givenFileName = "SITU_" + inputSituationName.text + ".xml";
        situationContainer.name = inputSituationName.text;
        situationContainer.write(fileSavePath + givenFileName);
        Debug.Log(givenFileName + " is successfully generated!");
    }

    public void buildAndLoad()
    {
        build();
        changeIssuedSituation("SITU_" + inputSituationName.text + ".xml");
        Application.LoadLevel(0);
    }

    private void changeIssuedSituation(string givenFileName)
    {
        StreamWriter writer = new StreamWriter(fileSavePath + issuedSituationFileName, false);
        writer.Write(givenFileName);
        writer.Close();
    }
}
