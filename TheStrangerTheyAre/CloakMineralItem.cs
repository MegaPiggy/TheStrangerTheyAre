using NewHorizons.Components.Props;
using UnityEngine;

namespace TheStrangerTheyAre;

public class CloakMineralItem : NHItem
{
    public void OnValidate()
    {
        _type = TheStrangerTheyAre.CloakMineralItemType;
        DisplayName = "MineralDisplayName";
        Droppable = true;
        PickupAudio = AudioType.ToolItemScrollPickUp;
        DropAudio = AudioType.ToolItemScrollDrop;
        SocketAudio = AudioType.ToolItemScrollInsert;
        UnsocketAudio = AudioType.ToolItemScrollRemove;
        PickupFact = "ANGLERS_EYE_MINE_MINERAL";
    }

    public override void Awake()
    {
        OnValidate();
        base.Awake();
    }

    public void Start()
    {
        enabled = false;
    }

    public override string GetDisplayName()
    {
        if (_translatedName == null)
        {
            _translatedName = TheStrangerTheyAre.NewHorizonsAPI.GetTranslationForOtherText(DisplayName);
        }
        return _translatedName;
    }
}
