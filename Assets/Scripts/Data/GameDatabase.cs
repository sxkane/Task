using System.Collections.Generic;
using Player;
using UnityEngine;
using Weapons;
using Weapons.Items;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Database")]
    public class GameDatabase : ScriptableObject
    {
        public List<PlayerData> players;
        public List<WeaponData> weapons;
        public List<ItemData> items;
    }
}