public class Signal
{
}

public class Signal<T> : Signal
{
    public T Value;
}

public class DirectionButtonPressedSignal : Signal<int>
{
}

public class RotateBlockSignal : Signal
{
}

public class DownArrowPressedSignal : Signal<bool>
{
}

public class UpdateScoreSignal : Signal<int>
{

}
public class DisplayScoreSignal : Signal<int>
{
}

public class BlockPlacedSignal : Signal
{
}

public class SpaceBarPressedSignal : Signal
{
}

public class GameStateUpdateSignal : Signal<GameState>
{
}