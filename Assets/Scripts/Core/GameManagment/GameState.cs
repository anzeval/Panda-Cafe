// Defines global game scene states.
// Used by GameManager and scene flow.
// Separates menu, map, shop, and gameplay modes.
namespace PandaCafe.Core.GameManagment
{
    public enum GameState
    {
        MainMenu,
        LevelMap,
        Shop,
        Game
    }
}