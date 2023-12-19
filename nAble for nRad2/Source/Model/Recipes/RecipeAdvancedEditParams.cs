namespace nAble.Model.Recipes
{
    public class RecipeAdvancedEditParams
    {
        public string Title { get; set; }
        public string Units { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }


        public double Accel { get; set; }
        public double Decel { get; set; }
        public double SCurve { get; set; }

        public double RecipeAccel { get; set; }
        public double RecipeDecel { get; set; }
        public double RecipeSCurve { get; set; }

        public bool AccelIsChanged => Accel != RecipeAccel;
        public bool DecelIsChanged => Decel != RecipeDecel;
        public bool SCurveIsChanged => SCurve != RecipeSCurve;
        public bool IsChanged => AccelIsChanged || DecelIsChanged || SCurveIsChanged;
    }
}
