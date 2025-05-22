using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Items/Items")]
public class Items : ScriptableObject
{
    public enum item{CigarettePack, Beer, Kebap, Apple, Coke}

    public item FavouriteItem;
    public Sprite itemIcon;
    public string description;
}
