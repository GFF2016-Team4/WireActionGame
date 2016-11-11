using UnityEngine.EventSystems;

public interface RopeCreateHandlar : IEventSystemHandler
{
    void OnRopeCreate(RopeSimulate rope);
}
