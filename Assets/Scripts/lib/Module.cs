using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {
    
	public int id;
	public Color32 color;
	public Vector3 size;

    public Vector3 position
    {
        get {
            return GetPosition();
        }
    }

    public GameObject moduleObject;

    public Vector3 centerOffset;    // transform.GetCenterOffset();
                                    // Вектор смещения центра префаба от начала его локальной системы координат,
                                    // в которой, согласно документации к данным средствам моделирования, должны
                                    // находиться все оси вращения подвижных элементов модуля.

    bool _isPhysical = true;
    public bool isPhysical
    {
        set
        {
            _isPhysical = value;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                rb.isKinematic = !value;
        }
        get
        {
            return _isPhysical;
        }
    }


    public Dictionary<string, Surface> surfaces;

    public Dictionary<string, Driver> drivers;

    // Calculate Size && Center offset:
    public void CalculateBounds()
    {
        size = transform.GetSize();
        centerOffset = transform.GetLocalCenter();
    }

    // Calculate Local Offsets:
    public void CalculateLocalOffsets()
    {
        //Dictionary<Transform, Vector3> transformObjects = new Dictionary<Transform, Vector3>();
        foreach (Surface surface in GetComponentsInChildren(typeof(Surface)))
        {
            /*if (!transformObjects.ContainsKey(surface.anchor))
                transformObjects.Add(surface.anchor, surface.anchor.GetSize());
            surface.anchorOffset = Vector3.Scale(transformObjects[surface.anchor], (surface.offset / 2));*/
            // Пережитки прошлого, ХОТЯ когда-нибудь могут пригодиться. Для чего-нибудь.

            surface.realOffset = Vector3.Scale(size, (surface.offset / 2)) + Vector3.Project(centerOffset, surface.offset);
        }
    }

    public void Prepare()
    {
        CalculateBounds();
        CalculateLocalOffsets();
        numberOfDrivers = 0;
        drivers = new Dictionary<string, Driver>();
        surfaces = new Dictionary<string, Surface>();
        foreach (Driver d in GetComponentsInChildren(typeof(Driver)))
        {
            print(d.name);
            drivers.Add(d.name, d);
            d.module = this;
            d.BuildKinematicGroups();
            numberOfDrivers++;
        }
        foreach (Surface s in GetComponentsInChildren(typeof(Surface)))
        {
            surfaces.Add(s.name, s);
            s.module = this;
        }
        foreach (Rigidbody rb in GetComponentsInChildren(typeof(Rigidbody))) {
            rb.maxDepenetrationVelocity /= 100;
            rb.mass = 2;
            //print(rb.maxDepenetrationVelocity);
        }
        initialised = true;
    }

    public Driver GetDriverByName(string driverName)
    {
        foreach (Driver drv in GetComponentsInChildren(typeof(Driver)))
        {
            if (drv.name.ToLower() == driverName.ToLower())
                return drv;
        }
        Debug.LogError(string.Format("Couldn't find driver of module {1} with name {0}.", driverName, transform.name));
        return null;
    }

    public Surface GetSurfaceByName(string surfaceName)
    {
        foreach (Surface surface in GetComponentsInChildren(typeof(Surface)))
        {
            if (surface.name.ToLower() == surfaceName.ToLower())
                return surface;
        }
        Debug.LogError(string.Format("Couldn't find surface of module {1} with name {0}.", surfaceName, transform.name));
        return null;
    }

    private Vector3 GetPosition() {
        Vector3 pos = Vector3.zero;
        int count = 0;
        foreach (Rigidbody rb in GetComponentsInChildren(typeof(Rigidbody)))
        {
            pos += rb.transform.position;
            count++;
        }
        if (count > 0)
            pos /= count;
        else
            Debug.LogError(string.Format("Can't locate module {0}. Module doesn't have any active Rigidbodies.", id));
        return pos;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        if (!initialised)
            Prepare();
    }

    // To be revised:
    //
    //

    public int numberOfDrivers { get; private set;}
	public float x=0, y=0, z=0;
	public float totalDistanceX, totalDistanceY, totalDistanceZ;
	private Vector3 currentposition, prevposition;
	public bool initialised = false;

    /*public class ModulePosition {
        public Vector3 startPosition;
        public float distance, path;
        public Vector3 coordinates;
        public Vector3 coordinatesInWorldSpace;
        public List<Vector3> checkpoints;

        public ModulePosition (Vector3 startPos, ModulePosition savedPosition = null) {
            
        }
    }

    public ModulePosition position;*/
    private List<Sensor> sensors;


    void FixedUpdate()
    {
        /*
        if (initialised)
        {
            currentposition = surfaces.bottom.anchor.transform.position;
            if (currentposition.x > prevposition.x)
                x += Vector3.Distance(currentposition, prevposition);
            else if (currentposition.x < prevposition.x)
                x -= Vector3.Distance(currentposition, prevposition);

            if (currentposition.y > prevposition.y)
                y += Vector3.Distance(currentposition, prevposition);
            else if (currentposition.y < prevposition.y)
                y -= Vector3.Distance(currentposition, prevposition);

            if (currentposition.z > prevposition.z)
                z += Vector3.Distance(currentposition, prevposition);
            else if (currentposition.z < prevposition.z)
                z -= Vector3.Distance(currentposition, prevposition);

            prevposition = currentposition;
        }
        */
    }

    //
    //
    //
}