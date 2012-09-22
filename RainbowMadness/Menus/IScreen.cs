using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadness
{
    public interface IScreen
    {
        bool IsPopup { get; }
        void Draw(SpriteBatch batch);
        void Update(float dt);
    }
}