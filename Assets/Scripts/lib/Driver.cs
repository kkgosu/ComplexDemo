using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

/// <summary>
/// Rotation Driver.
/// TODO:
/// Нужно подумать над тем, как должны работать функции Rotate и как они работают.
/// На данный момент, они устанавливают двигатель в заданное положение,
/// однако аналогичная функция Rotate в Unity имеет накапливающий принцип действия.
/// Думаю, стоит переименовать их в Set, а Rotate использовать для увеличения / уменьшения текущего угла.
/// </summary>
public class Driver : MonoBehaviour {
    
    private float A = 0, w = 0, ph = 0, O = 0;

    COM_Controller comController;

    public new string name;

    public ConfigurableJoint joint; // Переменная, содержащая ссылку на компонент Joint.
    public float qMin = 0;		// Минимальное значение угла поворота.
	public float qMax = 0;		// Максимальное значение угла поворота.
	public float qStart = 15;	// Начальное значение угла поворота.

	public Vector3 axis = Vector3.zero;	// Ось поворота элементов.

	public float speed = 10; // Deg/sec.

	[HideInInspector]
	public bool busy = false;	// Переменная, которая содержит информацию 
								// о том, работает ли двигатель в момент времени.
	[HideInInspector]
	public bool instantiated = false;

	//private float qOffset = 0;	// Внутренняя переменная, содержащая разницу между реальным
								// и полным (видимым глазу) углами поворота.

	private Module _module;       // Переменная, содержащая ссылку на компонент Module.
    public Module module
    {
        get
        {
            if (_module == null)
                _module = GetComponent<Module>();
            return _module;
        }
        set
        {
            _module = value;
        }
    }

    private float _qValue;
	public float qValue {	// Переменная, содержащая полный угол поворота.
		get {
			return _qValue;
		}
		set {
			_qValue = value;
			joint.targetRotation = Quaternion.Euler (value * axis);
		}
	}


    public void SetCPG(float A, float w, float ph, float O)
    {
        this.A = A;
        this.w = w;
        this.ph = ph;
        this.O = O;
    }

    private void RotateImmideatly(float value) {
        joint.targetRotation = Quaternion.Euler(new Vector3(value, 0, 0));
    }

	private List<Transform> groupA, groupB;

	public void BuildKinematicGroups () {
		// Инициализация двух групп объектов:
		groupA = new List<Transform> ();
		groupB = new List<Transform> ();

		List <Transform> rigidBodyTransforms = new List <Transform> ();	// Список всех тел внутри префаба модуля, необходим для особой проверки.
		List <Joint> joints = new List<Joint> ();						// Список всех соединений внутри модуля.

		// Следующая проверка дорогая, но она нужна для того,
		// чтобы убедиться в том, что все соединения находятся внутри модуля.
		// Можно было не утруждаться и просто засунуть все Joint'ы в joints.
		// * однако, за этой дополнительной проверкой стоит история: *
		foreach (Rigidbody rb in GetComponentsInChildren <Rigidbody>()) {
			rigidBodyTransforms.Add (rb.transform);
		}
		foreach (Joint j in GetComponentsInChildren(typeof(Joint))) {
			if (rigidBodyTransforms.Contains (j.transform) &&
				rigidBodyTransforms.Contains (j.connectedBody.transform))
				joints.Add (j);
		}

		// Размыкаем цепь из физических тел в том соединении, где находится двигатель.
		joints.Remove (joint);

		// Если модуль содержит всего одно соединение, создание очереди не имеет смысла.
		// Запишем составные части соединения в соответствующие группы.
		if (joints.Count == 0) {
			groupA.Add (joint.transform);
			groupB.Add (joint.connectedBody.transform);
			return;
		}
		
		// Объявляем очередь и начинаем иттерироваться
		// по всем физическим телам внутри модуля через их соединения.
		Queue <Transform> queue = new Queue<Transform> ();
		Transform t, connectedTransform = joint.transform;

		//GroupA:
		queue.Enqueue (connectedTransform);
		groupA.Add (connectedTransform);
		do {
			t = queue.Dequeue ();
			foreach (Joint j in joints) {
				if (j.transform == t)
					connectedTransform = j.connectedBody.transform;
				else if (j.connectedBody.transform == t)
					connectedTransform = j.transform;
				if (groupA.Contains (connectedTransform))
					continue;
				queue.Enqueue (connectedTransform);
				groupA.Add (connectedTransform);
			}
		} while (queue.Count > 0);

		//GroupB:
		connectedTransform = joint.connectedBody.transform;
		queue.Enqueue (connectedTransform);
		groupB.Add (connectedTransform);
		do {
			t = queue.Dequeue ();
			foreach (Joint j in joints) {
				if (j.transform == t)
					connectedTransform = j.connectedBody.transform;
				else if (j.connectedBody.transform == t)
					connectedTransform = j.transform;
				if (groupB.Contains (connectedTransform))
					continue;
				queue.Enqueue (connectedTransform);
				groupB.Add (connectedTransform);
			}
		} while (queue.Count > 0);

		// Функция дорогая для многократного обращения в рантайме, поэтому выполняется один раз 
		// при первом обращении к модулю этого типа.
		// Если получится, можно попробовать сделать ее более легковесной. Не смог придумать как.
		// Для очень сложных модулей можно переопределять класс двигателя и вместо автоматики
		// писать функции для конкретной структуры, самостоятельно определяя группы объектов.
		// 
		// Говоря об автоматике, можно было бы еще ввести проверку на то, что группы идентичны,
		// а значит где-то еще есть соединение, однако, при этом модуль и физически не будет работать,
		// поэтому тратить на это время и делать функцию еще более дорогой не хотелось. А еще - лень.
	}


