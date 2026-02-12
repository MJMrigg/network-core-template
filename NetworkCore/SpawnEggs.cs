using Godot;
using System;

public partial class SpawnEggs : Node2D
{

	[Export]
	public NetworkCore MyNetworkCore;

	[Export]
	public int SpawnIndex;

	[Export]
	public bool respawn;

	//This will be used to determine if the node is destroyed
	private Node v;


	public override void _Ready()
	{
		base._Ready();
		SlowStart();
	}

	public async void  SlowStart()
	{
		while (GenericCore.Instance == null || !GenericCore.Instance.IsConnected)
		{
			await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		}
		if(!GenericCore.Instance.IsServer)
		{
			//We are on a client we do not care.
			QueueFree();
		}
		GD.Print("!\n!\n!\nI am the server!");
		//More Code to Follow

		if(MyNetworkCore == null)
		{
			MyNetworkCore = (NetworkCore)GetTree().GetFirstNodeInGroup("MAIN_SPAWNER");
		}

		//We should be able to spawn now.
		v  = MyNetworkCore.NetCreateObject(SpawnIndex, 
			new Vector3(this.Position.X,this.Position.Y,0), Quaternion.Identity, 1);

		while(respawn)
		{
			while(IsInstanceValid(v))
			{
				await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
			}
			//This means the node was destroyed.
			v = MyNetworkCore.NetCreateObject(SpawnIndex,
				new Vector3(this.Position.X, this.Position.Y, 0), Quaternion.Identity, 1);
		}

		
	}

}
