using ISYNC_Contacts.EntityLogic.Categories.Interface;
using ISYNC_Contacts.EntityLogic.Contacts.Interface;
using ISYNC_Contacts.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ISYNC_Contacts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IContactsLogic _contactsLogic;
        private readonly ICategoryLogic _categoryLogic;



        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            //Fetches Services and loads DI Modules
            _contactsLogic = serviceProvider.GetRequiredService<IContactsLogic>();
            _categoryLogic = serviceProvider.GetRequiredService<ICategoryLogic>();

            //Loads CategoriesPage into the frameview under Categories tab
            CategoriesPage categoriesPage = new CategoriesPage(serviceProvider);

            categoriesFrame.Content = categoriesPage;

            //on load, populate filters from DB
            Loaded += OnMainWindowLoaded;
            //resize grid view
            SizeChanged += MainWindow_SizeChanged;

        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newHeight = e.NewSize.Height;
            ContactsDataGrid.Height = newHeight - 100;
        }


        //Mouse cursor interactions on listbox items
        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                listBoxItem.Cursor = Cursors.Hand;
            }
        }
        //Mouse cursor interactions on listbox items
        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                listBoxItem.Cursor = Cursors.Arrow; // Set it back to the default cursor
            }
        }

        //On load call where filter categories and data view populates with data from the DB
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadContacts();
            LoadCategory();

            List<ActiveState> cmbActiveItems = new List<ActiveState>()
            {
                new ActiveState() { active = true, Name = "Active" },
                new ActiveState() { active = false, Name = "Inactive" }
            };

            filterActiveInput.ItemsSource = cmbActiveItems;

            filterActiveInput.SelectedIndex = 0;
            filterActiveInput.SelectionChanged += ComboBox_SelectionChanged;

            filterNameInput.TextChanged += TextBox_TextChanged;

        }

        //Loads contacts from db
        private async void LoadContacts()
        {
            try
            {
                IEnumerable<Contacts> allContacts = await _contactsLogic.GetContacts();
                ContactsDataGrid.ItemsSource = allContacts;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"{ConvertToUserFriendlyMessage(ex)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //OnClick open editor for contact
        private void contact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ContactsDataGrid.SelectedItem != null)
                {
                    Contacts selectedItem = (Contacts)ContactsDataGrid.SelectedItem;
                    ContactEditor contactEditor = new ContactEditor(selectedItem, _categoryLogic, _contactsLogic);
                    contactEditor.EditorWindowClosed += EditorWindowClosedHandler;
                    contactEditor.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        //Load categories from db
        private async void LoadCategory()
        {
            try
            {
                IEnumerable<Categories> AllCategories = (IEnumerable<Categories>)await _categoryLogic.GetCategories();
                FilterCategoryInput.ItemsSource = AllCategories;
                FilterCategoryInput.SelectedIndex = 0;
                FilterCategoryInput.SelectionChanged += ComboBox_SelectionChanged;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"{ConvertToUserFriendlyMessage(ex)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Event listener on combox filters
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RoutedEventArgs routedEvent = new RoutedEventArgs();
            OnClick_FilterContacts(sender, routedEvent);
        }

        //event listener for textbox filters
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RoutedEventArgs routedEvent = new RoutedEventArgs();
            OnClick_FilterContacts(sender, routedEvent);
        }


        //filters contact list by fetching new data with updated params
        private async void OnClick_FilterContacts(object sender, RoutedEventArgs e)
        {

            string? name = String.IsNullOrEmpty(filterNameInput.Text) ? null : filterNameInput.Text;
            bool active = true;
            int? categoryid = null;
            if (filterActiveInput.SelectedItem is ActiveState selectedActiveFilter)
            {
                active = selectedActiveFilter.active;

            }

            if (FilterCategoryInput.SelectedItem is Models.Categories selectedCategoryFilter)
            {
                categoryid = selectedCategoryFilter.ID;
            }


            Contacts_Search_Params search_params = new Contacts_Search_Params();
            search_params.Active = active;
            search_params.FirstName = name;
            search_params.CategoryId = categoryid;


            try
            {
                IEnumerable<Contacts> allContacts = await _contactsLogic.GetContacts(search_params);
                ContactsDataGrid.ItemsSource = allContacts;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"{ConvertToUserFriendlyMessage(ex)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //Delete contact method and reload data sources
        private async void OnClick_DeleteContact(Object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                int id = (int)button.Tag;
                Contacts_Remove_Params contact = new Contacts_Remove_Params();
                contact.ID = id;

                MessageBoxResult response = MessageBox.Show("Are you sure you want to Remove this contact?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (response == MessageBoxResult.Yes)
                {
                    int result = await _contactsLogic.RemoveContact(contact);
                    if (result == -1)
                    {
                        MessageBox.Show($"There was an error Removing this Contact.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    LoadContacts();
                    LoadCategory();
                }
            }

        }

        //OnClick open creator for contact
        private void OnClick_AddContacts(Object sender, RoutedEventArgs e)
        {
            Contacts contact = new Contacts();
            ContactEditor contactEditor = new ContactEditor(contact, _categoryLogic, _contactsLogic);
            contactEditor.EditorWindowClosed += EditorWindowClosedHandler;
            contactEditor.ShowDialog();

        }




        //Handler to refresh data view when editors are closed
        private void EditorWindowClosedHandler(object sender, EventArgs e)
        {
            RoutedEventArgs re = new RoutedEventArgs();
            OnClick_FilterContacts(sender, re);
            FilterCategoryInput.SelectionChanged -= ComboBox_SelectionChanged;
            LoadCategory();
        }

        //Simple DB Error message to friendly message mapper
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
