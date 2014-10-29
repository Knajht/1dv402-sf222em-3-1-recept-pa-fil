using FiledRecipes.Domain;
using FiledRecipes.App.Mvp;
using FiledRecipes.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiledRecipes.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class RecipeView : ViewBase, IRecipeView
    {
//NEW METHODS
        public void Show(IRecipe recipe)
        {
            throw new NotImplementedException();
        }
         public void Show(IEnumerable<IRecipe> recipes)
        {
            throw new NotImplementedException();
        }
    }
}
