using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicWalkerCfg : MonoBehaviour
{
    private readonly float moduleLength = 0.2766909f;
    private float z = 0.14f;

    public readonly string SOFT_CLAY = "soft clay";
    public readonly string MEDIUM_CLAY = "medium clay";
    public readonly string HARD_CLAY = "hard clay";
    public readonly string SAND = "sand";
    public readonly string SILT = "silt";
    public readonly string LOOSE_SILT = "loose silt";
    public readonly string HEAVY_SILT = "heavy silt";

    private Dictionary<string, GroundParams> groundParams;

    public int CalculateLegNumber(string type, float q, float maxDelta)
    {
        return Mathf.CeilToInt(DeltaZ(q) / Pressure(type, maxDelta));
    }

    public int CalculateModuleQuantity(float height)
    {
        int modlesHeight;
        int quantity = 3;
        for (int i = 3; i <= 10; i++)
        {
            modlesHeight = Mathf.CeilToInt(i * moduleLength - 2 * moduleLength);
            quantity = i;
            if (modlesHeight >= height)
            {
                break;
            }
        }

        return quantity;
    }

    private float Pressure(string type, float maxDelta)
    {
        float radius = moduleLength / 2f;
        float numerator = 2 * radius * groundParams[type].psi * maxDelta;
        float divider = 1 - Mathf.Pow(groundParams[type].mu, 2);

        return numerator / divider;
    }

    private float DeltaZ(float q)
    {
        float radius = moduleLength / 2f;
        float Nw = 1 / (Mathf.PI * Mathf.Pow(1 + 2 * Mathf.Pow(radius / z, 2), 1.5f));

        return (q / Mathf.Pow(z, 2)) * Nw;
    }

    private class GroundParams
    {
        public int psi { get; }
        public float mu { get; }
        public GroundParams(int psi, float mu)
        {
            this.psi = psi;
            this.mu = mu;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        groundParams = new Dictionary<string, GroundParams>
        {
            { SOFT_CLAY, new GroundParams(2000, 0.45f) },
            { MEDIUM_CLAY, new GroundParams(4800, 0.35f) },
            { HARD_CLAY, new GroundParams(10600, 0.20f) },
            { SAND, new GroundParams(15600, 0.25f) },
            { SILT, new GroundParams(2000, 0.30f) },
            { LOOSE_SILT, new GroundParams(2500, 0.30f) },
            { HEAVY_SILT, new GroundParams(9500, 0.35f) }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
