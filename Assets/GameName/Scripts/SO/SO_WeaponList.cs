using UnityEngine;

[CreateAssetMenu(fileName = "WeaponList", menuName = "ScriptableObjects/WeaponList")]
public class SO_WeaponList : ScriptableObject
{
    public WeaponModelData[] weaponModelDataList;
}

[System.Serializable]
public class WeaponModelData
{
    public WeaponType WeaponType;
    public GameObject WeaponModel;
}

public enum WeaponType
{
    Sword_01,
    Sword_02,
    Sword_03,
    Sword_04,
    Sword_05,
    Sword_06,
    Sword_07,
    Sword_08,
    Sword_09,
    Sword_10,
    Sword_11,
    Sword_12,
    Sword_13,
    Sword_14,
    Sword_15,
    Sword_16,
    Sword_17,
    Sword_18,
    Sword_19
}
