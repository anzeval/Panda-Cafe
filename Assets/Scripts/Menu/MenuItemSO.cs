using UnityEngine;

namespace PandaCafe.Menu
{
    [CreateAssetMenu(fileName = "MenuItemSO", menuName = "Scriptable Objects/MenuItemSO")]
    public class MenuItemSO : ScriptableObject
    {
        public GameObject Prefab;
        public Sprite Sprite;

        public string Name;
        public int Price;
    }
}