using nAble;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTact.Recipes
{
    public interface IRecipeManager
    {
        #region Events

        event Action RecipeListUpdated;
        event Action<string> RecipeUpdated;

        #endregion

        #region Properties

        List<string> RecipeList { get; }

        Recipe DefaultRecipe { get; }

        string DefaultPath { get; }

        #endregion

        #region Functions

        void Initialize(IEnumerable<ITracksRecipeList> recipeListUpdateFollowers, IEnumerable<ITracksRecipeChanges> recipeChangeFollowers);

        bool HasRecipe(string name);

        Recipe FetchRecipe(string name);

        Recipe LoadRecipe(string filename);

        bool SaveRecipe(Recipe recipe, bool isDefault);

        bool SaveRecipeAs(Recipe recipe, string newFilename, bool overwrite);

        Recipe AddNew(string name);

        Recipe Copy(string newName, string copyFromName);

        void Delete(string name);

        #endregion
    }
}
