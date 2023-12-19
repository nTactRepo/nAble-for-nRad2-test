namespace CommonLibrary.Source.Model.CellStatus
{
    public class CellCfgInfo
    {
        #region Properties

        public bool UsePurgeScrub { get; set; } = false;
        public int PurgeScrubIdleTime { get; set; } = 60;


        #endregion

        #region Functions

        #region Constructors

        public CellCfgInfo()
        { }

        #endregion

        #endregion    
    }
}
