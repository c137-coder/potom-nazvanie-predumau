using UnityEngine;

public enum WeaponType
{
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Leo Game/Weapon")]
public class WeaponDefinition : ItemDefinition
{
    [SerializeField] private WeaponType weaponType = WeaponType.Melee;
    [SerializeField] private int damage;

    public WeaponType WeaponType => weaponType;
    public int Damage => damage;
}
