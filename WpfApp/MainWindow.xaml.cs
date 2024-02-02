using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        public MainWindowViewModel ViewModel { get => (MainWindowViewModel)DataContext; set => DataContext = value; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();

            Activated += ( s, e ) => UpdateKeys();
            Deactivated += ( s, e ) => UpdateKeys();
            PreviewKeyDown += ( s, e ) => UpdateKeys();
            PreviewKeyUp += ( s, e ) => UpdateKeys();

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds( 50 ),
            };
            _timer.Tick += ( s, e ) => ( (ICommand)ViewModel.TickCommand ).Execute( null );

            Loaded += ( s, e ) => _timer.IsEnabled = true;
        }

        private void UpdateKeys()
        {
            if ( IsActive )
            {
                var keys = Enum
                    .GetValues<Key>()
                    .Where( key => ( (int)key ) > 0 && ( (int)key ) < 256 )
                    .Where( key => Keyboard.IsKeyDown( key ) )
                    .Select( key => key.ToString() )
                    .Distinct();
                ViewModel.Keys = keys.ToImmutableHashSet();
            }
            else
            {
                ViewModel.Keys = [];
            }
        }
    }
}