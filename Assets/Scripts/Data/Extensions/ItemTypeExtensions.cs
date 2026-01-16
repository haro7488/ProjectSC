namespace Sc.Data
{
    /// <summary>
    /// ItemType 확장 메서드
    /// </summary>
    public static class ItemTypeExtensions
    {
        /// <summary>
        /// 장비 아이템 여부 (Weapon, Armor, Accessory)
        /// </summary>
        public static bool IsEquipment(this ItemType type)
        {
            return type == ItemType.Weapon ||
                   type == ItemType.Armor ||
                   type == ItemType.Accessory;
        }

        /// <summary>
        /// 스택 가능 아이템 여부 (Consumable, Material)
        /// </summary>
        public static bool IsStackable(this ItemType type)
        {
            return type == ItemType.Consumable ||
                   type == ItemType.Material;
        }

        /// <summary>
        /// 소모품 여부
        /// </summary>
        public static bool IsConsumable(this ItemType type)
        {
            return type == ItemType.Consumable;
        }
    }
}
