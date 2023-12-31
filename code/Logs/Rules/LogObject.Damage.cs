﻿using Sandbox;

namespace Mbk.Admin.Logs;

public partial class LogDamage : LogObject
{
	// Interface implementation
	public override string Name => "Damage";
	public override string Description => "When someone has been hit by a player or something else";

	/// <summary>
	/// The victim entity
	/// </summary>
	[Net] public Entity Victim { get; private set; }

	/// <summary>
	/// The inflictor entity
	/// </summary>
	[Net] public Entity Inflictor { get; private set; }

	/// <summary>
	/// The damage amount
	/// </summary>
	[Net] public float Damage { get; private set; }

	public LogDamage() { }

	public LogDamage(Entity victim, Entity inflictor, float damage, string format = "" ) : this()
	{
		Victim = victim;
		Inflictor = inflictor;
		Damage = damage;

		if ( format == "" )
			Format = $"{inflictor.Client.Name} has given {damage} damage to {victim.Client.Name}.";
	}
}
