using System;
using System.Collections.Generic;

namespace Mage
{
    public class Tracker
    {
        public Dictionary<float, List<Entity>> Entities { get; set; }
            = new Dictionary<float, List<Entity>>();

        public void Add(Entity entity)
        {
            // Do we have a depth layer already?
            if (!Entities.ContainsKey(entity.Depth))
                Entities.Add(entity.Depth, new List<Entity>());

            Entities[entity.Depth].Add(entity);
            entity.OnAdd();
        }

        public void Remove(Entity entity)
        {
            entity.OnRemove();
            Entities[entity.Depth].Remove(entity);
        }

        public void Clear()
        {
            Entities.Clear();
        }
    }
}