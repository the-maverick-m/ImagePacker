using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public class AtlasItem
    {
        public string Name { get; }

        public Rectangle Bounds { get; }

        public Atlas Atlas { get; }

        public Texture2D Texture => Atlas.Texture;

        public AtlasItem(Atlas atlas, string name, Rectangle bounds)
        {
            Atlas = atlas;
            Name = name;
            Bounds = bounds;
        }
    }

    public class Atlas : IDisposable
    {
        public Texture2D Texture { get; set; }

        private AtlasItem[] _items;
        public AtlasItem[] Items
        {
            get => _items;
            set
            {
                _items = value;
                _indexLookup = new Dictionary<string, int>(Items.Length);
                for (int i = 0; i < Items.Length; i++)
                {
                    string name = Items[i].Name;
                    if (!string.IsNullOrEmpty(name))
                        _indexLookup.Add(name, i);
                }
            }
        }

        public int Length => _items.Length;

        private Dictionary<string, int> _indexLookup;
        private bool _disposed;

        public AtlasItem this[int index] => Items[index];
        public AtlasItem this[string name]
        {
            get
            {
                int index;
                if (_indexLookup.TryGetValue(name, out index))
                    return Items[index];
                throw new ArgumentException("AtlasItem at specified index not found");
            }
        }

        // Prevent normal instantiation
        private Atlas() { }

        // We don't have a FromStream function since we are technically loading 2 files
        // Maybe we can make 2 split up FromStreams?
        public static Atlas FromFile(GraphicsDevice graphicsDevice, string path)
        {
            AtlasItem[] items;
            // Texture2D texture = Texture2D.FromFile(graphicsDevice, path + ".png");
            Texture2D texture;
            using (FileStream stream = File.OpenRead(path + ".img"))
                texture = PackedImage.Decode(graphicsDevice, stream);

            // Load Metadata
            Atlas result = new Atlas();
            result.Texture = texture;

            using (FileStream stream = File.OpenRead(path + ".bin"))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                items = new AtlasItem[reader.ReadInt32()];
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = new AtlasItem(result,
                    reader.ReadString(),
                    new Rectangle(
                        reader.ReadInt32(),
                        reader.ReadInt32(),
                        reader.ReadInt32(),
                        reader.ReadInt32()
                    ));
                    // items[i].Name = reader.ReadString();
                    // items[i].Bounds = new Rectangle(
                    //     reader.ReadInt32(),
                    //     reader.ReadInt32(),
                    //     reader.ReadInt32(),
                    //     reader.ReadInt32()
                    // );
                }
            }

            result.Items = items;

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                Texture.Dispose();
                _items = null;
                _indexLookup = null;

                _disposed = true;
            }
        }

        ~Atlas()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}