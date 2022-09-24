using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OfficeListCollection", order = 1)]
public class OfficeListCollection : ScriptableObject
{
    public InteriorItemType[] interiorItemTypes;
    public ExteriorItemType[] exteriorItemTypes;
}

public enum InteriorItemType
{
    Chair = 0,
    Table = 1,
    Couch = 2,
    Computer = 3,
    Lamp = 4,
    Paper = 5,
    Phone = 6,
    Printer = 7,
    Plant = 8
}

public enum ExteriorItemType
{
    NameLabel = 0,
    Sign = 1,
    Chair = 2,
    Table = 3,
    Couch = 4,
    Lamp = 5,
    Plant = 6
}
