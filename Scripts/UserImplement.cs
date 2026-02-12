using Godot;
using System;

public partial class UserImplement : Control
{
	[Export] public string PlayerName;
	[Export] public int Team;
	
	[Export] public TextEdit MyName;
	[Export] public ColorRect MyBG;
	[Export] public CheckBox MyCheck;
	
	[Export] public NetID MyNetID;
	[Export] public ItemList MyList;
	
	[Export] public bool IsReady;
	[Export] public Color MyColor;
	
	public override void _Ready(){
		base._Ready();
		SlowStart();
	}
	
	public async void SlowStart(){
		//Wait in case the NetworkID didn't get set yet
		await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
		IsReady = false;
		if(!MyNetID.IsLocal){
			//A player has just hidden their ItemsList and made they're name uneditable
			MyName.Editable = false;
			MyList.Hide();
		}
	}
	
	public override void _Process(double delta){
		base._Process(delta);
		//Keep the check box the same across all clients
		if(!MyNetID.IsLocal){
			MyCheck.ButtonPressed = IsReady;
		}
	}

	//Ask the server to change the team
	public void OnTeamChange(int n){
		//Only the local player should be asking the player to change their team
		//Prevents the local player from controlling other players' teams
		//Solves the issue of why player can control all PCs in a scene
		if(MyNetID.IsLocal){
			Rpc(MethodName.TeamChangeRPC, n);
		}
	}
	
	//Change the team of a player
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=false, TransferMode=MultiplayerPeer.TransferModeEnum.Reliable)]
	public void TeamChangeRPC(int n){
		if(GenericCore.Instance.IsServer){
			//Why not use Multiplayer.IsServer? Because that'll work if it's not connected to anything
			Team = n;
			switch(n){
				case 0: //Red
					MyBG.Color = new Color(1,0,0,1);
					break;
				case 1: //Blue
					MyBG.Color = new Color(0,0,1,1);
					break;
				case 2: //Green
					MyBG.Color = new Color(0,1,0,1);
					break;
				default:
					break;
			}
			MyColor = MyBG.Color;
			//Should just synchronize across all players
			//Don't forget to synchronize the color rect!
		}
	}
	
	//Ask the server to change our name
	public void OnNameChange(){
		if(MyNetID.IsLocal){
			Rpc(MethodName.NameChangeRPC, MyName.Text);
		}
	}
	
	//Change the name of a client
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=false, TransferMode=MultiplayerPeer.TransferModeEnum.Reliable)]
	public void NameChangeRPC(string Text){
		if(GenericCore.Instance.IsServer){
			PlayerName = Text;
			MyName.Text = Text;
		}
	}
	
	//Ask the server to change ready
	public void OnIsReady(bool Change){
		if(MyNetID.IsLocal){
			Rpc(MethodName.IsReadyChange, Change);
		}
	}
	
	//Set the client to be ready
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=false, TransferMode=MultiplayerPeer.TransferModeEnum.Reliable)]
	public void IsReadyChange(bool Change){
		if(!GenericCore.Instance.IsServer){
			return;
		}
		IsReady = Change;
	}
}
