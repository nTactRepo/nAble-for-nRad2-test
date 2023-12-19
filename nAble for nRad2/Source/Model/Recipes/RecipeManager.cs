using nAble;
using Support2;
using Support2.RegistryClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace nTact.Recipes
{
    public class RecipeManager : IRecipeManager
    {
        #region Events

        public event Action RecipeListUpdated;
        public event Action<string> RecipeUpdated;

        #endregion

        #region Constants

        public readonly string TemplateFilename = "";
        public const int DefaultRecipeRefreshIntervalInSeconds = 60;

        #endregion

        #region Properties

        public List<string> RecipeList => _recipes.Keys.ToList();

        public Recipe DefaultRecipe
        {
            get
            {
                RefreshDefaultIfNeeded();
                return _defaultRecipe;
            }
        }

        public string DefaultPath { get; private set; } = "";

        #endregion

        #region Member Data

        private Dictionary<string, Recipe> _recipes = new Dictionary<string, Recipe>();
        private FileSystemWatcher _recipeDirWatcher = null;

        private bool _updatingRecipes = false;
        private int _numUpdateRequests = 0;

        private Recipe _defaultRecipe = null;
        private DateTime _lastDefaultUpdate = DateTime.MinValue;

        private readonly NRadLicensing2 _licMgr = null;
        private readonly LogEntry _log = null;
        private readonly bool _useLicMgr = true;

        #endregion

        #region Functions

        #region Constructors

        public RecipeManager(NRadLicensing2 licMgr, LogEntry log, bool useLicMgr, string defaultRecipePath)
        {
            _log = log;
            _licMgr = licMgr;
            _useLicMgr = useLicMgr;
            DefaultPath = defaultRecipePath;

            TemplateFilename = Path.Combine(@"Data\Templates", "DefaultsTemplate.xml");
        }

        #endregion

        #region Public Functions

        public void Initialize(IEnumerable<ITracksRecipeList> recipeListUpdateFollowers, IEnumerable<ITracksRecipeChanges> recipeChangeFollowers)
        {
            LoadRecipes();

            _recipeDirWatcher = new FileSystemWatcher(DefaultPath, "*.xml")
            {
                EnableRaisingEvents = true
            };

            _recipeDirWatcher.Created += HandleRecipeDirectoryChange;
            _recipeDirWatcher.Deleted += HandleRecipeDirectoryChange;
            _recipeDirWatcher.Changed += HandleRecipeDirectoryChange;

            foreach (var follower in recipeListUpdateFollowers)
            {
                RecipeListUpdated += follower.HandleRecipeListChanged;
            }

            foreach (var follower in recipeChangeFollowers)
            {
                RecipeUpdated += follower.HandleRecipeChange;
            }
        }

        public bool HasRecipe(string name) => _recipes.ContainsKey(name);

        public Recipe FetchRecipe(string name)
        {
            Recipe recipe = null;

            if (_recipes.ContainsKey(name))
            {
                recipe = _recipes[name];
            }

            return recipe;
        }

        public Recipe AddNew(string name)
        {
            throw new NotImplementedException();
        }

        public Recipe Copy(string newName, string copyFromName)
        {
            throw new NotImplementedException();
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public Recipe LoadRecipe(string filename)
        {
            Recipe recipe = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Recipe));

                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    recipe = (Recipe)serializer.Deserialize(fs);
                }

                recipe.InitializeAfterLoad(filename);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Could not read Receipe File ({filename}) : {ex.Message}", "ERROR");
                recipe = null;
            }

            return recipe;
        }

        public bool SaveRecipe(Recipe recipe, bool isDefault)
        {
            try
            {
                if (string.IsNullOrEmpty(recipe.FileName))
                {
                    return false;
                }

                recipe.PrepareForSave();
                XmlSerializer serializer = new XmlSerializer(typeof(Recipe));
                string location = isDefault ? TemplateFilename : recipe.FileName;

                using (var fs = new FileStream(location, FileMode.Create))
                {
                    serializer.Serialize(fs, recipe);
                }

                if (isDefault)
                {
                    _defaultRecipe = LoadRecipe(TemplateFilename);
                    _lastDefaultUpdate = DateTime.Now;
                }

                RecipeUpdated?.Invoke(recipe.Name);
                return true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Could not save Receipe {recipe.Name} : {ex.Message}", "ERROR");
            }

            return false;
        }

        public bool SaveRecipeAs(Recipe recipe, string newFilename, bool overwrite)
        {
            try
            {
                if (string.IsNullOrEmpty(newFilename))
                {
                    return false;
                }

                recipe.FileName = newFilename;
                recipe.PrepareForSave();
                XmlSerializer serializer = new XmlSerializer(typeof(Recipe));

                using (var fs = new FileStream(recipe.FileName, FileMode.Create))
                {
                    serializer.Serialize(fs, recipe);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Could not save Receipe {recipe.Name} : {ex.Message}", "ERROR");
            }

            return false;
        }

        #endregion

        #region Event Handlers

        void HandleRecipeDirectoryChange(object sender, FileSystemEventArgs e)
        {
            LoadRecipes();
        }

        #endregion

        #region Private Functions

        private void LoadRecipes()
        {
            bool bContinue = true;

            if (!_updatingRecipes)
            {
                _updatingRecipes = true;
                _recipes.Clear();

                while (bContinue)
                {
                    Recipe curRecipe = null;
                    var di = new DirectoryInfo(DefaultPath);

                    foreach (var fi in di.GetFiles("*.xml"))
                    {
                        curRecipe = LoadRecipe(Path.Combine(fi.Directory.FullName, fi.Name));

                        if (curRecipe != null && !_recipes.ContainsKey(curRecipe.Name))
                        {
                            if (_useLicMgr)
                            {
                                if (!curRecipe.IsSegmented ||
                                    _licMgr.IsFeatureActive(LicensedFeautres.Feature1))
                                {
                                    _recipes.Add(curRecipe.Name, curRecipe);
                                }
                            }
                            else
                            {
                                _recipes.Add(curRecipe.Name, curRecipe);
                            }
                        }
                    }

                    if (_numUpdateRequests > 0)
                    {
                        _numUpdateRequests = 0;
                    }
                    else
                    {
                        bContinue = false;
                    }
                }

                _updatingRecipes = false;

                RecipeListUpdated?.Invoke();
            }
            else
            {
                _numUpdateRequests++;
            }
        }

        private void RefreshDefaultIfNeeded()
        {
            if (_defaultRecipe is null || (DateTime.Now - _lastDefaultUpdate).TotalSeconds > DefaultRecipeRefreshIntervalInSeconds)
            {
                _defaultRecipe = LoadRecipe(TemplateFilename);

                if (_defaultRecipe != null)
                {
                    _lastDefaultUpdate = DateTime.Now;
                }
            }
        }

        #endregion

        #endregion
    }
}
