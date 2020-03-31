using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHelper : MonoBehaviour {


    public static Text AddText (
        string text,
        GameObject parent = null,
        string name = "Текст"
    ) {
        if (parent == null)
            parent = GetCanvas ();
        DefaultControls.Resources resources = new DefaultControls.Resources();
        GameObject textObject = DefaultControls.CreateText(resources);
        textObject.transform.name = name;
        textObject.transform.SetParent(parent.transform, false);
        textObject.GetComponent<Text>().text = text;
        return textObject.GetComponent<Text>();
    }

    public static InputField AddInputField (
        string placeholder = "Введите текст...",
        string defaultText = "",
        GameObject parent = null,
        string name = "Текстовое поле",
        int characterLimit = 0,
        InputField.CharacterValidation characterValidation = InputField.CharacterValidation.None
    ) {
        if (parent == null)
            parent = GetCanvas();
        DefaultControls.Resources resources = new DefaultControls.Resources();
        //Set the InputField Background Image someBgSprite;
        //resources.inputField = someBgSprite;
        GameObject ifObject = DefaultControls.CreateInputField(resources);
        ifObject.transform.name = name;
        ifObject.transform.SetParent(parent.transform, false);
        ifObject.transform.GetChild(0).GetComponent<Text>().font =
                    (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        ifObject.transform.GetChild(0).GetComponent<Text>().text = placeholder;
        ifObject.GetComponent<InputField>().text = defaultText;
        ifObject.GetComponent<InputField>().characterLimit = characterLimit;
        ifObject.GetComponent<InputField>().characterValidation = characterValidation;
        return ifObject.GetComponent<InputField>();
    }

    public static Button AddButton (
        string buttonText,
        GameObject parent = null,
        string name = "Кнопка"
    ) {
        if (parent == null)
            parent = GetCanvas();
        DefaultControls.Resources resources = new DefaultControls.Resources();
        //resources.standard = Resources.Load<Sprite>("Sprites/UI Pack by Kenney/blue_button00");
        GameObject bObject = DefaultControls.CreateButton(resources);
        bObject.transform.name = name;
        bObject.transform.SetParent(parent.transform, false);
        bObject.GetComponentInChildren<Text>().text = buttonText;
        return bObject.GetComponent<Button>();
    }

    public static Dropdown AddDropdown (
        List<string> options,
        GameObject parent = null,
        string name = "Выпадающий список"
    ) {
        if (parent == null)
            parent = GetCanvas();
        DefaultControls.Resources resources = new DefaultControls.Resources();
        //Set the Dropdown Background and Handle Image someBgSprite;
        //resources.standard = Resources.Load<Sprite>("Sprites/UI Pack by Kenney/blue_button00");
        //Set the Dropdown Scrollbar Background Image someScrollbarSprite;
        //resources.background = Resources.Load<Sprite>("Sprites/UI Pack by Kenney/blue_button01");
        //Set the Dropdown Image someDropDownSprite;
        //resources.dropdown = Resources.Load<Sprite>("Sprites/UI Pack by Kenney/blue_boxTick");
        //Set the Dropdown Image someCheckmarkSprite;
        //resources.checkmark = Resources.Load<Sprite>("Sprites/UI Pack by Kenney/blue_button02");
        //Set the Dropdown Viewport Mask Image someMaskSprite;
        //resources.mask = Resources.Load<Sprite>("Sprites/UI Pack by Kenney/blue_button00");
        GameObject ddObject = DefaultControls.CreateDropdown(resources);
        ddObject.transform.SetParent(parent.transform, false);
        ddObject.GetComponent<Dropdown>().ClearOptions();
        ddObject.GetComponent<Dropdown>().AddOptions(options);
        return ddObject.GetComponent<Dropdown>();
    }

    public static Slider AddSlider () {
        return null;
    }

    public static Toggle AddToggle () {
        return null;
    }

    /*
    public static GameObject CreateImage(Resources resources);
    public static GameObject CreateRawImage(Resources resources);
    public static GameObject CreateScrollbar(Resources resources);
    public static GameObject CreateScrollView(Resources resources);
    public static GameObject CreateText(Resources resources);
    public static Button AddButtonWithImage (bool framed = true, sprite image, string text = "", Vector2 textPosition = new Vector2 (0, 1));*/

    /// <summary>
    /// Получаем Canvas (холст) по тегу "UI". Данный компонент необходим для
    /// отрисовки интерфейса. Если Canvas не найден, создадим его и EventSystem.
    /// </summary>
    /// <returns>GameObject, к которому подключен компонент Canvas.</returns>
    private static GameObject GetCanvas () {
        GameObject canvas = GameObject.FindWithTag("UI");
        if (canvas == null) {
            canvas = new GameObject()
            {
                name = "UI",
                tag = "UI"
            };
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            if (FindObjectOfType<EventSystem>() == null) {
                GameObject es = new GameObject("EventSystem", typeof(EventSystem));
                es.AddComponent<StandaloneInputModule>();
                es.AddComponent<BaseInput>();
            }
        }
        return canvas;
    }

    void Start () {
        //AddText("Hello?");
        //AddButton(name: "Сподобайка");
        //AddDropdown(new string[] {"--", "PORT3"});
        /*
        Module mod = Modules.Create(Modules.M3R, new Vector3 (0, 0, 0), new Quaternion (), ID: 0);
        foreach (MeshRenderer mr in mod.GetComponentsInChildren<MeshRenderer>()) {
            mr.materials[0].color = new Color32(60, 120, 120, 255);
        }*/
    }
}
