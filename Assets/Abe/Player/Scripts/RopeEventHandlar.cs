using UnityEngine.EventSystems;

public interface RopeEventHandlar : IEventSystemHandler
{
    void OnRopeCreate (RopeSimulate rope);
    void OnRopeRelease(RopeSimulate rope);
}