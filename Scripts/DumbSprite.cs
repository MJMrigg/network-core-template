using Godot;
using System;

public partial class DumbSprite : Sprite2D
{
	[Export] public NetID myId;
	[Export] public Color myColor;
	public override void _Ready(){
		base._Ready();
		//When the NetId is ready and has synchronized, do SlowStart
		//myId.NetIDReady += SlowStart();
	}
	public void SlowStart(){
		this.Modulate = myColor;
	}
}
