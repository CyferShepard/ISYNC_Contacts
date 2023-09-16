using ISYNC_Contacts.EntityLogic.Categories.Interface;
using ISYNC_Contacts.EntityLogic.Categories;
using ISYNC_Contacts.EntityLogic.Contacts.Interface;
using ISYNC_Contacts.EntityLogic.Contacts;
using ISYNC_Contacts.DataLayer.Interface;
using ISYNC_Contacts.DBLayer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace ISYNC_Contacts
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IConfiguration Configuration { get; private set; }

        public App()
        {

            var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = configBuilder.Build();

            var services = new ServiceCollection();
            services.AddTransient<IDbConnection>(provider => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IDapperContext, DapperContext>();
            services.AddTransient<IContactsLogic, ContactsLogic>();
            services.AddTransient<ICategoryLogic, CategoryLogic>();

            var serviceProvider = services.BuildServiceProvider();

            var mainWindow = new MainWindow(serviceProvider);

            // Show the main window
            mainWindow.Show();


        }
    }
}
