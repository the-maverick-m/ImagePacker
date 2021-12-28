using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TextureItem : IComparable<TextureItem>
{
    public Texture2D Texture { get; set; }
    public Rectangle Rectangle { get; set; }
    public string Name { get; set; }
    public int Area => Texture.Width * Texture.Height;

    public int CompareTo(TextureItem other)
    {
        // We want largest to smallest, so we reverse the order
        return -Area.CompareTo(other.Area);
    }
}