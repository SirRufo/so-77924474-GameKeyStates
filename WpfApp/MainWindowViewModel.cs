using System.Collections.Immutable;
using System.Reactive;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WpfApp;

public class MainWindowViewModel : ReactiveObject
{
    [Reactive] public ImmutableHashSet<string> Keys { get; set; } = [];
    public ReactiveCommand<Unit, Unit> TickCommand { get; }

    public Tank Player1 { get; } = new();
    public Tank Player2 { get; } = new();

    public MainWindowViewModel()
    {
        TickCommand = ReactiveCommand.Create( OnTíck );
        _lastProceedTimestamp = DateTimeOffset.UtcNow;
    }

    private readonly TimeSpan _timeslice = TimeSpan.FromSeconds( 1 ) / 60;
    private DateTimeOffset _lastProceedTimestamp;

    private void OnTíck()
    {
        var timePassed = DateTimeOffset.UtcNow - _lastProceedTimestamp;
        var slices = (int)Math.Truncate( timePassed.TotalMilliseconds / _timeslice.TotalMilliseconds );
        var timeProcessed = _timeslice * slices;
        _lastProceedTimestamp += timeProcessed;

        for ( int i = 0; i < slices; i++ )
        {
            Proceed();
        }
    }

    private void Proceed()
    {
        Player1.Tick();

        // Player1 Keys
        if ( Keys.Contains( "A" ) )
            Player1.TurnLeft();
        if ( Keys.Contains( "W" ) )
            Player1.Accelerate();
        if ( Keys.Contains( "S" ) )
            Player1.Break();
        if ( Keys.Contains( "D" ) )
            Player1.TurnRight();
        if ( Keys.Contains( "Space" ) )
            Player1.Fire();

        Player2.Tick();

        // Player2 Keys
        if ( Keys.Contains( "NumPad4" ) )
            Player2.TurnLeft();
        if ( Keys.Contains( "NumPad8" ) )
            Player2.Accelerate();
        if ( Keys.Contains( "NumPad5" ) )
            Player2.Break();
        if ( Keys.Contains( "NumPad6" ) )
            Player2.TurnRight();
        if ( Keys.Contains( "Down" ) )
            Player2.Fire();
    }
}

public class Tank : ReactiveObject
{
    [Reactive] public decimal MaxSpeed { get; private set; } = 68m * 1000m / 3600m;
    [Reactive] public decimal Acceleration { get; private set; } = 12m;
    [Reactive] public decimal Breaking { get; private set; } = 18.87m;
    [Reactive] public decimal Turning { get; private set; } = 45m;
    [Reactive] public decimal Speed { get; private set; }
    [Reactive] public decimal Heading { get; private set; }
    [Reactive] public decimal WeaponLoadingRemain { get; private set; }

    internal void Accelerate()
    {
        Speed = Math.Min( MaxSpeed, Speed + Acceleration / 60m );
    }

    internal void Break()
    {
        Speed = Math.Max( 0, Speed - Breaking / 60m );
    }

    internal void TurnLeft()
    {
        var newHeading = Heading - Turning / 60m;
        if ( newHeading < 0 )
            newHeading += 360m;
        Heading = newHeading;
    }

    internal void TurnRight()
    {
        var newHeading = Heading + Turning / 60m;
        if ( newHeading >= 360 )
            newHeading -= 360m;
        Heading = newHeading;
    }

    internal void Fire()
    {
        if ( WeaponLoadingRemain == 0 )
        {
            WeaponLoadingRemain = 1;
        }
    }

    internal void Tick()
    {
        WeaponLoadingRemain = Math.Max( 0, WeaponLoadingRemain - 0.009m );
    }
}