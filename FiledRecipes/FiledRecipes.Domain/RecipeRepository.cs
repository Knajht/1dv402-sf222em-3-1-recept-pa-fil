using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiledRecipes.Domain
{
    /// <summary>
    /// Holder for recipes.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {
        /// <summary>
        /// Represents the recipe section.
        /// </summary>
        private const string SectionRecipe = "[Recept]";

        /// <summary>
        /// Represents the ingredients section.
        /// </summary>
        private const string SectionIngredients = "[Ingredienser]";

        /// <summary>
        /// Represents the instructions section.
        /// </summary>
        private const string SectionInstructions = "[Instruktioner]";

        /// <summary>
        /// Occurs after changes to the underlying collection of recipes.
        /// </summary>
        public event EventHandler RecipesChangedEvent;

        /// <summary>
        /// Specifies how the next line read from the file will be interpreted.
        /// </summary>
        private enum RecipeReadStatus { Indefinite, New, Ingredient, Instruction };

        /// <summary>
        /// Collection of recipes.
        /// </summary>
        private List<IRecipe> _recipes;

        /// <summary>
        /// The fully qualified path and name of the file with recipes.
        /// </summary>
        private string _path;

        /// <summary>
        /// Indicates whether the collection of recipes has been modified since it was last saved.
        /// </summary>
        public bool IsModified { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the RecipeRepository class.
        /// </summary>
        /// <param name="path">The path and name of the file with recipes.</param>
        public RecipeRepository(string path)
        {
            // Throws an exception if the path is invalid.
            _path = Path.GetFullPath(path);

            _recipes = new List<IRecipe>();
        }

        /// <summary>
        /// Returns a collection of recipes.
        /// </summary>
        /// <returns>A IEnumerable&lt;Recipe&gt; containing all the recipes.</returns>
        public virtual IEnumerable<IRecipe> GetAll()
        {
            // Deep copy the objects to avoid privacy leaks.
            return _recipes.Select(r => (IRecipe)r.Clone());
        }

        /// <summary>
        /// Returns a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to get.</param>
        /// <returns>The recipe at the specified index.</returns>
        public virtual IRecipe GetAt(int index)
        {
            // Deep copy the object to avoid privacy leak.
            return (IRecipe)_recipes[index].Clone();
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="recipe">The recipe to delete. The value can be null.</param>
        public virtual void Delete(IRecipe recipe)
        {
            // If it's a copy of a recipe...
            if (!_recipes.Contains(recipe))
            {
                // ...try to find the original!
                recipe = _recipes.Find(r => r.Equals(recipe));
            }
            _recipes.Remove(recipe);
            IsModified = true;
            OnRecipesChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to delete.</param>
        public virtual void Delete(int index)
        {
            Delete(_recipes[index]);
        }

        /// <summary>
        /// Raises the RecipesChanged event.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected virtual void OnRecipesChanged(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = RecipesChangedEvent;

            // Event will be null if there are no subscribers. 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

//NEW METHODS
        /// <summary>
        /// Loads recipes from a text file.
        /// </summary>
        public void Load()
        {
            using (StreamReader reader = new StreamReader(_path))
            {
                string line;
                Recipe recipe = null;
                List<IRecipe> recipes = new List<IRecipe>();
                RecipeReadStatus status = RecipeReadStatus.Indefinite;

                //Iterates through the .txt file provided.
                while ((line = reader.ReadLine()) != null) 
                {
                    //Checks what kind of line it is, if it's a section indicator also reads the next line.
                    switch(line) 
                    {
                        case "":
                            continue;
                        case SectionRecipe:
                            status = RecipeReadStatus.New;
                            line = reader.ReadLine();
                            break;
                        case SectionIngredients:
                            status = RecipeReadStatus.Ingredient;
                            line = reader.ReadLine();
                            break;
                        case SectionInstructions:
                            status = RecipeReadStatus.Instruction;
                            line = reader.ReadLine();
                            break;
                        default:
                            break;
                    }
                    //If the line indicates a recipe, creates a new recipe object.
                    if (status == RecipeReadStatus.New) 
                    {
                        recipe = new Recipe(line);
                        recipes.Add(recipe);
                    }
                    //If the line indicates an ingredient list, split to a string array and add it to the active recipe.
                    else if(status == RecipeReadStatus.Ingredient) 
                    {
                        string[] ingredientArray = line.Split(new char[] { ';' });
                        if(ingredientArray.GetLength(0) != 3)
                        {
                            throw new FileFormatException("The ingredient in the text file does not have the required number (3) of parts.");
                        }
                        Ingredient ingredient = new Ingredient();
                        ingredient.Amount = ingredientArray[0];
                        ingredient.Measure = ingredientArray[1];
                        ingredient.Name = ingredientArray[2];
                        recipe.Add(ingredient);
                    }
                    //If the line indicates an instruction, adds the instruction to the active recipe.
                    else if(status == RecipeReadStatus.Instruction) 
                    {
                        recipe.Add(line);
                    }
                    else
                    {
                        throw new FileFormatException("An error encountered while reading .txt file");
                    }
                }
                //Clears the field _recipes and adds the recipes from text file by alphabetic order.
                _recipes.Clear();
                _recipes.AddRange(recipes.OrderBy(rec => rec.Name));
                _recipes.TrimExcess();
                IsModified = false;
                OnRecipesChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Saves recipes to the text file. NOTE: Writes over the old text file.
        /// </summary>
        public void Save()
        {
            //Creates a StreamWriter to write to the text file.
            using (StreamWriter writer = new StreamWriter(_path))
            {
                //Iterates through recipes, starts each with writing recipe indicator and recipe name
                foreach (Recipe recipe in _recipes)
                {
                    writer.WriteLine(SectionRecipe);
                    writer.WriteLine(recipe.Name);

                    //Writes ingredient indicator, then iterates through ingredients and writes them according to supplied format.
                    writer.WriteLine(SectionIngredients);
                    foreach (Ingredient ingredient in recipe.Ingredients)
                    {
                        writer.WriteLine(String.Format("{0};{1};{2}", ingredient.Amount, ingredient.Measure, ingredient.Name)); //Note: Maybe break out the separator char to a shared const of load()/save() methods if allowed? Better support for text files with other formats.
                    }

                    //Writes instruction indicator, then iterates through instructions and writes them.
                    writer.WriteLine(SectionInstructions);
                    foreach (string instruction in recipe.Instructions)
                    {
                        writer.WriteLine(instruction);
                    }
                }
            }
        }
    }
}
