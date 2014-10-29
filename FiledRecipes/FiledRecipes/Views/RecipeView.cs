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
            ShowPanel(recipe.Name, ConsoleColor.White, ConsoleColor.Magenta);
            Console.WriteLine();
            Console.WriteLine(" - Ingredienser -----------------------------\n"); //Try to find common methods for this output?

            foreach (Ingredient i in recipe.Ingredients)
            {
                Console.WriteLine(i.ToString());
            }
            Console.WriteLine();
            Console.WriteLine(" - Instruktioner ----------------------------\n"); //Try to find common methods for this output? Pad Left/Right?

            foreach (string i in recipe.Instructions)
            {
                Console.WriteLine(i); //Make it not split words?
            }
        }
         public void Show(IEnumerable<IRecipe> recipes)
        {
            foreach (IRecipe recipe in recipes)
            {
                Show(recipe);
                ContinueOnKeyPressed();
            }
        }
    }
}
