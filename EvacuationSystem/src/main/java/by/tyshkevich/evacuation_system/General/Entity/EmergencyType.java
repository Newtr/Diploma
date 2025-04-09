package by.tyshkevich.evacuation_system.General.Entity;

import java.util.Arrays;

public enum EmergencyType
{
    FIRE_BUILDING(1),
    FIRE_FOREST(2),
    SMOKE(3),
    DROUGHT(4);

    private final int id;

    EmergencyType(int id) {
        this.id = id;
    }

    public int getId() {
        return id;
    }

    public static EmergencyType fromId(int id)
    {
        for (EmergencyType type : EmergencyType.values())
        {
            if (type.getId() == id)
            {
                return type;
            }
        }
        throw new IllegalArgumentException("Нет такого типа ЧС для id: " + id);
    }
}
