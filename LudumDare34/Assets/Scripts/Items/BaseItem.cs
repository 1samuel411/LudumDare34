using UnityEngine;
using System.Collections;

public class BaseItem : MonoBehaviour, IPickUpable, IDestroyable
{
    public string name;
    public ItemType itemType;
    public Texture itemTexture;

    #region IDestroyable Members

    public void DestroyThisObject() {
        throw new System.NotImplementedException();
    }

    #endregion
}

public enum ItemType
{
    Misc = 0,
    Potion = 1,
    Weapon = 2
}