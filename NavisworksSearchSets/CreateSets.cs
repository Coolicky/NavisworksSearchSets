using System.Collections.Generic;
using System.Linq;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;

namespace NavisworksSearchSets
{

    [Plugin("PMPK.SearchSets.Create",
        "PMPK",
        ToolTip = "Create Search Sets based on available categories",
        DisplayName = "Create\nSearch Sets")]
    public class CreateSets : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            var doc = Application.ActiveDocument;
            
            var allItemsSearch = new Search();
            allItemsSearch.Selection.SelectAll();
            allItemsSearch.Locations = SearchLocations.DescendantsAndSelf;

            var models = new string[] { "_42_", "_43", "_44", "_46", "_48", "_49", };

            var documentTitle = SearchCondition.HasPropertyByDisplayName("Document", "Title");
            var elementCategory = SearchCondition.HasPropertyByDisplayName("Element", "Category");
            allItemsSearch.SearchConditions.Add(elementCategory);
            allItemsSearch.SearchConditions.Add(documentTitle);

            var elements = allItemsSearch.FindAll(doc, false);

            var modelCollections = new Dictionary<string, List<string>>();
            foreach (var model in models)
            {
                var categoryCollection = new List<string>();
                foreach (var element in elements)
                {
                    var title = element.PropertyCategories.FindPropertyByDisplayName("Document", "Title");
                    var titleValue = title.Value.ToString().Replace($"{title.Value.DataType.ToString()}:", "");
                    
                    var category = element.PropertyCategories.FindPropertyByDisplayName("Element", "Category");
                    var categoryValue = category.Value.ToString().Replace($"{category.Value.DataType.ToString()}:", "");
                    
                    if (titleValue.Contains(model))
                    {
                        categoryCollection.Add(categoryValue);
                    }
                }
                modelCollections.Add(model, categoryCollection);
            }

            foreach (var discipline in modelCollections)
            {
                if (discipline.Value.Count == 0) continue;
                var categories = discipline.Value.Distinct().ToList();
                foreach (var category in categories)
                {
                    var search = new Search();
                    var titleCondition = SearchCondition.HasPropertyByDisplayName("Document", "Title");
                    search.SearchConditions.Add(titleCondition.DisplayStringContains(discipline.Key));
                    var categoryCondition = SearchCondition.HasPropertyByDisplayName("Element", "Category");
                    search.SearchConditions.Add(categoryCondition.EqualValue(VariantData.FromDisplayString(category)));
                    
                    search.Selection.SelectAll();
                    search.Locations = SearchLocations.DescendantsAndSelf;

                    var searchSet = new SelectionSet(search);
                    searchSet.DisplayName = $"{discipline.Key.Replace("_", "")}-{category}";
                    Application.ActiveDocument.SelectionSets.AddCopy(searchSet);
                }
            }
            
            return 0;
        }
    }
}