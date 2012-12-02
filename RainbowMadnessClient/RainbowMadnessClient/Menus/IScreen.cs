using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadnessClient
{
    public interface IScreen
    {
        bool IsPopup { get; }
        void Draw(SpriteBatch batch);
        void Update(float dt);
        void OnClose(bool asCleanup);
    }
}