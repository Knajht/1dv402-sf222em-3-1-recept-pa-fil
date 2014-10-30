using FiledRecipes.Domain;
using FiledRecipes.App.Mvp;
using FiledRecipes.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiledRecipes.Views
{
    /// <summary>
    /// Displays one or all recipes.
    /// </summary>
    public class RecipeView : ViewBase, IRecipeView
    {
        /// <summary>
        /// Displays a single recipe.
        /// </summary>
        /// <param name="recipe">The recipe to be displayed</param>
        public void Show(IRecipe recipe)
        {
            //Uses ViewBase.ShowPanel() to display a header with the recipe name
            ShowPanel(recipe.Name, ConsoleColor.White, ConsoleColor.Magenta);

            //Uses the Strings resource to display ingredient header - supports multiple languages!
            string text = Strings.Ingredients.Trim() + " ";
            Console.WriteLine("\n - {0}\n", text.PadRight(42, '-'));

            //List the ingredients of the recipe
            foreach (Ingredient i in recipe.Ingredients)
            {
                Console.WriteLine(i.ToString());
            }

            //Uses the Strings resource to display instruction header - supports multiple languages!
            text = Strings.Instructions.Trim() + " ";
            Console.WriteLine("\n - {0}\n", text.PadRight(42, '-'));

            //List the instructions of the recipe
            foreach (string i in recipe.Instructions)
            {
                Console.WriteLine(i);
            }
        }
        /// <summary>
        /// Displays all recipes.
        /// </summary>
        /// <param name="recipes">The IEnumerable of the recipes</param>
         public void Show(IEnumerable<IRecipe> recipes)
        {
             //Iterates through all recipes and shows the one by one.
            foreach (IRecipe recipe in recipes)
            {
                Show(recipe);
                ContinueOnKeyPressed();
            }
        }
    }
}
