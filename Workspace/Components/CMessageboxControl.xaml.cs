using System.Media;
using System.Windows.Forms;
using System.Windows.Interop;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace LilySwapper.Workspace.Components;

public partial class CMessageboxControl : Window
{
    private readonly MessageBoxButton Button = MessageBoxButton.OK;
    private readonly bool Discord;
    private readonly bool Exit;
    private readonly string[] Links = null!;
    private readonly string[] Solutions = null!;
    private Storyboard DiscordHover = null!;
    public MessageBoxResult Result = MessageBoxResult.None;

    public CMessageboxControl(string header, string context, MessageBoxButton buttons = MessageBoxButton.OK,
        string[] links = null, string[] solutions = null, bool discord = false, bool exit = false)
    {
        InitializeComponent();
        Header.Text = header;
        Context.Text = context;
        Links = links;
        Solutions = solutions;
        Button = buttons;
        Discord = discord;
        Exit = exit;
    }

    private void MessageView_Loaded(object sender, RoutedEventArgs e)
    {
        if (Discord && !string.IsNullOrEmpty(Global.Discord))
        {
            DiscordLogo.Visibility = Visibility.Visible;
            DiscordLogo.IsEnabled = true;
        }

        switch (Button)
        {
            case MessageBoxButton.OK:
                Ok.IsEnabled = true;
                Ok.Visibility = Visibility.Visible;
                CloseButton.IsEnabled = true;
                break;
            case MessageBoxButton.YesNo:
                Yes.IsEnabled = true;
                Yes.Visibility = Visibility.Visible;
                No.IsEnabled = true;
                No.Visibility = Visibility.Visible;
                break;
            case MessageBoxButton.YesNoCancel:
                Yes.IsEnabled = true;
                Yes.Visibility = Visibility.Visible;
                No.IsEnabled = true;
                No.Visibility = Visibility.Visible;
                Cancel.IsEnabled = true;
                Cancel.Visibility = Visibility.Visible;
                break;
        }

        if (Solutions is not null)
        {
            Context.Text += "\n\nPlease try the following solutions:";
            Solutions.ToList().ForEach(delegate(string solution)
            {
                var context2 = Context;
                context2.Text = context2.Text + "\n・" + solution;
            });
        }

        //Load messagebox on active monitor
        if (Application.Current.MainWindow.IsActive)
        {
            var mainScreen = Screen.FromHandle(new WindowInteropHelper(Application.Current.MainWindow).Handle);
            Left = mainScreen.WorkingArea.Left + (mainScreen.WorkingArea.Width - Width) / 2;
            Top = mainScreen.WorkingArea.Top + (mainScreen.WorkingArea.Height - Height) / 2;
        }

        SystemSounds.Beep.Play();
    }

    private void Drag_Click(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void Close_Click(object sender, MouseButtonEventArgs e)
    {
        Result_Click(Cancel, null!);
    }

    private void Discord_Click(object sender, RoutedEventArgs e)
    {
        Global.Discord.UrlStart();
    }

    private void Discord_MouseEnter(object sender, MouseEventArgs e)
    {
        if (DiscordHover is not null) DiscordHover.Stop();

        DiscordHover = Interface.SetElementAnimations(
            new Interface.BaseAnim
            {
                Element = DiscordLogo,
                Property = new PropertyPath(HeightProperty),
                ElementAnim = new DoubleAnimation
                {
                    From = DiscordLogo.Height,
                    To = 30.0,
                    Duration = new TimeSpan(0, 0, 0, 0, 200)
                }
            },
            new Interface.BaseAnim
            {
                Element = DiscordLogo,
                Property = new PropertyPath(WidthProperty),
                ElementAnim = new DoubleAnimation
                {
                    From = DiscordLogo.Width,
                    To = 30.0,
                    Duration = new TimeSpan(0, 0, 0, 0, 200)
                }
            });

        DiscordHover.Begin();
    }

    private void Discord_MouseLeave(object sender, MouseEventArgs e)
    {
        if (DiscordHover is not null) DiscordHover.Stop();

        DiscordHover = Interface.SetElementAnimations(
            new Interface.BaseAnim
            {
                Element = DiscordLogo,
                Property = new PropertyPath(HeightProperty),
                ElementAnim = new DoubleAnimation
                {
                    From = DiscordLogo.Height,
                    To = 25.0,
                    Duration = new TimeSpan(0, 0, 0, 0, 200)
                }
            },
            new Interface.BaseAnim
            {
                Element = DiscordLogo,
                Property = new PropertyPath(WidthProperty),
                ElementAnim = new DoubleAnimation
                {
                    From = DiscordLogo.Width,
                    To = 25.0,
                    Duration = new TimeSpan(0, 0, 0, 0, 200)
                }
            });

        DiscordHover.Begin();
    }

    private void Result_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        switch (button.Name.ToUpper())
        {
            case "OK":
                Result = MessageBoxResult.OK;
                break;
            case "YES":
                Result = MessageBoxResult.Yes;
                break;
            case "NO":
                Result = MessageBoxResult.No;
                break;
            case "CANCEL":
                Result = MessageBoxResult.Cancel;
                break;
            default:
                Result = MessageBoxResult.Cancel;
                break;
        }

        if (Links is not null)
            Links.ToList().ForEach(delegate(string link)
            {
                if (!string.IsNullOrWhiteSpace(link)) link.UrlStart();
            });

        if (Exit)
            Environment.Exit(0);
        else
            Close();
    }
}