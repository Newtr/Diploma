package by.tyshkevich.evacuation_system.General.Services;

import by.tyshkevich.evacuation_system.General.Entity.EmergencyType;
import com.fasterxml.jackson.databind.ObjectMapper;
import jakarta.annotation.PostConstruct;
import org.springframework.stereotype.Service;
import com.fasterxml.jackson.databind.JsonNode;

import java.io.InputStream;

@Service
public class EmergencySimulator
{
    private JsonNode rootNode;

    @PostConstruct
    public void init()
    {
        try
        {
            ObjectMapper mapper = new ObjectMapper();
            InputStream is = getClass().getResourceAsStream("/config/emergencies.json");
            rootNode = mapper.readTree(is);
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    public String modelSituation(EmergencyType type)
    {
        String result = "";
        try
        {
            JsonNode emergenciesNode = rootNode.path("emergencies");
            switch (type)
            {
                case FIRE_BUILDING:
                    JsonNode building = emergenciesNode.path("fire").path("building");
                    result = formatEmergency(building);
                    break;
                case FIRE_FOREST:
                    JsonNode forest = emergenciesNode.path("fire").path("forest");
                    result = formatEmergency(forest);
                    break;
                case SMOKE:
                    JsonNode smoke = emergenciesNode.path("smoke");
                    result = formatEmergency(smoke);
                    break;
                case DROUGHT:
                    JsonNode drought = emergenciesNode.path("drought");
                    result = formatEmergency(drought);
                    break;
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();
            result = "Ошибка при моделировании ЧС.";
        }
        return result;
    }

    private String formatEmergency(JsonNode emergencyNode)
    {
        StringBuilder sb = new StringBuilder();
        sb.append("🔥 *").append(emergencyNode.path("title").asText()).append("*\n");
        sb.append(emergencyNode.path("description").asText()).append("\n\n");
        sb.append("Советы:\n");
        for (JsonNode advice : emergencyNode.path("advice"))
        {
            sb.append("• ").append(advice.asText()).append("\n");
        }
        return sb.toString();
    }
}
