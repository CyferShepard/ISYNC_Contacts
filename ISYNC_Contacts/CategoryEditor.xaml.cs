using ISYNC_Contacts.EntityLogic.Categories.Interface;

using ISYNC_Contacts.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ISYNC_Contacts
{
    /// <summary>
    /// Interaction logic for CategoryEditor.xaml
    /// </summary>
    public partial class CategoryEditor : Window
    {
        public event EventHandler EditorWindowClosed;
        private bool EditMode = false;
        private readonly ICategoryLogic _categoryLogic;
        private Categories _category;
        public CategoryEditor(Categories category, ICategoryLogic categoryLogic)
        {
            InitializeComponent();
            _category = category;
            _categoryLogic = categoryLogic;

            EditMode = _category.ID > 0;
            this.Title = EditMode ? "Edit Category" : "Create Category";
            this.FormHeading.Content = this.Title;

            List<ActiveState> cmbActiveItems = new List<ActiveState>()
            {
                new ActiveState() { active = true, Name = "Active" },
                new ActiveState() { active = false, Name = "Inactive" }
            };

            ActiveInput.ItemsSource = cmbActiveItems;
            ActiveInput.SelectedIndex = 0;

            if (EditMode)
            {
                CategoryNameInput.Text = _category.Name;
                ActiveInput.SelectedItem = ActiveInput.Items.OfType<ActiveState>().FirstOrDefault(item => item.active == _category.Active);
            }

        }


        //Checks if category name is not empty and doesnt already exist, then saves it to the db or updates existing record
        private async void OnClick_SaveCategory(object sender, RoutedEventArgs e)
        {
            int result = 0;
            if (String.IsNullOrEmpty(CategoryNameInput.Text))
            {
                MessageBox.Show($"Error: Category Name Cannot be Empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            _category.Name = CategoryNameInput.Text;

            List<Categories> allcategories = (List<Categories>)await _categoryLogic.GetCategories();
            allcategories = allcategories.Where(category => category.Name.ToLowerInvariant().Equals(_category.Name.ToLowerInvariant())).ToList();

            if (allcategories.Count() > 0 && !EditMode)
            {
                MessageBox.Show($"Error: A Category with this name already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            if (ActiveInput.SelectedItem is ActiveState selectedActiveState)
            {
                _category.Active = selectedActiveState.active;
            }

            if (EditMode)
            {
                result = await _categoryLogic.UpdateCategory(_category);
            }
            else
            {
                Categories_Insert_Params _Params = new Categories_Insert_Params();
                _Params.Name = _category.Name;
                _Params.Active = _category.Active;
                result = await _categoryLogic.InsertCategory(_Params);
            }

            if (result > 0)
            {
                MessageBox.Show($"Category Has been {(EditMode ? "Updated" : "Created")}", "Success", MessageBoxButton.OK);
                this.Close();
            }
            else
            {
                MessageBox.Show($"There was an error saving the category", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }



        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            EditorWindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
