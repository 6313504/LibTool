using System;
using System.Collections.Generic;


namespace DaiChong.Lib.Util
{
    public class TypeUtils
    {
        private static List<Item> GetListByEnum(System.Type type, bool hasNull)
        {
            List<Item> list = new List<Item>();
            if (hasNull)
            {
                list.Add(new Item() { Id = -1, Name = "--请选择--" });
            }
            foreach (int i in Enum.GetValues(type))
            {
                list.Add(new Item() { Id = i, Name = Enum.GetName(type, i) });
            }
            return list;
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