	// ===
	// Следующие функции желательно не вызывать сразу, как только модуль добавляется на сцену.
	// Подождите хотя бы один кадр. Проблема в том, что переменная module подхватывается в
	// функции Start(), которая срабатывает не сразу при добавлении модуля на сцену.
	// Пример проблемы: вызываем функцию из OnValidate() скрипта, прикрепленного к модулю,
	// без каких-либо проверок -> Unity падает. Дело в том, что OnValidate() срабатывает сразу,
	// как только подгружается инспектор.
	// На ум сразу приходит костыль: сделать module public'ом и добавлять через инспектор. Не надо так.
	// ===

	/// <summary>
	/// Rotate the specified value.
	/// </summary>
	/// <param name="value">Value.</param>
	public void Rotate (float value, float time = -1) {
		if (!busy) {
			if (module.isPhysical) {
				busy = true;
                StartCoroutine (RotatePhysically (Mathf.Clamp (qValue + value, qMin, qMax), time: time));
                print("qValue + value = " + (qValue + value));

                // ПОПРАВИТЬ:
                //comController = module.transform.parent.GetComponent<COM_Controller>();
                comController = module.transform.GetComponentInParent<COM_Controller>();
                if (comController != null)
                    comController.SetAngle(module.id, qValue + value, this);
            } else
				Debug.LogError (string.Format ("Cannot rotate drive {0}. " +
					"Physics disabled for Module {1}.", joint.transform.name, transform.name));
		} else
			Debug.LogError (string.Format ("Cannot rotate drive {0}. " +
				"Module {1} is already busy.", joint.transform.name, transform.name));
	}

    public void Set(float value, float time = -1)
    {
        if (!busy)
        {
            if (module.isPhysical)
            {
                busy = true;
                StartCoroutine(RotatePhysically(Mathf.Clamp(value, qMin, qMax), time: time));

                // ПОПРАВИТЬ:
                //comController = module.transform.parent.GetComponent<COM_Controller>();
                comController = module.transform.GetComponentInParent<COM_Controller>();
                if (comController != null)
                    comController.SetAngle(module.id, value, this);

            }
            else
                Debug.LogError(string.Format("Cannot set drive {0}. " +
                    "Physics disabled for Module {1}.", joint.transform.name, transform.name));
        }
        else
            Debug.LogError(string.Format("Cannot set drive {0}. " +
                "Module {1} is already busy.", joint.transform.name, transform.name));
    }

