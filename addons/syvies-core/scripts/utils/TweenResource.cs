using Godot;

namespace SyviesCore.Utils;

public partial class TweenResource : Resource
{
	[Export] public float Duration { get; private set; } = 1f;
	[Export] public Tween.TransitionType Transition { get; private set; } = Tween.TransitionType.Linear;
	[Export] public Tween.EaseType Ease { get; private set; } = Tween.EaseType.InOut;
}
