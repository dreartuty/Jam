using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Items item;
    //public List<Model> models;

    private void Awake()
    {
        //model = models[item.FavouriteItem];
    }

    public Items Pickup(Items newItem)
    {
        //model = models[newItem.FavouriteItem];
        Items temp = newItem;
        newItem = item;
        item = temp;
        if (item == null) Destroy(gameObject);
        return newItem;
    }
}
