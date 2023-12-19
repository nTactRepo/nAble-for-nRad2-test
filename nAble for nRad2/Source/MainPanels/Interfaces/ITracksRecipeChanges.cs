using nTact.Recipes;

namespace nAble
{
    public interface ITracksRecipeChanges
    {
        /// <summary>
        /// Objects with this interface are interested when a recipe is changed.
        /// Only fired on save, not on load
        /// </summary>
        /// <param name="recipe">The recipe that changed</param>
        void HandleRecipeChange(string recipeName);
    }
}
