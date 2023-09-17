using ISYNC_Contacts.EntityLogic.Categories.Interface;
using ISYNC_Contacts.EntityLogic.Contacts.Interface;
using ISYNC_Contacts.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISYNC_Contacts
{
    /// <summary>
    /// Interaction logic for CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        private readonly ICategoryLogic _categoryLogic;
        private readonly IContactsLogic _contactLogic;


        public CategoriesPage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _categoryLogic = serviceProvider.GetRequiredService<ICategoryLogic>();
            _contactLogic= serviceProvider.GetRequiredService<IContactsLogic>();
            Loaded += OnMainWindowLoaded;
            //resize grid view
            SizeChanged += MainWindow_SizeChanged;
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newHeight = e.NewSize.Height;
            CategoryDataGrid.Height = newHeight - 100;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {

            LoadCategory();



        }


        private void contact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CategoryDataGrid.SelectedItem != null)
                {
                    Categories selectedItem = (Categories)CategoryDataGrid.SelectedItem;
                    CategoryEditor categoryEditor = new CategoryEditor(selectedItem,_categoryLogic);
                    categoryEditor.EditorWindowClosed += EditorWindowClosedHandler;
                    categoryEditor.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }





        private async void LoadCategory()
        {
            try
            {
                List<Categories> AllCategories = (List<Categories>)await _categoryLogic.GetCategories();
                AllCategories = AllCategories.Where(category => !category.Name.ToUpperInvariant().Equals("ALL")).ToList();
                CategoryDataGrid.ItemsSource = AllCategories;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"{ConvertToUserFriendlyMessage(ex)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OnClick_AddCategory(Object sender, RoutedEventArgs e)
        {
            Categories categories = new Categories();
            CategoryEditor categoryEditor = new CategoryEditor(categories,_categoryLogic);
            categoryEditor.EditorWindowClosed += EditorWindowClosedHandler;
            categoryEditor.ShowDialog();

        }
        private async void OnClick_DeleteCategory(Object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                int id = (int)button.Tag;
                Categories_Remove_Params _params = new Categories_Remove_Params();
                _params.ID = id;

                Contacts_Search_Params contacts_Search_Params = new Contacts_Search_Params();
                contacts_Search_Params.CategoryId = id;
                List<Contacts> existingContacts = (List<Contacts>)await _contactLogic.GetContacts(contacts_Search_Params);

                if(existingContacts.Count()>0)
                {
                    MessageBox.Show($"Please Remove All Contacts from this Category first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBoxResult response = MessageBox.Show("Are you sure you want to Remove this Category?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (response == MessageBoxResult.Yes)
                {
                    int result = await _categoryLogic.RemoveCategory(_params);
                    if (result == -1)
                    {
                        MessageBox.Show($"There was an error Removing this Category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    LoadCategory();
            
                }
            }

        }
        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                listBoxItem.Cursor = Cursors.Hand;
            }
        }

        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                listBoxItem.Cursor = Cursors.Arrow;
            }
        }
        private void EditorWindowClosedHandler(object sender, EventArgs e)
        {
            LoadCategory();
        }

        public static string ConvertToUserFriendlyMessage(Exception exception)
        {
            if (exception is SqlException sqlException)
            {

                switch (sqlException.Number)
                {
                    case 4060:
                        return "Database not found or user does not have permission to access it.";
                    case 547:
                        return "Foreign key constraint violation.";
                    case 18456:
                        return "Unable to connect with the specified credentials.";
                    case 233:
                        return "A connection was established to the server but the provided database was not found.";

                    default:
                        return sqlException.Message;
                }
            }
            else
            {
                return exception.Message;
            }
        }
    }
}
