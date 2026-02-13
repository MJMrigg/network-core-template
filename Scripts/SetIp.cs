using Godot;
using System;

public partial class SetIp : Button
{
	[Export] public TextEdit IpAddress;
	[Export] public TextEdit Port;
	
	[Signal] public delegate void JoinGameEventHandler();
	[Signal] public delegate void CreateGameEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Set the IP address of the local server
	public void SetIpConfig(){
		if(IpAddress.Text != null && IpAddress.Text != ""){
			//If it was provided by the user, set it to what they provided
			GenericCore.Instance.SetIP(IpAddress.Text);
		}else{
			//If there was no input, just use local host
			GenericCore.Instance.SetIP("127.0.0.1");
		}
	}
	
	public void JoinNewGame(){
		SetIpConfig();
		EmitSignal(SignalName.JoinGame);
	}
	
	public void StartNewGame(){
		EmitSignal(SignalName.CreateGame);
	}
}
