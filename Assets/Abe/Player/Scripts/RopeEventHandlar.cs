using UnityEngine.EventSystems;

public interface RopeEventHandlar : IEventSystemHandler
{
    void OnNormalRopeCreate (NormalRopeSimulate rope);
    void OnNormalRopeRelease(NormalRopeSimulate rope);
}