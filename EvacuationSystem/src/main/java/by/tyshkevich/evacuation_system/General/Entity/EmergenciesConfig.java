package by.tyshkevich.evacuation_system.General.Entity;

import org.aopalliance.reflect.Metadata;

import java.util.Map;

public class EmergenciesConfig
{
    private Map<String, Object> emergencies;

    private Metadata metadata;

    public static class Metadata {

        private String last_updated;
        private String data_source;
        private String version;
    }
}
