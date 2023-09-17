using ISYNC_Contacts.EntityLogic.Categories.Interface;
using ISYNC_Contacts.EntityLogic.Contacts.Interface;
using ISYNC_Contacts.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISYNC_Contacts
{
    /// <summary>
    /// Interaction logic for ContactEditor.xaml
    /// </summary>
    public partial class ContactEditor : Window
    {
        public event EventHandler EditorWindowClosed;
        private bool EditMode = false;
        private readonly IContactsLogic _contactsLogic;
        private readonly ICategoryLogic _categoryLogic;
        private Contacts _contact;
        public ContactEditor(Contacts contact, ICategoryLogic categoryLogic, IContactsLogic contactsLogic)
        {
            InitializeComponent();
            _contact = contact;
            _contactsLogic = contactsLogic;
            _categoryLogic = categoryLogic;
            EditMode = contact.ID > 0;
            this.Title = EditMode ? "Edit Contact" : "Create Contact";
            this.FormHeading.Content = this.Title;

            List<ActiveState> cmbActiveItems = new List<ActiveState>()
            {
                new ActiveState() { active = true, Name = "Active" },
                new ActiveState() { active = false, Name = "Inactive" }
            };


            ActiveInput.ItemsSource = cmbActiveItems;
            ActiveInput.SelectedIndex = 0;

            ContactImageInput.MouseLeftButtonDown += ChangeImage;

            Loaded += OnMainWindowLoaded;

        }

        private async void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {

            LoadCategory();
            CategoryInput.SelectedIndex = 0;

            //Checks if edit mode and populates form with existing data
            if (EditMode)
            {
                Contacts_Search_Params search_params = new Contacts_Search_Params();
                search_params.ID = _contact.ID;


                try
                {
                    List<Contacts> allContacts = (List<Contacts>)await _contactsLogic.GetContacts(search_params);
                    if (allContacts.Count() > 0)
                    {
                        _contact = allContacts.FirstOrDefault();
                    }


                    CategoryInput.SelectedItem = CategoryInput.Items.OfType<Categories>().FirstOrDefault(item => item.ID == _contact.CategoryId);
                    FirstNameInput.Text = _contact.FirstName;
                    LastNameInput.Text = _contact.LastName;
                    DOBInput.SelectedDate = _contact.DateOfBirth;
                    CellInput.Text = _contact.CellNumber;
                    EmailInput.Text = _contact.EMail;

                    using (MemoryStream stream = new MemoryStream(_contact.Image))
                    {

                        BitmapImage imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        imageSource.StreamSource = stream;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();

                        ContactImageInput.Source = imageSource;
                    }
                    ActiveInput.SelectedItem = ActiveInput.Items.OfType<ActiveState>().FirstOrDefault(item => item.active == _contact.Active);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }





            }
            else
            {
                string selectedFilePath = "./resources/default.png";
                BitmapImage bitmapImage = new BitmapImage();

                try
                {
                    string relativeImagePath = "./resources/default.png"; // Replace with your relative file path

                    Uri imageUri = new Uri(relativeImagePath, UriKind.Relative);

                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = imageUri;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur when loading the image
                    Console.WriteLine("Error loading image: " + ex.Message);
                }

                try
                {
                    ContactImageInput.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading the image: {ex.Message}");
                }

            }
        }
        private async void LoadCategory()
        {
            try
            {
                List<Categories> AllCategories = ((List<Categories>)await _categoryLogic.GetCategories()).Where(category => category.Active || category.ID==_contact.CategoryId).ToList();
                CategoryInput.ItemsSource = AllCategories;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeImage(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Get the selected file path
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    ContactImageInput.Source = new BitmapImage(new Uri(selectedFilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading the image: {ex.Message}");
                }
            }
        }

        private async void OnClick_SaveContact(object sender, RoutedEventArgs e)
        {
            int result = 0;
            try
            {
                if (CategoryInput.SelectedItem is Models.Categories selectedCategory)
                {
                    _contact.CategoryId = selectedCategory.ID;
                }

                _contact.FirstName = FirstNameInput.Text;
                _contact.LastName = LastNameInput.Text;
                _contact.DateOfBirth = DOBInput.SelectedDate ?? DateTime.Now;
                _contact.CellNumber = CellInput.Text;
                _contact.EMail = EmailInput.Text;

                if (ActiveInput.SelectedItem is ActiveState selectedActiveState)
                {
                    _contact.Active = selectedActiveState.active;
                }

                BitmapImage imageSource = (BitmapImage)ContactImageInput.Source;

                if (imageSource != null)
                {

                    using (MemoryStream ms = new MemoryStream())
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imageSource));
                        encoder.Save(ms);

                        _contact.Image = ms.ToArray();
                    }

                }

                if (String.IsNullOrEmpty(_contact.FirstName) || String.IsNullOrEmpty(_contact.LastName) || String.IsNullOrEmpty(_contact.EMail))
                {
                    MessageBox.Show($"Error: 1 or More Required fields are blank", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                //validate cell number input
                string cellRegex = @"^(?:\+\d+|\d+)$";
                bool isValidCell = Regex.IsMatch(_contact.CellNumber, cellRegex);

                if (_contact.CellNumber.Length != 0 && (!isValidCell || (_contact.CellNumber.Length != 10 && _contact.CellNumber.Length != 12) || (_contact.CellNumber.Length == 10 && _contact.CellNumber.Contains("+"))))
                {
                    MessageBox.Show($"Error: Cell Number Format is Invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //validate email input
                string emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                bool isValidEmail = Regex.IsMatch(_contact.EMail, emailRegex);

                if (!isValidEmail)
                {
                    MessageBox.Show($"Error: Email Format is Invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Contacts_Search_Params search_params = new Contacts_Search_Params();
                search_params.EMail = _contact.EMail.ToLowerInvariant();
                List<Contacts> email_exists = (List<Contacts>) await _contactsLogic.GetContacts(search_params);

                if (email_exists.Count() > 0 && !EditMode)
                {
                    MessageBox.Show($"Error: Email already exists in the Contact List", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if (EditMode)
                {
                    Contacts_Update_Params updateParams = new Contacts_Update_Params();
                    updateParams.ID = _contact.ID;
                    updateParams.CategoryId = _contact.CategoryId;
                    updateParams.FirstName = _contact.FirstName;
                    updateParams.LastName = _contact.LastName;
                    updateParams.DateOfBirth = _contact.DateOfBirth;
                    updateParams.CellNumber = _contact.CellNumber;
                    updateParams.EMail = _contact.EMail;
                    updateParams.Active = _contact.Active;
                    updateParams.Image = _contact.Image;

                    result = await _contactsLogic.UpdateContact(updateParams);
                }
                else
                {

                    Contacts_Insert_Params insert_params = new Contacts_Insert_Params();
                    insert_params.CategoryId = _contact.CategoryId;
                    insert_params.FirstName = _contact.FirstName;
                    insert_params.LastName = _contact.LastName;
                    insert_params.DateOfBirth = _contact.DateOfBirth;
                    insert_params.CellNumber = _contact.CellNumber;
                    insert_params.EMail = _contact.EMail;
                    insert_params.Image = _contact.Image;
                    result = await _contactsLogic.InsertContact(insert_params);
                }

                if (result > 0)
                {
                    MessageBox.Show($"Contact Has been {(EditMode ? "Updated" : "Created")}", "Success", MessageBoxButton.OK);
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"There was an error saving the contact", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            EditorWindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
