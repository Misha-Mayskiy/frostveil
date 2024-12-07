using UnityEngine;

public class AirPurifier : Building
{
    private float purificationRate = 0.3f; // Скорость очистки воздуха (30% в зоне 20)
    private float purificationRadius = 20f; // Радиус очистки

    protected override void Start()
    {
        base.Start();
        category = "Specialized";
        workersRequired = 0;
        // Логика инициализации здания очистки
        ReducePollutionRateInRadius();
    }

    protected override void Update()
    {
        base.Update();
        // Логика обновления здания очистки
    }

    public float GetPurificationRate()
    {
        return purificationRate;
    }

    public float GetPurificationRadius()
    {
        return purificationRadius;
    }

    private void ReducePollutionRateInRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, purificationRadius);
        foreach (var collider in colliders)
        {
            Building building = collider.GetComponent<Building>();
            if (building != null && (building is ThermalDrill || building is Mine || building is ProcessingBuilding))
            {
                if (building.GetPollutionRate() > purificationRate)
                {
                    building.ReducePollutionRate(purificationRate);
                }
            }
        }
    }

    public override float GetPollutionCount()
    {
        return 0f; // Здание очистки не загрязняет
    }
}
