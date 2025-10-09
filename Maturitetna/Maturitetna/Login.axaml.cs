using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Maturitetna;


public partial class Login : Window
{

    private readonly MainWindow _mainWindow;
    private readonly AddPlaylist _addPlaylist;
    private readonly PlayListItem _playList;
    private readonly string _conn;
    public Login(MainWindow mainWindow, AddPlaylist _addPlaylist, PlayListItem playList)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _playList = playList;
        this._addPlaylist = _addPlaylist;
        var reader = new AppSettingsReader("appsettings.json");
        _conn = reader.GetStringValue("ConnectionStrings:MyConnectionString");
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var register = new Register();
        register.Show();
        this.Close();

    }
    public void SignIn_OnClick(object? sender, RoutedEventArgs e)
    {

        string username = Username.Text;
        string password = Password.Text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Username.Text = "";
            Password.Text = "";
            prazno.IsVisible = true;
            wrong.IsVisible = false;
            return;

        }

        using MySqlConnection connection = new MySqlConnection(_conn);
        connection.Open();
        try
        {
            const string sql = "SELECT user_id, username FROM user  WHERE username = @username AND password = @password";
            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            using MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                MainWindow.userId = reader.GetInt32("user_id");
                PlayListItem.username = reader.GetString("username");
                this.Close();
                _mainWindow.ShowProfile();
                _mainWindow.NaloizIzDatabaze();
                _mainWindow.CreatePlaylistButton.IsVisible = true;
                _mainWindow.collab.IsVisible = true;
                _addPlaylist.IzpisiPlayliste();
                _playList.NaloziCollabanje();
                _addPlaylist.PrikaziReacent();

            }
            else
            {
                Username.Text = "";
                Password.Text = "";
                wrong.IsVisible = true;
                prazno.IsVisible = false;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Šef neki ne štima {exception}");
            throw;
        }
    }

}
