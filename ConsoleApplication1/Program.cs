﻿using Glc;
using System;
using System.Collections.Generic;

class Program {
	static void Main(string[] args)
	{
		Glance.BuildSetting.outputDir = @"D:\Glc\out\";
		Glance.BuildSetting.sourceDir = @"D:\Glc\src\";
		Glance.BuildSetting.includeDir = @"D:\Glc\include\";
		Glance.BuildSetting.libDir = @"D:\Glc\lib\SFML\";
		Glance.BuildSetting.settingsDir = @"D:\Glc\settings\";
		Glance.BuildSetting.scriptsDir = @"D:\Glc\scripts\";
		Glance.BuildSetting.compilerDir = @"D:\Glc\compiler\LLVM\bin\";

		Glance.BuildSetting.libs.Add(@"sfml-graphics.lib");
		Glance.BuildSetting.libs.Add(@"sfml-window.lib");
		Glance.BuildSetting.libs.Add(@"sfml-system.lib");
		Glance.BuildSetting.libs.Add(@"sfml-audio.lib");
		Glance.BuildSetting.libs.Add(@"sfml-network.lib");

		Glance.BuildSetting.compilerKeys = @"/EHsc " + " /I" + Glance.BuildSetting.sourceDir + " /I" + Glance.BuildSetting.includeDir;
		Glance.BuildSetting.linkerKeys = @"/LIBPATH:" + Glance.BuildSetting.libDir;

		Glance.BuildSetting.exeName = "main.exe";

		Glance.BuildSetting.isClearSrcDir = true;
		Glance.BuildSetting.isGenerateCode = true;
		Glance.BuildSetting.isRecompile = true;
		Glance.BuildSetting.isRunAppAfterCompiling = true;

		Glance.Init();
		//------------------------------------------------

		game();
	}
	static void emptyGame()
	{
		var scene = new Scene();
		var layer = new Layer();
		Glance.AddScene(scene);
		scene.AddLayer(layer);
		//-------------Client code ends here
		Glance.Build();

		Console.ReadKey();
	}
	static void game()
	{
		//-------------Client code starts here

		var scene = new Scene();
		var layer1 = new Layer();
		layer1.ClassName = "ObjectsLayer";
		var enemyLayer = new Layer();
		enemyLayer.ClassName = "EnemyLayer";

		var hero = new RenderableObject(new Vec2(400, 300));
		hero.ClassName = "Hero";
		var graph = new Glc.Component.GraphicalComponent.Animation(Glc.Component.GraphicalComponent.AnimationType.Cyclic);
		graph.AddFrame(new SpriteFrame(@"resources\soldier\Soldier1.png", 400));
		graph.AddFrame(new SpriteFrame(@"resources\soldier\Soldier2.png", 400));
		hero.GraphComponent = graph;
		hero.AddComponent(new Glc.Component.Script(@"player.gcs"));

		var bullet = new RenderableObject(new Vec2());
		bullet.ClassName = "Bullet";
		bullet.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\bullet.jpg");
		bullet.AddComponent(new Glc.Component.Script(@"bullet.gcs"));
		bullet.AddComponent(new Glc.Component.Collider(Glc.Component.Collider.Type.Circle).SetRadius(5));
		bullet.IsRenderableAtStart = false;

		Random rand = new Random();

		for (var i = 0; i < 70; ++i)
		{
			var enemy = new Glc.RenderableObject(new Vec2((float)rand.Next(0, 800), (float)600.54));
			var eGraph = new Glc.Component.GraphicalComponent.Animation(Glc.Component.GraphicalComponent.AnimationType.Cyclic);
			eGraph.AddFrame(new SpriteFrame(@"resources\enemy\enemy1.png", (float)rand.Next(300, 600)));
			eGraph.AddFrame(new SpriteFrame(@"resources\enemy\enemy2.png", (float)rand.Next(300, 600)));
			enemy.GraphComponent = eGraph;
			enemy.AddComponent(new Glc.Component.Collider(Glc.Component.Collider.Type.Rectangle).SetSize(new Vec2(54, 94)));
			enemy.AddComponent(new Glc.Component.Script(@"enemy.gcs"));
			enemyLayer.AddObject(enemy);
		}

		layer1.AddObject(hero);
		layer1.AddObject(bullet);
		scene.AddLayer(enemyLayer);
		scene.AddLayer(layer1);
		Glance.AddScene(scene);
		//-------------Client code ends here
		Glance.Build();

		Console.ReadKey();
	}
}