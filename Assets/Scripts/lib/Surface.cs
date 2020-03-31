using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour {

	public new string name;
	public Transform anchor;

	public Vector3 offset;      // Вектор положения интерфейсной площадки внутри модели относительно ЕЕ ЦЕНТРА
                                // (НЕ НАЧАЛА ЛОКАЛЬНОЙ СИСТЕМЫ КООРДИНАТ).
                                // Задается разработчиком модуля. Рекомендуется использовать значения
                                // в диапазоне {-1; 1}. Единица равна половине размера соответствующего
                                // измерения модели. Например вектор {0, 1, 0} будет соответствовать самой высокой точке по Y,
                                // находящейся в центре плоскости XZ (для модуля прямоугольной формы - это верхняя плоскость).
    //[HideInInspector]
    public Vector3 realOffset;  // Рассчитанное программой смещение интерфейсной 
                                // площадки относительно начала локальной системы
                                // координат префаба модуля.

    private ConfigurableJoint joint;

	public Surface connectedSurface;

    private Module _module;
    public Module module {
        get {
            if (_module == null)
                _module = GetComponent<Module>();
            return _module;
        }
        set {
            _module = value;
        }
    }

    public Module Add(Module moduleType, Surface connectingSurface, float[] angles = null, float tilt = 0, Transform parent = null, int ID = -1) {
		
        Vector3 relativePosition, position;
		Quaternion relativeRotation, rotation;

		Quaternion tiltedRotation = Quaternion.AngleAxis (tilt, connectingSurface.realOffset);

        // Расчет относительного положения модуля относительно текущего.
        relativePosition = realOffset.AddVectorWithinDirection(connectingSurface.realOffset);
        // Расчитаем необходимый поворот модуля, чтобы интерфейсные площадки модулей
        // смотрели в сторону друг друга. Для этого перевернем один вектор.
        relativeRotation = Quaternion.FromToRotation(connectingSurface.realOffset * (-1), realOffset) * tiltedRotation;

		// Расчет ориентации нового модуля:
		rotation = anchor.rotation * relativeRotation;
        // Расчет положения нового модуля:
        // Изменение относительного положения модуля в соответствии с ориентацией.
        relativePosition = anchor.rotation * relativePosition;
        // Сложение относительного положения нового модуля и координат уже существующего.
        position = anchor.position + relativePosition - (relativeRotation * connectingSurface.anchor.localPosition);

        Module connectingModule = Modules.Create (moduleType, position, rotation, parent: parent, ID: ID);
		connectingModule.isPhysical = false;
		string surfaceName = connectingSurface.name;
		connectingSurface = connectingModule.GetSurfaceByName (surfaceName);

		if (connectingSurface == null) {
			Debug.LogError (string.Format ("Couldn't add module. Attaching surface of module {1} " +
				"with name {0} doesn't exist.", surfaceName, connectingModule.transform.name));
            Destroy(connectingModule.gameObject);
			return null;
		}

		int c = 0;
		if (angles != null && anchor != null) {
			// НЕПРАВИЛЬНЫЙ ИТТЕРАТОР:
			// ОШИБКА.
			foreach (Driver driver in connectingModule.GetComponents (typeof(Driver))) {
                if (c < angles.Length)
				    driver.RotateKinematic (connectingSurface.anchor, angles [c]);
				c++;
			}

		}

		Connect (connectingSurface);
		connectingModule.isPhysical = true;
		return connectingModule;
	}

	public void Connect(Surface toSurface) {

        if (connectedSurface != null)
        {
            Debug.LogError(string.Format("Couldn't connected modules. Surface {0} of module {1} " +
                                         "is already connected to module {2}. Use Disconnect() first.",
                                         name, module.id, connectedSurface.module.id));
            return;
        }

        if (toSurface.connectedSurface != null)
        {
            Debug.LogError(string.Format("Couldn't connected modules. Surface {0} of module {1} " +
                                         "is already connected to module {2}. Use Disconnect() first.",
                                         toSurface.name, toSurface.module.id, toSurface.connectedSurface.module.id));
            return;
        }

        joint = anchor.gameObject.AddComponent <ConfigurableJoint> ();
		joint.connectedBody = toSurface.anchor.GetComponent <Rigidbody> ();
		joint.enablePreprocessing = false;
        //joint.enableCollision = true;
		joint.projectionMode = JointProjectionMode.PositionAndRotation;
		joint.axis = Vector3.up;
		joint.secondaryAxis = Vector3.up;
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;

		/*FixedJoint joint;
		joint = anchor.gameObject.AddComponent <FixedJoint> ();
		joint.connectedBody = toSurface.anchor.GetComponent <Rigidbody> ();
		joint.enableCollision = false;*/

		connectedSurface = toSurface;
        toSurface.connectedSurface = this;
        toSurface.joint = joint;
	}

	public void Disconnect() {
        connectedSurface.connectedSurface = null;
		connectedSurface = null;
        Destroy(joint);
	}
}