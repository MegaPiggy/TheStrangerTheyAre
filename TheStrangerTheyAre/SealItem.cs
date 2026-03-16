using NewHorizons.Components.Props;
using System;
using UnityEngine;

namespace TheStrangerTheyAre;

public class SealItem : NHItem
{
    [SerializeField]
    public SealID sealID;

    private string _sealName;
    private string _sealFactID;

    public void OnValidate()
    {
        _type = TheStrangerTheyAre.SealItemType;
        _sealName = GetSealName(sealID);
        _sealFactID = GetSealFactID(sealID);
        DisplayName = "SealDisplayName";
        Droppable = true;
        PickupAudio = AudioType.ToolItemSharedStonePickUp;
        DropAudio = AudioType.ToolItemSharedStoneDrop;
        SocketAudio = AudioType.ToolItemSharedStoneInsert;
        UnsocketAudio = AudioType.ToolItemSharedStoneRemove;
        PickupFact = _sealFactID;
    }

    public override void Awake()
    {
        OnValidate();
        base.Awake();
    }

    public override string GetDisplayName()
    {
        if (_translatedName == null)
        {
            _translatedName = string.Format(TheStrangerTheyAre.NewHorizonsAPI.GetTranslationForOtherText(DisplayName), TheStrangerTheyAre.NewHorizonsAPI.GetTranslationForUI(_sealName));
        }
        return _translatedName;
    }

    public static string GetSealName(SealID id)
    {
        return id switch
        {
            SealID.Desert => "Sizzling Sands",
            SealID.Ringed => "Ringed Giant",
            SealID.Crimson => "Velvet Vortex",
            SealID.CrimsonMoon => "Burning Bombardier",
            SealID.Enigma => "Distant Enigma",
            _ => throw new ArgumentException("Invalid SealID")
        };
    }

    public static string GetSealFactID(SealID id)
    {
        return id switch
        {
            SealID.Desert => "DESERT_SEAL_E",
            SealID.Ringed => "RING_SEAL_E",
            SealID.Crimson => "CRIMSON_SEAL_E",
            SealID.CrimsonMoon => "VOLCANO_SEAL_E",
            SealID.Enigma => "QUANTUM_SEAL_E",
            _ => throw new ArgumentException("Invalid SealID")
        };
    }
}