    /// <summary>
    /// Rotates the kinematic.
    /// </summary>
    /// <param name="basis">Basis.</param>
    /// <param name="value">Value.</param>
    /// <param name="time">Time.</param>
    public void RotateKinematic (Transform basis, float value, float time = 0) {
		// Модуль не выполняет никакую другую работу:
		if (!busy) {
			// Физика модуля отключена:
			if (!module.isPhysical) {
				if (basis == null)
					basis = groupA [0];
				// Выбор группы элементов для перемещения.
				if (groupA.Contains (basis)) {
					// Если время поворота равно нулю, выполнить движение моментально.
					if (time == 0)
						SetRotatonKinematic (groupB, value);
					// Иначе, начать выполнение корутины.
					else if (time > 0) {
						busy = true;
						StartCoroutine (RotateKinematically (groupB, value, time));
					} else
						Debug.LogError (string.Format ("Cannot rotate drive {0} of Module {1}, time cannot " +
							"be less than 0.", joint.transform.name, transform.name));
				} else if (groupB.Contains (basis)) {
					// GroupA всегда содержит Transform, на котором находится двигатель, а значит по умолчанию
					// является основанием. Если же за основание принять GroupB, вращение следует осуществлять
					// в другом направлении.
					if (time == 0)
						SetRotatonKinematic (groupA, value, inversed: true);
					else if (time > 0) {
						busy = true;
						StartCoroutine (RotateKinematically (groupA, value, time, inversed: true));
					} else
						Debug.LogError (string.Format ("Cannot rotate drive {0} of Module {1}, time cannot " +
							"be less than 0.", joint.transform.name, transform.name));
				} else
					Debug.LogError (string.Format ("Cannot rotate drive {0} of Module {1}, couldn't find " +
						"basis Transform with name {2}.", joint.transform.name, transform.name, basis.name));
			} else
				Debug.LogError (string.Format ("Cannot rotate drive {0}. Physics enabled for Module {1}, " +
					"but tried to rotate kinematically.", joint.transform.name, transform.name));
		} else
			Debug.LogError (string.Format ("Cannot rotate drive {0} kinematically. Module {1} is already " +
				"busy.", joint.transform.name, transform.name));
	}

	// ===
	// Следующие корутины содержат логику движения во времени.
	// ===

	IEnumerator RotatePhysically (float value, float time = -1, float speedModifier = 1) {
		float elapsedTime = 0;
		Quaternion fromRotation = joint.targetRotation;
        Quaternion toRotation = Quaternion.Euler(value * axis);

        float totalTime;

        if (time >= 0)
            totalTime = time;
        else
		    totalTime = (Mathf.Abs (value - qValue) / speed) * speedModifier;

        if (totalTime <= float.Epsilon)
            Debug.LogWarning("Time for driver movement is around 0. It's HIGHLY RECOMMENDED " +
                             "to use not Physical but Kinematic Rotation for immediate movements.");

		while(elapsedTime < totalTime)
		{
			joint.targetRotation = Quaternion.Lerp (fromRotation, toRotation, (elapsedTime / totalTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		qValue = value;
		busy = false;
	}

	IEnumerator RotateKinematically (List<Transform> ts, float value, float time, bool inversed = false) {
		float startAngle = qValue;
		float elapsedTime = 0;

		while (elapsedTime < time)
		{
			SetRotatonKinematic (ts, Mathf.Lerp(startAngle, value, (elapsedTime / time)));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		SetRotatonKinematic (ts, value, inversed);
		busy = false;
	}

	private void SetRotatonKinematic (List<Transform> ts, float value, bool inversed = false) {
		foreach (Transform t in ts) {
			t.RotateAround (joint.transform.position, (transform.rotation * new Vector3 (0, 0, 1)), (inversed ? value * (-1) : value) - qValue);
		}
		qValue = value;
	}

	// Инициализация компонента:

	void Start () {
		//qValue = qStart;
		instantiated = true;
	}

	void FixedUpdate () {
        //RotateImmideatly(A * Mathf.Sin(w * Time.fixedTime + ph) + O);
    }
}