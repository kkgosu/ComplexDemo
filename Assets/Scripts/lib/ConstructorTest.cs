using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructorTest : MonoBehaviour {

    Ray ray;
    RaycastHit hit;

    Camera cam;
    GameObject canvas;

    GameObject constructorUI;

	// Use this for initialization
	void Start () {
        Module mod = Modules.Create(Modules.M3R, new Vector3(0, -1.3025f, 0), new Quaternion(), parent: transform);

        cam = GameObject.Find("Main Camera").GetComponent<Camera> ();

        if (GameObject.Find("Constructor UI") == null)
        {
            constructorUI = new GameObject();
            constructorUI.transform.parent = cam.transform;
            constructorUI.transform.name = "Constructor UI";
        }

        DefaultControls.Resources uiResources = new DefaultControls.Resources();
        var text = DefaultControls.CreateText(uiResources);
        text.transform.parent = canvas.gameObject.transform;
        text.GetComponent<Text>().text = "Hello?";
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                Debug.LogWarning(string.Format("Mouse over collider with name {0}.", hit.collider.name));
        }
	}
}

/* public class UIWindowSimple {

    public GameObject panel;

    private DefaultControls.Resources resources;
    private GameObject canvas;

    public Text AddText () {


    }

    public Button AddButton () {


    }

    public InputField AddInputField () {


    }

    public Scrollbar AddScrollbar () {


    }

    public GameObject AddToogle () {


    }

    public Dropdown AddDropdown () {


    }

    public Slider AddSlider () {



    }

    public UIWindowSimple (Canvas c) {
        resources = new DefaultControls.Resources();
        canvas = c.gameObject;

        resources.background = someBgSprite; 
        GameObject uiPanel = DefaultControls.CreatePanel(resources);
        uiPanel.transform.SetParent(canvas.transform, false);
    }

}

public class UIWindowTest : UIWindowSimple {




} */