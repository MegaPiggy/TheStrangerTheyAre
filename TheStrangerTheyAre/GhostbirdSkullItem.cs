using NewHorizons.Components.Props;
using UnityEngine;

namespace TheStrangerTheyAre;

public class GhostbirdSkullItem : NHItem
{
    private readonly Vector3 _defaultScale = Vector3.one;
    private readonly Vector3 _smallerScale = Vector3.one * 0.3f;

    public void OnValidate()
    {
        _type = TheStrangerTheyAre.GhostbirdSkullItemType;
        DisplayName = "SkullDisplayName";
        Droppable = true;
        PickupAudio = AudioType.ToolItemSharedStonePickUp;
        DropAudio = AudioType.ToolItemSharedStoneDrop;
        SocketAudio = AudioType.ToolItemSharedStoneInsert;
        UnsocketAudio = AudioType.ToolItemSharedStoneRemove;
        PickupFact = "QUANTUM_CORPSE_SKULL";
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

    public override void DropItem(Vector3 position, Vector3 normal, Transform parent, Sector sector, IItemDropTarget customDropTarget)
    {
        base.DropItem(position, normal, parent, sector, customDropTarget);
        transform.localScale = _defaultScale;
    }

    public override void PickUpItem(Transform holdTranform)
    {
        base.PickUpItem(holdTranform);
        transform.localScale = _smallerScale;
    }
}
